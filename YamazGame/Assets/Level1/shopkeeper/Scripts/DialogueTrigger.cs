using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.SceneManagement;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue firstTimeDialogue;

    [Header("Repeat line")]
    public string repeat = "Hell has rewarded you again� Will you claim what it offers through me?";
    public string noLine = "Then go. The chambers will test you further.";

    public GameObject interactPrompt;

    private static bool hasMet = false;

    private bool playerInRange = false;
    private PlayerInputHandler input;
    private int playerOverlapCount = 0;

    //public void TriggerDialogue()
    //{
    //    if (hasTriggered) return;

    //    hasTriggered = true;
    //    DialogueManager.Instance.StartDialogue(dialogue);
    //}

    private void Update()
    {
        if (!playerInRange) return;
        if (input == null) return;

        if (input.InteractPressed)
        {
            TriggerInteraction();
        }
    }

    private void TriggerInteraction()
    {
        if (DialogueManager.Instance != null && DialogueManager.Instance.isDialogueActive) return;
        var dm = DialogueManager.Instance;
        if (dm == null) return;
        if (dm.isDialogueActive || dm.IsShopTransitioning) return;

        if (!hasMet)
        {
            hasMet = true;
            DialogueManager.Instance.StartDialogue(firstTimeDialogue);
        }
        else
        {
            LoadDefaultSpeaker();
            DialogueManager.Instance.StartChoice(
                repeat,
                onYes: () =>
                {
                    // OPENM PANEL HERE
                    //Debug.Log("todo");
                    DialogueManager.Instance.OpenShop();
                },
                onNo: () =>
                {
                    DialogueManager.Instance.StartSingleLine(noLine);
                    if (CursorManager.Instance != null)
                     CursorManager.Instance.ShowCursor();
                }
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var handler = collision.GetComponentInParent<PlayerInputHandler>();
        if (handler == null) return;
        input = handler;
        playerOverlapCount++;
        playerInRange = playerOverlapCount > 0;

        interactPrompt.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var handler = collision.GetComponentInParent<PlayerInputHandler>();
        if (handler == null) return;
        playerOverlapCount = Mathf.Max(0, playerOverlapCount - 1);
        playerInRange = playerOverlapCount > 0;

        if (!playerInRange)
        {
            input = null;
            interactPrompt.SetActive(false);
        }
    }
    private void LoadDefaultSpeaker()
    {
        var dm = DialogueManager.Instance;
        if (dm == null) return;

        var character = firstTimeDialogue.dialogueLines[0].character;
        if (character == null) return;

        dm.characterIcon.sprite = character.icon;
        dm.characterName.text = "Sherma";
    }
}
