
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class HeartsManager : UdonSharpBehaviour
{
    public int hearts = 3;
    public int getHearts()
    {
        return hearts;
    }

    public void setHearts(int newHearts)
    {
        hearts = newHearts;
    }
}
