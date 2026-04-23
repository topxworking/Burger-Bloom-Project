using UnityEngine;

public enum FridgeType { Beef, Chicken, Bun }

public class Fridge : MonoBehaviour, IInteractable
{
    [Header("Settings")]
    public FridgeType fridgeType;
    public GameObject ingredientPrefab;
    public Transform spawnPoint;

    private GameInputs input;
    private InteractionSystem interaction;

    void Awake()
    {
        input = new GameInputs();
        interaction = FindAnyObjectByType<InteractionSystem>();
    }

    void OnEnable()
    {
        input.Enable();
        input.Player.Interact.performed += _ => TryGive();
    }

    void OnDisable()
    {
        input.Disable();
        input.Player.Interact.performed -= _ => TryGive();
    }

    void TryGive()
    {
        if (!DayManager.Instance.IsOpen) return;
        if (interaction == null) return;
        if (interaction.IsHolding()) return;

        Camera cam = Camera.main;
        if (!Physics.Raycast(cam.transform.position, cam.transform.forward,
                             out RaycastHit hit, 2.5f)) return;
        if (hit.collider.gameObject != gameObject) return;

        bool hasStock = fridgeType switch
        {
            FridgeType.Beef => StockManager.Instance.UseBeef(),
            FridgeType.Chicken => StockManager.Instance.UseChicken(),
            FridgeType.Bun => StockManager.Instance.UseBun(),
            _ => false
        };

        if (!hasStock) return;

        GameObject obj = Instantiate(ingredientPrefab,
                                     spawnPoint.position,
                                     spawnPoint.rotation);

        var pickable = obj.GetComponent<IPickable>();
        if (pickable != null)
            interaction.ForcePickup(pickable);
    }

    public string InteractPrompt => fridgeType switch
    {
        FridgeType.Beef => "Beef Patty",
        FridgeType.Chicken => "Chicken Patty",
        FridgeType.Bun => "Bun",
        _ => "Fridge"
    };

    public UIPrompt UIPrompt => GetComponentInChildren<UIPrompt>(true);
}