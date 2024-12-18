using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class RecenterOnStart : MonoBehaviour
{
    void Start ()
    {
        XRInputSubsystem subsystem = null;
        var subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subsystems);

        if (subsystems.Count > 0)
        {
            subsystem = subsystems[0];
        }

        if (subsystem != null)
        {
            subsystem.TryRecenter();
        }
    }

    private void OnEnable()
    {
        XRInputSubsystem subsystem = null;
        var subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subsystems);

        if (subsystems.Count > 0)
        {
            subsystem = subsystems[0];
        }

        if (subsystem != null)
        {
            subsystem.TryRecenter();
        }
    }
}
