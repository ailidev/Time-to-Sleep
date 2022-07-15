using System;
using UnityEngine;
using UnityEngine.Events;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using Aili.Inputs;
#endif

namespace Aili.Player
{
    #if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
    #endif
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour
    {
        [Space]
        [Header("DEBUG")]
        public bool m_CanMove = true;
        public bool m_IsGrounded;
        public bool m_IsCrouching;
        public bool m_IsJumping;
        public bool m_IsRunning;
        public bool m_IsDie;

        [Space]
        [Header("CUSTOM")]
        [Range(0.1f, 20f)]
        public float m_WalkSpeed = 4f;
        [Range(0.1f, 20f)]
        public float m_RunSpeed = 6f;
        [Range(0.1f, 20f)]
        public float m_CrouchSpeed = 3f;
        [Range(0.1f, 20f)]
        public float m_JumpForce = 4f;
        [Range(0.01f, 1f)]
        public float m_MovementSmoothing = 0.05f;
        public bool m_AirControl = true;
        public SpriteDirection m_SpriteDirection = SpriteDirection.Right;
        public bool m_FreezePositionX = true;
        public Collider2D m_FullBodyCollider;
        public Collider2D m_CrouchCollider;
        public Collider2D m_GroundDetector;
        public LayerMask m_Ground;

        public enum SpriteDirection
        {
            Left,
            Right
        }

        [Space]
        [Header("EVENTS")]
        public UnityEvent m_OnLanding;
        public UnityEvent m_OnGround;
        public UnityEvent m_OnCrouch;

        Vector2 m_MoveInput, m_Velocity;

        SpriteRenderer m_SpriteRenderer;
        Animator m_Animator;
        Rigidbody2D m_Rigidbody2D;

#if UNITY_EDITOR
        void OnValidate()
        {
            GetComponents();

            switch (m_SpriteDirection)
            {
                case SpriteDirection.Right:
                    m_SpriteRenderer.flipX = false;
                    break;
                case SpriteDirection.Left:
                    m_SpriteRenderer.flipX = true;
                    break;
            }

            if (m_FreezePositionX)
            {
                FreezeConstraints();
            }
            else
            {
                NormalizeConstraints();
            }
        }
#endif

        void Awake()
        {
            GetComponents();
        }

        void Update()
        {
            if (m_CanMove)
            {
                HandleCrouch();
                HandleJump();
                Die();
            }
        }

        void FixedUpdate()
        {
            if (m_CanMove)
            {
                HandleMovement();
            }

            CheckGround();
        }

        void GetComponents()
        {
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
            m_Animator = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
        }

        void Die()
        {
            if (m_IsDie)
            {
                m_Rigidbody2D.constraints = RigidbodyConstraints2D.None;
            }
            else
            {
                NormalizeConstraints();
            }
        }

        void NormalizeConstraints()
        {
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        void FreezeConstraints()
        {
            m_Rigidbody2D.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        void HandleMovement()
        {
            if (m_IsGrounded || m_AirControl)
            {
                float moveSpeed = 0;

                if (m_MoveInput.x != 0)
                {
                    if (!m_IsRunning)
                    {
                        moveSpeed = m_MoveInput.x * m_WalkSpeed;
                    }
                    else if (m_IsRunning)
                    {
                        moveSpeed = m_MoveInput.x * m_RunSpeed;
                    }
                    else if (m_IsCrouching)
                    {
                        moveSpeed = m_MoveInput.x * m_CrouchSpeed;
                    }
                }

                Vector2 velocity = new Vector2(moveSpeed, m_Rigidbody2D.velocity.y);
                m_Rigidbody2D.velocity = Vector2.SmoothDamp(m_Rigidbody2D.velocity, velocity, ref m_Velocity, m_MovementSmoothing);

                if (m_MoveInput.x != 0 && m_IsGrounded && !m_IsJumping)
                {
                    m_Animator.SetBool("IsWalk", true);
                }
                else
                {
                    m_Animator.SetBool("IsWalk", false);
                }
            }

            if (m_MoveInput != Vector2.zero)
            {
                if (m_FreezePositionX)
                {
                    NormalizeConstraints();
                }
            }
            else
            {
                if (m_FreezePositionX)
                {
                    FreezeConstraints();
                }
            }

            FlipSprite();
        }

        void HandleCrouch()
        {
            if ((m_FullBodyCollider != null) || (m_CrouchCollider != null))
            {
                if (m_IsCrouching)
                {
                    m_Animator.SetBool("IsCrouch", true);
                    m_FullBodyCollider.enabled = false;
                    m_CrouchCollider.enabled = true;
                }
                else
                {
                    m_Animator.SetBool("IsCrouch", false);
                    m_FullBodyCollider.enabled = true;
                    m_CrouchCollider.enabled = false;
                }
            }
            else
            {
                throw new NullReferenceException("Missing one or more collider in Player! Please assign them!");
            }
        }

        void HandleJump()
        {
            if (!m_IsGrounded)
            {
                m_IsJumping = true;
            }
            else
            {
                m_IsJumping = false;
            }

            if (m_IsJumping)
            {
                m_Animator.SetBool("IsJump", true);
            }
            else
            {
                m_Animator.SetBool("IsJump", false);
            }
        }

        void CheckGround()
        {
            m_IsGrounded = Physics2D.IsTouchingLayers(m_GroundDetector, m_Ground);

            if (m_IsGrounded)
            {
                m_OnGround.Invoke();
            }
        }

        void FlipSprite()
        {
            if (m_MoveInput.x > 0)
            {
                m_SpriteDirection = SpriteDirection.Right;
                m_SpriteRenderer.flipX = false;
            }
            else if (m_MoveInput.x < 0)
            {
                m_SpriteDirection = SpriteDirection.Left;
                m_SpriteRenderer.flipX = true;
            }
        }

        public void LogMessage(string message)
        {
            Debug.Log(message);
        }

        #region Inputs
        public void Move(InputAction.CallbackContext ctx)
        {
            if (m_CanMove)
            {
                m_MoveInput = ctx.ReadValue<Vector2>();
            }
        }

        public void Jump(InputAction.CallbackContext ctx)
        {
            if (m_CanMove)
            {
                if (ctx.performed && m_IsGrounded)
                {
                    m_Rigidbody2D.AddForce(new Vector2(0, m_JumpForce * 100f));
                    m_IsJumping = true;
                }
                else
                {
                    m_OnLanding.Invoke();
                }
            }
        }

        public void Crouch(InputAction.CallbackContext ctx)
        {
            if (m_CanMove)
            {
                if (ctx.performed)
                {
                    m_IsCrouching = true;
                    m_OnCrouch.Invoke();
                }
                else
                {
                    m_IsCrouching = false;
                }
            }
        }

        public void Run(InputAction.CallbackContext ctx)
        {
            if (m_CanMove)
            {
                if (ctx.performed)
                {
                    m_IsRunning = true;
                }
                else
                {
                    m_IsRunning = false;
                }
            }
        }
        #endregion
    }
}
