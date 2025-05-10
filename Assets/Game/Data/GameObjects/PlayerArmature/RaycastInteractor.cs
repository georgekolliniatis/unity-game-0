using Assets.Game.Data.InteractionSystem;
using UnityEngine;

namespace Assets.Game.Data.GameObjects.PlayerArmature
{
    public class RaycastInteractor : BaseInteractor
    {
        [SerializeField] protected float m_RaycastMaxDistance = 10.0f;

        protected override void InitializeInteractor()
        {
            base.InitializeInteractor();

            StopAllCoroutines();

            StartCoroutine(CheckForInteractables(() =>
            {
                if (Physics.Raycast(transform.position + Vector3.up, transform.forward, out RaycastHit hitInfo, m_RaycastMaxDistance))
                    FocusOnOne(hitInfo.collider.gameObject, true);
                else UnfocusFromAll();
            }));
        }
    }
}
