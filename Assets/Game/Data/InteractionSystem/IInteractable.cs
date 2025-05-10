using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Data.InteractionSystem
{
    public interface IInteractable
    {
        bool MakeSelectiveInteractions { get; }
        List<GameObject> AllowedInteractors { get; }

        void Focus(IInteractor interactor);

        void Unfocus(IInteractor interactor);

        void SetOriginalInteractable(IInteractable interactable);
    }
}
