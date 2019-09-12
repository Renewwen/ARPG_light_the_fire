using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Character
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        PlayerControl player = null;

        void Start()
        {
            player = GetComponent<PlayerControl>();
        }

        public override void Use(GameObject target)
        {
            var playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.Heal((config as SelfHealConfig).GetExtraHealth());
            PlayParticleEffect();
            PlayAbilitySound();
            PlayAbilityAnimation();
        }

    }
}
