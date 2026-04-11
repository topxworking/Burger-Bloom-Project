using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ServingCounter : MonoBehaviour, IInteractable
{
    [Header("Slot")]
    [SerializeField] private Transform _placeSlot;
    //[SerializeField] private int _maxPlates = 3;

    [Header("References")]
    [SerializeField] private OrderBoard _orderBoard;
    [SerializeField] private CustomerSpawner _spawner;

    private BurgerAssembly _plateBurger;

    public string GetPromptText() => 
        _plateBurger == null ? "Place Burger Here [E]" : "Pick Up Plate [E]";

    public bool CanInteract(PlayerInteract player) => true;

    public void Interact(PlayerInteract player)
    {
        if (player.Hands.IsHolding &&
            player.Hands.HeldIngredient is BurgerAssemblyIngredient bai &&
            _plateBurger == null)
        {
            player.Hands.PlaceAt(_placeSlot);
            _plateBurger = bai.Assembly;
            TryAutoServe();
            return;
        }
    }

    private void TryAutoServe()
    {
        if (_plateBurger == null) return;
        if (_orderBoard == null) return;

        foreach (var order in _orderBoard.ActiveOrders)
        {
            float score = _plateBurger.ScoreAgainstOrder(order);
            if (score >= 0.7)
            {
                EventBus.Publish(new OnOrderCompleted
                {
                    Order = order,
                    Success = true,
                    Tip = Mathf.RoundToInt(score * 15f)
                });

                Destroy(_plateBurger.gameObject, 0.5f);
                _plateBurger = null;
                return;
            }
        }
    }
}
