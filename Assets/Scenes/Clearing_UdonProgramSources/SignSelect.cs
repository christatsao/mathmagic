using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class SignSelect : UdonSharpBehaviour
{
    [Header("Problem data")]
    public DerivativeProblemBankImages problemBank;
    public int problemIndex;

    public HeartsManager heartScript;

    public AudioSource wrongSound;
    public AudioSource rightSound;

    public TMPro.TextMeshProUGUI problemText;

    [Header("UI Images on the sign")]
    public Image problemImage;
    public Image option0Image;
    public Image option1Image;
    public Image option2Image;

    [Header("Path through the world (0 = start, last = end)")]
    public Transform[] playerPoints;   // where the PLAYER should be at each stage
    public Transform[] signPoints;     // where the PLAY AREA (this object + children) goes for stages > 0

    [SerializeField]
    private int stageIndex = 0;        // current stage index

    private int correctSlot = -1;

    // --- NEW: cache the original transform of the play area (stage 0) ---
    private Vector3 initialPosition;
    private Quaternion initialRotation;

    // --------------------------------------------------------------------
    //  Debug helper
    // --------------------------------------------------------------------
    private void LogStageInfo(string context, int stage, Transform playerPoint, Transform signPoint)
    {
        string pp = (playerPoint == null) ? "null" : playerPoint.position.ToString();
        string sp = (signPoint == null) ? "null" : signPoint.position.ToString();

        Debug.Log($"[SignSelect] {context} stage {stage} :: playerPoints[{stage}] = {pp}, signPoints[{stage}] = {sp}");
    }

    // --------------------------------------------------------------------
    //  Unity / Udon lifecycle
    // --------------------------------------------------------------------
    void Start()
    {
        // Whatever you set for this object in the editor is STAGE 0
        initialPosition = transform.position;
        initialRotation = transform.rotation;

        SnapToStage(0);
        SetupProblem();
    }

    // --------------------------------------------------------------------
    //  Move the entire PLAY AREA (this object + children) to a stage
    // --------------------------------------------------------------------
    private void SnapToStage(int index)
    {
        stageIndex = index;

        if (stageIndex == 0)
        {
            // Stage 0: use the exact transform from the editor
            transform.SetPositionAndRotation(initialPosition, initialRotation);
        }
        else
        {
            // Stages 1..N: use signPoints[stageIndex] to position the PLAY AREA
            if (signPoints != null &&
                stageIndex >= 0 &&
                stageIndex < signPoints.Length &&
                signPoints[stageIndex] != null)
            {
                Transform sp = signPoints[stageIndex];

                // Move the whole PlayArea to this point; keep the same rotation as stage 0
                // (if you want stage-specific rotations instead, change initialRotation -> sp.rotation)
                Vector3 pos = sp.position;
                transform.SetPositionAndRotation(pos, initialRotation);
            }
        }

        // Make sure children (sign, ghost, dialogue, etc.) are active
        foreach (Transform childTransform in transform)
        {
            childTransform.gameObject.SetActive(true);
        }

        Debug.Log("[SignSelect] Snapped PLAY AREA to stage " + stageIndex);
    }

    // --------------------------------------------------------------------
    //  Problem setup
    // --------------------------------------------------------------------
    public void SetupProblem()
    {
        if (problemBank == null)
        {
            Debug.LogWarning("[SignSelect] Problem bank is not assigned.");
            return;
        }

        int count = problemBank.GetProblemCount();
        if (count <= 0)
        {
            Debug.LogWarning("[SignSelect] No problems in bank.");
            return;
        }

        if (problemIndex < 0 || problemIndex >= count)
        {
            problemIndex = 0;
        }

        Sprite answer = problemBank.answerSprites[problemIndex];
        Sprite wrong1 = problemBank.bSprites[problemIndex];
        Sprite wrong2 = problemBank.cSprites[problemIndex];

        if (problemImage != null)
            problemImage.sprite = problemBank.GetProblemSprite(problemIndex);

        // random correct slot
        correctSlot = Random.Range(0, 3);

        if (correctSlot == 0)
        {
            if (option0Image != null) option0Image.sprite = answer;
            if (option1Image != null) option1Image.sprite = wrong1;
            if (option2Image != null) option2Image.sprite = wrong2;
        }
        else if (correctSlot == 1)
        {
            if (option0Image != null) option0Image.sprite = wrong1;
            if (option1Image != null) option1Image.sprite = answer;
            if (option2Image != null) option2Image.sprite = wrong2;
        }
        else
        {
            if (option0Image != null) option0Image.sprite = wrong1;
            if (option1Image != null) option1Image.sprite = wrong2;
            if (option2Image != null) option2Image.sprite = answer;
        }

        Debug.Log("[SignSelect] Stage " + stageIndex + " using problem " + problemIndex + " with correctSlot " + correctSlot);
    }

    // --------------------------------------------------------------------
    //  Typing effect
    // --------------------------------------------------------------------
    private int index = 0;
    private string message = "";

    public void TypeNextCharacter()
    {
        if (index >= message.Length) return;

        problemText.text += message[index];
        index++;

        SendCustomEventDelayedSeconds(nameof(TypeNextCharacter), 0.05f);
    }

    // --------------------------------------------------------------------
    //  Option selection
    // --------------------------------------------------------------------
    public void OnOptionSelected(int chosenIndex, GameObject sender)
    {
        Debug.Log("[SignSelect] Option clicked: " + chosenIndex + " (correctSlot = " + correctSlot + ")");

        if (chosenIndex == correctSlot)
        {
            Debug.Log("[SignSelect] Correct answer chosen! Teleporting to next stage.");
            if(rightSound != null) rightSound.Play();
            TeleportToNextStage();
        }
        else
        {
            Debug.Log("[SignSelect] Wrong answer selected.");

            sender.SetActive(false);

            heartScript.setHearts(heartScript.getHearts() - 1);
            if (wrongSound != null) wrongSound.Play();
            message = "Nice try... try again!";
            index = 0;
            problemText.text = "";
            TypeNextCharacter();

            Debug.Log("[SignSelect] Heart health: " + heartScript.getHearts());
            if (heartScript.getHearts() == 0)
            {
                // restart everything at stage 0
                message = "You got lost... We're restarting from the beginning";
                heartScript.setHearts(3);

                // move PlayArea back to original editor position
                SnapToStage(0);

                // teleport player back to stage 0 player point
                if (playerPoints != null && playerPoints.Length > 0 && playerPoints[0] != null)
                {
                    Transform targetPoint = playerPoints[0];
                    VRCPlayerApi player = Networking.LocalPlayer;
                    if (player != null)
                    {
                        player.TeleportTo(targetPoint.position, targetPoint.rotation);
                    }
                }

                RandomizeProblem();
            }
        }
    }

    // --------------------------------------------------------------------
    //  Teleport to next stage (player + play area)
    // --------------------------------------------------------------------
    private void TeleportToNextStage()
    {
        int nextStage = stageIndex + 1;

        if (playerPoints == null || nextStage >= playerPoints.Length)
        {
            Debug.Log("[SignSelect] Reached final stage or no more points defined.");
            return;
        }

        Transform playerPoint = playerPoints[nextStage];
        Transform signPoint = (signPoints != null && nextStage < signPoints.Length) ? signPoints[nextStage] : null;

        LogStageInfo("Before teleport", nextStage, playerPoint, signPoint);

        // Teleport PLAYER
        if (playerPoint == null)
        {
            Debug.LogWarning("[SignSelect] Next player point is null.");
        }
        else
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (player == null)
            {
                Debug.Log("[SignSelect] LocalPlayer is null (editor). Would teleport to: " + playerPoint.position);
            }
            else
            {
                Debug.Log($"[SignSelect] Teleporting player to {playerPoint.position} rot {playerPoint.rotation.eulerAngles}");
                player.TeleportTo(playerPoint.position, playerPoint.rotation);
            }
        }

        // Move the entire PLAY AREA (this object + children) to next stage
        SnapToStage(nextStage);

        LogStageInfo("After SnapToStage", nextStage, playerPoint, signPoint);

        RandomizeProblem();
    }

    // --------------------------------------------------------------------
    //  Randomize problem index for new stage
    // --------------------------------------------------------------------
    private void RandomizeProblem()
    {
        if (problemBank == null) return;

        int count = problemBank.GetProblemCount();
        if (count <= 0) return;

        int oldIndex = problemIndex;
        int newIndex = oldIndex;

        if (count > 1)
        {
            int safety = 0;
            while (newIndex == oldIndex && safety < 10)
            {
                newIndex = Random.Range(0, count);
                safety++;
            }
        }

        problemIndex = newIndex;
        SetupProblem();
    }
}
