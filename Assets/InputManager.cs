// GENERATED AUTOMATICALLY FROM 'Assets/InputManager.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputManager : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputManager()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputManager"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""ce8b8d77-0066-4f35-8719-1369b8f69ef7"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""9484fbe5-a982-4e93-bee3-d4bee251ecaa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""ea0201f4-bafa-453e-992a-3865c3a97622"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""ADHorizontal"",
                    ""id"": ""64ef3f88-f05b-40be-8492-effe8f2bf00d"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""c8adf155-3d11-446f-930b-1a39c34a9e7f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""b70b9942-e35f-4fca-a950-fe334a99df98"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""LeftRightHorizontal"",
                    ""id"": ""6f10441d-9d68-4c41-87b2-65f54daf772a"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""efb04f3d-75a7-44a4-85cb-659480158b2a"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""7aeb4eeb-f220-426c-9dc0-2f93c8db62e5"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""DPadHorizontal"",
                    ""id"": ""c197a765-c782-48ba-9756-4be151922d77"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""230780ec-acd3-4e64-9f2f-ff5928239552"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""9957b9ae-4468-42aa-bbc0-ea8b61d60368"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""StickHorizontal"",
                    ""id"": ""d82cfdcb-e7e9-45c2-b58b-6c9ce5defc1e"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""4a94cb79-592f-468f-ae03-731719f02704"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""15ac148d-28dd-43e7-bd4a-f5cb3a8f0148"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""c08aeb19-c767-48d3-a89a-3b78838d796f"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c54b118f-8cc7-4ca9-a35f-e735bdff7b58"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Debug"",
            ""id"": ""36b14c2e-7c6a-41e1-a9a5-25b2c07db956"",
            ""actions"": [
                {
                    ""name"": ""DebugLog"",
                    ""type"": ""Button"",
                    ""id"": ""2084785d-0533-4734-8479-ee71037032ef"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DebugReset"",
                    ""type"": ""Button"",
                    ""id"": ""63a7a194-40fa-4cfb-94c8-0ad7fbf4341e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""56dc8a65-e82a-4859-8502-5bdf7c7da7da"",
                    ""path"": ""<Keyboard>/backquote"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""DebugLog"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cbc55f3e-6367-48fe-a00c-e776c23bea7b"",
                    ""path"": ""<Keyboard>/numpadPlus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""DebugLog"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""12b043e8-eae4-4929-8e5c-1560d762fb26"",
                    ""path"": ""<Keyboard>/minus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""DebugReset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a8504dcf-42bd-441a-ac1a-99680c8815fb"",
                    ""path"": ""<Keyboard>/numpadMinus"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""DebugReset"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""PlayerUI"",
            ""id"": ""c00624e4-4ea4-40b3-80e9-b79cfb92b4fd"",
            ""actions"": [
                {
                    ""name"": ""CraftMenu"",
                    ""type"": ""Button"",
                    ""id"": ""ef6d27dc-a05f-4539-b0d1-b762b2d42086"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Inventory"",
                    ""type"": ""Button"",
                    ""id"": ""b084c7e0-a4bb-4d50-a4e5-fe3302d5fee5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EscapeMenu"",
                    ""type"": ""Button"",
                    ""id"": ""f84eaaaa-3da6-48ae-b553-c5972dc1c7ad"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""70309511-55ca-46f4-baf1-cf7b7bd4f632"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""CraftMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""85571113-bbc5-4b76-9a2a-c5fb8dec98f3"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""CraftMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""16b0c599-24ee-4b45-b080-1c743e9eebed"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5b21e33f-17db-4252-b95a-eedf0e2461e3"",
                    ""path"": ""<Keyboard>/b"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""893b9318-6636-49b7-9746-fafdd3ba49be"",
                    ""path"": ""<Gamepad>/leftShoulder"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3149b131-8006-4f5a-a57b-746d2a599ecc"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""PC"",
                    ""action"": ""EscapeMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""18cdcb7d-ed09-4604-b4b8-26b9048a8da8"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""EscapeMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""PC"",
            ""bindingGroup"": ""PC"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Movement = m_Player.FindAction("Movement", throwIfNotFound: true);
        m_Player_Jump = m_Player.FindAction("Jump", throwIfNotFound: true);
        // Debug
        m_Debug = asset.FindActionMap("Debug", throwIfNotFound: true);
        m_Debug_DebugLog = m_Debug.FindAction("DebugLog", throwIfNotFound: true);
        m_Debug_DebugReset = m_Debug.FindAction("DebugReset", throwIfNotFound: true);
        // PlayerUI
        m_PlayerUI = asset.FindActionMap("PlayerUI", throwIfNotFound: true);
        m_PlayerUI_CraftMenu = m_PlayerUI.FindAction("CraftMenu", throwIfNotFound: true);
        m_PlayerUI_Inventory = m_PlayerUI.FindAction("Inventory", throwIfNotFound: true);
        m_PlayerUI_EscapeMenu = m_PlayerUI.FindAction("EscapeMenu", throwIfNotFound: true);
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

    // Player
    private readonly InputActionMap m_Player;
    private IPlayerActions m_PlayerActionsCallbackInterface;
    private readonly InputAction m_Player_Movement;
    private readonly InputAction m_Player_Jump;
    public struct PlayerActions
    {
        private @InputManager m_Wrapper;
        public PlayerActions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Player_Movement;
        public InputAction @Jump => m_Wrapper.m_Player_Jump;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerActionsCallbackInterface.OnJump;
            }
            m_Wrapper.m_PlayerActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
            }
        }
    }
    public PlayerActions @Player => new PlayerActions(this);

    // Debug
    private readonly InputActionMap m_Debug;
    private IDebugActions m_DebugActionsCallbackInterface;
    private readonly InputAction m_Debug_DebugLog;
    private readonly InputAction m_Debug_DebugReset;
    public struct DebugActions
    {
        private @InputManager m_Wrapper;
        public DebugActions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @DebugLog => m_Wrapper.m_Debug_DebugLog;
        public InputAction @DebugReset => m_Wrapper.m_Debug_DebugReset;
        public InputActionMap Get() { return m_Wrapper.m_Debug; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(DebugActions set) { return set.Get(); }
        public void SetCallbacks(IDebugActions instance)
        {
            if (m_Wrapper.m_DebugActionsCallbackInterface != null)
            {
                @DebugLog.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnDebugLog;
                @DebugLog.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnDebugLog;
                @DebugLog.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnDebugLog;
                @DebugReset.started -= m_Wrapper.m_DebugActionsCallbackInterface.OnDebugReset;
                @DebugReset.performed -= m_Wrapper.m_DebugActionsCallbackInterface.OnDebugReset;
                @DebugReset.canceled -= m_Wrapper.m_DebugActionsCallbackInterface.OnDebugReset;
            }
            m_Wrapper.m_DebugActionsCallbackInterface = instance;
            if (instance != null)
            {
                @DebugLog.started += instance.OnDebugLog;
                @DebugLog.performed += instance.OnDebugLog;
                @DebugLog.canceled += instance.OnDebugLog;
                @DebugReset.started += instance.OnDebugReset;
                @DebugReset.performed += instance.OnDebugReset;
                @DebugReset.canceled += instance.OnDebugReset;
            }
        }
    }
    public DebugActions @Debug => new DebugActions(this);

    // PlayerUI
    private readonly InputActionMap m_PlayerUI;
    private IPlayerUIActions m_PlayerUIActionsCallbackInterface;
    private readonly InputAction m_PlayerUI_CraftMenu;
    private readonly InputAction m_PlayerUI_Inventory;
    private readonly InputAction m_PlayerUI_EscapeMenu;
    public struct PlayerUIActions
    {
        private @InputManager m_Wrapper;
        public PlayerUIActions(@InputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @CraftMenu => m_Wrapper.m_PlayerUI_CraftMenu;
        public InputAction @Inventory => m_Wrapper.m_PlayerUI_Inventory;
        public InputAction @EscapeMenu => m_Wrapper.m_PlayerUI_EscapeMenu;
        public InputActionMap Get() { return m_Wrapper.m_PlayerUI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerUIActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerUIActions instance)
        {
            if (m_Wrapper.m_PlayerUIActionsCallbackInterface != null)
            {
                @CraftMenu.started -= m_Wrapper.m_PlayerUIActionsCallbackInterface.OnCraftMenu;
                @CraftMenu.performed -= m_Wrapper.m_PlayerUIActionsCallbackInterface.OnCraftMenu;
                @CraftMenu.canceled -= m_Wrapper.m_PlayerUIActionsCallbackInterface.OnCraftMenu;
                @Inventory.started -= m_Wrapper.m_PlayerUIActionsCallbackInterface.OnInventory;
                @Inventory.performed -= m_Wrapper.m_PlayerUIActionsCallbackInterface.OnInventory;
                @Inventory.canceled -= m_Wrapper.m_PlayerUIActionsCallbackInterface.OnInventory;
                @EscapeMenu.started -= m_Wrapper.m_PlayerUIActionsCallbackInterface.OnEscapeMenu;
                @EscapeMenu.performed -= m_Wrapper.m_PlayerUIActionsCallbackInterface.OnEscapeMenu;
                @EscapeMenu.canceled -= m_Wrapper.m_PlayerUIActionsCallbackInterface.OnEscapeMenu;
            }
            m_Wrapper.m_PlayerUIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @CraftMenu.started += instance.OnCraftMenu;
                @CraftMenu.performed += instance.OnCraftMenu;
                @CraftMenu.canceled += instance.OnCraftMenu;
                @Inventory.started += instance.OnInventory;
                @Inventory.performed += instance.OnInventory;
                @Inventory.canceled += instance.OnInventory;
                @EscapeMenu.started += instance.OnEscapeMenu;
                @EscapeMenu.performed += instance.OnEscapeMenu;
                @EscapeMenu.canceled += instance.OnEscapeMenu;
            }
        }
    }
    public PlayerUIActions @PlayerUI => new PlayerUIActions(this);
    private int m_PCSchemeIndex = -1;
    public InputControlScheme PCScheme
    {
        get
        {
            if (m_PCSchemeIndex == -1) m_PCSchemeIndex = asset.FindControlSchemeIndex("PC");
            return asset.controlSchemes[m_PCSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IPlayerActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
    }
    public interface IDebugActions
    {
        void OnDebugLog(InputAction.CallbackContext context);
        void OnDebugReset(InputAction.CallbackContext context);
    }
    public interface IPlayerUIActions
    {
        void OnCraftMenu(InputAction.CallbackContext context);
        void OnInventory(InputAction.CallbackContext context);
        void OnEscapeMenu(InputAction.CallbackContext context);
    }
}
