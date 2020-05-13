using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
using MLAPI;
//Handles communications between multiple scripts and classes (Multiplayer included)
public class WorldManager : NetworkedBehaviour
{
    public bool CalculatePathesAtStart = true;//Should we calculate bot pathfinding at the start of the game ?
    private AStarPathfinder pathfinder;//The pathfinder used for bot path calculations

    //Reflection probes
    private int ReflectionProbesResolution;
    private ReflectionProbeRefreshMode ReflectionProbesRefreshMode;
    //Cameras and postprocessing
    private GameConfigHandlerScript.CameraConfig CameraConfig;
    private GameConfigHandlerScript.VolumeConfig VolumeConfig;
    // Start is called before the first frame update
    void Start()
    {

    }
    public override void NetworkStart()
    {
        base.NetworkStart();   
        //Run the world update only on the server  
        if (CalculatePathesAtStart && IsServer)
        {
            pathfinder = FindObjectOfType<AStarPathfinder>();//Init base terrain
            pathfinder.MakeTerrainGrid();
            WorldUpdate();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    #region Configurations
    //Sets the parameters about the reflection probes that are going to be spawned
    public void SetReflectionProbeConfig(int resolution, bool refreshEveryFrame)
    {
        ReflectionProbesResolution = resolution;
        if (refreshEveryFrame) ReflectionProbesRefreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;
        else ReflectionProbesRefreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
    }
    //Set camera and postprocessing configs
    public void SetCameraAndVolumesConfig(GameConfigHandlerScript.CameraConfig cameraConfig, GameConfigHandlerScript.VolumeConfig volumeConfig)
    {
        CameraConfig = cameraConfig; VolumeConfig = volumeConfig;
    }
    //Loads the camera config
    public void LoadCameraConfig(Camera camera, PostProcessLayer cameraLayer) 
    {
        cameraLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
        if (CameraConfig.useAntiAliasing == 1) cameraLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
        if (CameraConfig.useAntiAliasing == 2) cameraLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
        if (CameraConfig.useAntiAliasing == 3) cameraLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
        cameraLayer.fog.enabled = CameraConfig.useFog;
        cameraLayer.finalBlitToCameraTarget = CameraConfig.fastPostProcessing;
        cameraLayer.enabled = CameraConfig.usePostProcessing;//Whether or not to use postprocessing effects
        if (CameraConfig.fastrender) camera.renderingPath = RenderingPath.Forward;
        else { camera.renderingPath = RenderingPath.DeferredShading; }
    }
    //Loads the postprocessing config for a volume
    public void LoadPostProcessingVolumeConfig(PostProcessVolume volume) 
    {
        //Change what post processing effects are enabled
        PostProcessProfile profile = volume.sharedProfile;
        profile.settings[0].active = VolumeConfig.useColorGrading;
        profile.settings[1].active = VolumeConfig.useChromaticAberration;
        profile.settings[2].active = VolumeConfig.useBloom;
        profile.settings[3].active = VolumeConfig.useVignette;
        profile.settings[4].active = VolumeConfig.useAutoExposure;
        profile.settings[5].active = VolumeConfig.useMotionBlur;
        profile.settings[6].active = VolumeConfig.useAmbientOcclusion;
        profile.settings[7].active = VolumeConfig.useDepthOfField;
        profile.settings[8].active = VolumeConfig.useScreenSpaceReflections;
        profile.settings[9].active = VolumeConfig.useGrain;
        profile.settings[10].active = VolumeConfig.useLensDistortion;
    }
    //Loads the reflection probe configs
    public void LoadReflectionProbeConfig(ReflectionProbe reflectionProbe) 
    {
        reflectionProbe.resolution = ReflectionProbesResolution;
        reflectionProbe.refreshMode = ReflectionProbesRefreshMode;
    }
    #endregion
    //Called internally when map has changed
    private IEnumerator WorldUpdateCoroutine() 
    {
        #region Bots path calculations/recalculations
        //Recalculates every bots's path and updates pathfinding grid
        BotPathfinderScript[] pathfinders = FindObjectsOfType<BotPathfinderScript>();
        if (pathfinders != null || pathfinders.Length != 0)//Recalculate pathes since we have valid pathfinding bots
        {
            pathfinder.MakeGrid();//Recalculate grid
            yield return new WaitForSecondsRealtime(1f);//Wait before calculating the bot path to make it seem cool hehe
            for (int i = 0; i < pathfinders.Length; i++)
            {
                yield return new WaitForSecondsRealtime(0.5f);//Wait before calculating the bot path to make it seem cool hehe
                pathfinders[i].Pathfind();
            }
        }
        #endregion
    }
    //Called externally by scripts to start coroutine to start map update
    public void WorldUpdate() 
    {
        Debug.Log("World Update");
        StartCoroutine("WorldUpdateCoroutine");
    }

    //Detects when an object has spawned using the ObjectSpawnDetectionScript
    public void OnObjectSpawn(GameObject otherGameObject, string StringTag)
    {
        Debug.Log("Object with tag " + StringTag + " has been spawned");
        if (StringTag == "Camera") { LoadCameraConfig(otherGameObject.GetComponent<Camera>(), otherGameObject.GetComponent<PostProcessLayer>()); return; }
        if (StringTag == "PostProcessVolume") { LoadPostProcessingVolumeConfig(otherGameObject.GetComponent<PostProcessVolume>()); return; }
        if (StringTag == "ReflectionProbe") { LoadReflectionProbeConfig(otherGameObject.GetComponent<ReflectionProbe>()); return; }
    }

}
