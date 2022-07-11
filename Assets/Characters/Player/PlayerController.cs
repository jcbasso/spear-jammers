using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Characters.Player
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int IsMoving = Animator.StringToHash("isMoving");
        private static readonly int SwordAttack = Animator.StringToHash("swordAttack");
        private static readonly int Roll = Animator.StringToHash("roll");

        public float moveSpeed = 1f;
        public float rollSpeed = 2f;
        public float rollSlowDownSpeed = 0.15f;
        public float collisionOffset = 0.05f;
        public ContactFilter2D movementFilter;
        public SwordAttackScript swordAttackScript;

        private Animator _animator;
        private bool _canMove = true;
        private readonly List<RaycastHit2D> _castCollisions = new();
        private Vector2 _movementInput;
        private Rigidbody2D _rb;
        private SpriteRenderer _spriteRenderer;
        private Vector2 _facingDirection;
        private float _currentRollSpeed;

        private State _state;
        private enum State
        {
            Normal,
            Roll
        }

        // Start is called before the first frame update
        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _animator.SetBool(IsMoving, false);
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _state = State.Normal;
            _facingDirection = new Vector2(1,0);
        }

        private void FixedUpdate()
        {
            switch (_state)
            {
                case State.Normal:
                    HandleMovement();
                    break;
                case State.Roll:
                    HandleRoll();
                    break;
            }
        }

        private void HandleMovement()
        {
            if (!_canMove) return;
            if (_movementInput == Vector2.zero)
            {
                _animator.SetBool(IsMoving, false);
                return;
            }

            var success = TryMove(_movementInput, moveSpeed);

            // TODO: Fix this
            // if (!success)
            // {
            //     success = TryMove(new Vector2(_movementInput.x, 0));
            //     if (!success)
            //     {
            //         TryMove(new Vector2(0, _movementInput.y));
            //     }
            // }


            _spriteRenderer.flipX = _movementInput.x < 0;
            _animator.SetBool(IsMoving, success);
        }

        private void HandleRoll()
        {
            TryMove(_facingDirection, _currentRollSpeed);
            _currentRollSpeed -= _currentRollSpeed * rollSlowDownSpeed * Time.deltaTime;
        }

        private bool TryMove(Vector2 direction, float speed)
        {
            print(direction.ToString());
            if (direction == Vector2.zero) return false;
            
            var movementSize = speed * Time.fixedDeltaTime;
            var distance = movementSize + collisionOffset; // The amount to cast equal to the movement plus an offset
            var count = _rb.Cast(
                direction, // X and Y values between -1 and 1 that represent the direction from the body to look for collisions
                movementFilter, // The settings that determine where a collision can occur on such as layers to collide with
                _castCollisions, // List of collisions to store the found collisions into after the Cast is finished
                distance
            );
            Debug.Log($"TryMove[direction: {direction.ToString()}, distance: {distance.ToString()}, results: {_castCollisions}] = {count.ToString()}");
            if (count != 0) return false;
            print("moviendome");
            _rb.MovePosition(_rb.position + direction * movementSize);
            return true;
        }

        private void OnMove(InputValue movementValue)
        {
            _movementInput = movementValue.Get<Vector2>();
            if (_movementInput != Vector2.zero)
            {
                _facingDirection = _movementInput;
            }
        }

        private void OnFire()
        {
            _animator.SetTrigger(SwordAttack);
        }

        private void OnRoll()
        {
            _animator.SetTrigger(Roll);
        }

        public void DoSwordAttack()
        {
            LockMovement();
            if (_spriteRenderer.flipX)
            {
                swordAttackScript.AttackLeft();
            }
            else
            {
                swordAttackScript.AttackRight();
            }
        }

        public void EndSwordAttack()
        {
            UnlockMovement();
            swordAttackScript.StopAttack();
        }

        public void DoRoll()
        {
            LockMovement();
            _currentRollSpeed = rollSpeed;
            _state = State.Roll;
        }

        public void EndRoll()
        {
            _state = State.Normal;
            UnlockMovement();
        }

        private void LockMovement()
        {
            _canMove = false;
        }

        private void UnlockMovement()
        {
            _canMove = true;
        }
    }
}