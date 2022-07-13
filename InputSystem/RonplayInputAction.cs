// GENERATED AUTOMATICALLY FROM 'Assets/RonplayBoxGameDev/InputSystem/RonplayInputAction.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @RonplayInputAction : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @RonplayInputAction()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""RonplayInputAction"",
    ""maps"": [
        {
            ""name"": ""Default"",
            ""id"": ""491b8a27-3f71-4e28-b45a-93b988a1282e"",
            ""actions"": [
                {
                    ""name"": ""EnterKey"",
                    ""type"": ""Button"",
                    ""id"": ""5fe3bdb8-f0a0-4bb3-8f28-45148029a2ad"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""02ff604d-35d7-43fb-a48f-6045106f551c"",
                    ""path"": ""<Keyboard>/enter"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EnterKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""45f89f2b-b2ae-4af6-9bfc-244b49d9deaf"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EnterKey"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Default
        m_Default = asset.FindActionMap("Default", throwIfNotFound: true);
        m_Default_EnterKey = m_Default.FindAction("EnterKey", throwIfNotFound: true);
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

    // Default
    private readonly InputActionMap m_Default;
    private IDefaultActions m_DefaultActionsCallbackInterface;
    private readonly InputAction m_Default_EnterKey;
    public struct DefaultActions
    {
        private @RonplayInputAction m_Wrapper;
        public DefaultActions(@RonplayInputAction wrapper) { m_Wrapper = wrapper; }
        public InputAction @EnterKey => m_Wrapper.m_Default_EnterKey;
        public InputActionMap Get() { return m_Wrapper.m_Default; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DefaultActions set) { return set.Get(); }
        public void SetCallbacks(IDefaultActions instance)
        {
            if (m_Wrapper.m_DefaultActionsCallbackInterface != null)
            {
                @EnterKey.started -= m_Wrapper.m_DefaultActionsCallbackInterface.OnEnterKey;
                @EnterKey.performed -= m_Wrapper.m_DefaultActionsCallbackInterface.OnEnterKey;
                @EnterKey.canceled -= m_Wrapper.m_DefaultActionsCallbackInterface.OnEnterKey;
            }
            m_Wrapper.m_DefaultActionsCallbackInterface = instance;
            if (instance != null)
            {
                @EnterKey.started += instance.OnEnterKey;
                @EnterKey.performed += instance.OnEnterKey;
                @EnterKey.canceled += instance.OnEnterKey;
            }
        }
    }
    public DefaultActions @Default => new DefaultActions(this);
    public interface IDefaultActions
    {
        void OnEnterKey(InputAction.CallbackContext context);
    }
}
