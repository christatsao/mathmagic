using UdonSharp;
using UnityEngine;

public class FieldEquationParser : UdonSharpBehaviour
{
    public int fieldType = 0;  // 0=rotation, 1=source, 2=sink, 3=gradient, 4=custom
    public float a = 1, b = 1, c = 1;

    public Vector3 Evaluate(Vector3 P)
    {
        float x = P.x, y = P.y, z = P.z;

        switch (fieldType)
        {
            case 0: return new Vector3(-y, x, 0);                 // rotation
            case 1: return new Vector3(x, y, z);                   // source
            case 2: return new Vector3(-x, -y, -z);                // sink
            case 3: return new Vector3(2 * a * x, 2 * b * y, 2 * c * z); // grad φ
            case 4: return new Vector3(a * x - b * y, b * x + a * y, c * z); // param mix
        }
        return Vector3.zero;
    }
}
