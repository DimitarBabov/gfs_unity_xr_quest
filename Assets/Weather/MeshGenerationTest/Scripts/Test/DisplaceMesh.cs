using UnityEngine;

public class DisplaceMesh : MonoBehaviour
{
    public Texture2D heightMap; // Grayscale PNG texture
    public float displacementAmount = 10.0f; // Scale factor for displacement

    private Mesh mesh;

    void Start()
    {
        // Get the width and height of the texture to create a matching mesh grid
        int textureWidth = heightMap.width;
        int textureHeight = heightMap.height;

        // Create a flat grid mesh based on the texture width and height
        mesh = new Mesh();
        CreateGridMesh(textureWidth, textureHeight);

        // Apply heightmap displacement using the texture data
        ApplyHeightMapDisplacement(textureWidth, textureHeight);

        // Assign the mesh to the MeshFilter component
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void CreateGridMesh(int width, int height)
    {
        Vector3[] vertices = new Vector3[width * height];
        Vector2[] uvs = new Vector2[width * height];  // UV coordinates
        int[] triangles = new int[(width - 1) * (height - 1) * 6];

        // Create a flat grid of vertices and UVs
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                int index = i * width + j;
                vertices[index] = new Vector3(j, 0, i); // Flat grid in XZ plane

                // Assign UVs based on vertex positions, normalized to [0, 1]
                uvs[index] = new Vector2((float)j / (width - 1), (float)i / (height - 1));
            }
        }

        // Create triangles to connect the vertices
        int triIndex = 0;
        for (int i = 0; i < height - 1; i++)
        {
            for (int j = 0; j < width - 1; j++)
            {
                int current = i * width + j;

                triangles[triIndex] = current;
                triangles[triIndex + 1] = current + width;
                triangles[triIndex + 2] = current + 1;

                triangles[triIndex + 3] = current + 1;
                triangles[triIndex + 4] = current + width;
                triangles[triIndex + 5] = current + width + 1;

                triIndex += 6;
            }
        }

        // Assign vertices, triangles, and UVs to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs; // Assign UVs to the mesh
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    void ApplyHeightMapDisplacement(int textureWidth, int textureHeight)
    {
        // Get the pixel data from the heightmap texture
        Color[] pixelData = heightMap.GetPixels();

        // Get the vertices of the mesh
        Vector3[] vertices = mesh.vertices;

        // Loop through each vertex and apply height based on the texture data
        for (int i = 0; i < textureHeight; i++)
        {
            for (int j = 0; j < textureWidth; j++)
            {
                int vertexIndex = i * textureWidth + j;

                // Get the corresponding pixel index in the texture
                int pixelIndex = i * textureWidth + j;

                // Get the height value from the grayscale texture (use red channel)
                float heightValue = pixelData[pixelIndex].r;

                // Apply the height displacement to the Y coordinate of the vertex
                vertices[vertexIndex].y = heightValue * displacementAmount;
            }
        }

        // Update the mesh vertices
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
