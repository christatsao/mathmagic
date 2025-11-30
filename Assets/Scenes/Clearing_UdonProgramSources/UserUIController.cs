
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class UserUIController : UdonSharpBehaviour
{
    public Vector3 offset = new Vector3(0, 0, 0.2f); // distance in front of eyes

    public GameObject playerUI;

    void Start()
    {
        playerUI = Instantiate(playerUI);
    }

    void Update()
    {
        VRCPlayerApi player = Networking.LocalPlayer;
        var head = player.GetTrackingData(VRCPlayerApi.TrackingDataType.Head);

        playerUI.transform.position = head.position;
        playerUI.transform.rotation = head.rotation;

        // The Canvas sits at localOffset relative to this handle
        playerUI.transform.localPosition += offset;
    }


}
