using UnityEngine;
using System.Collections;

public class TextureSimulator : MonoBehaviour
{
    public TextureController textureController; // Reference to TextureController
    public float intervalTime = 1f; // Interval time in seconds to switch textures

    private int currentTextureIndex = 0;
    private bool isPlaying = false;
    private Coroutine playCoroutine;
    public Renderer objectRenderer;

    void Start()
    {
       // objectRenderer = GetComponent<Renderer>();
       // textureController = GetComponentInParent<TextureController>();
      // if(isPlaying)
            Play();
    }

    // Play function to start cycling through textures
    public void Play()
    {
        if (!isPlaying)
        {
            isPlaying = true;
            playCoroutine = StartCoroutine(CycleTextures());
        }
    }

    // Pause function to stop cycling through textures without resetting the current index
    public void Pause()
    {
        if (isPlaying)
        {
            isPlaying = false;
            StopCoroutine(playCoroutine);
        }
    }

    // Stop function to stop cycling and reset to the first texture
    public void Stop()
    {
        if (isPlaying)
        {
            isPlaying = false;
            StopCoroutine(playCoroutine);
        }
        currentTextureIndex = 0;
        UpdateMaterialTexture();
    }

    // Forward function to move to the next texture
    public void Forward()
    {
        if (textureController.textures.Length > 0)
        {
            currentTextureIndex = (currentTextureIndex + 1) % textureController.textures.Length;
            UpdateMaterialTexture();
        }
    }

    // Back function to move to the previous texture
    public void Back()
    {
        if (textureController.textures.Length > 0)
        {
            currentTextureIndex = (currentTextureIndex - 1 + textureController.textures.Length) % textureController.textures.Length;
            UpdateMaterialTexture();
        }
    }

    // Coroutine to cycle through textures at the given interval time
    private IEnumerator CycleTextures()
    {
        while (isPlaying)
        {
            yield return new WaitForSeconds(intervalTime);
            Forward();
        }
    }

    // Update the material's shared texture
    private void UpdateMaterialTexture()
    {
        if (objectRenderer != null && textureController.textures.Length > 0)
        {
            objectRenderer.sharedMaterial.mainTexture = textureController.textures[currentTextureIndex];
        }
    }
}