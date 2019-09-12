using UnityEngine;
using UnityEngine.UI;

namespace RPG.Character
{
    public class SpecialAbilities : MonoBehaviour
    {
        [SerializeField] AbilityConfig[] abilities;

        [SerializeField] Image energyOrb;
        [SerializeField] float maxEnergyPoints = 100f;
        [SerializeField] float regenPointsPerSecond = 1f;

        [SerializeField] AudioClip outOfEnergy = null;

        float currentEnergyPoints;
        AudioSource audioSource;

        float EnergyAsPercent { get { return currentEnergyPoints / maxEnergyPoints; } }

        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();

            currentEnergyPoints = maxEnergyPoints;
            AttachInitialAbilities();
            UpdateEnergyBar();
        }

        void Update()
        {
            if(currentEnergyPoints< maxEnergyPoints)
            {
                AddEnegryPoints();
                UpdateEnergyBar();
            }
        }

        private void AttachInitialAbilities()
        {
            for (int abilityIndex = 0; abilityIndex < abilities.Length; abilityIndex++)
            {
                abilities[abilityIndex].AttachAbilityTo(gameObject);
            }
        }

        public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            var energyComponent = GetComponent<SpecialAbilities>();
            var energyCost = abilities[abilityIndex].GetEnergyCost();

            if (energyCost <= currentEnergyPoints)
            {
                ConsumeEnergy(energyCost);
                abilities[abilityIndex].Use(target);
            }
            else 
            { 
                audioSource.PlayOneShot(outOfEnergy);
            }
        }

        public int GetNumberOfAbilities()
        {
            return abilities.Length;
        }

        private void AddEnegryPoints()
        {
            var pointsToAdd = regenPointsPerSecond * Time.deltaTime;
            currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + pointsToAdd, 0, maxEnergyPoints);
        }

        public bool IsEnergyAvailable(float amount)
        {
            return amount <= currentEnergyPoints;
        }

        public void ConsumeEnergy(float amount)
        {
            float newEnergyPoints = currentEnergyPoints - amount;
            currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0f, maxEnergyPoints);
            UpdateEnergyBar();
        }

        private void UpdateEnergyBar()
        {
            energyOrb.fillAmount = EnergyAsPercent;
        }

    }
}