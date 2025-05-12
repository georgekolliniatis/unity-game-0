using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace Assets.Game.Data.GameObjects.NonPlayerCharacters
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(NavMeshAgent))]

    public class NonPlayerCharacter : MonoBehaviour
    {
        [SerializeField] protected Transform[] m_Points;
        [SerializeField] protected UnityEvent m_OnStop;
        [SerializeField] protected UnityEvent m_OnWave;
        [SerializeField] protected UnityEvent m_OnTalk;
        [SerializeField] protected UnityEvent m_OnGo;

        protected Animator m_Animator;
        protected NavMeshAgent m_Agent;
        protected int m_PointIndex;
        protected int m_Step;
        protected bool m_IsWaiting;

        protected readonly string m_WalkingStatusName = "IsWalking";
        protected readonly string m_WavingStatusName = "IsWaving";

        protected virtual void InitializeNPC()
        {
            m_Animator = GetComponent<Animator>();

            m_Agent = GetComponent<NavMeshAgent>();

            m_PointIndex = 1;

            m_Step = -1;

            m_Animator.SetBool(m_WalkingStatusName, false);

            if (m_Points.Length > 0)
                StartCoroutine(CheckAgentStatus());
        }

        protected virtual void Start() => InitializeNPC();

        protected virtual IEnumerator CheckAgentStatus()
        {
            while (true)
            {
                if (m_Agent.remainingDistance <= m_Agent.stoppingDistance && !m_IsWaiting && !m_Agent.pathPending)
                    StartCoroutine(GoToNextPoint());

                yield return new WaitForSeconds(1.0f);
            }
        }

        protected virtual IEnumerator GoToNextPoint()
        {
            m_IsWaiting = true;

            m_Animator.SetBool(m_WalkingStatusName, false);

            if (m_PointIndex == 0 || m_PointIndex == m_Points.Length - 1)
            {
                m_Step *= -1;

                yield return new WaitForSeconds(Random.Range(1, 4));
            }

            m_PointIndex += m_Step;

            m_Agent.SetDestination(m_Points[m_PointIndex].position);

            m_IsWaiting = false;

            m_Animator.SetBool(m_WalkingStatusName, true);
        }

        protected virtual void Go()
        {
            m_OnGo.Invoke();

            m_IsWaiting = false;

            m_Animator.SetBool(m_WalkingStatusName, true);

            m_Agent.isStopped = false;
        }

        protected virtual void Stop()
        {
            m_OnStop.Invoke();

            m_IsWaiting = true;

            m_Animator.SetBool(m_WalkingStatusName, false);

            m_Agent.isStopped = true;
        }

        protected virtual void Wave()
        {
            m_OnWave.Invoke();

            m_Animator.SetBool(m_WavingStatusName, true);
        }

        protected virtual void Talk() => m_OnTalk.Invoke();

        protected virtual void OnTriggerStay(Collider other) => gameObject.transform.LookAt(other.gameObject.transform.position);

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Stop();

                Wave();

                Talk();
            }
        }

        protected virtual void OnTriggerExit(Collider _) => Go();
    }
}
