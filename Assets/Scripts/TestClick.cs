
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class TestClick : UdonSharpBehaviour
{
    public void TestButton()
    {
        Debug.Log("[UiClickTest] Unity button click reached.");
    }
}
