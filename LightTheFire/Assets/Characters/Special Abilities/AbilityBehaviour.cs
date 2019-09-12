using System.Collections;
using UnityEngine;

namespace RPG.Character
{
    public abstract class AbilityBehaviour : MonoBehaviour
    {
        protected AbilityConfig config;

        const string ATTACT_TRIGGER = "Attack";
        const string DEFAULT_ATTACK_STATE = "DEFAULT ATTACK";
        const float PARTICLE_CLEAN_UP_DELAY = 7f;

        public abstract void Use(GameObject target = null);

        public void SetConfig(AbilityConfig configToSet)
        {
            config = configToSet;
        }

        protected void PlayParticleEffect()
        {
            var particlePrefab = config.GetParticlePrefab();
            var particleObject = Instantiate(
                particlePrefab, 
                transform.position, 
                particlePrefab.transform.rotation
                );

            // TODO decide if particle system attaches to player
            particleObject.transform.parent = transform; // set world space is prefab if required

            particleObject.GetComponent<ParticleSystem>().Play();
            StartCoroutine(DestoryParticleWhenFinished(particleObject));
        }

        IEnumerator DestoryParticleWhenFinished(GameObject particlePrefab)
        { 
            while (particlePrefab.GetComponent<ParticleSystem>().isPlaying)
            {
                yield return new WaitForSeconds(PARTICLE_CLEAN_UP_DELAY);
            }
            Destroy(particlePrefab);
            yield return new WaitForEndOfFrame();
        }

        protected void PlayAbilityAnimation()
        {
            var animatorOverrideController = GetComponent<Character>().GetOverrideController();
            var animator = GetComponent<Animator>();
            animator.runtimeAnimatorController = animatorOverrideController;
            animatorOverrideController[DEFAULT_ATTACK_STATE] = config.GetAbilityAnimation();
            animator.SetTrigger(ATTACT_TRIGGER);
        }

        protected void PlayAbilitySound()
        {
            var abilitySound = config.GetRandomAudioClip();
            var audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(abilitySound);
        }
    }
}