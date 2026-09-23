using UnityEngine;
using UnityEngine.InputSystem;

public class JoystickLocomotion : MonoBehaviour
{
    [Header("Références")]
    public Transform head; // Main Camera du XR Origin

    [Header("Déplacement (joystick gauche)")]
    public float moveSpeed = 2f;

    [Header("Rotation (joystick droit)")]
    public float turnAngle = 45f;      // 30 ou 45
    public float turnThreshold = 0.7f;

    InputAction moveAction;
    InputAction turnAction;
    CharacterController cc;
    bool turnReady = true;

    void Awake()
    {
        // Joystick gauche = déplacement
        moveAction = new InputAction("Move", InputActionType.Value,
            "<XRController>{LeftHand}/{Primary2DAxis}");
        // Joystick droit = rotation
        turnAction = new InputAction("Turn", InputActionType.Value,
            "<XRController>{RightHand}/{Primary2DAxis}");

        cc = GetComponent<CharacterController>();
        if (head == null) head = Camera.main.transform;
    }

    void OnEnable()  { moveAction.Enable();  turnAction.Enable(); }
    void OnDisable() { moveAction.Disable(); turnAction.Disable(); }

    void Update()
    {
        // --- Déplacement : avant / arrière / gauche / droite ---
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 forward = Vector3.ProjectOnPlane(head.forward, Vector3.up).normalized;
        Vector3 right   = Vector3.ProjectOnPlane(head.right,   Vector3.up).normalized;
        Vector3 move = (forward * input.y + right * input.x) * moveSpeed * Time.deltaTime;

        if (cc != null && cc.enabled) cc.Move(move);
        else transform.position += move;

        // --- Rotation par pas (snap turn) ---
        float x = turnAction.ReadValue<Vector2>().x;
        if (turnReady && Mathf.Abs(x) > turnThreshold)
        {
            transform.RotateAround(head.position, Vector3.up, Mathf.Sign(x) * turnAngle);
            turnReady = false;
        }
        else if (Mathf.Abs(x) < 0.2f)
        {
            turnReady = true; // le joystick est revenu au centre
        }
    }
}