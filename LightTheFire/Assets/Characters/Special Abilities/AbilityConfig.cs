using UnityEngine;
using RPG.Utility;

namespace RPG.Character
{
    public abstract class AbilityConfig : ScriptableObject
    {
        [Header("Special Ability General")]
        [SerializeField] float energyCost = 10f;
        [SerializeField] GameObject particlePrefab = null;
        [SerializeField] AnimationClip abilityAnimation;
        [SerializeField] AudioClip[] audioClips = null;

        protected AbilityBehaviour behaviour;

        public abstract AbilityBehaviour GetBehaviourComponent(GameObject objectToattachTo);

        public void AttachAbilityTo(GameObject objectToattachTo)
        {
            AbilityBehaviour behaviourComponet = GetBehaviourComponent(objectToattachTo);
            behaviourComponet.SetConfig(this);
            behaviour = behaviourComponet;
        }

        public void Use(GameObject target)
        {
            behaviour.Use(target);
        }

        public float GetEnergyCost()
        {
            return energyCost;
        }

        public GameObject GetParticlePrefab() 
        {
            return particlePrefab;
        }

        public AnimationClip GetAbilityAnimation()
        {
            return abilityAnimation;
        }

        public AudioClip GetRandomAudioClip()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }

    }

}