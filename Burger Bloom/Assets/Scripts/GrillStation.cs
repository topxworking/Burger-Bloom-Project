using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrillStation : MonoBehaviour
{
    [Header("Cook Timing")]
    public float timeToCooked = 5f;
    public float timeToBurnt = 4f;

    public UpgradeData upgradeData;

    private Dictionary<Ingredient, Coroutine> cooking = new();

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Ingredient meat)) return;
        if (meat.ingredientType != IngredientType.Meat) return;

        if (meat.cookState == CookState.Burnt) return;
        if (cooking.ContainsKey(meat)) return;

        meat.SetOnGrill(true);
        meat.StartSmoke();

        Coroutine c = meat.cookState == CookState.Raw
        ? StartCoroutine(CookRoutine(meat))
        : StartCoroutine(BurnRoutine(meat));

        cooking[meat] = c;
        UpdateGrillSound();
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Ingredient meat)) return;
        StopCooking(meat);
    }

    IEnumerator CookRoutine(Ingredient meat)
    {
        yield return new WaitForSeconds(timeToCooked);
        if (meat == null) { cooking.Remove(meat); UpdateGrillSound(); yield break; }
        meat.SetCookState(CookState.Cooked);

        yield return new WaitForSeconds(timeToBurnt);
        if (meat == null) { cooking.Remove(meat); UpdateGrillSound(); yield break; }
        meat.SetCookState(CookState.Burnt);

        cooking.Remove(meat);
        UpdateGrillSound();
    }

    public void StopCooking(Ingredient meat)
    {
        if (cooking.TryGetValue(meat, out Coroutine c))
        {
            StopCoroutine(c);
            cooking.Remove(meat);
        }

        meat?.SetOnGrill(false);
        meat?.StopSmoke();
        UpdateGrillSound();
    }

    public void RefreshCookTimes()
    {
        if (upgradeData == null) return;
        timeToCooked = upgradeData.GetCookTime();
        timeToBurnt = upgradeData.GetBurnTime();
    }

    IEnumerator BurnRoutine(Ingredient meat)
    {
        yield return new WaitForSeconds(timeToBurnt);
        if (meat == null) { cooking.Remove(meat); UpdateGrillSound(); yield break; }
        meat.SetCookState(CookState.Burnt);

        cooking.Remove(meat);
        UpdateGrillSound();
    }

    void UpdateGrillSound()
    {
        if (cooking.Count > 0)
            SoundManager.Instance.PlayGrill();
        else
            SoundManager.Instance.StopGrill();
    }
}
