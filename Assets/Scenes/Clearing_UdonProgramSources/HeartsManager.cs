
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEngine.UI;

public class HeartsManager : UdonSharpBehaviour
{
    public int hearts = 3;
    public Image heart1;
    public Image heart2;
    public Image heart3;
    public int getHearts()
    {
        return hearts;
    }

    public void setHearts(int newHearts)
    {
        hearts = newHearts;
        heart1.enabled = hearts >= 1;
        heart2.enabled = hearts >= 2;
        heart3.enabled = hearts >= 3;
    }
}
