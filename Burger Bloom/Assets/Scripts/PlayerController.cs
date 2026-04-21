using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float gravity = -9.81f;

    [Header("Camera")]
    public Transform cameraTransform;
    public float mouseSensitivity = 20f;

    private CharacterController cc;
    private GameInputs input;
    private Vector3 velocity;
    private float xRotation;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        input = new GameInputs();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnEnable() => input.Enable();
    void OnDisable() => input.Disable();

    void Update()
    {
        HandleLook();
        HandleMove();
    }

    void HandleLook()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        Vector2 look = Mouse.current.delta.ReadValue() * mouseSensitivity * Time.deltaTime;
        xRotation -= look.y;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * look.x);
    }

    void HandleMove()
    {
        Vector2 moveInput = input.Player.Move.ReadValue<Vector2>();
        bool sprinting = input.Player.Sprint.IsPressed();
        float speed = sprinting ? runSpeed : walkSpeed;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        cc.Move(move * speed * Time.deltaTime);

        if (cc.isGrounded && velocity.y < 0) velocity.y = -2f;
        velocity.y += gravity * Time.deltaTime;
        cc.Move(velocity * Time.deltaTime);
    }
}
