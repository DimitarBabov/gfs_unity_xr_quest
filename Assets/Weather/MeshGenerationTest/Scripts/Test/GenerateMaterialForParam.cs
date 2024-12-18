using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;  // For TextMeshPro

public class GenerateMaterialForParam : MonoBehaviour
{
    public string serverUrl = "http://127.0.0.1:5000/create_png";  // Flask server URL for PNG creation
    public string param = "HGT";  // Example parameter (e.g., HGT for geopotential height)
    public string level = "500_mb";  // Level as a string (e.g., 500_mb)
    public float minLat = 21.1381f;  // Min latitude
    public float maxLat = 64.750f;  // Max latitude
    public float minLon = 237.2805f;  // Min longitude
    public float maxLon = 312.25f;  // Max longitude
    public Shader targetShader;  // Shader to use for dynamically creating a material
    public Material targetMaterial;  // Reference to the newly generated material (starts as null)
  
    private string uniqueId;

    private void Start()
    {
        // Start the process to generate the material for the parameter
        StartCoroutine(CreatePngAndGenerateMaterial());
    }

    private IEnumerator CreatePngAndGenerateMaterial()
    {
        // Construct the request URL for PNG creation
        string requestUrl = $"{serverUrl}?param={param}&level={level}&min_lat={minLat}&max_lat={maxLat}&min_lon={minLon}&max_lon={maxLon}";

        // Send the request to create the PNG
        using (UnityWebRequest webRequest = UnityWebRequest.Get(requestUrl))
        {
            yield return webRequest.SendWebRequest();

            // Check for errors
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error creating PNG: {webRequest.error}");
            }
            else
            {
                // Parse the JSON response
                string jsonResponse = webRequest.downloadHandler.text;
                CreatePngResponse response = JsonUtility.FromJson<CreatePngResponse>(jsonResponse);

                // Store the unique ID from the response
                uniqueId = response.unique_id;

                // Start downloading the PNG and Meta files
                StartCoroutine(DownloadPng(response.png_download_url));
                StartCoroutine(DownloadMeta(response.meta_download_url));
            }
        }
    }

    private IEnumerator DownloadPng(string pngUrl)
    {
        // Download the PNG file
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(pngUrl))
        {
            yield return webRequest.SendWebRequest();

            // Check for errors
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error downloading PNG: {webRequest.error}");
            }
            else
            {
                // Get the texture from the response
                Texture2D downloadedTexture = ((DownloadHandlerTexture)webRequest.downloadHandler).texture;

                // Dynamically create a material using the assigned shader
                if (targetShader != null)
                {
                    // Create a new material using the shader
                    targetMaterial = new Material(targetShader);
                    targetMaterial.mainTexture = downloadedTexture;  // Assign the texture to the material

                    Debug.Log("Material successfully created and applied with the downloaded texture!");
                }
                else
                {
                    Debug.LogError("Target shader is not assigned.");
                }
            }
        }
    }

    private IEnumerator DownloadMeta(string metaUrl)
    {
        // Download the meta file (as plain text)
        using (UnityWebRequest webRequest = UnityWebRequest.Get(metaUrl))
        {
            yield return webRequest.SendWebRequest();

            // Check for errors
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"Error downloading meta file: {webRequest.error}");
            }
            else
            {
                // Get the content of the meta file
                string metaContent = webRequest.downloadHandler.text;

                
            }
        }
    }

    // Class to deserialize the JSON response from the create_png route
    [System.Serializable]
    public class CreatePngResponse
    {
        public string unique_id;
        public string png_download_url;
        public string meta_download_url;
    }
}
