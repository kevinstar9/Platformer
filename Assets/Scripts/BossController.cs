using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
public class BossController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float detectionRange = 6f;
    public float attackCooldown = 0f;
    private bool isAttackingNow = false;
    public Transform attackPoint;
    public Vector2 attackBoxSize = new Vector2(1.1f, 1.2f);
    private bool isPreparingAttack = false;
    public Transform hitboxPoint;
    public Vector2 hitboxSize = new Vector2(1.1f, 1.2f);
    public LayerMask playerLayer;
    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isDead = false;
    public LayerMask groundLayer;
    //fire attack
    private bool isBurning = false;
    public GameObject fireEffect;
    //hpBar
    public Slider hpBar;
    public int maxHP = 50;
    private int currentHP;
    public Transform hpBarTransform;
    private float displayedHP;
    public float hpLerpSpeed = 5f;
    //hitted
    private SpriteRenderer spriteRenderer;
    public float hitFlashDuration = 0.3f;
    //DamageText
    public GameObject damageTextPrefab;
    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        currentHP = maxHP;
        UpdateHPBarSmooth();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        displayedHP = maxHP;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Update()
    {
        if (rb.bodyType != RigidbodyType2D.Dynamic) return;

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

            // change direction
            if (dir.x > 0) transform.localScale = new Vector3(1, 1, 1);
            else if (dir.x < 0) transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            animator.SetBool("isRunning", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        displayedHP = Mathf.Lerp(displayedHP, currentHP, Time.deltaTime * hpLerpSpeed);
        UpdateHPBarSmooth();
    }
    void LateUpdate()
    {
        if (hpBarTransform != null)
        {
            hpBarTransform.position = transform.position + new Vector3(0, 0.5f, 0);
            hpBarTransform.forward = Camera.main.transform.forward;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        animator.SetTrigger("isHurt");

        ShowDamageText(damage);

        StartCoroutine(HitFlash());

        if (currentHP <= 0)
        {
            Die();
        }
        UpdateHPBar();
    }
    void ShowDamageText(int dmg)
    {
        if (damageTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + new Vector3(0, -1, 0);
            GameObject textObj = Instantiate(damageTextPrefab, spawnPos, quaternion.identity);
            textObj.GetComponent<DamageText>().SetText(dmg);
        }
    }
    IEnumerator HitFlash()
    {
        Color originalColor = spriteRenderer.color;
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }
    void UpdateHPBar()
    {
        if (hpBar != null)
        {
            hpBar.value = (float)currentHP / maxHP;
        }
    }
    void UpdateHPBarSmooth()
    {
        if (hpBar != null)
        {
            hpBar.value = (float)currentHP / maxHP;
        }
    }
    public void ApplyBurn(float duration = 3f, int burnDamage = 1)
    {
        if (!isBurning)
        {
            if (fireEffect != null)
            {
                fireEffect.SetActive(true);
                fireEffect.transform.localPosition = new Vector3(0, 0, 0);
            }
            StartCoroutine(BurnCoroutine(duration, burnDamage));
        }
    }
    private IEnumerator BurnCoroutine(float duration, int damagePerTick)
    {
        isBurning = true;
        rb.velocity = Vector2.zero;
        float tickInterval = 1f;
        float time = -0f;

        while (time < duration)
        {
            TakeDamage(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
            time += tickInterval;
        }
        isBurning = false;
        if (fireEffect != null)
        {
            fireEffect.SetActive(false);
        }
    }
    // Die 함수 내부 수정
    public void Die()
    {
        if (isDead) return;

        isDead = true;
        animator.SetTrigger("Die");
        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Static;

        CapsuleCollider2D col = GetComponent<CapsuleCollider2D>();
        if (col != null)
        {
            col.enabled = false;
        }

        GetComponent<BossController>().enabled = false;

        if (fireEffect != null)
        {
            fireEffect.SetActive(false);
        }

        Destroy(gameObject, 2f);
    }


    IEnumerator Attack()
    {
        isAttackingNow = true;
        animator.SetTrigger("isAttacking1");
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
            PlayerHP playerHP = hit.GetComponent<PlayerHP>();
            if (playerHP != null)
            {
                playerHP.TakeDamage(1, transform); // 자기 위치를 전달 → 넉백 방향용
            }
        }
    }
}
