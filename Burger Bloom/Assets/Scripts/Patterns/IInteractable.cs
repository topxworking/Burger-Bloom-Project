public interface IInteractable
{
    string GetPromptText();
    bool CanInteract(PlayerInteract player);
    void Interact(PlayerInteract player);
}