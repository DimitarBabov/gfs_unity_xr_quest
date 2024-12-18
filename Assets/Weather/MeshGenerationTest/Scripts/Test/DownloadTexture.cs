using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;  // For TextMeshPro


public class DownloadPngAndMeta : MonoBehaviour
{
    public string serverUrl = "http://127.0.0.1:5000/create_png";  // Flask server URL for PNG creation
    public string param = "HGT";  // Example parameter
    public int level = 500;  // Example level in mb
    public float minLat = 21.1381f;  // Min latitude
    public float maxLat = 64.750f;  // Max latitude
    public float minLon = 237.2805f;  // Min longitude
    public float maxLon = 312.25f;  // Max longitude
    public Material targetMaterial;  // Material to which the texture will be applied
   
    private string uniqueId;

    
    private void Start()
    {
        // Start the PNG creation process
        StartCoroutine(CreatePngAndDownload());
    }

    private IEnumerator CreatePngAndDownload()
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

                // Apply the texture to the material
                if (targetMaterial != null)
                {
                    targetMaterial.mainTexture = downloadedTexture;
                    Debug.Log("Texture successfully applied to material!");
                }
                else
                {
                    Debug.LogError("Target material is not assigned.");
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

                // Assign the content to the TextMeshProUGUI field
               
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
