using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A script on all reflection probes that sets the setting for them at the start of the game
[RequireComponent(typeof(ReflectionProbe))]
public class ReflectionProbeSetupScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        WorldManager wm = FindObjectOfType<WorldManager>();
        ReflectionProbe rp = GetComponent<ReflectionProbe>();
        rp.resolution = wm.ReflectionProbesResolution;
        rp.refreshMode = wm.ReflectionProbesRefreshMode;
    }
}
