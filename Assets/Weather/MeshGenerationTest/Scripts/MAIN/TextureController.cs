using System.Collections;

using UnityEngine;

using System.Linq;
using TMPro;
using System.Text.RegularExpressions;
using System;


public class TextureController : MonoBehaviour
{
    // Public variables
    public string param = "HGT";  // Example parameter (e.g., HGT for geopotential height)
    public string level = "500_mb";  // Level as a string (e.g., 500_mb)
    
    // Public variables
    public Texture2D[] textures;  //NOT USED YET Array to hold the sorted textures
    public string[] textureInfo;  //// Array to hold the sorted meta content for each texture

    public Texture2D[] overlayTextures;  // Array to hold the sorted textures
    public TextMeshProUGUI metaTextDisplay;

    public string overlayParam = "ISOBARICHGT";
    public string overlayLevel = "500_mb";

    string[] displayInfo;

     
    public void ShowTexture(int textureIndex)
    {
        Renderer renderer = GetComponentInChildren<Renderer>();

        // Set the main texture
        if (textures != null && textureIndex < textures.Length)
        {
            renderer.sharedMaterial.mainTexture = textures[textureIndex];
        }

        // Set the overlay texture
        if (overlayTextures != null && textureIndex < overlayTextures.Length)
        {
            renderer.sharedMaterial.SetTexture("_OverlayTex", overlayTextures[textureIndex]);
        }

        
        // Update the meta text display if available
        if (metaTextDisplay != null && displayInfo != null && displayInfo.Count() > 0)
        {
            metaTextDisplay.text = displayInfo[textureIndex];
        }
    }


    IEnumerator Start()
    {
       
        while (!XrAssetsContainer.Instance.IsInitialized())
        {
            yield return null;
        }
        
        metaTextDisplay = XrAssetsContainer.Instance.metaTextDisplay;
        
        GlobalSimulatorWip globalSimulator = GetComponentInParent<GlobalSimulatorWip>();

        if (textures != null && textures.Length > 0)
        {
            // Extract textureInfo from the texture name if textures are already in the Inspector
            ExtractDisplayInfoFromName();
        }else
        {
            if(XrAssetsContainer.Instance.texturesByParam.ContainsKey(param + '-' + level))     
                textures = XrAssetsContainer.Instance.texturesByParam[param +'-'+level].ToArray();

            if (textures != null && textures.Length > 0)
                ExtractDisplayInfoFromName();
            

            if (XrAssetsContainer.Instance.textureInfoByParam.ContainsKey(param + '-' + level))
                textureInfo = XrAssetsContainer.Instance.textureInfoByParam[param + '-' + level].ToArray();

            if (XrAssetsContainer.Instance.texturesByParam.ContainsKey(overlayParam + '-' + overlayLevel))
                overlayTextures = XrAssetsContainer.Instance.texturesByParam[overlayParam + '-' + overlayLevel].ToArray();
            
        }
       
    }

    private void ExtractDisplayInfoFromName()
    {
        displayInfo = new string[textures.Length];

        for (int i = 0; i < textures.Length; i++)
        {
            if (textures[i] != null)
            {
                string name = textures[i].name;
                displayInfo[i] = GenerateDisplayInfoFromName(name);
                Debug.Log($"Texture info extracted for {name}: {displayInfo[i]}");
            }
            else
            {
                Debug.LogWarning($"Texture at index {i} is null.");
            }
        }
    }

   

    private string GenerateDisplayInfoFromName(string textureName)
    {
        // Log the texture name to help diagnose the issue
        Debug.Log($"Parsing texture name: {textureName}");

        // Find the position of "gfs" to correctly split the param and level
        int gfsIndex = textureName.IndexOf("_gfs");
        if (gfsIndex == -1)
        {
            return "Invalid texture name format: 'gfs' not found.";
        }

        // Extract the parameter (before the first "_") and level (between first "_" and "_gfs")
        string param = textureName.Substring(0, textureName.IndexOf('_'));  // TMP, HGT, etc.
        string level = textureName.Substring(textureName.IndexOf('_') + 1, gfsIndex - textureName.IndexOf('_') - 1);  // Everything between param and gfs

        // Split the remaining part (after "_gfs") to get date, cycle, and hour
        string[] remainingParts = textureName.Substring(gfsIndex + 5).Split('_');

        if (remainingParts.Length < 3)
        {
            return "Invalid texture name format after 'gfs'.";
        }

        // Extract the date, cycle, and hour
        string date = remainingParts[0];  // Expected format is 20241008 (YYYYMMDD)
        string cycle = remainingParts[1]; // Expected format is 12 (HH)
        string hour = remainingParts[2].Substring(1); // f096 -> 096

        // Log extracted values for date, cycle, and hour for debugging
        Debug.Log($"Extracted date: {date}, cycle: {cycle}, hour: {hour}");

        // Validate that the date string is 8 characters long
        if (date.Length != 8)
        {
            return $"Invalid date format in texture name: {textureName}";
        }

        // Validate that the cycle and hour are not empty and have the expected length
        if (cycle.Length != 2 || hour.Length != 3)
        {
            return $"Invalid cycle or hour format in texture name: {textureName}";
        }

        // Format the textureInfo string
        return $"Parameter: {param}\nLevel: {level}\nDate: {date.Substring(0, 4)}-{date.Substring(4, 2)}-{date.Substring(6, 2)}\nCycle: {cycle}\nHour: {hour}";
    }

}