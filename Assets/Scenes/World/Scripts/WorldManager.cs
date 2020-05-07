using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
//Handles communications between multiple scripts and classes (Multiplayer included)
public class WorldManager : MonoBehaviour
{
    public bool CalculatePathesAtStart = true;//Should we calculate bot pathfinding at the start of the game ?


    //Reflection probes
    private int ReflectionProbesResolution;
    private ReflectionProbeRefreshMode ReflectionProbesRefreshMode;
    //Cameras and postprocessing
    private GameConfigHandlerScript.CameraConfig CameraConfig;
    private GameConfigHandlerScript.VolumeConfig VolumeConfig;
    // Start is called before the first frame update
    void Start()
    {
        if (CalculatePathesAtStart)
        {
            FindObjectOfType<AStarPathfinder>().MakeTerrainGrid();//Init base terrain
            StartCoroutine("WorldUpdateCoroutine");
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
            FindObjectOfType<AStarPathfinder>().MakeGrid();//Recalculate grid
            yield return new WaitForSecondsRealtime(1.0f);
            for (int i = 0; i < pathfinders.Length; i++)
            {
                yield return new WaitForSecondsRealtime(1.0f);
                pathfinders[i].FindPath();
            }
        }
        #endregion
    }
    //Called externally by scripts to start coroutine to start map update
    public void WorldUpdate() 
    {
        StartCoroutine("WorldUpdateCoroutine");
    }
    #region Scene Management
    //Switch to the world map
    public void StartWorldMap() { ChangeScene("TestMap"); }
    //Switch to the MainMenu
    public void StartMainMenu() { ChangeScene("MainMenuMap"); }    
    //Switches to a specific map
    public void ChangeScene(string sceneName) 
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
    //Are we in that scene ?
    public bool IsScene(string sceneName) { return SceneManager.GetActiveScene().name == sceneName; }
    #endregion

    //Detects when an object has spawned using the ObjectSpawnDetectionScript
    public void OnObjectSpawn(GameObject otherGameObject, string StringTag)
    {
        Debug.Log("Object with tag " + StringTag + " has been spawned");
        if (StringTag == "Camera") { LoadCameraConfig(otherGameObject.GetComponent<Camera>(), otherGameObject.GetComponent<PostProcessLayer>()); return; }
        if (StringTag == "PostProcessVolume") { LoadPostProcessingVolumeConfig(otherGameObject.GetComponent<PostProcessVolume>()); return; }
        if (StringTag == "ReflectionProbe") { LoadReflectionProbeConfig(otherGameObject.GetComponent<ReflectionProbe>()); return; }
    }

}
