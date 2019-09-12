using UnityEngine;
using UnityEngine.AI;

namespace RPG.Character
{
    [SelectionBase]
    public class Character : MonoBehaviour
    {
        [Header("Audio")]
        [SerializeField] float audioSourceSpatialBlend = 0.5f;

        [Header("Animator")]
        [SerializeField] RuntimeAnimatorController animatorController;
        [SerializeField] AnimatorOverrideController animatorOverrideController;
        [SerializeField] Avatar charaterAvater;

        [Header("Capsule Collider Settings")]
        [SerializeField] Vector3 colliderCenter = new Vector3(0, 0.8f, 0);
        [SerializeField] float colliderRadius = 0.2f;
        [SerializeField] float colliderHeight = 2f;

        [Header("Movment Properties")]
        [SerializeField] float moveSpeedMultiplier = .7f;
        [SerializeField] float animationSpeedMultiplier = 1.2f;
        [SerializeField] float m_MovingTurnSpeed = 360;
        [SerializeField] float m_StationaryTurnSpeed = 180;
        [SerializeField] float moveThreshold = 1f;

        [Header("Nav Mesh Agent")]
        [SerializeField] float navMeshAgentSteeringSpeed = 5.0f;
        [SerializeField] float navMeshAgentStoppingDistance = 1.3f;
        [SerializeField] float navMeshAgentAcceleration = 120f;

        float turnAmount;
        float forwardAmount;
        NavMeshAgent navMeshAgent;
        Animator animator;
        Rigidbody myRididBody;
        bool isAlive = true;

        private void Awake()
        {
            AddRequiredComponents();
        }

        private void AddRequiredComponents()
        {
            var capsuleCollider = gameObject.AddComponent<CapsuleCollider>();
            capsuleCollider.center = colliderCenter;
            capsuleCollider.radius = colliderRadius;
            capsuleCollider.height = colliderHeight;

            myRididBody = gameObject.AddComponent<Rigidbody>();
            myRididBody.constraints = RigidbodyConstraints.FreezeRotation;

            var audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = audioSourceSpatialBlend;

            animator = gameObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = animatorController;
            animator.avatar = charaterAvater;

            navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
            navMeshAgent.speed = navMeshAgentSteeringSpeed;
            navMeshAgent.stoppingDistance = navMeshAgentStoppingDistance;
            navMeshAgent.acceleration = navMeshAgentAcceleration;
            navMeshAgent.autoBraking = false;
            navMeshAgent.updateRotation = false;
            navMeshAgent.updatePosition = true;
        }

        private void Update()
        {
            if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance && isAlive)
            {
                Move(navMeshAgent.desiredVelocity);
            }
            else
            {
                Move(Vector3.zero);
            }
        }

        public float GetAnimSpeedMultiplier()
        {
            return animationSpeedMultiplier;
        }

        public void SetDestination(Vector3 worldPos)
        {
            navMeshAgent.destination = worldPos;
        }

        public AnimatorOverrideController GetOverrideController()
        {
            return animatorOverrideController;
        }

        void Move(Vector3 movement)
        {
            SetForwardAndTurn(movement);
            ApplyExtraTurnRotation();
            UpdateAnimator();
        }

        public void Kill()
        {
            isAlive = false;
        }

        void SetForwardAndTurn(Vector3 movement)
        {
            if (movement.magnitude > moveThreshold)
            {
                movement.Normalize();
            }
            var localMove = transform.InverseTransformDirection(movement);
            turnAmount = Mathf.Atan2(localMove.x, localMove.z);
            forwardAmount = localMove.z;
        }

        void UpdateAnimator()
        {
            // update the animator parameters
            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
            animator.speed = animationSpeedMultiplier;
        }

        void ApplyExtraTurnRotation()
        {
            // help the character turn faster (this is in addition to root rotation in the animation)
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        private void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (Time.deltaTime > 0)
            {
                Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                velocity.y = myRididBody.velocity.y;
                myRididBody.velocity = velocity;
            }
        }

    }
}
