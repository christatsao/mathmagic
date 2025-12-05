using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class GhostDialogueManager : UdonSharpBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;          // The panel (worldspace canvas) that holds the text
    public TextMeshProUGUI dialogueText;      // The actual text component

    [Header("Dialogue Lines")]
    [TextArea(2, 4)]
    public string[] lines;                    // Fill these in the inspector

    [Header("Options")]
    public bool autoHideAtEnd = true;         // Hide panel after last line
    public bool startOnFirstInteract = true;  // First interact starts the dialogue

    private int currentIndex = -1;
    private bool dialogueActive;

    private void Start()
    {
        // Make sure panel starts hidden
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }
    }

    public override void Interact()
    {
        // Player clicks on the ghost
        if (!dialogueActive && startOnFirstInteract)
        {
            StartDialogue();
        }
        else if (dialogueActive)
        {
            NextLine();
        }
    }

    public void StartDialogue()
    {
        if (lines == null || lines.Length == 0 || dialogueText == null || dialoguePanel == null)
        {
            Debug.LogWarning("[GhostDialogueManager] Not set up correctly.");
            return;
        }

        dialogueActive = true;
        currentIndex = 0;

        dialoguePanel.SetActive(true);
        ShowCurrentLine();
    }

    public void NextLine()
    {
        if (!dialogueActive) return;

        currentIndex++;

        if (currentIndex >= lines.Length)
        {
            EndDialogue();
            return;
        }

        ShowCurrentLine();
    }

    private void ShowCurrentLine()
    {
        dialogueText.text = lines[currentIndex];
    }

    public void EndDialogue()
    {
        dialogueActive = false;
        currentIndex = -1;

        if (autoHideAtEnd && dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // If you want to trigger something after dialogue, you can
        // SendCustomEvent to another UdonBehaviour here.
    }

    // Optional: if you want to start from a button instead of Interact
    public void _StartFromButton()
    {
        StartDialogue();
    }

    // Optional: if you want “Next” button on the UI instead of clicking ghost
    public void _NextFromButton()
    {
        NextLine();
    }
}
