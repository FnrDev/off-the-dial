using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UHFPS.Input;
using UHFPS.Runtime;

public class IntroGuide : MonoBehaviour
{
    private const string ContinueHint = "\n\n<size=70%><i>Press [E] to continue · [Backspace] to go back.</i></size>";
    private const string ContinueHintFirstPage = "\n\n<size=70%><i>Press [E] to continue.</i></size>";

    [Tooltip("Show even when the game is being loaded from a save. Default off — intro only on new games.")]
    public bool ShowOnLoadGame = false;

    [TextArea(6, 20)]
    public string[] Pages = new[]
    {
        "Welcome to Oxley — a quiet island town that fell silent overnight.\nThe ferry never came back. The radio dropped to static. Whatever happened here, it left the streets empty and the dial humming with frequencies that shouldn't exist.\n\nYou wash ashore with nothing but your wits and a flashlight that flickers when something's watching.",
        "Six places anchor this story. Walk them in order:\n\n• <b>Post Office</b> — letters, frequencies, and the first real clue.\n• <b>Church</b> — the town's heart, and where its secrets were buried.\n• <b>Duke's Dockyard</b> — the hangar by the water, full of cargo nobody claimed.\n• <b>Gas Station</b> — fuel, a radio that won't shut up, and something restless.\n• <b>Diner</b> — past the town limits. The owner left in a hurry.\n• <b>The Tower</b> — the last broadcast. The only signal that still goes out.",
        "How to survive long enough to tune in:\n\n• <b>Move:</b> WASD. Hold <b>Shift</b> to sprint.\n• <b>Interact / pick up:</b> press <b>E</b>.\n• <b>Inventory:</b> press <b>Tab</b> — combine items, examine them, set shortcuts.\n• <b>Examine closely:</b> hold <b>Right Mouse</b> on an item.\n• <b>Flashlight:</b> press <b>F</b>. Batteries don't last forever.\n• <b>Ghosts</b> hunt by sound. If one spots you, hide or run — don't fight.\n\nFollow your task list in the top corner. It tells you which zone to head to next. Find the clues, tune the radio, and don't stop moving."
    };

    private GameManager gameManager;
    private PlayerPresenceManager presence;

    private IEnumerator Start()
    {
        if (Pages == null || Pages.Length == 0) yield break;
        if (!ShowOnLoadGame && SaveGameManager.GameActuallyLoad) yield break;

        yield return null;
        gameManager = GameManager.Instance;
        presence = PlayerPresenceManager.Instance;
        if (gameManager == null || presence == null) yield break;

        while (!presence.PlayerIsUnlocked) yield return null;

        presence.FreezePlayer(true);

        int i = 0;
        while (i < Pages.Length)
        {
            string hint = i > 0 ? ContinueHint : ContinueHintFirstPage;
            gameManager.ShowPaperInfo(true, false, Pages[i] + hint);

            yield return null;
            while (true)
            {
                if (InputManager.ReadButtonOnce(this, Controls.USE)) { i++; break; }
                if (i > 0 && BackPressed()) { i--; break; }
                yield return null;
            }
        }

        gameManager.ShowPaperInfo(false, false);
        presence.FreezePlayer(false);
    }

    private static bool BackPressed()
    {
        var kb = Keyboard.current;
        return kb != null && kb.backspaceKey.wasPressedThisFrame;
    }
}
