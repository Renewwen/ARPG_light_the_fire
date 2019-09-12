using System.Collections;
using UnityEngine;

using RPG.Weapons_N;

namespace RPG.Character
{
    public class WeaponSystem : MonoBehaviour
    {
        [SerializeField] float baseDamage = 20f;
        [SerializeField] WeaponConfig weaponInUse;
        [SerializeField] GameObject weaponSocket;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        GameObject target;
        GameObject weaponObject;
        Animator animator;
        Character character;
        float laslHitTime = 0f;

        void Start()
        {
            animator = GetComponent<Animator>();
            character = GetComponent<Character>();

            PutWeaponInHand(weaponInUse);
            SetAttackAnimation();
        }

        void Update()
        {
            bool targetIsDead;
            bool targetIsOutOfRange;
            if(target == null)
            {
                targetIsDead = false;
                targetIsOutOfRange = false;
            }
            else
            {
                var targetHealth = target.GetComponent<HealthSystem>().healthAsPercentage;
                targetIsDead = targetHealth <= Mathf.Epsilon;

                var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
                targetIsOutOfRange = distanceToTarget > weaponInUse.GetMaxAttackRange();
            }

            float characterHealth = GetComponent<HealthSystem>().healthAsPercentage;
            bool characterIsDead = (characterHealth <= Mathf.Epsilon);

            if (characterIsDead || targetIsOutOfRange || targetIsDead)
            {
                StopAllCoroutines();
            }
        }

        public void PutWeaponInHand(WeaponConfig weaponToUse)
        {
            weaponInUse = weaponToUse;
            var weaponPrefab = weaponToUse.GetWeaponPrefab();
            Destroy(weaponObject); // empty hands
            weaponObject = Instantiate(weaponPrefab, weaponSocket.transform);
            weaponObject.transform.localPosition = weaponInUse.gripTransform.localPosition;
            weaponObject.transform.localRotation = weaponInUse.gripTransform.localRotation;
        }

        public WeaponConfig GetCurrentWeaponInUse() 
        {
            return weaponInUse;
        }

        private void SetAttackAnimation()
        {
            if(!character.GetOverrideController())
            {
                Debug.Break();
                Debug.LogAssertion("Please provide" + gameObject + "with an animator override controller!");
            }
            else 
            { 
                var animatorOverrideController = character.GetOverrideController();
                animator.runtimeAnimatorController = animatorOverrideController;
                animatorOverrideController[DEFAULT_ATTACK] = weaponInUse.GetAttackAnimClip();
            }
        }

        public void AttackTarget(GameObject targetToAttack)
        {
            target = targetToAttack;
            StartCoroutine(AttackTargetRepeatedly());
        }

        public void StopAttacking()
        {
            animator.StopPlayback();
            StopAllCoroutines();
        }

        IEnumerator AttackTargetRepeatedly()
        {
            bool attackerStillAlive = GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;
            bool targetStillAlive = target.GetComponent<HealthSystem>().healthAsPercentage >= Mathf.Epsilon;

            while (attackerStillAlive && targetStillAlive)
            {
                float weaponHitPeriod = weaponInUse.GetMinTimeBetweenHits();
                float timeToWait = weaponHitPeriod * character.GetAnimSpeedMultiplier();

                bool isTimeToHitAgain = Time.time - laslHitTime > timeToWait;
                if(isTimeToHitAgain)
                {
                    AttackTargetOnce();
                    laslHitTime = Time.time;
                }
                yield return new WaitForSeconds(timeToWait);
            }
        }

        private void AttackTargetOnce()
        {
            transform.LookAt(target.transform);
            animator.SetTrigger(ATTACK_TRIGGER);
            float damageDelay = weaponInUse.GetDamageDelay();
            SetAttackAnimation();
            StartCoroutine(DamageAfterDelay(damageDelay));
        }

        IEnumerator DamageAfterDelay(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            target.GetComponent<HealthSystem>().TakeDamage(CalculateDamage());
        }

        private void AttackTarget()
        {
            if (Time.time - laslHitTime > weaponInUse.GetMinTimeBetweenHits())
            {
                SetAttackAnimation();
                animator.SetTrigger(ATTACK_TRIGGER);
                laslHitTime = Time.time;
            }
        }

        private float CalculateDamage()
        {
            //bool isCriticalHit = UnityEngine.Random.Range(0f, 1.0f) <= criticalHitChance;
            //float damageBeforeCritical = baseDamage + weaponInUse.GetAdditionalDamage();
            return baseDamage + weaponInUse.GetAdditionalDamage();
            //if (isCriticalHit)
            //{
            //    cirticalHitParticale.Play();
            //    return damageBeforeCritical * criticalHitMutiplier;
            //}
            //else
            //{
            //    return damageBeforeCritical;
            //}
        }

    }
}