using Assets.Game.Data.InteractionSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Assets.Game.Data.GameObjects.WoodenHouseDoor
{
    [RequireComponent(typeof(Animator))]

    public class WoodenHouseDoorInteractable : BaseInteractable
    {
        [SerializeField] protected UnityEvent m_OnOpen;
        [SerializeField] protected UnityEvent m_OnClose;

        Animator m_WoodenHouseDoorAnimator;
        readonly string m_OpenActionName = "Open";
        readonly string m_CloseActionName = "Close";

        public override void Focus(IInteractor interactor)
        {
            base.Focus(interactor);

            interactor.AddInputActions(new InputAction[]
            {
                new(m_OpenActionName, InputActionType.Button, "<Keyboard>/e"),
                new(m_CloseActionName, InputActionType.Button, "<Keyboard>/q")
            });

            Debug.LogWarning("Focused on door");
        }

        public override void Unfocus(IInteractor interactor)
        {
            base.Unfocus(interactor);

            interactor.RemoveInputActions(new string[]
            {
                m_OpenActionName,
                m_CloseActionName
            });

            Debug.LogWarning("Unfocused from door");
        }

        void InitializeInteractable() => m_WoodenHouseDoorAnimator = GetComponent<Animator>();

        void Start() => InitializeInteractable();

        public void OnOpen() => RunActionCallback(nameof(OnOpen), () =>
        {
            Debug.LogWarning("Opening door");

            if (m_WoodenHouseDoorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Wooden House Door Closed"))
                m_WoodenHouseDoorAnimator.Play("Base Layer.Wooden House Door Open");
        });

        public void OnClose() => RunActionCallback(nameof(OnClose), () =>
        {
            Debug.LogWarning("Closing door");

            if (m_WoodenHouseDoorAnimator.GetCurrentAnimatorStateInfo(0).IsName("Base Layer.Wooden House Door Opened"))
                m_WoodenHouseDoorAnimator.Play("Base Layer.Wooden House Door Close");
        });
    }
}
