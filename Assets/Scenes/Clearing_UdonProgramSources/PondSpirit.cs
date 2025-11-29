using UdonSharp;
using UnityEngine;

public class PondSpirit : UdonSharpBehaviour
{
    [Range(0, 1)] public float peace = 0.6f;
    public VectorFieldManager field;
    public Renderer spiritRenderer;
    public AudioSource ambience;

    void Update()
    {
        field.intensity = Mathf.Lerp(0.4f, 2.5f, 1f - peace);

        Color c = Color.Lerp(
            new Color(0.6f, 1f, 1f),
            new Color(1f, 0.3f, 0.3f),
            1f - peace
        );
        spiritRenderer.material.SetColor("_EmissionColor", c * 2f);

        if (ambience != null)
            ambience.pitch = Mathf.Lerp(1.05f, 0.85f, 1f - peace);
    }
}
