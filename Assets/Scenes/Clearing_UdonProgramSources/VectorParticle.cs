using UdonSharp;
using UnityEngine;

public class VectorParticle : UdonSharpBehaviour
{
    public FieldEquationParser field;
    public VectorFieldManager Manager;
    public float baseSpeed = 1f;

    void Update()
    {
        if (field == null || Manager == null) return;

        Vector3 p = transform.position;
        Vector3 F = field.Evaluate(p);

        float s = Manager.intensity * baseSpeed;
        Vector3 v = F.normalized * s * Time.deltaTime;
        transform.position += v;

        // respawn if out of bounds
        Vector3 b = Manager.bounds;
        if (Mathf.Abs(transform.position.x) > b.x ||
            Mathf.Abs(transform.position.y) > b.y ||
            Mathf.Abs(transform.position.z) > b.z)
        {
            transform.position = new Vector3(
                Random.Range(-b.x, b.x),
                Random.Range(-b.y, b.y),
                Random.Range(-b.z, b.z)
            );
        }
    }
}
