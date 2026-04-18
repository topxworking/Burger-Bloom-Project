using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrillStation : MonoBehaviour
{
    [Header("Cook Timing (seconds)")]
    public float timeToCooked = 5f;
    public float timeToBurnt = 4f;

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
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Ingredient meat)) return;

        if (cooking.TryGetValue(meat, out Coroutine c))
        {
            StopCoroutine(c);
            cooking.Remove(meat);
        }
    }

    IEnumerator CookRoutine(Ingredient meat)
    {
        yield return new WaitForSeconds(timeToCooked);
        if (meat == null) yield break;

        meat.SetCookState(CookState.Cooked);

        yield return new WaitForSeconds(timeToBurnt);
        if (meat == null) yield break;

        meat.SetCookState(CookState.Burnt);
    }

    public void StopCooking(Ingredient meat)
    {
        if (cooking.TryGetValue(meat, out Coroutine c))
        {
            StopCoroutine(c);
            cooking.Remove(meat);
        }
    }
}
