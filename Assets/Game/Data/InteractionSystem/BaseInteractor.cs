using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Data.Interactions
{
    public class UniversalInteractor : MonoBehaviour, IInteractor
    {
        [SerializeField] float m_RaycastMaxDistance = 10.0f;

        IInteractable m_Interactable;

        bool m_IsInteracting;

        public void OnStopInteraction()
        {
            if (m_Interactable != null && m_IsInteracting)
            {
                m_Interactable.OnStopInteraction();

                m_IsInteracting = false;
            }
        }

        public void OnUnfocus()
        {
            if (m_Interactable != null)
            {
                OnStopInteraction();

                m_Interactable.OnUnfocus();

                m_Interactable = null;
            }
        }

        public void OnFocus(IInteractable interactable)
        {
            OnUnfocus();

            m_Interactable = interactable;
            m_Interactable.OnFocus();
        }

        public void OnStartInteraction()
        {
            if (m_Interactable != null)
            {
                m_IsInteracting = true;

                m_Interactable.OnStartInteraction();

                OnStopInteraction();
            }
        }

        public void OnCheckForInteractable()
        {
            if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hitInfo, m_RaycastMaxDistance) && hitInfo.collider.TryGetComponent(out IInteractable interactable))
            {
                if (m_Interactable != interactable)
                    OnFocus(interactable);
            }
            else OnUnfocus();
        }

        void InitializeComponent()
        {
            InvokeRepeating(nameof(OnCheckForInteractable), 0.0f, 1.0f);
        }

        void FinalizeComponent()
        {
            CancelInvoke();
        }

        void OnEnable()
        {
            InitializeComponent();
        }

        void OnDisable()
        {
            FinalizeComponent();
        }
    }
}
