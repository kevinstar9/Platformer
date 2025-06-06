using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEditor;
using UnityEngine.Assertions;
using UnityEditor.Callbacks;
using Unity.VisualScripting; // 테스트를 위한 Assert 클래스 사용

public partial class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    public float maxSpeed; // 최대 이동 속도
    public float jumpPower; // 점프 힘
    public bool isJumping = false; // 점프 중인지 여부
    private bool isFacingRight = true; // 공격 방향을 저장 (true: 오른쪽, false: 왼쪽)
    private bool isAttacking = false; // 공격 중인지 여부
    public float dashSpeed = 3f; // 대쉬 속도
    public float dashTime = 0.2f; // 대쉬 지속 시간
    private bool isDashing = false; // 대쉬 중인지 여부
    public Transform groundCheck; // 땅을 체크하는 위치
    public float groundCheckRadius = 0.2f; // 땅 체크 범위
    public LayerMask groundLayer; // 땅 레이어
    public Transform attackPoint1; // 첫 번째 공격 위치
    public Transform attackPoint2; // 두 번째 공격 위치
    public Transform attackPoint3; // 대쉬 공격 위치
    public Vector2 attackBoxSize1 = new Vector2(0.72f, 1.2f); // 첫 번째 공격 히트박스 크기
    public Vector2 attackBoxSize2 = new Vector2(0.5f, 1.3f); // 두 번째 공격 히트박스 크기
    public Vector2 attackBoxSize3 = new Vector3(0.9f, 1.1f); // 대쉬 공격 히트박스 크기
    public int attackDamage = 1; // 일반 공격 데미지
    public int Dash_attackDamage = 2; // 대쉬 공격 데미지
    public LayerMask enemyLayer; // 적 레이어

    private bool isGrounded; // 땅에 닿아 있는지 여부
    public Rigidbody2D rigid; // 물리 엔진
    SpriteRenderer spriteRenderer; // 스프라이트 렌더러
    Animator animator; // 애니메이터
    CapsuleCollider2D capsuleCollider; // 캡슐 충돌체
    AudioClip audioClip;
    EnemyController enemyController;
    public AudioClip slashSound1; // 첫 번째 공격 소리
    public AudioClip slashSound2; // 두 번째 공격 소리
    private AudioSource audioSource;
    //stage2 starting point
    public Transform stage2Start;
    //fireAttack
    public bool hasFireAttack = false;

    void Awake()
    {
        // 컴포넌트 초기화
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // 키 입력 처리 및 애니메이션 제어

        if (Input.GetAxisRaw("Horizontal") == 0 && !isJumping) // 가만히 있을 때
        {
            animator.SetBool("isWalking", false); // 걷기 애니메이션 중지
        }

        if (Input.GetMouseButtonDown(0) && !isAttacking) // 마우스 왼쪽 버튼 클릭 시 공격
        {
            isAttacking = true;
            if (isDashing)
            {
                animator.SetTrigger("DashAttack"); // 대쉬 공격 애니메이션 실행
            }
            else
            {
                animator.SetTrigger("Attack"); // 일반 공격 애니메이션 실행
            }
        }

        float move = Input.GetAxisRaw("Horizontal"); // 좌우 이동 입력 받기
        if (move > 0) // 오른쪽으로 이동
        {
            transform.localScale = new Vector3(1, 1, 1); // 캐릭터 방향 오른쪽으로
            isFacingRight = true;
        }
        else if (move < 0) // 왼쪽으로 이동
        {
            transform.localScale = new Vector3(-1, 1, 1); // 캐릭터 방향 왼쪽으로
            isFacingRight = false;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && !isAttacking) // LeftShift 키를 누르면 대쉬
        {
            StartCoroutine(Dash()); // 대쉬 코루틴 실행
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer); // 땅에 닿아 있는지 체크
        animator.SetBool("isJumping", !isGrounded); // 점프 애니메이션 제어
        animator.SetBool("isFalling", rigid.velocity.y < -0.1f && !isGrounded); // 낙하 애니메이션 제어

        if (Input.GetButtonDown("Jump") && isGrounded && !isAttacking) // 점프 버튼을 누르면 점프
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse); // 위쪽으로 힘을 가함
        }
    }

    void FixedUpdate()
    {
        // 물리 엔진을 사용하는 움직임 처리

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (isDashing || stateInfo.IsTag("Attack")) // 대쉬 중이거나 공격 중이면
        {
            rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y); // 현재 속도 유지
            animator.SetBool("isWalking", false); // 걷기 애니메이션 중지
            return;
        }

        if (stateInfo.IsTag("Attack")) // 공격 애니메이션 중이면
        {
            animator.SetBool("isWalking", false); // 걷기 애니메이션 중지
            return;
        }

        float h = Input.GetAxisRaw("Horizontal");   // 좌우 이동 입력 받기
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse); // 좌우로 힘을 가함

        if (rigid.velocity.x > maxSpeed) // 최대 속도 제한 (오른쪽)
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);

        else if (rigid.velocity.x < (-1) * maxSpeed) // 최대 속도 제한 (왼쪽)
            rigid.velocity = new Vector2(-maxSpeed, rigid.velocity.y);

        if (!isJumping)
        {
            animator.SetBool("isWalking", isGrounded && Mathf.Abs(rigid.velocity.x) > 0.1f); // 걷기 애니메이션 제어
        }
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Finish")
        {
            if (stage2Start != null)
            {
                transform.position = stage2Start.position;
            }
        }
    }

    IEnumerator Dash()
    {
        // 대쉬 코루틴

        isDashing = true;
        animator.SetTrigger("Dash"); // 대쉬 애니메이션 실행

        float originalGravity = rigid.gravityScale; // 원래 중력 저장
        rigid.gravityScale = 0; // 대쉬 중에는 중력 영향 없음

        float dashDirection = isFacingRight ? 1f : -1f; // 대쉬 방향 결정
        rigid.velocity = new Vector2(dashDirection * dashSpeed, 0); // 대쉬 속도 적용

        yield return new WaitForSeconds(dashTime); // 대쉬 지속 시간 동안 대기

        rigid.gravityScale = originalGravity; // 원래 중력 복원
        isDashing = false; // 대쉬 종료
    }

    void HitWithBox(Transform point, Vector2 size, int damage)
    {
        // 히트박스를 이용한 공격 판정

        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(point.position, size, 0f, enemyLayer); // 히트박스 내의 적 감지

        foreach (Collider2D enemy in hitEnemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();

            if (enemyController != null)
            {
                enemyController.TakeDamage(damage);

                if (hasFireAttack && Random.value < 0.5f)
                {
                    enemyController.ApplyBurn();
                }
            }

            BossController boss = enemy.GetComponent<BossController>();
            if (boss != null)
            {
                boss.TakeDamage(damage);
                if (hasFireAttack && Random.value < 0.5f)
                {
                    boss.ApplyBurn();
                }
            }
        }
    }

    public void DealDamage1() // 첫 번째 공격
    {
        Debug.Log("First Slash(DealDamage1)");
        HitWithBox(attackPoint1, attackBoxSize1, attackDamage); // 히트박스 공격
        audioSource.PlayOneShot(slashSound1); // 공격 소리 재생
    }
    public void DealDamage2() // 두 번째 공격
    {
        Debug.Log("Second Slash(DealDamage2)");
        HitWithBox(attackPoint2, attackBoxSize2, attackDamage); // 히트박스 공격
        audioSource.PlayOneShot(slashSound2); // 공격 소리 재생
    }
    public void DealDamage3() // 대쉬 공격
    {
        Debug.Log("DashAttack");
        HitWithBox(attackPoint3, attackBoxSize3, Dash_attackDamage); // 히트박스 공격
        audioSource.PlayOneShot(slashSound2); // 공격 소리 재생
    }
    public void EndAttack()
    {
        isAttacking = false; // 공격 종료
    }
}