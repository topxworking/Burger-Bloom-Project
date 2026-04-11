using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float _reach = 2.5f;
    [SerializeField] private LayerMask _interactMask;
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private PlayerHands _hands;

    [Header("UI Feedback")]
    [SerializeField] private GameObject _interactPrompt;
    [SerializeField] private TMPro.TextMeshProUGUI _promptText;

    private GameInputs _input;
    private Camera _cam;
    private IInteractable _focused;

    private void Awake()
    {
        _input = new GameInputs();
        _cam = Camera.main;
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.Interact.performed += OnInteract;
    }

    private void OnDisable()
    {
        _input.Disable();
        _input.Player.Interact.performed -= OnInteract;
    }

    private void Update()
    {
        ScanForInteractable();
    }

    private void ScanForInteractable()
    {
        Ray ray = new Ray(_cam.transform.position, _cam.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, _reach, _interactMask))
        {
            var interactable = hit.collider.GetComponentInParent<IInteractable>();
            if (interactable != null && interactable.CanInteract(this))
            {
                SetFocused(interactable);
                return;
            }
        }

        SetFocused(null);
    }

    private void SetFocused(IInteractable next)
    {
        if (_focused == next) return;
        _focused = next;

        if (_focused != null)
        {
            _interactPrompt.SetActive(true);
            _promptText.text = _focused.GetPromptText();
        }
        else
        {
            _interactPrompt.SetActive(false);
        }
    }

    private void OnInteract(InputAction.CallbackContext ctx)
    {
        _focused?.Interact(this);
    }

    public PlayerHands Hands => _hands;
    public Transform HoldPoint => _holdPoint;    
}
