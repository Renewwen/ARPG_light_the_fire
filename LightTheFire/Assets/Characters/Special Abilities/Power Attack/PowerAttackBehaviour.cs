using UnityEngine;

namespace RPG.Character
{
    public class PowerAttackBehaviour : AbilityBehaviour
    {

        public override void Use(GameObject target)
        {
            DealDamage(target);
            PlayParticleEffect();
            PlayAbilitySound();
            PlayAbilityAnimation();
        }

        private void DealDamage(GameObject target)
        {
            float damageToDeal = (config as PowerAttackConfig).GetExtraDamage();
            target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
        }
    }
}