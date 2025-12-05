using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using TMPro;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class IntroDialogue : UdonSharpBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;          // World-space panel or root UI object
    public TextMeshProUGUI dialogueText;      // TMP text component for the dialogue

    [Header("Dialogue Lines")]
    [TextArea(2, 4)]
    public string[] lines;                    // Fill in inspector

    [Header("Flow Options")]
    public bool autoHideAtEnd = true;         // Hide UI after last line
    public bool startOnFirstInteract = true;  // First click starts dialogue

    [Header("Intro -> Stage 0 Teleport")]
    public bool teleportToStage0OnEnd = false; // Turn ON for IntroStage ghost
    public Transform stage0SpawnPoint;         // Where Stage 0 starts (position + rotation)

    private int currentIndex = -1;
    private bool dialogueActive;

    private void Start()
    {
        // Make sure dialogue panel starts hidden
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
        if (dialogueText != null && currentIndex >= 0 && currentIndex < lines.Length)
        {
            dialogueText.text = lines[currentIndex];
        }
    }

    public void EndDialogue()
    {
        dialogueActive = false;
        currentIndex = -1;

        if (autoHideAtEnd && dialoguePanel != null)
        {
            dialoguePanel.SetActive(false);
        }

        // Teleport to Stage 0 after Intro dialogue finishes
        if (teleportToStage0OnEnd)
        {
            TeleportToStage0();
        }
    }

    private void TeleportToStage0()
    {
        VRCPlayerApi localPlayer = Networking.LocalPlayer;
        if (localPlayer == null)
        {
            Debug.LogWarning("[GhostDialogueManager] No local player to teleport.");
            return;
        }

        if (stage0SpawnPoint == null)
        {
            Debug.LogWarning("[GhostDialogueManager] stage0SpawnPoint not assigned.");
            return;
        }

        localPlayer.TeleportTo(
            stage0SpawnPoint.position,
            stage0SpawnPoint.rotation
        );
    }

    // Optional: call these from UI buttons instead of Interact

    public void _StartFromButton()
    {
        StartDialogue();
    }

    public void _NextFromButton()
    {
        NextLine();
    }
}
