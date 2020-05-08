using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
//A handler script that handles information flow between GameConfigSaverLoader class and the game config itself
public class GameConfigHandlerScript : MonoBehaviour
{
    private GameConfigSaverLoader gameConfigSaverLoader;//Config saver/loader
    private GameConfig currentGameConfig;
    private GameConfigHandlerScript instance;//Using an instance method to avoid duplicates
    private WorldManager wm;//The world manager for the current scene
    public struct CameraConfig 
    {
        public bool fastrender;
        public bool usePostProcessing;
        public bool fastPostProcessing;
        public bool useFog;
        public int  useAntiAliasing;
    }
    public struct VolumeConfig 
    {
        public bool useColorGrading;
        public bool useChromaticAberration;
        public bool useBloom;
        public bool useVignette;
        public bool useAutoExposure;
        public bool useMotionBlur;
        public bool useAmbientOcclusion;
        public bool useDepthOfField;
        public bool useScreenSpaceReflections;
        public bool useGrain;
        public bool useLensDistortion;
    }
    // Start is called before the first frame update
    //When object is inialized
    void Start()
    {
        SceneManager.sceneLoaded += OnSceneChange;
        DontDestroyOnLoad(gameObject);


        wm = FindObjectOfType<WorldManager>();
        gameConfigSaverLoader = new GameConfigSaverLoader();
        gameConfigSaverLoader.SetupPathes();//Setup config path
        currentGameConfig = new GameConfig();//Make default config to make sure that default variables are always present
        GameConfig loadedConfig = gameConfigSaverLoader.LoadConfig();//Load config file
        if (loadedConfig == null) currentGameConfig = new GameConfig();//Make a new game config if there isnt one
        else currentGameConfig = loadedConfig;//Set the game config to use the config.txt data

        gameConfigSaverLoader.SaveConfig(currentGameConfig);//Save the config file to set default values if they didnt exist yet
        LoadConfig(currentGameConfig);
        Debug.Log("Finished reading game config");
        //gameConfigSaverLoader.SaveConfig(SaveConfig());
    }
    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        wm = FindObjectOfType<WorldManager>();
        LoadConfig(currentGameConfig);//Load config and apply it to the objects of the current scene
        Debug.Log("Finished reading game config on scene change");
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }
    //Turn current game config to GameConfig class
    private GameConfig SaveConfig()
    {
        GameConfig outconfig = new GameConfig();
        //Post-processing
        outconfig.usePostProcessing = true;
        outconfig.fastPostProcessing = false;
        outconfig.useFog = true;
        outconfig.useColorGrading = true;
        outconfig.useChromaticAberration = true;
        outconfig.useBloom = true;
        outconfig.useVignette = true;
        outconfig.useAutoExposure = true;
        outconfig.useMotionBlur = true;
        outconfig.useAmbientOcclusion = true;
        outconfig.useDepthOfField = true;
        outconfig.useScreenSpaceReflections = true;
        outconfig.useGrain = true;
        outconfig.useLensDistortion = true;
        outconfig.useAntiAliasing = 1;

        outconfig.ScreenWidth = 1920;
        outconfig.ScreenHeight = 1080;
        outconfig.TargetFrameRate = 60;
        outconfig.Fullscreen = true;

        outconfig.PixelLightCount = QualitySettings.pixelLightCount;
        outconfig.TextureQuality = QualitySettings.masterTextureLimit;
        outconfig.AnisotropicTextures = QualitySettings.anisotropicFiltering.ToString();
        outconfig.SoftParticles = QualitySettings.softParticles;
        outconfig.RealtimeReflectionProbes = QualitySettings.realtimeReflectionProbes;
        outconfig.ReflectionProbesResolution = 64;
        outconfig.ReflectionProbesRefreshEveryFrame = true;
        outconfig.BillboardsFaceCameraPosition = QualitySettings.billboardsFaceCameraPosition;
        outconfig.ResolutionScalingFixedDPI = QualitySettings.resolutionScalingFixedDPIFactor;
        outconfig.TextureStreaming = QualitySettings.streamingMipmapsActive;
        outconfig.FastRendering = false;

        outconfig.ShadowsType = QualitySettings.shadows.ToString();
        outconfig.ShadowsResolution = QualitySettings.shadowResolution.ToString();
        outconfig.ShadowDistance = QualitySettings.shadowDistance;

        outconfig.SkinWeights = QualitySettings.skinWeights.ToString();
        outconfig.VSync = QualitySettings.vSyncCount;
        outconfig.LODBias = QualitySettings.lodBias;
        outconfig.MaxLODLevel = QualitySettings.maximumLODLevel;
        return outconfig;
    }
    //Turn GameConfig class into current game config
    private void LoadConfig(GameConfig inconfig)
    {
        Screen.SetResolution(inconfig.ScreenWidth, inconfig.ScreenHeight, inconfig.Fullscreen);
        Application.targetFrameRate = inconfig.TargetFrameRate;

        QualitySettings.pixelLightCount = inconfig.PixelLightCount;
        QualitySettings.masterTextureLimit = inconfig.TextureQuality;
        QualitySettings.anisotropicFiltering = (AnisotropicFiltering)System.Enum.Parse(typeof(AnisotropicFiltering), inconfig.AnisotropicTextures);
        QualitySettings.softParticles = inconfig.SoftParticles;
        QualitySettings.realtimeReflectionProbes = inconfig.RealtimeReflectionProbes;
        SetReflectionProbesSettings(inconfig.ReflectionProbesResolution, inconfig.ReflectionProbesRefreshEveryFrame);
        QualitySettings.billboardsFaceCameraPosition = inconfig.BillboardsFaceCameraPosition;
        QualitySettings.resolutionScalingFixedDPIFactor = inconfig.ResolutionScalingFixedDPI;
        QualitySettings.streamingMipmapsActive = inconfig.TextureStreaming;

        QualitySettings.shadows = (ShadowQuality)System.Enum.Parse(typeof(ShadowQuality), inconfig.ShadowsType);
        QualitySettings.shadowResolution = (ShadowResolution)System.Enum.Parse(typeof(ShadowResolution), inconfig.ShadowsResolution);
        QualitySettings.shadowDistance = inconfig.ShadowDistance;

        QualitySettings.skinWeights = (SkinWeights)System.Enum.Parse(typeof(SkinWeights), inconfig.SkinWeights);
        QualitySettings.vSyncCount = inconfig.VSync;
        QualitySettings.lodBias = inconfig.LODBias;
        QualitySettings.maximumLODLevel = inconfig.MaxLODLevel;

        SetCamerasConfig(
            inconfig.FastRendering,
            //Set post-processing   
            inconfig.usePostProcessing,
            inconfig.fastPostProcessing,
            inconfig.useFog,
            inconfig.useColorGrading,
            inconfig.useChromaticAberration,
            inconfig.useBloom,
            inconfig.useVignette,
            inconfig.useAutoExposure,
            inconfig.useMotionBlur,
            inconfig.useAmbientOcclusion,
            inconfig.useDepthOfField,
            inconfig.useScreenSpaceReflections,
            inconfig.useGrain,
            inconfig.useLensDistortion,
            inconfig.useAntiAliasing
        );

        LoadAllConfigs();//Load configs for all current scene objects
    }
    //Load the configs on all current objects of the scene
    private void LoadAllConfigs() 
    {
        Camera[] cameras = FindObjectsOfType<Camera>();
        PostProcessVolume[] volumes = FindObjectsOfType<PostProcessVolume>();
        ReflectionProbe[] reflectionProbes = FindObjectsOfType<ReflectionProbe>();

        foreach (var camera in cameras)
        {
            wm.LoadCameraConfig(camera, camera.GetComponent<PostProcessLayer>());//This is indeed very unoptimized
        }
        foreach (var volume in volumes)
        {
            wm.LoadPostProcessingVolumeConfig(volume);
        }
        foreach (var reflectionProbe in reflectionProbes)
        {
            wm.LoadReflectionProbeConfig(reflectionProbe);
        }

    }
    //Changes the settings of every reflection probe that is going to be spawned
    private void SetReflectionProbesSettings(int res, bool refreshEveryFrame)
    {
        //Just to make sure that res is one of the following numbers. If not then make the default 64
        if (res != 16 && res != 32 && res != 64 && res != 128 && res != 256 && res != 512 && res != 1024 && res != 2048) res = 64;
        WorldManager wm = FindObjectOfType<WorldManager>();
        wm.SetReflectionProbeConfig(res, refreshEveryFrame);//Apply the settings
    }
    //Sets the cameras settings like whether or not to use fast rendering. Also set postprocessing settings
    private void SetCamerasConfig(bool fastrender, bool usepostprocessing, bool fastpostprocessing, bool fog, bool colorgrading, bool chromaticaberration, bool bloom, bool vignette, bool autoexposure, bool motionblur, bool ambientocclusion, bool depthoffield, bool screenspacereflections, bool grain, bool lensdistortion, int antialiasing) 
    {
        //Set config for camera
        CameraConfig newCameraConfig;
        newCameraConfig.fastrender = fastrender;
        newCameraConfig.usePostProcessing = usepostprocessing;
        newCameraConfig.fastPostProcessing = fastpostprocessing;
        newCameraConfig.useFog = fog;
        newCameraConfig.useAntiAliasing = antialiasing;

        //Set config for volumes
        VolumeConfig newVolumeConfig;
        newVolumeConfig.useColorGrading = colorgrading;
        newVolumeConfig.useChromaticAberration = chromaticaberration;
        newVolumeConfig.useBloom = bloom;
        newVolumeConfig.useVignette = vignette;
        newVolumeConfig.useAutoExposure = autoexposure;
        newVolumeConfig.useMotionBlur = motionblur;
        newVolumeConfig.useAmbientOcclusion = ambientocclusion;
        newVolumeConfig.useDepthOfField = depthoffield;
        newVolumeConfig.useScreenSpaceReflections = screenspacereflections;
        newVolumeConfig.useGrain = grain;
        newVolumeConfig.useLensDistortion = lensdistortion;
        //Apply configs

        wm.SetCameraAndVolumesConfig(newCameraConfig, newVolumeConfig);
    }             
}                 
                  