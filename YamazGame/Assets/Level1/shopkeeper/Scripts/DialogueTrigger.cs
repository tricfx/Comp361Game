using System.Collections.Generic;
using UnityEngine;

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
    public string repeat = "Hell has rewarded you again… Will you claim what it offers through me?";
    public string noLine = "Then go. The chambers will test you further.";

    public GameObject interactPrompt;

    private bool hasMet = false;

    private bool playerInRange = false;
    private PlayerInputHandler input;

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
                }
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        playerInRange = true;
        input = collision.GetComponent<PlayerInputHandler>();
        if (interactPrompt != null)
        {
            interactPrompt.SetActive(true);
            Debug.Log("Prompt enabled: " + interactPrompt.activeSelf);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;

        playerInRange = false;
        input = null;

        if (interactPrompt != null) interactPrompt.SetActive(false);
    }
}
