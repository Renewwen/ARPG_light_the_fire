
using UnityEngine;
using RPG.Utility;
using System;

namespace RPG.Character
{
    public class AreaEffectBehaviour : AbilityBehaviour
    {

        public override void Use(GameObject target)
        {
            DealAOEDamage();
            PlayParticleEffect();
            PlayAbilitySound();
            PlayAbilityAnimation();
        }

        private void DealAOEDamage()
        {
            //static sphere cast for target
            RaycastHit[] hits = Physics.SphereCastAll(
                transform.position,
                (config as AreaEffectConfig).GetRadius(),
                Vector3.up,
                (config as AreaEffectConfig).GetRadius());

            // for each hit
            // if damageable
            // deal damage to target + player base damage
            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<HealthSystem>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<PlayerControl>();
                if (damageable != null && !hitPlayer)
                {
                    var damageToDeal = (config as AreaEffectConfig).GetDamageToEachTarget();
                    damageable.TakeDamage(damageToDeal);
                }
            }
        }

    }
}