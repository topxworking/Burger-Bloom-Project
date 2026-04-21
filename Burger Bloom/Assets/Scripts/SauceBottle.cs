using UnityEngine;

public enum SauceType { Tomato, Chili, Mustard }

public class SauceBottle : MonoBehaviour, IPickable
{
    public SauceType sauceType;

    private Rigidbody rb;
    private Collider col;
    private bool isHeld;

    public bool CanPickup => !isHeld;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    public void OnPickup(Transform holdPoint)
    {
        if (holdPoint == null) return;
        isHeld = true;
        rb.isKinematic = true;
        rb.useGravity = false;
        col.enabled = false;
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public void OnDrop()
    {
        isHeld = false;
        rb.isKinematic = false;
        rb.useGravity = true;
        col.enabled = true;
        transform.SetParent(null);
    }

    public void OnThrow(Vector3 force)
    {
        OnDrop();
        rb.AddForce(force, ForceMode.Impulse);
    }
}