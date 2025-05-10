using UnityEngine.InputSystem;

namespace Assets.Game.Data.InteractionSystem
{
    public interface IInteractor
    {
        void AddInputActions(InputAction[] inputActions);

        void RemoveInputActions(string[] inputActionNames);
    }
}
