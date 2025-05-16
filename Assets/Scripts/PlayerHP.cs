using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
public class PlayerHP : MonoBehaviour
{

    public GameObject heartPrefab;
    public Transform heartContainer;
    private List<GameObject> hearts = new List<GameObject>();
    private Vector3 originalPosition;
    public int maxHP = 5;
    private int currentHP;
    private Rigidbody2D rb;
    private bool isInvincible = false;
    public float invincibleDuration = 1f;
    private Animator animator;
    public float knockbackForce = 5f;
    public AudioClip HurtSound;
    private AudioSource audioSource;

    void Awake()
    {
        currentHP = maxHP;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Start() 
    {
        currentHP = maxHP;
        CreateHearts();
    }
    
    public void TakeDamage(int damage, Transform attacker)
    {
        if (isInvincible ) return;

        currentHP -= damage;
        UpdateHearts();
        Debug.Log("Player took damage. CurrentHP: " + currentHP);
        audioSource.PlayOneShot(HurtSound);
        animator.SetTrigger("isHurt");

        PlayerMove move = GetComponent<PlayerMove>();
        if(move != null)
        {
            var moveType = move.GetType();
            moveType.GetField("isAttacking", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(move, false);
            moveType.GetField("isDashing", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.SetValue(move, false);
        }

        Vector2 knockDir = (transform.position - attacker.position).normalized;
        knockDir.y = 0.5f;
        rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);

        FindObjectOfType<ScreenFlash>().Flash();
        originalPosition = transform.position;

        if(currentHP <= 0)
        {
            Die();
            return;
        }
            StartCoroutine(InvincibilityCoroutine());
        }
    public void Die()
    {
        transform.position = originalPosition;
        animator.SetTrigger("isDead");
        GetComponent<Collider2D>().enabled = false;
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;
        GetComponent<PlayerMove>().enabled = false;
        transform.position = new Vector3(transform.position.x,transform.position.y - 0.5f, transform.position.y);

    }
    IEnumerator InvincibilityCoroutine() 
    {
        isInvincible = true;

        //glowing effect
        SpriteRenderer sr = GetComponentInParent<SpriteRenderer>();
        for (int i = 0; i < 5; i++)
        {
            sr.color = new Color(1, 1, 1, 0.5f);
            yield return new WaitForSeconds(invincibleDuration / 10f);
            sr.color = new Color (1, 1, 1, 1f);
            yield return new WaitForSeconds(invincibleDuration / 10f);
        }
        isInvincible = false;
    }

    void CreateHearts()
    {
        for (int i = 0; i < maxHP; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer);
            hearts.Add(heart);
        }
    }
    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Count; i++)
        {
            hearts[i].SetActive(i < currentHP);
        }
    }
}
