using UnityEngine;

public class TrashCan : MonoBehaviour
{
    [Header("SFX")]
    public AudioClip trashSound;
    public float trashVolume = 0.2f;

    [Header("VFX")]
    public ParticleSystem trashEffect;

    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ingredient ing))
        {
            if (ing.IsHeld()) return;
            PlayTrashSound(other.transform.position);
            PlayEffects(other.transform.position);
            Destroy(ing.gameObject);
            return;
        }

        if (other.TryGetComponent(out BurgerStack burger))
        {
            if (burger.IsHeld()) return;
            PlayTrashSound(other.transform.position);
            PlayEffects(other.transform.position);
            Destroy(burger.gameObject);
            return;
        }
    }

    void PlayTrashSound(Vector3 pos)
    {
        if (trashSound)
            AudioSource.PlayClipAtPoint(trashSound, pos, trashVolume);
    }

    void PlayEffects(Vector3 pos)
    {
        if (trashSound)
            AudioSource.PlayClipAtPoint(trashSound, pos, trashVolume);

        if (trashEffect)
        {
            trashEffect.transform.position = pos;
            trashEffect.Play();
        }
    }
}