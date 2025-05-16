using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float detectionRange = 5f;
    public float attackRange = 1f;
    public float attackCooldown = 1.0f;
    private bool isAttackingNow = false;
    public int maxHP = 3;
    private int currentHP;
    public Transform attackPoint;
    public Vector2 attackBoxSize = new Vector2(1.1f, 1.2f);
    private bool isPreparingAttack = false;
    public Transform hitboxPoint;
    public Vector2 hitboxSize = new Vector2(1.1f, 1.2f);
    public int attackDamage = 1;
    public LayerMask playerLayer;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isDead = false;
    public LayerMask groundLayer;
    public AudioClip GoblinSound;
    private AudioSource audioSource;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Attack
            // 1. 공격 준비 or 공격 중이면 → 무조건 멈추고 리턴
        if (isPreparingAttack || isAttackingNow)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isRunning", false);
            return;
        }

        // 2. 공격 실행 조건
        if (PlayerInAttackBox())
        {   
            StartCoroutine(PrepareAndAttack());
            return;
        }

        // Chase
        else if (distance < detectionRange)
        {
            animator.SetBool("isRunning", true);
            Vector2 dir = (player.position - transform.position).normalized;
            rb.velocity = new Vector2(dir.x * moveSpeed, rb.velocity.y);
            
            audioSource.PlayOneShot(GoblinSound);
            // change direction
            if (dir.x > 0) transform.localScale = new Vector3(1,1,1);
            else if (dir.x < 0) transform.localScale = new Vector3(-1, 1 ,1);
        }
        else{
            animator.SetBool("isRunning", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        animator.SetTrigger("isHurt");


        if (currentHP <= 0)
        {
            isDead = true;
            animator.SetBool("isDead", true);
            Die();
            //GetComponent<Collider2D>().enabled = false;
        }
    }
     public void Die()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.734941f, transform.position.z);
        animator.SetTrigger("isDead");

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
        if (col != null) 
        {
            col.enabled = false;
            Debug.Log("콜라이더 비활성화 상태: " + col.enabled); 
        }
        GetComponent<EnemyController>().enabled = false;

        Destroy(gameObject, 2f);
    }

    IEnumerator Attack()
    {
        isAttackingNow = true;
        animator.SetTrigger("isAttacking");
        yield return new WaitForSeconds(attackCooldown);
        isAttackingNow = false;
    }
    private bool PlayerInAttackBox()
    {
        Collider2D hit = Physics2D.OverlapBox(
            attackPoint.position,attackBoxSize,0f,playerLayer
        );
        //if (hit != null) Debug.Log(">> Player detected in hitbox: " + hit.name);

        return hit != null;
    }

    IEnumerator PrepareAndAttack()
    {
        isPreparingAttack = true;

        rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(1f);

        StartCoroutine(Attack());
        isPreparingAttack = false;
    }   
    
    public void DealDamage()
    {
        Collider2D hit = Physics2D.OverlapBox(
            hitboxPoint.position,
            hitboxSize,
            0f,
            playerLayer
        );

        if (hit != null)
        {
            Debug.Log("💥 Player hit!");
            PlayerHP playerHP = hit.GetComponent<PlayerHP>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(1, transform); // 자기 위치를 전달 → 넉백 방향용
            }
        }
    }
}
