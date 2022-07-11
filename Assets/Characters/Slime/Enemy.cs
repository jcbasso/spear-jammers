using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Animator _animator;
    public float Health
    {
        set
        {
            health = value;
            print("Health");
            if (health <= 0)
            {
                Defeated();
            }
        }
        get => health;
    }

    public float health = 1;
    private static readonly int Deafeted = Animator.StringToHash("Defeated");

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Defeated()
    {
        _animator.SetTrigger(Deafeted);
    }

    public void RemoveEnemy()
    {
        Destroy(gameObject);
    }
}