
using UnityEngine;

namespace RPG.Character
{
    [CreateAssetMenu(menuName = ("RPG/Special Ability/Power Attack"))]
    public class PowerAttackConfig : AbilityConfig
    {
        [Header("Power Attack Specific")]
        [SerializeField] float extraDamage = 20f;

        public override AbilityBehaviour GetBehaviourComponent(GameObject objectToattachTo)
        {
            return objectToattachTo.AddComponent<PowerAttackBehaviour>();
        }

        public float GetExtraDamage()
        {
            return extraDamage;
        }

    }
}