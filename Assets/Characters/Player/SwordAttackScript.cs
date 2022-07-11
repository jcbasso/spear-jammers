using System;
using System.Globalization;
using UnityEngine;

namespace Characters.Player
{
    public class SwordAttackScript : MonoBehaviour
    {
        public float damage = 3;
        // public Collider2D swordCollider;
        private Collider2D swordCollider;

        private Vector2 _rightAttackOffset;

        private void Start()
        {
            _rightAttackOffset = transform.position;
            swordCollider = GetComponent<Collider2D>();
        }

        public void AttackRight()
        {
            transform.localPosition = _rightAttackOffset;
            swordCollider.enabled = true;
        }


        public void AttackLeft()
        {
            transform.localPosition = new Vector2(_rightAttackOffset.x * -1, _rightAttackOffset.y);
            swordCollider.enabled = true;
        }

        public void StopAttack()
        {
            swordCollider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            print("Trigger");
            if (col.CompareTag("Enemy"))
            {
                var enemy = col.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.Health -= damage;
                }
            }
        }
    }
}