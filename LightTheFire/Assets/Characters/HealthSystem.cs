using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RPG.Character {
    public class HealthSystem : MonoBehaviour
    {
        [SerializeField] float maxHealthPoints = 100f;
        [SerializeField] Image healthBar;

        [SerializeField] AudioClip[] damageSounds;
        [SerializeField] AudioClip[] deathSounds;

        [SerializeField] float deathVanishSeconds = 2.0f;

        const string DEATH_TRIGGER = "Death";

        float currentHealthPoints;
        Animator animator;
        AudioSource audioSource;
        Character characterMovement;

        public float healthAsPercentage
        {
            get
            {
                return currentHealthPoints / maxHealthPoints;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            characterMovement = GetComponent<Character>();

            currentHealthPoints = maxHealthPoints;
        }

        // Update is called once per frame
        void Update()
        {
            UpdateHealthBar();
        }

        void UpdateHealthBar()
        {
            if (healthBar)
            {
                healthBar.fillAmount = healthAsPercentage;
            }
        }

        public void TakeDamage(float damage)
        {
            bool playerDies = (currentHealthPoints - damage <= 0);
            // ReduceHealth
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - damage, 0f, maxHealthPoints);
            var clip = damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)];
            audioSource.PlayOneShot(clip);
            if (playerDies) // if player die!
            {
                StartCoroutine(KillCharacter());
            }
        }

        public void Heal(float points)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + points, 0f, maxHealthPoints);
        }

        IEnumerator KillCharacter()
        {
            characterMovement.Kill();
            animator.SetTrigger(DEATH_TRIGGER);

            var playerComponent = GetComponent<PlayerControl>();
            if(playerComponent && playerComponent.isActiveAndEnabled)
            {
                audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
                audioSource.Play();
                yield return new WaitForSecondsRealtime(audioSource.clip.length);
                SceneManager.LoadScene(0);
            }
            else 
            {
                DestroyObject(gameObject, deathVanishSeconds);    
            }

        }

    }
}