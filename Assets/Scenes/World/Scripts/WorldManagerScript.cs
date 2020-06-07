using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering;
using MLAPI;
//Handles communications between multiple scripts and classes (Multiplayer included)
public class WorldManagerScript : NetworkedBehaviour
{
    public bool calculatePathesAtStart = true;//Should we calculate bot pathfinding at the start of the game ?
    private AStarPathfinderScript pathfinder;//The pathfinder used for bot path calculations
    private string currentScene;//The current scene that we are in

    //Reflection probes
    private int reflectionProbesResolution;
    private ReflectionProbeRefreshMode reflectionProbesRefreshMode;
    //Cameras and postprocessing
    private GameConfigHandlerScript.CameraConfig cameraConfig;
    private GameConfigHandlerScript.VolumeConfig volumeConfig;
    // Start is called before the first frame update
    void Start()
    {
        currentScene = SceneManager.GetActiveScene().name;
        if(currentScene == "MainMenuMap") 
        {
            //----Only ran at the start of the game----\\


            //Load the textures from the streaming assets folder
            TexturesManager.LoadAllTextures();
            //Load items from resource folder at the start of the game
            ItemsManager.LoadAllItems();


            //Load all the key binds at the start of the game
            InputManager.SetupKeybinds();
        }
    }
    public override void NetworkStart()
    {
        base.NetworkStart();   
        //Run the world update only on the server  
        if (calculatePathesAtStart && IsServer)
        {
            pathfinder = FindObjectOfType<AStarPathfinderScript>();//Init base terrain
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
        reflectionProbesResolution = resolution;
        if (refreshEveryFrame) reflectionProbesRefreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;
        else reflectionProbesRefreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;
    }
    //Set camera and postprocessing configs
    public void SetCameraAndVolumesConfig(GameConfigHandlerScript.CameraConfig _cameraConfig, GameConfigHandlerScript.VolumeConfig _volumeConfig)
    {
        cameraConfig = _cameraConfig; volumeConfig = _volumeConfig;
    }
    //Loads the camera config
    public void LoadCameraConfig(Camera camera, PostProcessLayer cameraLayer) 
    {
        cameraLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
        if (cameraConfig.useAntiAliasing == 1) cameraLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
        if (cameraConfig.useAntiAliasing == 2) cameraLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
        if (cameraConfig.useAntiAliasing == 3) cameraLayer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
        cameraLayer.fog.enabled = cameraConfig.useFog;
        cameraLayer.finalBlitToCameraTarget = cameraConfig.fastPostProcessing;
        cameraLayer.enabled = cameraConfig.usePostProcessing;//Whether or not to use postprocessing effects
        if (cameraConfig.fastRender) camera.renderingPath = RenderingPath.Forward;
        else { camera.renderingPath = RenderingPath.DeferredShading; }
    }
    //Loads the postprocessing config for a volume
    public void LoadPostProcessingVolumeConfig(PostProcessVolume volume) 
    {
        //Change what post processing effects are enabled
        PostProcessProfile profile = volume.sharedProfile;
        profile.settings[0].active = volumeConfig.useColorGrading;
        profile.settings[1].active = volumeConfig.useChromaticAberration;
        profile.settings[2].active = volumeConfig.useBloom;
        profile.settings[3].active = volumeConfig.useVignette;
        profile.settings[4].active = volumeConfig.useAutoExposure;
        profile.settings[5].active = volumeConfig.useMotionBlur;
        profile.settings[6].active = volumeConfig.useAmbientOcclusion;
        profile.settings[7].active = volumeConfig.useDepthOfField;
        profile.settings[8].active = volumeConfig.useScreenSpaceReflections;
        profile.settings[9].active = volumeConfig.useGrain;
        profile.settings[10].active = volumeConfig.useLensDistortion;
    }
    //Loads the reflection probe configs
    public void LoadReflectionProbeConfig(ReflectionProbe reflectionProbe) 
    {
        reflectionProbe.resolution = reflectionProbesResolution;
        reflectionProbe.refreshMode = reflectionProbesRefreshMode;
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
                if (pathfinders[i] != null) { pathfinders[i].Pathfind(); }//Haha error go brrrrr
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
    public void OnObjectSpawn(GameObject otherGameObject, string stringTag)
    {
        Debug.Log("Object with tag " + stringTag + " has been spawned");
        if (stringTag == "Camera") { LoadCameraConfig(otherGameObject.GetComponent<Camera>(), otherGameObject.GetComponent<PostProcessLayer>()); return; }
        if (stringTag == "PostProcessVolume") { LoadPostProcessingVolumeConfig(otherGameObject.GetComponent<PostProcessVolume>()); return; }
        if (stringTag == "ReflectionProbe") { LoadReflectionProbeConfig(otherGameObject.GetComponent<ReflectionProbe>()); return; }
    }

}
