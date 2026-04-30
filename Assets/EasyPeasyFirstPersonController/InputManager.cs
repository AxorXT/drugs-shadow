using EasyPeasyFirstPersonController;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, IInputManager
{
    private PlayerInputActions inputActions;

    public Vector2 moveInput { get; private set; }
    public Vector2 lookInput { get; private set; }
    public bool jump { get; private set; }
    public bool sprint { get; private set; }
    public bool crouch { get; private set; }
    public bool slide { get; private set; }
    public bool weedPressed { get; private set; }

    public bool pausePressed { get; private set; }

    private void LateUpdate()
    {
        weedPressed = false;
    }

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        inputActions.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        inputActions.Player.Look.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
        inputActions.Player.Look.canceled += ctx => lookInput = Vector2.zero;

        inputActions.Player.Jump.performed += ctx => jump = true;
        inputActions.Player.Jump.canceled += ctx => jump = false;

        inputActions.Player.Sprint.performed += ctx => sprint = true;
        inputActions.Player.Sprint.canceled += ctx => sprint = false;

        inputActions.Player.Crouch.performed += ctx => crouch = true;
        inputActions.Player.Crouch.canceled += ctx => crouch = false;

        inputActions.Player.Slide.performed += ctx => slide = true;
        inputActions.Player.Slide.canceled += ctx => slide = false;

        inputActions.Player.Weed.performed += ctx => weedPressed = true;
        inputActions.Player.Weed.canceled += ctx => weedPressed = false;

        inputActions.Player.Pause.performed += ctx => pausePressed = true;
        inputActions.Player.Pause.canceled += ctx => pausePressed = false;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
