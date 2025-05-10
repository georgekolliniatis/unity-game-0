using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Game.Data.InteractionSystem
{
    [RequireComponent(typeof(PlayerInput))]

    public class BaseInteractor : MonoBehaviour, IInteractor
    {
        [SerializeField] protected float m_ChecksPerSecond = 1.0f;
        [SerializeField] protected List<GameObject> m_InteractableGameObjectsInSight;

        protected PlayerInput m_PlayerInput;
        protected InputActionMap m_ActionMap;
        protected Dictionary<IInteractable, Component> m_InteractablesToComponentCopies;

        public virtual void AddInputActions(InputAction[] inputActions)
        {
            m_ActionMap.Disable();

            foreach (InputAction inputAction in inputActions)
                m_ActionMap.AddAction(
                    inputAction.name,
                    inputAction.type,
                    inputAction.bindings.Count > 0 ? inputAction.bindings[0].path : null,
                    inputAction.interactions,
                    inputAction.processors,
                    inputAction.expectedControlType
                );

            m_ActionMap.Enable();

            RefreshInputActions();
        }

        public virtual void RemoveInputActions(string[] inputActionNames)
        {
            m_ActionMap.Disable();

            foreach (string inputActionName in inputActionNames)
                m_ActionMap.FindAction(inputActionName).RemoveAction();

            m_ActionMap.Enable();

            RefreshInputActions();
        }

        protected virtual void RefreshInputActions() => typeof(PlayerInput).GetMethod("CacheMessageNames", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(m_PlayerInput, null);

        protected virtual void FocusOnOne(GameObject interactableGameObjectInSight, bool withUnfocusFromAll = false)
        {
            if (
                !interactableGameObjectInSight.TryGetComponent(typeof(IInteractable), out Component interactableComponentInSightOriginal)
                || !interactableComponentInSightOriginal.TryGetComponent(out IInteractable interactableInSightOriginal)
                || m_InteractablesToComponentCopies.ContainsKey(interactableInSightOriginal)
                || (interactableInSightOriginal.MakeSelectiveInteractions && !interactableInSightOriginal.AllowedInteractors.Contains(gameObject))
            )
                return;

            if (withUnfocusFromAll)
                UnfocusFromAll();

            m_InteractableGameObjectsInSight.Add(interactableGameObjectInSight);

            m_InteractablesToComponentCopies.Add(interactableInSightOriginal, gameObject.AddComponent(interactableInSightOriginal.GetType()));

            TryGetComponent(out IInteractable interactableInSightCopy);

            interactableInSightCopy.SetOriginalInteractable(interactableInSightOriginal);

            interactableInSightOriginal.Focus(this);
        }

        protected virtual void FocusOnAll()
        {
            UnfocusFromAll();

            m_InteractableGameObjectsInSight.ForEach(interactableGameObjectInSight => FocusOnOne(interactableGameObjectInSight));
        }

        protected virtual void UnfocusFromOne(GameObject interactableGameObjectInSight, bool withRemoveElements = true)
        {
            interactableGameObjectInSight.TryGetComponent(out IInteractable interactableInSight);

            interactableInSight.Unfocus(this);

            Destroy(m_InteractablesToComponentCopies.GetValueOrDefault(interactableInSight));

            if (withRemoveElements)
            {
                m_InteractablesToComponentCopies.Remove(interactableInSight);

                m_InteractableGameObjectsInSight.RemoveAll(interactableGameObject => interactableGameObject == interactableGameObjectInSight);
            }
        }

        protected virtual void UnfocusFromAll()
        {
            m_InteractableGameObjectsInSight.ForEach(interactableGameObjectInSight => UnfocusFromOne(interactableGameObjectInSight, false));

            m_InteractablesToComponentCopies.Clear();

            m_InteractableGameObjectsInSight.Clear();
        }

        protected virtual IEnumerator CheckForInteractables(Action action, bool onlyOnce = false)
        {
            while (true)
            {
                action.Invoke();

                if (m_ChecksPerSecond > 0 && m_ChecksPerSecond <= 60)
                    yield return new WaitForSeconds(1.0f / m_ChecksPerSecond);
                else yield return new WaitForSeconds(1.0f);

                if (onlyOnce)
                    yield break;
            }
        }

        protected virtual void InitializeInteractor()
        {
            TryGetComponent(out m_PlayerInput);

            m_ActionMap = m_PlayerInput.actions.FindActionMap(m_PlayerInput.defaultActionMap);

            m_InteractablesToComponentCopies = new();

            StartCoroutine(CheckForInteractables(() => FocusOnAll(), true));
        }

        protected virtual void FinalizeInteractor()
        {
            StopAllCoroutines();

            UnfocusFromAll();
        }

        protected virtual void OnEnable() => InitializeInteractor();

        protected virtual void OnDisable() => FinalizeInteractor();
    }
}
