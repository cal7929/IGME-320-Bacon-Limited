using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// Reads the player's input and passes it on to the camera, the possession handler,
/// and the creature currently being controlled. I've seperate movement and possession 
/// mechanics to those scripts, this only handles input.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    OrbitalCamera orbitalCamera;

    [SerializeField]
    PossessionHandler possessionHandler;

    public string menuName = "Level Selector";

    //Connects to the input action system
    InputAction move, look, jump, interact, reload, mainmenu;

    void Awake()
    {
        move = InputSystem.actions.FindAction("Player/Move", true);
        look = InputSystem.actions.FindAction("Player/Look", true);
        jump = InputSystem.actions.FindAction("Player/Jump", true);
        interact = InputSystem.actions.FindAction("Player/Interact", true);
        reload = InputSystem.actions.FindAction("Player/Reload", true);
        mainmenu = InputSystem.actions.FindAction("Player/MainMenu", true);
    }

    void Start()
    {
        //Hide and lock the mouse cursor so mouse movement only rotates the camera
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        //Read mouse input every frame so the orbital camera can function
        orbitalCamera.Look(look.ReadValue<Vector2>());

        //Interact possesses and un-possesses a host creature
        if (interact.WasPerformedThisFrame())
        {
            possessionHandler.Interact();
        }

        //Jump only when the button was pressed this frame
        if (jump.WasPressedThisFrame())
        {
            possessionHandler.CurrentMovement.Jump();
        }

        if (reload.WasPerformedThisFrame())
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (mainmenu.WasPerformedThisFrame())
        {
            SceneManager.LoadScene(menuName);
        }
    }

    void FixedUpdate()
    {
        //Movement is relative to the camera
        possessionHandler.CurrentMovement.Move(move.ReadValue<Vector2>(), orbitalCamera.Yaw);
    }
}