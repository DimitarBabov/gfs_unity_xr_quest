using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlaneMeshGenerator : MonoBehaviour
{
    public Texture2D heightMap; // The heightmap texture
    public float heightScale = 10f; // Scale for height displacement
    private Mesh mesh;

    void Start()
    {
        if (heightMap != null)
        {
            // Get the texture's dimensions
            int xSize = heightMap.width;
            int ySize = heightMap.height;

            GenerateMesh(xSize, ySize);
        }
        else
        {
            Debug.LogError("Heightmap texture is missing!");
        }
    }

    void GenerateMesh(int xSize, int ySize)
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        int[] triangles = new int[xSize * ySize * 6];
        Vector2[] uv = new Vector2[vertices.Length];

        // Create vertices and UV coordinates
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, 0, y); // Flat plane, Y (or Z) as zero initially
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize); // UV mapping
            }
        }

        // Create triangles
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 2] = vi + 1;
                triangles[ti + 3] = vi + 1;
                triangles[ti + 4] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;
        mesh.RecalculateNormals();
    }
}
