using UnityEngine;

using RPG.Utility;
using RPG.Weapons_N;
using System.Collections;
using System;

namespace RPG.Character
{
    [RequireComponent(typeof(HealthSystem))]
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(WeaponSystem))]
    public class EnemyAI : MonoBehaviour
    {
        [SerializeField] float chaseRadius = 6f;
        [SerializeField] WaypointContainer patrolPath;
        [SerializeField] float waypointTolerance = 2f;
        [SerializeField] float waypointDwellTime = 0.5f;

        float currentWeaponRange;
        float distanceToPlayer;
        int nextWaypointIndex;
        PlayerControl player = null;
        Character character;

        enum State { 
            idel, 
            patrolling,
            attacking,
            chasing 
        }
        State state = State.idel;

        private void Start()
        {
            player = FindObjectOfType<PlayerControl>();
            character = GetComponent<Character>();
        }

        private void Update()
        {
            distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeaponInUse().GetMaxAttackRange();

            bool inWeaponCircle = distanceToPlayer <= currentWeaponRange;
            bool inChaseCircle = distanceToPlayer > currentWeaponRange 
                                && distanceToPlayer <= chaseRadius;
            bool outsideChaseRing = distanceToPlayer > chaseRadius;

            if (outsideChaseRing && state != State.patrolling)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(Patrol());
            }
            if (inChaseCircle && state != State.chasing)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(ChasePlayer());
            }
            if(inWeaponCircle && state != State.attacking)
            {
                StopAllCoroutines();
                weaponSystem.AttackTarget(player.gameObject);
            }
        }


        IEnumerator Patrol()
        {
            state = State.patrolling;
            while(true)
            {
                Vector3 nextWaypointPos = patrolPath.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWaypointPos);
                CycleWaypointWhenClose(nextWaypointPos);
                yield return new WaitForSeconds(waypointDwellTime);
            }
        }

        private void CycleWaypointWhenClose(Vector3 nextWaypointPos)
        {
            if (Vector3.Distance(transform.position, nextWaypointPos) <= waypointTolerance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolPath.transform.childCount;
            }
        }

        IEnumerator ChasePlayer()
        {
            state = State.chasing;
            while (distanceToPlayer >= currentWeaponRange)
            {
                character.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }

        void OnDrawGizmos()
        {
            // Draw attack sphere 
            Gizmos.color = new Color(255f, 0, 0, .5f);
            Gizmos.DrawWireSphere(transform.position, currentWeaponRange);

            // Draw chase sphere 
            Gizmos.color = new Color(0, 0, 255, .5f);
            Gizmos.DrawWireSphere(transform.position, chaseRadius);
        }

    }
}