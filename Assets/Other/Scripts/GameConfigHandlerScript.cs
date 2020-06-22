using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
//A handler script that loads and saves game configs and applies it
public class GameConfigHandlerScript : MonoBehaviour
{
    public GameConfig currentGameConfig;
    public GameConfigHandlerScript instance;//Using an instance method to avoid duplicates
    private WorldManagerScript wm;//The world manager for the current scene
    public struct CameraConfig 
    {
        public bool fastRender;
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

        wm = FindObjectOfType<WorldManagerScript>();
        currentGameConfig = (GameConfig) SaverLoader.Load("config.json", new GameConfig(), typeof(GameConfig));//Load config file

        ApplyConfig(currentGameConfig);
        Debug.Log("Finished reading game config");
    }
    private void OnSceneChange(Scene scene, LoadSceneMode mode)
    {
        wm = FindObjectOfType<WorldManagerScript>();
        ApplyConfig(currentGameConfig);//Load config and apply it to the objects of the current scene
        Debug.Log("Finished reading game config on scene change");
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }    
    //Turn GameConfig class into current game config
    private void ApplyConfig(GameConfig inconfig)
    {
        Screen.SetResolution(inconfig.ScreenWidth, inconfig.ScreenHeight, inconfig.Fullscreen);
        if (inconfig.TargetFrameRate != 0) { Application.targetFrameRate = inconfig.TargetFrameRate; }

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
            inconfig.UsePostProcessing,
            inconfig.FastPostProcessing,
            inconfig.UseFog,
            inconfig.UseColorGrading,
            inconfig.UseChromaticAberration,
            inconfig.UseBloom,
            inconfig.UseVignette,
            inconfig.UseAutoExposure,
            inconfig.UseMotionBlur,
            inconfig.UseAmbientOcclusion,
            inconfig.UseDepthOfField,
            inconfig.UseScreenSpaceReflections,
            inconfig.UseGrain,
            inconfig.UseLensDistortion,
            inconfig.UseAntiAliasing
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
        WorldManagerScript wm = FindObjectOfType<WorldManagerScript>();
        wm.SetReflectionProbeConfig(res, refreshEveryFrame);//Apply the settings
    }
    //Sets the cameras settings like whether or not to use fast rendering. Also set postprocessing settings
    private void SetCamerasConfig(bool fastrender, bool usepostprocessing, bool fastpostprocessing, bool fog, bool colorgrading, bool chromaticaberration, bool bloom, bool vignette, bool autoexposure, bool motionblur, bool ambientocclusion, bool depthoffield, bool screenspacereflections, bool grain, bool lensdistortion, int antialiasing) 
    {
        //Set config for camera
        CameraConfig newCameraConfig;
        newCameraConfig.fastRender = fastrender;
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
//Class that holds information about the game configuration
public class GameConfig
{
    #region QualitySettings
    //Rendering    
    public bool Fullscreen = true; public int TargetFrameRate = 0;
    public int ScreenHeight = 1080; public int ScreenWidth = 1920;
    public int PixelLightCount = 5;
    public int TextureQuality = 2;
    public string AnisotropicTextures = "ForceEnable";
    public bool SoftParticles = true;
    public bool RealtimeReflectionProbes = true;
    public int ReflectionProbesResolution = 64;
    public bool ReflectionProbesRefreshEveryFrame = true;
    public bool BillboardsFaceCameraPosition = true;
    public float ResolutionScalingFixedDPI = 1f;
    public bool TextureStreaming = true;
    public bool FastRendering = false;
    #endregion
    #region Post-processing
    //Post-processing
    public bool UsePostProcessing = true;
    public bool FastPostProcessing = false;
    public bool UseFog = true;
    public bool UseColorGrading = true;
    public bool UseChromaticAberration = true;
    public bool UseBloom = true;
    public bool UseVignette = true;
    public bool UseAutoExposure = true;
    public bool UseMotionBlur = true;
    public bool UseAmbientOcclusion = true;
    public bool UseDepthOfField = true;
    public bool UseScreenSpaceReflections = true;
    public bool UseGrain = true;
    public bool UseLensDistortion = true;
    public int UseAntiAliasing = 1;
    #endregion
    #region Shadows
    //Shadows
    public string ShadowsType = "All";
    public string ShadowsResolution = "VeryHigh";
    public float ShadowDistance = 100;
    #endregion
    #region Other
    //Other
    public string SkinWeights = "Unlimited";
    public int VSync = 1;
    public float LODBias = 3;
    public int MaxLODLevel = 0;
    #endregion
}

