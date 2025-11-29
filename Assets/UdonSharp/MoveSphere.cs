
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MoveSphere : UdonSharpBehaviour
{
    void Start()
    {
    }

    private void Update()
    {
        this.gameObject.transform.Translate(new Vector3(1, 0, 0) * Time.deltaTime);
    }
}
