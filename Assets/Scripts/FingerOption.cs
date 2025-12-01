using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class FingerOption : UdonSharpBehaviour
{
    [Header("Reference to the sign controller")]
    public SignSelect sign;   // drag your sign here

    [Header("Which option is this finger? 0, 1, or 2")]
    public int optionIndex;

    public override void Interact()
    {
        if (sign == null)
        {
            Debug.LogWarning("[FingerOption] Sign reference not set.");
            return;
        }

        Debug.Log("[FingerOption] Interact on option " + optionIndex);
        sign.OnOptionSelected(optionIndex, this.gameObject);
    }
}
