using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.None)]
public class ProceduralSurfaceUdon : UdonSharpBehaviour
{
    [Header("Surface Resolution & Scale")]
    public int resolution = 100;      // number of vertices per axis
    public float size = 20f;          // world width/depth
    public float heightScale = 3f;    // vertical exaggeration

    [Header("Function Domain (θ1, θ2)")]
    public float domainMin = -2f;
    public float domainMax = 2f;

    private Mesh mesh;

    void Start()
    {
        GenerateSurface();
    }

    void GenerateSurface()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[resolution * resolution];
        int[] triangles = new int[(resolution - 1) * (resolution - 1) * 6];

        // --- Generate vertices from J(θ1, θ2) ---
        for (int z = 0, i = 0; z < resolution; z++)
        {
            for (int x = 0; x < resolution; x++, i++)
            {
                float θ1 = Mathf.Lerp(domainMin, domainMax, (float)x / (resolution - 1));
                float θ2 = Mathf.Lerp(domainMin, domainMax, (float)z / (resolution - 1));

                // J(θ1, θ2) = θ1² + θ2² + 0.2 sin(3θ1) sin(3θ2)
                float J = Mathf.Abs(
    0.6f * Mathf.Sin(2f * θ1) * Mathf.Sin(1.5f * θ2)
  + 0.3f * Mathf.Sin(4.2f * θ1 + 1.3f * θ2)
  + 0.15f * Mathf.Sin(7f * θ1 - 3f * θ2)
) - 0.1f * (θ1 * θ1 + θ2 * θ2);


                // Map to world coordinates
                float wx = (x / (float)(resolution - 1) - 0.5f) * size;
                float wz = (z / (float)(resolution - 1) - 0.5f) * size;
                float wy = J * heightScale;

                vertices[i] = new Vector3(wx, wy, wz);
            }
        }

        // --- Triangles ---
        int t = 0;
        for (int z = 0; z < resolution - 1; z++)
        {
            for (int x = 0; x < resolution - 1; x++)
            {
                int i = z * resolution + x;
                triangles[t++] = i;
                triangles[t++] = i + resolution;
                triangles[t++] = i + 1;
                triangles[t++] = i + 1;
                triangles[t++] = i + resolution;
                triangles[t++] = i + resolution + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshCollider collider = GetComponent<MeshCollider>();
        if (collider != null)
        {
            collider.sharedMesh = mesh;
        }

    }
}
