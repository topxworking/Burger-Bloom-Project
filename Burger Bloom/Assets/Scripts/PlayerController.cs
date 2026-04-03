using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 3f;
    [SerializeField] private float _sprintSpeed = 5.5f;
    [SerializeField] private float _gravity = -9.81f;

    [Header("Look Settings")]
    [SerializeField] private float _mouseSensitivity = 100f;
    [SerializeField] private float _maxPitch = 80f;
    [SerializeField] private Transform _cameraRoot;

    [Header("Ground Check")]
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundDistance = 0.3f;
    [SerializeField] private LayerMask _groundMask;

    private CharacterController _cc;
    private GameInputs _input;

    private Vector2 _moveInput;
    private Vector2 _lookInput;
    private bool _sprinting;
    private Vector3 _velocity;
    private float _pitch;
    private bool _isGrounded;
    private bool _locked;

    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        _input = new GameInputs();
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.Move.performed += ctx => _moveInput = ctx.ReadValue<Vector2>();
        _input.Player.Move.canceled += ctx => _moveInput = Vector2.zero;
        _input.Player.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _input.Player.Look.canceled += ctx => _lookInput = Vector2.zero;
        _input.Player.Sprint.performed += ctx => _sprinting = true;
        _input.Player.Sprint.canceled += ctx => _sprinting = false;
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (_locked) return;
        HandleLook();
        HandleMove();
    }

    private void HandleLook()
    {
        float yaw = _lookInput.x * _mouseSensitivity * Time.deltaTime;
        float pitch = _lookInput.y * _mouseSensitivity * Time.deltaTime;

        transform.Rotate(Vector3.up * yaw);

        _pitch = Mathf.Clamp(_pitch - pitch, -_maxPitch, _maxPitch);
        _cameraRoot.localRotation = Quaternion.Euler(_pitch, 0f, 0f);
    }

    private void HandleMove()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        float speed = _sprinting ? _sprintSpeed : _walkSpeed;
        Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;
        _cc.Move(move * (speed * Time.deltaTime));

        _velocity.y += _gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }

    public void SetLocked(bool locked)
    {
        _locked = locked;
        Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !locked;
    }
}
