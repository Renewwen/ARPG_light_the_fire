
using UnityEngine;

namespace RPG.Character
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Self Heal"))]
    public class SelfHealConfig : AbilityConfig
    {
        [Header("Self Heal Specific")]
        [SerializeField] float extraHealth = 50f;

        public override AbilityBehaviour GetBehaviourComponent(GameObject objectToattachTo)
        {
            return objectToattachTo.AddComponent<SelfHealBehaviour>();
        }

        public float GetExtraHealth()
        {
            return extraHealth;
        }
    }
}