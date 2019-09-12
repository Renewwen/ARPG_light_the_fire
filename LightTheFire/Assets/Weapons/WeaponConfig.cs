using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons_N
{
    [CreateAssetMenu(menuName = ("RPG/Weapon"))]
    public class WeaponConfig : ScriptableObject
    {
        public Transform gripTransform;

        [SerializeField] GameObject weaponPrefab;
        [SerializeField] AnimationClip attackAniamtion;
        [SerializeField] float minTimeBetweenHits = .5f;
        [SerializeField] float maxAttackRange = 2f;
        [SerializeField] float additionalDamage = 12f;
        [SerializeField] float damageDelay = 0.5f;

        public float GetMinTimeBetweenHits() 
        {
            // TODO Consider whether we take animation time into account
            return minTimeBetweenHits;
        }

        public float GetMaxAttackRange()
        {
            return maxAttackRange;
        }

        public float GetDamageDelay()
        {
            return damageDelay;
        }

        public GameObject GetWeaponPrefab()
        {
            return weaponPrefab;
        }

        public AnimationClip GetAttackAnimClip()
        {
            RemoveAnimationEvents();
            return attackAniamtion;
        }

        public float GetAdditionalDamage()
        {
            return additionalDamage;
        }

        //So that Asset packs cannot cause crashes
        private void RemoveAnimationEvents()
        {
            attackAniamtion.events = new AnimationEvent[0];
        }
    }
}
