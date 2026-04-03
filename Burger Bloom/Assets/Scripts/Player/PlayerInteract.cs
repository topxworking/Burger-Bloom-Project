using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
    [Header("Interaction Settings")]
    [SerializeField] private float _reach = 2.5f;
    [SerializeField] private LayerMask _interactMask;
    [SerializeField] Transform _holdPoint;

    [Header("UI Feedback")]
    [SerializeField] private GameObject _interactPrompt;
    [SerializeField] private TMPro.TextMeshProUGUI _promptText;

    private GameInputs _input;
    private Camera _cam;
}
