using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UHFPS.Runtime;

/// <summary>
/// Task list HUD. Sits below the FPS counter and shows the next task the
/// player needs to finish (only the first incomplete task is visible at a
/// time). The whole UI is built programmatically at runtime.
///
/// Drive it from gameplay code via the static helpers, e.g.:
///   TaskListUI.Complete("head_post_office");
/// </summary>
public class TaskListUI : MonoBehaviour
{
    [Serializable]
    public class TaskEntry
    {
        [Tooltip("Unique id used by gameplay code to complete / progress this task.")]
        public string Id;

        [Tooltip("Optional location / group shown before the colon. Leave empty to show only TaskName.")]
        public string Category = "";

        [Tooltip("Short task description, e.g. \"Head to Post office\".")]
        public string TaskName = "";

        [Tooltip("How many steps to finish this task. 1 = simple task with no (x/y) counter.")]
        [Min(1)] public int RequiredCount = 1;

        [HideInInspector] public int CurrentCount;
        [HideInInspector] public bool Completed;

        // Created at runtime.
        [NonSerialized] public TMP_Text Label;
    }

    [Header("Tasks")]
    [Tooltip("Tasks the player must finish. Can also be added at runtime via AddTask().")]
    public List<TaskEntry> Tasks = new()
    {
        new TaskEntry { Id = "head_post_office",   Category = "", TaskName = "Head to Post office",    RequiredCount = 1 },
        new TaskEntry { Id = "head_church",        Category = "", TaskName = "Head to church",         RequiredCount = 1 },
        new TaskEntry { Id = "head_dukes_dockyard",Category = "", TaskName = "Head to duke's dockyard",RequiredCount = 1 },
        new TaskEntry { Id = "head_gas_station",   Category = "", TaskName = "Head to gas station",    RequiredCount = 1 },
        new TaskEntry { Id = "head_tower",         Category = "", TaskName = "Head to tower",          RequiredCount = 1 },
    };

    public enum Placement
    {
        /// <summary>Sit on whichever corner the FPS counter is anchored to, just below it.</summary>
        FollowFpsCounter,
        TopRight,
        TopLeft,
    }

    [Header("Layout")]
    [Tooltip("Where the panel sits. TopRight/TopLeft still drop it below the FPS counter's row.")]
    public Placement PanelPlacement = Placement.TopLeft;
    [Tooltip("Width of the task panel in canvas units.")]
    public float PanelWidth = 360f;
    [Tooltip("Vertical gap between the FPS counter and the top of this panel.")]
    public float GapBelowFps = 8f;
    [Tooltip("Margin from the screen corner, used when the FPS counter can't be found.")]
    public Vector2 FallbackCornerMargin = new(12f, 12f);
    public float FontSize = 16f;
    public float HeaderFontSize = 16f;

    [Header("Style")]
    public string HeaderText = "TOTAL TASKS COMPLETED";
    public Color IncompleteColor = Color.white;
    public Color CompleteColor = new(0.30f, 0.85f, 0.30f, 1f);
    public Color BarBackColor = new(0f, 0f, 0f, 0.55f);
    public Color BarFillColor = new(0.30f, 0.85f, 0.30f, 1f);
    public bool ShowProgressBar = false;

    public static TaskListUI Instance { get; private set; }

    private RectTransform _root;
    private RectTransform _listRoot;
    private Image _barFill;
    private TMP_Text _barLabel;
    private TMP_FontAsset _font;
    private bool _built;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void Start() => Build();

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
        if (_root != null) Destroy(_root.gameObject);
    }

    // ---------------------------------------------------------------- Public API

    /// <summary>Mark a task fully complete.</summary>
    public static void Complete(string id) => Instance?.CompleteTask(id);

    /// <summary>Advance a task's counter (default +1). Auto-completes when it reaches RequiredCount.</summary>
    public static void AddProgress(string id, int amount = 1) => Instance?.AddTaskProgress(id, amount);

    public void CompleteTask(string id)
    {
        var t = Find(id);
        if (t == null) return;
        t.CurrentCount = t.RequiredCount;
        t.Completed = true;
        Refresh();
    }

    public void AddTaskProgress(string id, int amount = 1)
    {
        var t = Find(id);
        if (t == null) return;
        t.CurrentCount = Mathf.Clamp(t.CurrentCount + amount, 0, t.RequiredCount);
        if (t.CurrentCount >= t.RequiredCount) t.Completed = true;
        Refresh();
    }

    public void AddTask(string id, string category, string taskName, int requiredCount = 1)
    {
        if (Find(id) != null) return;
        Tasks.Add(new TaskEntry
        {
            Id = id,
            Category = category,
            TaskName = taskName,
            RequiredCount = Mathf.Max(1, requiredCount)
        });
        Rebuild();
    }

    public void RemoveTask(string id)
    {
        int i = Tasks.FindIndex(t => t.Id == id);
        if (i < 0) return;
        Tasks.RemoveAt(i);
        Rebuild();
    }

    public void ResetAll()
    {
        foreach (var t in Tasks) { t.CurrentCount = 0; t.Completed = false; }
        Refresh();
    }

    private TaskEntry Find(string id)
    {
        var t = Tasks.Find(x => x.Id == id);
        if (t == null) Debug.LogWarning($"[TaskListUI] No task with id '{id}'.", this);
        return t;
    }

    // ---------------------------------------------------------------- UI building

    private void Rebuild()
    {
        if (!_built) return;
        Destroy(_root.gameObject);
        _built = false;
        Build();
    }

    private void Build()
    {
        if (_built) return;

        var fps = FindFirstObjectByType<FPSCounter>(FindObjectsInactive.Include);
        Canvas canvas = fps != null ? fps.GetComponentInParent<Canvas>(true) : null;
        if (canvas == null) canvas = ResolveOrCreateCanvas();
        if (canvas == null) return;

        if (fps != null && fps.FPSText != null) _font = fps.FPSText.font;
        if (_font == null) _font = TMP_Settings.defaultFontAsset;

        // Root panel.
        var rootGo = new GameObject("TaskListUI", typeof(RectTransform));
        _root = (RectTransform)rootGo.transform;
        _root.SetParent(canvas.transform, false);

        var vlg = rootGo.AddComponent<VerticalLayoutGroup>();
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;
        vlg.spacing = 4f;
        var fitter = rootGo.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        AnchorBelowFps(fps);

        if (ShowProgressBar) BuildProgressBar(rootGo.transform);

        var listGo = new GameObject("Tasks", typeof(RectTransform));
        _listRoot = (RectTransform)listGo.transform;
        _listRoot.SetParent(rootGo.transform, false);
        var listVlg = listGo.AddComponent<VerticalLayoutGroup>();
        listVlg.childControlWidth = true;
        listVlg.childControlHeight = true;
        listVlg.childForceExpandWidth = true;
        listVlg.childForceExpandHeight = false;
        listVlg.spacing = 2f;

        foreach (var t in Tasks) t.Label = CreateTaskLabel(_listRoot);

        _built = true;
        Refresh();
    }

    /// <summary>
    /// Place the panel directly below the FPS counter, mirroring whichever
    /// screen corner the counter is anchored to (top-left or top-right).
    /// </summary>
    private void AnchorBelowFps(FPSCounter fps)
    {
        var le = _root.gameObject.GetComponent<LayoutElement>() ?? _root.gameObject.AddComponent<LayoutElement>();
        le.preferredWidth = PanelWidth;
        _root.sizeDelta = new Vector2(PanelWidth, _root.sizeDelta.y);

        RectTransform fpsRt = fps != null ? fps.GetComponent<RectTransform>() : null;

        if (PanelPlacement == Placement.FollowFpsCounter && fpsRt != null)
        {
            // Sit on the same corner as the FPS counter, just below it.
            Vector2 corner = fpsRt.anchorMin;
            _root.anchorMin = _root.anchorMax = corner;
            _root.pivot = new Vector2(corner.x, 1f);
            float x = fpsRt.anchoredPosition.x;
            float y = fpsRt.anchoredPosition.y - fpsRt.rect.height - GapBelowFps;
            _root.anchoredPosition = new Vector2(x, y);
            return;
        }

        // Explicit corner. Drop below the FPS counter's row when we know its
        // height, otherwise just use the fallback top margin.
        bool right = PanelPlacement != Placement.TopLeft;
        float cornerX = right ? 1f : 0f;
        _root.anchorMin = _root.anchorMax = new Vector2(cornerX, 1f);
        _root.pivot = new Vector2(cornerX, 1f);

        float marginX = right ? -FallbackCornerMargin.x : FallbackCornerMargin.x;
        float topY = -FallbackCornerMargin.y;
        if (fpsRt != null)
            topY = fpsRt.anchoredPosition.y - fpsRt.rect.height - GapBelowFps;
        _root.anchoredPosition = new Vector2(marginX, topY);
    }

    private void BuildProgressBar(Transform parent)
    {
        var barGo = new GameObject("ProgressBar", typeof(RectTransform), typeof(Image));
        var barRt = (RectTransform)barGo.transform;
        barRt.SetParent(parent, false);
        barGo.GetComponent<Image>().color = BarBackColor;
        var barLe = barGo.AddComponent<LayoutElement>();
        barLe.preferredHeight = HeaderFontSize + 12f;
        barLe.minHeight = HeaderFontSize + 12f;

        var fillGo = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        var fillRt = (RectTransform)fillGo.transform;
        fillRt.SetParent(barGo.transform, false);
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = fillRt.offsetMax = Vector2.zero;
        _barFill = fillGo.GetComponent<Image>();
        _barFill.color = BarFillColor;
        _barFill.type = Image.Type.Filled;
        _barFill.fillMethod = Image.FillMethod.Horizontal;
        _barFill.fillOrigin = (int)Image.OriginHorizontal.Left;
        _barFill.fillAmount = 0f;

        _barLabel = MakeText(barGo.transform, HeaderText, HeaderFontSize, IncompleteColor, FontStyles.Bold);
        var lblRt = _barLabel.rectTransform;
        lblRt.anchorMin = Vector2.zero;
        lblRt.anchorMax = Vector2.one;
        lblRt.offsetMin = new Vector2(8f, 0f);
        lblRt.offsetMax = new Vector2(-8f, 0f);
        _barLabel.alignment = TextAlignmentOptions.Left;
    }

    private TMP_Text CreateTaskLabel(Transform parent)
    {
        var txt = MakeText(parent, string.Empty, FontSize, IncompleteColor, FontStyles.Bold);
        var le = txt.gameObject.AddComponent<LayoutElement>();
        le.preferredHeight = FontSize + 6f;
        txt.alignment = TextAlignmentOptions.Left;
        txt.enableWordWrapping = false;
        txt.overflowMode = TextOverflowModes.Overflow;
        return txt;
    }

    private TMP_Text MakeText(Transform parent, string content, float size, Color color, FontStyles style)
    {
        var go = new GameObject("Text", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var txt = go.AddComponent<TextMeshProUGUI>();
        if (_font != null) txt.font = _font;
        txt.text = content;
        txt.fontSize = size;
        txt.color = color;
        txt.fontStyle = style;
        txt.raycastTarget = false;
        return txt;
    }

    private Canvas ResolveOrCreateCanvas()
    {
        foreach (var c in FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            if (c.renderMode == RenderMode.ScreenSpaceOverlay || c.renderMode == RenderMode.ScreenSpaceCamera)
                return c;

        var go = new GameObject("TaskListUI Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = go.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        return canvas;
    }

    // ---------------------------------------------------------------- Refresh

    private void Refresh()
    {
        if (!_built) return;

        int done = 0;
        bool activeShown = false;
        foreach (var t in Tasks)
        {
            if (t.Completed) done++;
            if (t.Label == null) continue;

            // Only the first incomplete task is visible; completed and future
            // tasks stay hidden until their turn.
            bool isActive = !t.Completed && !activeShown;
            t.Label.gameObject.SetActive(isActive);
            if (!isActive) continue;
            activeShown = true;

            string label = string.IsNullOrEmpty(t.Category)
                ? t.TaskName
                : $"{t.Category}: {t.TaskName}";
            if (t.RequiredCount > 1)
                label += $" ({t.CurrentCount}/{t.RequiredCount})";

            t.Label.text = label;
            t.Label.color = IncompleteColor;
        }

        if (_barFill != null)
            _barFill.fillAmount = Tasks.Count > 0 ? (float)done / Tasks.Count : 0f;
        if (_barLabel != null)
            _barLabel.text = $"{HeaderText}  {done}/{Tasks.Count}";
    }
}
