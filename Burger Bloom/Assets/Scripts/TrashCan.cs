using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    [Header("SFX")]
    public AudioClip trashSound;
    public float trashVolume = 0.2f;

    public string InteractPrompt => "Trash Can";
    public UIPrompt UIPrompt => GetComponentInChildren<UIPrompt>(true);

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ingredient ing))
        {
            if (ing.IsHeld()) return;
            PlayTrashSound(other.transform.position);
            Destroy(ing.gameObject);
            return;
        }

        if (other.TryGetComponent(out BurgerStack burger))
        {
            if (burger.IsHeld()) return;
            PlayTrashSound(other.transform.position);
            Destroy(burger.gameObject);
            return;
        }
    }

    void PlayTrashSound(Vector3 pos)
    {
        if (trashSound)
            AudioSource.PlayClipAtPoint(trashSound, pos, trashVolume);
    }
}