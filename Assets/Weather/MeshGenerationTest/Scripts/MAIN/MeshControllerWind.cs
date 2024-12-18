using System.Collections;
using UnityEngine;
using TMPro;  // For TextMeshPro

public class MeshControllerWind : MonoBehaviour
{
    public TextureController textureController;  // Reference to the TextureController script
    public float switchInterval = 1f;  // Time interval to switch textures, default 1 second
    public float heightScale = 10.0f;  // Public float to control the _HeightScale in the shader
   
    public Material targetMaterial;  // Shader to use for dynamically applying textures
    public Material mapMaterial;  // Material for the simplified plane
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private int currentTextureIndex = 0;  // To keep track of the current texture
    private int numVertices;  // Number of vertices based on the texture resolution

    bool isAlreadyStarted = false;

    void Start()
    {
        // Get the MeshRenderer component on the object
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
       
        // Ensure it uses the shared material (not an instance)
        if (meshRenderer != null && targetMaterial != null)
        {
            meshRenderer.sharedMaterial = targetMaterial;
        }
        // Wait until TextureController has textures ready
        StartCoroutine(WaitForTexturesAndCreateMesh());
        isAlreadyStarted = true;
    }
    private void OnEnable()
    {
        if (isAlreadyStarted)
        {
            //StartCoroutine(CycleTextures());
        }
    }
    private IEnumerator WaitForTexturesAndCreateMesh()
    {
        // Wait until TextureController has downloaded textures
        while (textureController.textures == null || textureController.textures.Length == 0)
        {
            yield return null;  // Wait for the next frame
        }

        // Assume all textures have the same resolution
        if (textureController.textures[0] != null)
        {
            Texture2D texture = textureController.textures[0];
            CreatePlaneMeshFromTexture(texture);
            CreateSimplifiedPlaneMesh();
        }

        // Start cycling through textures
        //StartCoroutine(CycleTextures());
    }

    private void CreatePlaneMeshFromTexture(Texture2D texture)
    {
        int width = texture.width;
        int height = texture.height;

        // Calculate number of vertices (equal to the number of pixels)
        numVertices = width * height;

        // Create a new GameObject with a MeshFilter and MeshRenderer
        GameObject meshObject = new GameObject();
        meshObject.name = transform.parent.name + " Mesh";
        meshObject.transform.SetParent(this.transform);  // Attach to the same GameObject
        meshObject.transform.localPosition = Vector3.zero;  // Align to parent's local position
        meshObject.transform.localRotation = Quaternion.identity;  // Align rotation
        meshObject.transform.localScale = Vector3.one;  // Set scale to one to work in local space

        // Add MeshFilter and MeshRenderer components
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshRenderer = meshObject.AddComponent<MeshRenderer>();

        // Create the mesh
        Mesh mesh = new Mesh();
        Vector3[] vertices = new Vector3[numVertices];
        Vector2[] uv = new Vector2[numVertices];
        int[] triangles = new int[(width - 1) * (height - 1) * 6];

        // Generate vertices and UVs based on the texture size
        float halfWidth = width / 2.0f;
        float halfHeight = height / 2.0f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int index = y * width + x;
                vertices[index] = new Vector3(
                    (x - halfWidth) * transform.lossyScale.x, // Scale by parent's X scale
                    0,
                    (y - halfHeight) * transform.lossyScale.z // Scale by parent's Z scale
                );  // Center the mesh in local space and apply scaling
                uv[index] = new Vector2((float)x / (width - 1), (float)y / (height - 1));  // UV mapping
            }
        }

        // Generate triangles for the plane mesh
        int t = 0;
        for (int y = 0; y < height - 1; y++)
        {
            for (int x = 0; x < width - 1; x++)
            {
                int index = y * width + x;

                // First triangle
                triangles[t++] = index;
                triangles[t++] = index + width;
                triangles[t++] = index + width + 1;

                // Second triangle
                triangles[t++] = index;
                triangles[t++] = index + width + 1;
                triangles[t++] = index + 1;
            }
        }

        // Assign vertices, UVs, and triangles to the mesh
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Set the mesh to the MeshFilter
        meshFilter.mesh = mesh;

        // Apply the first material with the texture
        meshRenderer.sharedMaterial = targetMaterial;
        meshRenderer.sharedMaterial.mainTexture = texture;  // Start with the first texture
        

        // Set the initial _HeightScale value in the shader
        meshRenderer.sharedMaterial.SetFloat("_HeightScale", heightScale * transform.lossyScale.x);

        Debug.Log("Mesh generated from texture with " + width + "x" + height + " vertices.");
    }

    private void CreateSimplifiedPlaneMesh()
    {
        // Create a new GameObject with a simplified plane mesh
        GameObject simplifiedMeshObject = new GameObject("Map Mesh");
        simplifiedMeshObject.transform.SetParent(this.transform);  // Attach to the same GameObject
        simplifiedMeshObject.transform.localPosition = Vector3.zero;  // Align to parent's local position
        simplifiedMeshObject.transform.localRotation = Quaternion.identity;  // Align rotation
        simplifiedMeshObject.transform.localScale = Vector3.one;  // Set scale to one to work in local space

        // Add MeshFilter and MeshRenderer components
        MeshFilter simplifiedMeshFilter = simplifiedMeshObject.AddComponent<MeshFilter>();
        MeshRenderer simplifiedMeshRenderer = simplifiedMeshObject.AddComponent<MeshRenderer>();

        // Create the simplified plane mesh
        Mesh simplifiedMesh = new Mesh();
        Vector3[] vertices = new Vector3[4];
        Vector2[] uv = new Vector2[4];
        int[] triangles = new int[6];

        float halfWidth = textureController.textures[0].width / 2.0f * transform.lossyScale.x;
        float halfHeight = textureController.textures[0].height / 2.0f * transform.lossyScale.z;

        // Define vertices for the simplified plane
        vertices[0] = new Vector3(-halfWidth, 0, -halfHeight);
        vertices[1] = new Vector3(halfWidth, 0, -halfHeight);
        vertices[2] = new Vector3(-halfWidth, 0, halfHeight);
        vertices[3] = new Vector3(halfWidth, 0, halfHeight);

        // Define UVs
        uv[0] = new Vector2(0, 0);
        uv[1] = new Vector2(1, 0);
        uv[2] = new Vector2(0, 1);
        uv[3] = new Vector2(1, 1);

        // Define triangles
        triangles[0] = 0;
        triangles[1] = 2;
        triangles[2] = 1;

        triangles[3] = 1;
        triangles[4] = 2;
        triangles[5] = 3;

        // Assign vertices, UVs, and triangles to the mesh
        simplifiedMesh.vertices = vertices;
        simplifiedMesh.uv = uv;
        simplifiedMesh.triangles = triangles;
        simplifiedMesh.RecalculateNormals();

        // Set the mesh to the MeshFilter
        simplifiedMeshFilter.mesh = simplifiedMesh;

        // Apply the map material
        simplifiedMeshRenderer.sharedMaterial = mapMaterial;

        Debug.Log("Simplified plane mesh created with 4 vertices.");
    }

}
