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

    [Header("UI Images on the sign")]
    public Image problemImage;
    public Image option0Image;
    public Image option1Image;
    public Image option2Image;

    [Header("Path through the world (0 = start, last = end)")]
    public Transform[] playerPoints;   // size 10: where player goes at each stage
    public Transform[] signPoints;     // size 10: where sign should be at each stage

    // current stage (0..playerPoints.Length-1)
    private int stageIndex = 0;

    // which finger is correct this time? 0, 1, or 2
    private int correctSlot = -1;

    void Start()
    {
        // put sign & player at the first stage position
        SnapToStage(0);
        SetupProblem();
    }

    private void SnapToStage(int index)
    {
        stageIndex = index;

        // move sign
        if (signPoints != null && stageIndex >= 0 && stageIndex < signPoints.Length && signPoints[stageIndex] != null)
        {
            transform.SetPositionAndRotation(new Vector3(signPoints[stageIndex].position.x, signPoints[stageIndex].position.y + 0.7f, signPoints[stageIndex].position.z), Quaternion.Euler(-90, 0, 0));
        }

        // NOTE: we don't teleport the player here; only on correct answer
        Debug.Log("[SignSelect] Snapped sign to stage " + stageIndex);
    }

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

        // clamp / wrap index if needed
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

    // called from FingerOption.Interact()
    public void OnOptionSelected(int chosenIndex)
    {
        Debug.Log("[SignSelect] Option clicked: " + chosenIndex + " (correctSlot = " + correctSlot + ")");

        if (chosenIndex == correctSlot)
        {
            Debug.Log("[SignSelect] Correct answer chosen! Teleporting to next stage.");
            TeleportToNextStage();
        }
        else
        {
            Debug.Log("[SignSelect] Wrong answer selected.");
        }
    }

    private void TeleportToNextStage()
    {
        int nextStage = stageIndex + 1;

        // last stage? you can end the game here
        if (playerPoints == null || nextStage >= playerPoints.Length)
        {
            Debug.Log("[SignSelect] Reached final stage or no more points defined.");
            // maybe play a victory effect here
            return;
        }

        Transform targetPoint = playerPoints[nextStage];
        if (targetPoint == null)
        {
            Debug.LogWarning("[SignSelect] Next player point is null.");
        }
        else
        {
            VRCPlayerApi player = Networking.LocalPlayer;
            if (player == null)
            {
                Debug.Log("[SignSelect] LocalPlayer is null (editor). Would teleport to: " + targetPoint.position);
            }
            else
            {
                player.TeleportTo(targetPoint.position, targetPoint.rotation);
            }
        }

        // move sign to the next stage location
        SnapToStage(nextStage);

        // pick a new random problem for the new location
        RandomizeProblem();
    }

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
