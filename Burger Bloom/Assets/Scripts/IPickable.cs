using UnityEngine;

public interface IPickable
{
    bool CanPickup { get; }
    void OnPickup(Transform holdPoint);
    void OnDrop();
    void OnThrow(Vector3 force);
}
