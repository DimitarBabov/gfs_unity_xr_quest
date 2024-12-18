using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GlobalSimulatorWip : MonoBehaviour
{
   
    public bool useLocalServer = false;
    public List<TextureController> textureControllers; // List of MeshController references
    public float intervalTime = 1f; // Interval time in seconds to switch textures

    public GameObject[] TMPs=new GameObject[2];
    public GameObject[] HGTs = new GameObject[2];
    public GameObject[] ABSVs = new GameObject[2];
    public GameObject[] CRAINs = new GameObject[2];

    public int currentTextureIndex = 0;
    public int currentParameterIndex = 0;
    private bool isPlaying = false;
    private Coroutine playCoroutine;

    public bool isFlat = true;
    
    
    public void setTMP()
    {
        TMPs[0].gameObject.SetActive(!isFlat);
        TMPs[1].gameObject.SetActive(isFlat);
        for(int i=0;i<2;i++)
        {
            HGTs[i].SetActive(false);
            ABSVs[i].SetActive(false);
            CRAINs[i].SetActive(false);
        }
        ShowTextureToAll(currentTextureIndex);
    }
    public void setHGT()
    {
        HGTs[0].gameObject.SetActive(!isFlat);
        HGTs[1].gameObject.SetActive(isFlat);
        for (int i = 0; i < 2; i++)
        {
            TMPs[i].SetActive(false);
            ABSVs[i].SetActive(false);
            CRAINs[i].SetActive(false);
        }
        ShowTextureToAll(currentTextureIndex);
    }

    public void setCRAIN()
    {
        CRAINs[0].gameObject.SetActive(!isFlat);
        CRAINs[1].gameObject.SetActive(isFlat);
        for (int i = 0; i < 2; i++)
        {
            TMPs[i].SetActive(false);
            ABSVs[i].SetActive(false);
            HGTs[i].SetActive(false);
        }
        ShowTextureToAll(currentTextureIndex);
    }

    public void setABSV()
    {
        ABSVs[0].gameObject.SetActive(!isFlat);
        ABSVs[1].gameObject.SetActive(isFlat);
        for (int i = 0; i < 2; i++)
        {
            TMPs[i].SetActive(false);
            CRAINs[i].SetActive(false);
            HGTs[i].SetActive(false);
        }
        ShowTextureToAll(currentTextureIndex);
    }
    public void ToggleCurvedFlat()
    {
        isFlat = !isFlat;
        if (HGTs[0].activeSelf)
        {
            HGTs[1].SetActive(true);
            HGTs[0].SetActive(false);
        }
        else if (HGTs[1].activeSelf)
        {
            HGTs[0].SetActive(true);
            HGTs[1].SetActive(false);
        }
        else if (TMPs[0].activeSelf)
        {
            TMPs[1].SetActive(true);
            TMPs[0].SetActive(false);
        }
        else if (TMPs[1].activeSelf)
        {
            TMPs[0].SetActive(true);
            TMPs[1].SetActive(false);
        }
        else if (CRAINs[0].activeSelf)
        {
            CRAINs[1].SetActive(true);
            CRAINs[0].SetActive(false);
        }
        else if (CRAINs[1].activeSelf)
        {
            CRAINs[0].SetActive(true);
            CRAINs[1].SetActive(false);
        }
        else if (ABSVs[0].activeSelf)
        {
            ABSVs[1].SetActive(true);
            ABSVs[0].SetActive(false);
        }
        else if (ABSVs[1].activeSelf)
        {
            ABSVs[0].SetActive(true);
            ABSVs[1].SetActive(false);
        }
        ShowTextureToAll(currentTextureIndex);
    }
    private void Awake()
    {
        
    }
    IEnumerator Start()
    {
        while (!XrAssetsContainer.Instance.IsInitialized())
        {
            yield return null;
        }
        setHGT();
        Pause();
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
        ShowTextureToAll(currentTextureIndex);
    }

    // Forward function to move to the next texture
    public void Forward()
    {
        int totalNumOfTextures = GetTotalNumOfTextures();
        if (totalNumOfTextures > 0)
        {
            currentTextureIndex = (currentTextureIndex + 1) % totalNumOfTextures;
            ShowTextureToAll(currentTextureIndex);
        }
    }

    // Backward function to move to the previous texture
    public void Backward()
    {
        int totalNumOfTextures = GetTotalNumOfTextures();
        if (totalNumOfTextures > 0)
        {
            currentTextureIndex--;
            if (currentTextureIndex < 0)
            {
                currentTextureIndex = totalNumOfTextures - 1;
            }
            ShowTextureToAll(currentTextureIndex);
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

    // Show the texture to all enabled MeshControllers
    private void ShowTextureToAll(int textureIndex)
    {
        foreach (var controller in textureControllers)
        {
            if (controller != null && controller.isActiveAndEnabled)
            {
                controller.ShowTexture(textureIndex);                
}
        }
    }

    // Get the total number of textures from the first active MeshController
    private int GetTotalNumOfTextures()
    {
        foreach (var controller in textureControllers)
        {
            if (controller != null && controller.isActiveAndEnabled )
            {
                return controller.textures.Length;
            }
        }
        return 0;
    }

   
}