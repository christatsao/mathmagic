
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI; 

public class ghostLooking : UdonSharpBehaviour
{

    void LateUpdate()
    {
        Vector3 pos = Networking.LocalPlayer.GetPosition();
        pos.y += 1;
        transform.LookAt(pos);

    }
}
