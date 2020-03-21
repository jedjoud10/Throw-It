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
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ChangeBot"",
                    ""type"": ""Button"",
                    ""id"": ""457bfcf3-a598-4f02-9090-d5c9a19b9fa5"",
                    ""expectedControlType"": """",
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
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""1a6e9a05-dcbf-490b-9963-0a8e426e2868"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""b6850ecc-ba56-4464-b31b-5ef986378a56"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
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
        m_Resolution_Newaction = m_Resolution.FindAction("New action", throwIfNotFound: true);
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
    private readonly InputAction m_Resolution_Newaction;
    public struct ResolutionActions
    {
        private @DebugControls m_Wrapper;
        public ResolutionActions(@DebugControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Resolution_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Resolution; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(ResolutionActions set) { return set.Get(); }
        public void SetCallbacks(IResolutionActions instance)
        {
            if (m_Wrapper.m_ResolutionActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_ResolutionActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_ResolutionActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_ResolutionActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_ResolutionActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public ResolutionActions @Resolution => new ResolutionActions(this);
    public interface IBotSpawningActions
    {
        void OnSpawnBot(InputAction.CallbackContext context);
        void OnChangeBot(InputAction.CallbackContext context);
    }
    public interface IResolutionActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
