using UdonSharp;
using UnityEngine;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class DerivativeProblemBankImages : UdonSharpBehaviour
{
    [Header("Problem statements ( [#]P.png )")]
    // e.g. 1P, 2P, 3P, ...
    public Sprite[] problemSprites;

    [Header("Correct answer option ( [#]ANSWER.png )")]
    // e.g. 1ANSWER, 2ANSWER, 3ANSWER, ...
    public Sprite[] answerSprites;

    [Header("Incorrect option B ( [#]B.png )")]
    // e.g. 1B, 2B, 3B, ...
    public Sprite[] bSprites;

    [Header("Incorrect option C ( [#]C.png )")]
    // e.g. 1C, 2C, 3C, ...
    public Sprite[] cSprites;

    [Header("Correct option index for each problem")]
    [Tooltip("0 = ANSWER sprite, 1 = B sprite, 2 = C sprite")]
    public int[] correctIndex;

    public int GetProblemCount()
    {
        return problemSprites == null ? 0 : problemSprites.Length;
    }

    // Convenience helpers if you want them:

    public Sprite GetProblemSprite(int problemIndex)
    {
        return problemSprites[problemIndex];
    }

    public Sprite GetOptionSprite(int problemIndex, int optionIndex)
    {
        // 0 = ANSWER, 1 = B, 2 = C
        if (optionIndex == 0) return answerSprites[problemIndex];
        if (optionIndex == 1) return bSprites[problemIndex];
        return cSprites[problemIndex];
    }

    public int GetCorrectOptionIndex(int problemIndex)
    {
        return correctIndex[problemIndex];
    }
}


