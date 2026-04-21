using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrillStation : MonoBehaviour
{
    [Header("Cook Timing")]
    public float timeToCooked = 5f;
    public float timeToBurnt = 4f;

    [Header("VFX")]
    public ParticleSystem smokeEffect;

    private Dictionary<Ingredient, Coroutine> cooking = new();

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Ingredient meat)) return;
        if (meat.type != IngredientType.Meat) return;
        if (meat.cookState != CookState.Raw) return;

        var rb = other.GetComponent<Rigidbody>();
        rb.isKinematic = true;

        Coroutine c = StartCoroutine(CookRoutine(meat));
        cooking[meat] = c;

        UpdateSmoke();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Ingredient meat)) return;
        StopCooking(meat);
        UpdateSmoke();
    }

    IEnumerator CookRoutine(Ingredient meat)
    {
        yield return new WaitForSeconds(timeToCooked);
        if (meat == null) yield break;
        meat.SetCookState(CookState.Cooked);

        yield return new WaitForSeconds(timeToBurnt);
        if (meat == null) yield break;
        meat.SetCookState(CookState.Burnt);

        cooking.Remove(meat);
        UpdateSmoke();
    }

    public void StopCooking(Ingredient meat)
    {
        if (cooking.TryGetValue(meat, out Coroutine c))
        {
            StopCoroutine(c);
            cooking.Remove(meat);
        }
        UpdateSmoke();
    }

    void UpdateSmoke()
    {
        if (smokeEffect == null) return;

        if (cooking.Count > 0)
        {
            if (!smokeEffect.isPlaying)
                smokeEffect.Play();
        }
        else
        {
            if (smokeEffect.isPlaying)
                smokeEffect.Stop();
        }
    }
}
