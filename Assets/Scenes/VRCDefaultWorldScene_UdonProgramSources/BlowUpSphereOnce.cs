
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class BlowUpSphereOnce : UdonSharpBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject targetSphere;        // The sphere to "blow up"
    public ParticleSystem explosionEffect; // The explosion particle FX
    public AudioSource explosionAudio;     // The explosion sound

    private bool hasExploded = false;      // Tracks if it already happened

    public override void Interact()
    {
        if (hasExploded) return;  // Stop if already triggered
        hasExploded = true;

        // Hide the sphere
        if (targetSphere != null)
            targetSphere.SetActive(false);

        // Play FX and sound
        if (explosionEffect != null)
            explosionEffect.Play();

        if (explosionAudio != null)
            explosionAudio.Play();
    }
}