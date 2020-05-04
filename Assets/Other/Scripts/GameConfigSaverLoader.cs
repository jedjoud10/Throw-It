using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
//Loads and saves a string that contains the game's configuration
public class GameConfigSaverLoader
{
    string config_path;//Path of config file
    //Setup pathes fo config saver loader
    public void SetupPathes()
    {
        config_path = Application.persistentDataPath + "/config.txt";
    }
    //Save the string into config file
    public void SaveConfig(GameConfig gameConfig)
    {
        string content = JsonUtility.ToJson(gameConfig, true);//Transforms the gameConfig to json then string. Then save it
        File.WriteAllText(config_path, content);
    }
    //Load the string from config file
    public GameConfig LoadConfig()
    {
        if (File.Exists(config_path))
        {
            return JsonUtility.FromJson<GameConfig>(File.ReadAllText(config_path));//From string to gameConfig then load
        }
        else
        {
            Debug.LogWarning("Config file does not exist !");
        }
        return null;
    }
}
//Class that holds information about the game configuration
public class GameConfig
{
    #region QualitySettings
    //Rendering    
    public bool Fullscreen = true; public int TargetFrameRate = 60;
    public int ScreenHeight = 1920; public int ScreenWidth = 1080;
    public int PixelLightCount = 5;
    public int TextureQuality = 2;
    public string AnisotropicTextures = "ForceEnable";
    public bool SoftParticles = true;
    public bool RealtimeReflectionProbes = true;
    public int ReflectionProbesResolution = 64;
    public int ReflectionProbesRefresh = 0;
    public bool BillboardsFaceCameraPosition = true;
    public float ResolutionScalingFixedDPI = 1f;
    public bool TextureStreaming = true;
    public bool FastRendering = false;
    #endregion
    #region Post-processing
    //Post-processing
    public bool usePostProcessing = true;
    public bool fastPostProcessing = false;
    public bool useFog = true;
    public bool useColorGrading = true;
    public bool useChromaticAberration = true;
    public bool useBloom = true;
    public bool useVignette = true;
    public bool useAutoExposure = true;
    public bool useMotionBlur = true;
    public bool useAmbientOcclusion = true;
    public bool useDepthOfField = true;
    public bool useScreenSpaceReflections = true;
    public bool useGrain = true;
    public bool useLensDistortion = true;
    public int useAntiAliasing = 1;
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
