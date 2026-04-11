using UnityEngine;

public class PlayerHands : MonoBehaviour
{
    [SerializeField] private Transform _holdPoint;
    [SerializeField] private float _smoothSpeed = 20f;

    private Ingredient _heldIngredient;
    private Rigidbody _heldRb;

    public bool IsHolding => _heldIngredient != null;
    public Ingredient HeldIngredient => _heldIngredient;

    private void Update()
    {
        if (_heldRb == null) return;
        // Smooth follow
        _heldRb.transform.position = Vector3.Lerp(
            _heldRb.transform.position,
            _holdPoint.position,
            Time.deltaTime * _smoothSpeed);
        _heldRb.transform.rotation = Quaternion.Slerp(
            _heldRb.transform.rotation,
            _holdPoint.rotation,
            Time.deltaTime * _smoothSpeed);
    }

    public bool PickUp(Ingredient ingredient)
    {
        if (IsHolding) return false;

        _heldIngredient = ingredient;
        _heldRb = ingredient.GetComponent<Rigidbody>();

        if (_heldRb != null)
        {
            _heldRb.useGravity = false;
            _heldRb.isKinematic = false;
            _heldRb.linearVelocity = Vector3.zero;
            _heldRb.angularVelocity = Vector3.zero;
        }

        ingredient.OnPickedUp();
        EventBus.Publish(new OnIngredientPickedUp { IngredientId = ingredient.IngredientId });
        return true;
    }

    public Ingredient Drop()
    {
        if (!IsHolding) return null;

        var dropped = _heldIngredient;

        if (_heldRb != null)
            _heldRb.useGravity = true;

        dropped.OnDropped();
        EventBus.Publish(new OnIngredientPlaced { IngredientId = dropped.IngredientId });

        _heldIngredient = null;
        _heldRb = null;
        return dropped;
    }

    public Ingredient PlaceAt(Transform target)
    {
        var dropped = _heldIngredient;
        if (dropped == null) return null;

        if (_heldRb != null)
        {
            _heldRb.useGravity = false;
            _heldRb.isKinematic = true;
        }

        dropped.transform.SetParent(target);
        dropped.transform.localPosition = Vector3.zero;
        dropped.transform.localRotation = Quaternion.identity;
        dropped.OnPlaced();

        _heldIngredient = null;
        _heldRb = null;
        return dropped;
    }
}
