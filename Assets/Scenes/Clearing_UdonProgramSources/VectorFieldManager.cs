using UdonSharp;
using UnityEngine;

public class VectorFieldManager : UdonSharpBehaviour
{
    public FieldEquationParser parser;
    public VectorParticle[] particles;
    public float intensity = 1f;  // driven by PondSpirit
    public float speedScale = 1f;
    public Vector3 bounds = new Vector3(6, 3, 6);

    void Start()
    {
        foreach (var p in particles)
        {
            p.Manager = this;
            p.field = parser;
        }
    }
}
