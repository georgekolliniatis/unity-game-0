using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Game.Data.InteractionSystem
{
    public class BaseInteractable : MonoBehaviour, IInteractable
    {
        [SerializeField] protected bool m_MakeSelectiveInteractions;
        public bool MakeSelectiveInteractions { get => m_MakeSelectiveInteractions; }

        [SerializeField] protected List<GameObject> m_AllowedInteractors;
        public List<GameObject> AllowedInteractors { get => m_AllowedInteractors; }

        [SerializeField] protected UnityEvent m_OnFocus;
        [SerializeField] protected UnityEvent m_OnUnfocus;

        protected IInteractable m_OriginalInteractable;

        public virtual void Focus(IInteractor _) => m_OnFocus.Invoke();

        public virtual void Unfocus(IInteractor _) => m_OnUnfocus.Invoke();

        public virtual void SetOriginalInteractable(IInteractable interactable) => m_OriginalInteractable = interactable;

        protected virtual void RunActionCallback(string methodName, Action action)
        {
            if (m_OriginalInteractable == null)
                action.Invoke();
            else GetType().GetMethod(methodName).Invoke(m_OriginalInteractable, null);
        }
    }
}
