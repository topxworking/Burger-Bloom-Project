using System.Collections.Generic;
using UnityEngine;

public class GrillStation : MonoBehaviour, IInteractable
{
    [Header("Slots")]
    [SerializeField] private Transform[] _slots;
    [SerializeField] private int _maxSlots = 4;

    [Header("Heat Effects")]
    [SerializeField] private ParticleSystem[] _grillHeatVFX;
    [SerializeField] private AudioSource _sizzleAudio;

    private MeatPatty[] _patties;

    private void Awake()
    {
        _patties = new MeatPatty[_maxSlots];
    }

    public string GetPromptText()
    {
        return "Place / Flip Patty [E]";
    }

    public bool CanInteract(PlayerInteract player) => true;

    public void Interact(PlayerInteract player)
    {
        if (player.Hands.IsHolding &&
            player.Hands.HeldIngredient is MeatPatty patty)
        {
            TryPlacePatty(patty, player);
            return;
        }

        TryFlipOrRemove(player);
    }

    private void TryPlacePatty(MeatPatty patty, PlayerInteract player)
    {
        for (int i = 0; i < _maxSlots; i++)
        {
            if (_patties[i] == null)
            {
                player.Hands.PlaceAt(_slots[i]);
                _patties[i] = patty;
                patty.PlaceOnGrill();
                _grillHeatVFX[i]?.Play();
                PlaySizzle();
                return;
            }
        }
        Debug.Log("[Grill] No empty slots!");
    }

    private void TryFlipOrRemove(PlayerInteract player)
    {
        for (int i = 0; i < _maxSlots; i++)
        {
            if (_patties[i] == null) continue;

            var p = _patties[i];

            if (p.ReadyToFlip)
            {
                p.Flip();
                return;
            }

            if (p.IsCooked || p.IsBurnt)
            {
                p.RemoveFromGrill();
                _grillHeatVFX[i]?.Stop();
                p.transform.SetParent(null);
                player.Hands.PickUp(p);
                _patties[i] = null;
                return;
            }
        }
    }

    private void PlaySizzle()
    {
        if (_sizzleAudio != null && !_sizzleAudio.isPlaying)
        {
            _sizzleAudio.Play();
        }
    }

    private void Update()
    {
        bool hasAny = false;
        foreach (var p in _patties) if (p != null) { hasAny = true; break; }
        if (!hasAny && _sizzleAudio != null) _sizzleAudio.Stop();
    }
}
