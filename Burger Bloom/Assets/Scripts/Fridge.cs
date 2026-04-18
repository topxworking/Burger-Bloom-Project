using UnityEngine;

public class Fridge : MonoBehaviour
{
    public GameObject ingredientPrefab;
    public Transform spawnPoint;

    private GameInputs input;
    private InteractionSystem playerInteraction;

    void Awake() => input = new GameInputs();
    void OnEnable()
    {
        input.Enable();
        input.Player.Interact.performed += _ => TrySpawn();
    }
    void OnDisable()
    {
        input.Disable();
        input.Player.Interact.performed -= _ => TrySpawn();
    }

    void TrySpawn()
    {
        Camera cam = Camera.main;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward,
                            out RaycastHit hit, 2.5f))
        {
            if (hit.collider.gameObject == gameObject)
            {
                var interaction = FindAnyObjectByType<InteractionSystem>();
                if (interaction != null && interaction.IsHolding()) return;

                Instantiate(ingredientPrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
    }
}
