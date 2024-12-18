using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class ARSessionInitializer : MonoBehaviour
{
    public GameObject mainGeo;  // The main scene or geometry you want to activate after initialization
   //public RecenterOnStart recenterOnStart;
    void Start()
    {
        
        // Deactivate main geometry at the start
        mainGeo.SetActive(false);
        //recenterOnStart.enabled=true;

        // Start monitoring the AR session state
        StartCoroutine(CheckARSessionInitialization());
    }

    private System.Collections.IEnumerator CheckARSessionInitialization()
    {
        // Wait until the AR session is initialized
        while (ARSession.state != ARSessionState.SessionTracking)
        {
            yield return null;  // Wait for the next frame
        }

        // AR session is now initialized, activate the main geometry
        mainGeo.SetActive(true);
        Debug.Log("AR session initialized. Main geometry activated.");
    }
}
