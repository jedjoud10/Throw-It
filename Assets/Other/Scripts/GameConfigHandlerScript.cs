using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
//A handler script that handles information flow between GameConfigSaverLoader class and the game config itself
public class GameConfigHandlerScript : MonoBehaviour
{
    GameConfigSaverLoader gameConfigSaverLoader;//Config saver/loader
    // Start is called before the first frame update
    void Start()
    {
        gameConfigSaverLoader = new GameConfigSaverLoader();
        gameConfigSaverLoader.SetupPathes();//Setup config path
        GameConfig config = gameConfigSaverLoader.LoadConfig();
        if (config != null) LoadConfig(gameConfigSaverLoader.LoadConfig());
        else gameConfigSaverLoader.SaveConfig(SaveConfig());
        Debug.Log("Finished reading game config");
        //gameConfigSaverLoader.SaveConfig(SaveConfig());
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

        outconfig.PixelLightCount = QualitySettings.pixelLightCount;
        outconfig.TextureQuality = QualitySettings.masterTextureLimit;
        outconfig.AnisotropicTextures = QualitySettings.anisotropicFiltering.ToString();
        outconfig.SoftParticles = QualitySettings.softParticles;
        outconfig.RealtimeReflectionProbes = QualitySettings.realtimeReflectionProbes;
        outconfig.ReflectionProbesResolution = 64;
        outconfig.ReflectionProbesRefresh = 0;
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
        //Set post-processing   
        SetPostProcessingConfig(
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
        QualitySettings.pixelLightCount = inconfig.PixelLightCount;
        QualitySettings.masterTextureLimit = inconfig.TextureQuality;
        QualitySettings.anisotropicFiltering = (AnisotropicFiltering)System.Enum.Parse(typeof(AnisotropicFiltering), inconfig.AnisotropicTextures);
        QualitySettings.softParticles = inconfig.SoftParticles;
        QualitySettings.realtimeReflectionProbes = inconfig.RealtimeReflectionProbes;
        SetReflectionProbeResolution(inconfig.ReflectionProbesResolution);
        SetReflectionRefresh(inconfig.ReflectionProbesRefresh);
        QualitySettings.billboardsFaceCameraPosition = inconfig.BillboardsFaceCameraPosition;
        QualitySettings.resolutionScalingFixedDPIFactor = inconfig.ResolutionScalingFixedDPI;
        QualitySettings.streamingMipmapsActive = inconfig.TextureStreaming;
        SetCamerasConfig(inconfig.FastRendering);

        QualitySettings.shadows = (ShadowQuality)System.Enum.Parse(typeof(ShadowQuality), inconfig.ShadowsType);
        QualitySettings.shadowResolution = (ShadowResolution)System.Enum.Parse(typeof(ShadowResolution), inconfig.ShadowsResolution);
        QualitySettings.shadowDistance = inconfig.ShadowDistance;

        QualitySettings.skinWeights = (SkinWeights)System.Enum.Parse(typeof(SkinWeights), inconfig.SkinWeights);
        QualitySettings.vSyncCount = inconfig.VSync;
        QualitySettings.lodBias = inconfig.LODBias;
        QualitySettings.maximumLODLevel = inconfig.MaxLODLevel;
    }
    //Changes the resolution of every reflection probe in the scene
    private void SetReflectionProbeResolution(int res)
    {
        //Just to make sure that res is one of the following numbers. If not then make the default 64
        if (res != 16 && res != 32 && res != 64 && res != 128 && res != 256 && res != 512 && res != 1024 && res != 2048) res = 64;
        ReflectionProbe[] probes = FindObjectsOfType<ReflectionProbe>();//All probes
        for (int i = 0; i < probes.Length; i++)
        {
            probes[i].resolution = res;
        }
    }
    //Changes the type of refreshion for every reflection prode in the scene
    private void SetReflectionRefresh(int type)
    {
        ReflectionProbe[] probes = FindObjectsOfType<ReflectionProbe>();//All probes
        for (int i = 0; i < probes.Length; i++)//Set type for every probe
        {
            if (type == 0)
                probes[i].refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.OnAwake;//Higher quality and faster reflections
            if (type == 1)
                probes[i].refreshMode = UnityEngine.Rendering.ReflectionProbeRefreshMode.EveryFrame;//Lower quality and static reflection only
        }
    }
    //Changes if we use post-processing or not
    private void SetPostProcessingConfig(bool usepostprocessing, bool fastpostprocessing, bool fog, bool colorgrading, bool chromaticaberration, bool bloom, bool vignette, bool autoexposure, bool motionblur, bool ambientocclusion, bool depthoffield, bool screenspacereflections, bool grain, bool lensdistortion,int antialiasing) 
    {
        PostProcessLayer[] layers = GameObject.FindObjectsOfType<PostProcessLayer>();//Get the cameras post-processing layers
        foreach (var layer in layers)
        {
            if (antialiasing != 1 || antialiasing != 2 || antialiasing != 3) layer.antialiasingMode = PostProcessLayer.Antialiasing.None;
            if (antialiasing == 1) layer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
            if (antialiasing == 2) layer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
            if (antialiasing == 3) layer.antialiasingMode = PostProcessLayer.Antialiasing.TemporalAntialiasing;
            layer.fog.enabled = fog;
            layer.finalBlitToCameraTarget = fastpostprocessing;
            layer.enabled = usepostprocessing;//Whether or not to use postprocessing effects
        }
        PostProcessVolume[] volumes = GameObject.FindObjectsOfType<PostProcessVolume>();//Get all post-processing volumes so we can change every post-processing profile
        for (int i = 0; i < volumes.Length; i++)
        {
            PostProcessProfile postprocess = volumes[i].sharedProfile;
            //Set post-process effects
            postprocess.settings[0].active = colorgrading;
            postprocess.settings[1].active = chromaticaberration;
            postprocess.settings[2].active = bloom;
            postprocess.settings[3].active = vignette;
            postprocess.settings[4].active = autoexposure;
            postprocess.settings[5].active = motionblur;
            postprocess.settings[6].active = ambientocclusion;
            postprocess.settings[7].active = depthoffield;
            postprocess.settings[8].active = screenspacereflections;
            postprocess.settings[9].active = grain;
            postprocess.settings[10].active = lensdistortion;            
        }

    }
    //Sets the cameras settings like whether or not to use fast rendering
    private void SetCamerasConfig(bool fastrender) 
    {
        Camera[] cameras = GameObject.FindObjectsOfType<Camera>();//Find all cameras in current scene
        foreach (var camera in cameras)
        {
            if (fastrender) camera.renderingPath = RenderingPath.Forward;
            else { camera.renderingPath = RenderingPath.DeferredShading; }
        }
    }
}
