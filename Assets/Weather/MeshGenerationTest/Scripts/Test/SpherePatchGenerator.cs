using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SpherePatchGenerator : MonoBehaviour
{
    public float minLat = -10f;
    public float maxLat = 10f;
    public float minLon = -10f;
    public float maxLon = 10f;
    public float radius = 1f;
    public int latResolution = 10;
    public int lonResolution = 10;
    public bool useTextureSize = false;
    public TextureController textureController = null;
    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    public Material targetMaterial;

    private Mesh mesh;
    private bool isMeshReady = false;

    void Start()
    {
        textureController = GetComponentInParent<TextureController>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null && targetMaterial != null)
        {
            meshRenderer.sharedMaterial = targetMaterial;
        }

        if (useTextureSize)
            StartCoroutine(WaitForTexturesAndCreateMesh());
        else
            GenerateSpherePatch();
    }

    private IEnumerator WaitForTexturesAndCreateMesh()
    {
        Debug.Log("Waiting for textures...");
        while (textureController.textures == null || textureController.textures.Length == 0)
        {
            yield return null;
        }

        if (textureController.textures[0] != null)
        {
            Texture2D texture = textureController.textures[0];
            meshRenderer.sharedMaterial.mainTexture = texture;
        }

        Debug.Log("Waiting for overlay textures...");
        // Only wait if overlayTextures are expected, but do not block execution if none are set
        if (textureController.overlayTextures != null && textureController.overlayTextures.Length > 0)
        {
            while (textureController.overlayTextures == null || textureController.overlayTextures.Length == 0)
            {
                yield return null;
            }

            // Set the overlay texture if available
            if (textureController.overlayTextures[0] != null)
            {
                meshRenderer.sharedMaterial.SetTexture("_OverlayTex", textureController.overlayTextures[0]);
            }
        }

        Debug.Log("Textures ready, generating sphere patch.");
        GenerateSpherePatch();
        isMeshReady = true;
    }

    void GenerateSpherePatch()
    {
        if (useTextureSize && meshRenderer.sharedMaterial != null && meshRenderer.sharedMaterial.mainTexture != null)
        {
            Texture texture = meshRenderer.sharedMaterial.mainTexture;
            latResolution = texture.height - 1;
            lonResolution = texture.width - 1;
        }

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        Vector3[] vertices = new Vector3[(latResolution + 1) * (lonResolution + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[latResolution * lonResolution * 6];

        float minLatRad = Mathf.Deg2Rad * minLat;
        float maxLatRad = Mathf.Deg2Rad * maxLat;
        float minLonRad = Mathf.Deg2Rad * minLon;
        float maxLonRad = Mathf.Deg2Rad * maxLon;

        float latStep = (maxLatRad - minLatRad) / latResolution;
        float lonStep = (maxLonRad - minLonRad) / lonResolution;

        for (int lat = 0; lat <= latResolution; lat++)
        {
            float theta = minLatRad + lat * latStep;
            for (int lon = 0; lon <= lonResolution; lon++)
            {
                float phi = minLonRad + lon * lonStep;

                float x = radius * Mathf.Cos(theta) * Mathf.Cos(phi);
                float y = radius * Mathf.Sin(theta);
                float z = radius * Mathf.Cos(theta) * Mathf.Sin(phi);

                int index = lat * (lonResolution + 1) + lon;
                vertices[index] = new Vector3(x, y, z);
                uv[index] = new Vector2((float)lon / lonResolution, (float)lat / latResolution);
            }
        }

        int triIndex = 0;
        for (int lat = 0; lat < latResolution; lat++)
        {
            for (int lon = 0; lon < lonResolution; lon++)
            {
                int current = lat * (lonResolution + 1) + lon;
                int next = current + lonResolution + 1;

                triangles[triIndex++] = current;
                triangles[triIndex++] = next;
                triangles[triIndex++] = current + 1;

                triangles[triIndex++] = next;
                triangles[triIndex++] = next + 1;
                triangles[triIndex++] = current + 1;
            }
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uv;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }
}
