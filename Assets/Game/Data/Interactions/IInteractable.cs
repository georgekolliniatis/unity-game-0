namespace Assets.Game.Data.Interactions
{
    public interface IInteractable
    {
        void OnStopInteraction();

        void OnUnfocus();

        void OnFocus();

        void OnStartInteraction();
    }
}
