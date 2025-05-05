namespace Assets.Game.Data.Interactions
{
    public interface IInteractor
    {
        void OnStopInteraction();

        void OnUnfocus();

        void OnFocus(IInteractable _);

        void OnStartInteraction();

        void OnCheckForInteractable();
    }
}
