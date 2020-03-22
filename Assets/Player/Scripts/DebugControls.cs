// GENERATED AUTOMATICALLY FROM 'Assets/Player/Scripts/DebugControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @DebugControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @DebugControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""DebugControls"",
    ""maps"": [
        {
            ""name"": ""BotSpawning"",
            ""id"": ""d5f96afe-23e9-4a9b-9788-8a61d1d8ac21"",
            ""actions"": [
                {
                    ""name"": ""SpawnBot"",
                    ""type"": ""Button"",
                    ""id"": ""c75806f0-bc44-4052-b023-aa6d8db0ae90"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ChangeBot"",
                    ""type"": ""Button"",
                    ""id"": ""457bfcf3-a598-4f02-9090-d5c9a19b9fa5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""6eae19d7-f943-4da1-8294-e5b10abf58b8"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SpawnBot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6a2eefe0-0ae2-424d-92a8-6b09097e80cc"",
                    ""path"": ""<Keyboard>/n"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeBot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Resolution"",
            ""id"": ""06bc96dd-dcb9-43e6-9ad2-27b333f8de7e"",
            ""actions"": [
                {
                    ""name"": ""ChangeResolution"",
                    ""type"": ""PassThrough"",
                    ""id"": ""1a6e9a05-dcbf-490b-9963-0a8e426e2868"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b6850ecc-ba56-4464-b31b-5ef986378a56"",
                    ""path"": ""<Keyboard>/l"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ChangeResolution"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Map"",
            ""id"": ""f393e2e9-e12a-4865-b927-6b1d4d68e8b3"",
            ""actions"": [
                {
                    ""name"": ""SwitchMap"",
                    ""type"": ""PassThrough"",
                    ""id"": ""a605ec48-9fd5-4f1b-b367-e535a26e4056"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""RecalculatePathfinder"",
                    ""type"": ""PassThrough"",
                    ""id"": ""cb4dddb7-b564-407f-9b02-9b23c7d7074a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""dd4003ce-d364-4789-9ac9-3b557828d97b"",
                    ""path"": ""<Keyboard>/h"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SwitchMap"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""56d37cc8-534d-4d90-bdc7-1547bec875cb"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""RecalculatePathfinder"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // BotSpawning
        m_BotSpawning = asset.FindActionMap("BotSpawning", throwIfNotFound: true);
        m_BotSpawning_SpawnBot = m_BotSpawning.FindAction("SpawnBot", throwIfNotFound: true);
        m_BotSpawning_ChangeBot = m_BotSpawning.FindAction("ChangeBot", throwIfNotFound: true);
        // Resolution
        m_Resolution = asset.FindActionMap("Resolution", throwIfNotFound: true);
        m_Resolution_ChangeResolution = m_Resolution.FindAction("ChangeResolution", throwIfNotFound: true);
        // Map
        m_Map = asset.FindActionMap("Map", throwIfNotFound: true);
        m_Map_SwitchMap = m_Map.FindAction("SwitchMap", throwIfNotFound: true);
        m_Map_RecalculatePathfinder = m_Map.FindAction("RecalculatePathfinder", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // BotSpawning
    private readonly InputActionMap m_BotSpawning;
    private IBotSpawningActions m_BotSpawningActionsCallbackInterface;
    private readonly InputAction m_BotSpawning_SpawnBot;
    private readonly InputAction m_BotSpawning_ChangeBot;
    public struct BotSpawningActions
    {
        private @DebugControls m_Wrapper;
        public BotSpawningActions(@DebugControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @SpawnBot => m_Wrapper.m_BotSpawning_SpawnBot;
        public InputAction @ChangeBot => m_Wrapper.m_BotSpawning_ChangeBot;
        public InputActionMap Get() { return m_Wrapper.m_BotSpawning; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(BotSpawningActions set) { return set.Get(); }
        public void SetCallbacks(IBotSpawningActions instance)
        {
            if (m_Wrapper.m_BotSpawningActionsCallbackInterface != null)
            {
                @SpawnBot.started -= m_Wrapper.m_BotSpawningActionsCallbackInterface.OnSpawnBot;
                @SpawnBot.performed -= m_Wrapper.m_BotSpawningActionsCallbackInterface.OnSpawnBot;
                @SpawnBot.canceled -= m_Wrapper.m_BotSpawningActionsCallbackInterface.OnSpawnBot;
                @ChangeBot.started -= m_Wrapper.m_BotSpawningActionsCallbackInterface.OnChangeBot;
                @ChangeBot.performed -= m_Wrapper.m_BotSpawningActionsCallbackInterface.OnChangeBot;
                @ChangeBot.canceled -= m_Wrapper.m_BotSpawningActionsCallbackInterface.OnChangeBot;
            }
            m_Wrapper.m_BotSpawningActionsCallbackInterface = instance;
            if (instance != null)
            {
                @SpawnBot.started += instance.OnSpawnBot;
                @SpawnBot.performed += instance.OnSpawnBot;
                @SpawnBot.canceled += instance.OnSpawnBot;
                @ChangeBot.started += instance.OnChangeBot;
                @ChangeBot.performed += instance.OnChangeBot;
                @ChangeBot.canceled += instance.OnChangeBot;
            }
        }
    }
    public BotSpawningActions @BotSpawning => new BotSpawningActions(this);

    // Resolution
    private readonly InputActionMap m_Resolution;
    private IResolutionActions m_ResolutionActionsCallbackInterface;
    private readonly InputAction m_Resolution_ChangeResolution;
    public struct ResolutionActions
    {
        private @DebugControls m_Wrapper;
        public ResolutionActions(@DebugControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @ChangeResolution => m_Wrapper.m_Resolution_ChangeResolution;
        public InputActionMap Get() { return m_Wrapper.m_Resolution; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ResolutionActions set) { return set.Get(); }
        public void SetCallbacks(IResolutionActions instance)
        {
            if (m_Wrapper.m_ResolutionActionsCallbackInterface != null)
            {
                @ChangeResolution.started -= m_Wrapper.m_ResolutionActionsCallbackInterface.OnChangeResolution;
                @ChangeResolution.performed -= m_Wrapper.m_ResolutionActionsCallbackInterface.OnChangeResolution;
                @ChangeResolution.canceled -= m_Wrapper.m_ResolutionActionsCallbackInterface.OnChangeResolution;
            }
            m_Wrapper.m_ResolutionActionsCallbackInterface = instance;
            if (instance != null)
            {
                @ChangeResolution.started += instance.OnChangeResolution;
                @ChangeResolution.performed += instance.OnChangeResolution;
                @ChangeResolution.canceled += instance.OnChangeResolution;
            }
        }
    }
    public ResolutionActions @Resolution => new ResolutionActions(this);

    // Map
    private readonly InputActionMap m_Map;
    private IMapActions m_MapActionsCallbackInterface;
    private readonly InputAction m_Map_SwitchMap;
    private readonly InputAction m_Map_RecalculatePathfinder;
    public struct MapActions
    {
        private @DebugControls m_Wrapper;
        public MapActions(@DebugControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @SwitchMap => m_Wrapper.m_Map_SwitchMap;
        public InputAction @RecalculatePathfinder => m_Wrapper.m_Map_RecalculatePathfinder;
        public InputActionMap Get() { return m_Wrapper.m_Map; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MapActions set) { return set.Get(); }
        public void SetCallbacks(IMapActions instance)
        {
            if (m_Wrapper.m_MapActionsCallbackInterface != null)
            {
                @SwitchMap.started -= m_Wrapper.m_MapActionsCallbackInterface.OnSwitchMap;
                @SwitchMap.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnSwitchMap;
                @SwitchMap.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnSwitchMap;
                @RecalculatePathfinder.started -= m_Wrapper.m_MapActionsCallbackInterface.OnRecalculatePathfinder;
                @RecalculatePathfinder.performed -= m_Wrapper.m_MapActionsCallbackInterface.OnRecalculatePathfinder;
                @RecalculatePathfinder.canceled -= m_Wrapper.m_MapActionsCallbackInterface.OnRecalculatePathfinder;
            }
            m_Wrapper.m_MapActionsCallbackInterface = instance;
            if (instance != null)
            {
                @SwitchMap.started += instance.OnSwitchMap;
                @SwitchMap.performed += instance.OnSwitchMap;
                @SwitchMap.canceled += instance.OnSwitchMap;
                @RecalculatePathfinder.started += instance.OnRecalculatePathfinder;
                @RecalculatePathfinder.performed += instance.OnRecalculatePathfinder;
                @RecalculatePathfinder.canceled += instance.OnRecalculatePathfinder;
            }
        }
    }
    public MapActions @Map => new MapActions(this);
    public interface IBotSpawningActions
    {
        void OnSpawnBot(InputAction.CallbackContext context);
        void OnChangeBot(InputAction.CallbackContext context);
    }
    public interface IResolutionActions
    {
        void OnChangeResolution(InputAction.CallbackContext context);
    }
    public interface IMapActions
    {
        void OnSwitchMap(InputAction.CallbackContext context);
        void OnRecalculatePathfinder(InputAction.CallbackContext context);
    }
}
