using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHP : MonoBehaviour
{
    public Vector3 originalPosition;
    public int maxHP = 5;
    private int currnetHP;
    private Rigidbody2D rb;
    private Animator animator;
    private AudioSource audioSource;
    public AudioClip HurtSound;

    void Awake()
    {
        currnetHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();   
        audioSource = GetComponent<AudioSource>();
    }

    void Start() => currnetHP = maxHP; 

    public void TakeDamage(int damage)
    {
        currnetHP -= damage;
        Debug.Log("Enemy took damage. CurrentHP: " + currnetHP);
        audioSource.PlayOneShot(HurtSound);
        animator.SetTrigger("isHurt");
    }
}
