using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//A handler script that handles information flow between GameConfigSaverLoader class and the game config itself
public class GameConfigHandlerScript : MonoBehaviour
{
    GameConfigSaverLoader gameConfigSaverLoader;//Config saver/loader
    // Start is called before the first frame update
    void Start()
    {
        gameConfigSaverLoader = new GameConfigSaverLoader();
        gameConfigSaverLoader.SetupPathes();//Setup config path
        LoadConfig(gameConfigSaverLoader.LoadConfig());
        //gameConfigSaverLoader.SaveConfig(SaveConfig());
    }
    //Turn current game config to GameConfig class
    private GameConfig SaveConfig() 
    {
        GameConfig outconfig = new GameConfig();
        outconfig.PixelLightCount = QualitySettings.pixelLightCount;
        outconfig.TextureQuality = QualitySettings.masterTextureLimit;
        outconfig.AnisotropicTextures = QualitySettings.anisotropicFiltering.ToString();
        outconfig.AntiAliasing = QualitySettings.antiAliasing;
        outconfig.SoftParticles = QualitySettings.softParticles;
        outconfig.RealtimeReflectionProbes = QualitySettings.realtimeReflectionProbes;
        outconfig.BillboardsFaceCameraPosition = QualitySettings.billboardsFaceCameraPosition;
        outconfig.ResolutionScalingFixedDPI = QualitySettings.resolutionScalingFixedDPIFactor;
        outconfig.TextureStreaming = QualitySettings.streamingMipmapsActive;

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
        QualitySettings.pixelLightCount = inconfig.PixelLightCount;
        QualitySettings.masterTextureLimit = inconfig.TextureQuality;
        QualitySettings.anisotropicFiltering =  (AnisotropicFiltering)System.Enum.Parse(typeof(AnisotropicFiltering), inconfig.AnisotropicTextures);
        QualitySettings.antiAliasing = inconfig.AntiAliasing;
        QualitySettings.softParticles = inconfig.SoftParticles;
        QualitySettings.realtimeReflectionProbes = inconfig.RealtimeReflectionProbes;
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
    }
}
