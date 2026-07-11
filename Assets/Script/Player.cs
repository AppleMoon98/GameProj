using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public enum PlayerState
{
    Idle,
    Attack,
    Crop
}

public class Player : MonoBehaviour
{
    // 설정
    [SerializeField] GameObject managers;
    [SerializeField] Transform visual;
    [SerializeField] GameObject gauge;
    [SerializeField] Image gaugeBar;
    public Vector2 inputVec;
    public float speed;
    public float jumpPower;
    public float attackCooldown;
    public float cropCooldown;

    // 테스트용
    public float floatvalue;

    // 컴포넌트
    Rigidbody2D rigid;
    Animator animator;
    GameManager manager;

    // 플랫폼 충돌 판정
    RaycastHit2D rayhitGround;
    RaycastHit2D rayhitWall;
    Vector2 rayMove;

    // 상태
    public PlayerState currentState;
    bool isSprint;
    Interactable currentTarget;

    // 트리거 충돌 판정
    private List<Collider2D> currentTriggers = new();

    private void Awake()
    {
        // 컴포넌트 참조
        rigid = GetComponent<Rigidbody2D>();
        animator = visual.GetComponent<Animator>();
        manager = managers.GetComponent<GameManager>();

        // 초기값
        currentState = PlayerState.Idle;
        rayMove = Vector2.right;
        isSprint = false;
    }

    private void Update()
    {
        HandleFlip();
        HandleAnimation();
        UpdateInteractionTarget();
    }

    private void FixedUpdate()
    {
        Move();
        GroundCheck();
    }

    void HandleFlip()
    {
        if (currentState != PlayerState.Idle)
            return;

        if (inputVec.x == 0)
            return;

        bool isMove = inputVec.x > 0;

        // 시각적 요소 뒤집기
        visual.localScale = new Vector3(isMove ? 1 : -1, 1, 1);
        visual.localPosition = new Vector3(isMove ? 0.227f : -0.227f, 0, 0);
    }

    void HandleAnimation()
    {
        bool isMove = inputVec.x != 0;
        bool isJump = rayhitGround.collider != null;

        animator.SetBool("isMove", isMove);
        animator.SetBool("isJump", isJump);
        animator.SetBool("isSprint", isSprint);
    }

    void Move()
    {
        float moveSpeed;

        // 입력 좌우 판정
        if (inputVec.x != 0)
            rayMove = inputVec.x > 0 ? Vector2.right : Vector2.left;
        Vector2 rayPosition = (Vector2)transform.position + rayMove * 0.24f;

        // 세로 판정
        //Debug.DrawRay(rayPosition + Vector2.down * 0.4f, rayMove * 0.1f, Color.green);
        rayhitWall = Physics2D.Raycast(rayPosition + Vector2.down * 0.4f, rayMove, 0.1f, LayerMask.GetMask("Platform"));
        if (rayhitWall.collider != null)
            return;

        // 가로 판정
        //Debug.DrawRay(rayPosition + rayMove * 0.05f, Vector2.down * 0.5f, Color.green);
        rayhitWall = Physics2D.Raycast(rayPosition + rayMove * 0.05f, Vector2.down, 0.5f, LayerMask.GetMask("Platform"));
        if (rayhitWall.collider != null)
            return;

        // 실제 이동
        if (currentState != PlayerState.Idle)
            moveSpeed = 0;
        else
            moveSpeed = isSprint ? speed * 1.5f : speed;
        rigid.linearVelocity = new Vector2(inputVec.x * moveSpeed, rigid.linearVelocity.y);
    }

    void Crop()
    {
        if (currentState != PlayerState.Idle)
            return;

        StartCoroutine(CropCo());
        animator.SetTrigger("doCrop");
    }

    void GroundCheck()
    {
        Vector2 rayPosition = (Vector2)transform.position + Vector2.down * 0.7f;
        //Debug.DrawRay(rayPosition, Vector2.down * 0.15f, Color.red);
        rayhitGround = Physics2D.Raycast(rayPosition, Vector2.down, 0.15f, LayerMask.GetMask("Platform"));
    }

    void UpdateInteractionTarget()
    {
        Interactable newTarget = null;

        // 가장 최근에 들어온 트리거의 Interactable 컴포넌트를 가져옴
        if (currentTriggers.Count > 0)
            newTarget = currentTriggers[^1].GetComponent<Interactable>();

        // 현재 타겟과 새 타겟이 같으면 아무 작업도 하지 않음
        if (currentTarget == newTarget)
            return;

        // 현재 타겟이 있으면 하이라이트 제거
        currentTarget?.SetHighlight(false);

        currentTarget = newTarget;

        // 새 타겟이 있으면 하이라이트 설정
        currentTarget?.SetHighlight(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!currentTriggers.Contains(other))
        {
            currentTriggers.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (currentTriggers.Contains(other))
            currentTriggers.Remove(other);

        if (manager != null && manager.itemManager.storage != null)
            if (manager.itemManager.storage.GetActive() && other.CompareTag("Chest"))
                manager.userInterfaceManager.StorageClose();
    }

    private IEnumerator ActionCooldownCo(PlayerState actionState, float cooldownTime)
    {
        // 행동 상태 전환
        currentState = actionState;
        yield return new WaitForSeconds(cooldownTime);
        currentState = PlayerState.Idle;
    }

    private IEnumerator AnimCooldownCo(string animBool, float cooldownTime)
    {
        // 애니메이션 상태 전환
        animator.SetBool(animBool, true);
        yield return new WaitForSeconds(cooldownTime);
        animator.SetBool(animBool, false);
    }

    private IEnumerator CropCo()
    {
        currentState = PlayerState.Crop;
        animator.SetBool("isCrop", true);

        gauge.SetActive(true);
        gaugeBar.fillAmount = 0;

        float timer = 0;
        while (timer < cropCooldown)
        {
            timer += Time.deltaTime;
            gaugeBar.fillAmount = timer / cropCooldown;
            yield return null;
        }

        gaugeBar.fillAmount = 1;
        gauge.SetActive(false);

        animator.SetBool("isCrop", false);
        manager.AddItem(101, 50);
        currentState = PlayerState.Idle;
    }

    void OnMove(InputValue value)
    {
        if (currentState != PlayerState.Idle)
        {
            inputVec = Vector2.zero;
            return;
        }

        inputVec = value.Get<Vector2>();
    }

    void OnJump()
    {
        // 공격 중에는 점프 불가
        if (currentState != PlayerState.Idle)
            return;

        if (rayhitGround.collider != null)
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
    }

    void OnSprint(InputValue value)
    {
        isSprint = value.isPressed;
    }

    void OnAttack()
    {
        if (currentState != PlayerState.Idle)
            return;

        StartCoroutine(ActionCooldownCo(PlayerState.Attack, attackCooldown));
        animator.SetTrigger("isAttack");
    }

    void OnInteract()
    {
        if (currentTriggers.Count == 0)
        {
            Debug.Log("No interactable objects nearby.");
            return;
        }

        if (currentTriggers.Count > 0)
        {
            Collider2D targetTrigger = currentTriggers[^1]; // 가장 최근에 들어온 트리거 선택
            if (targetTrigger.CompareTag("Tree"))
            {
                // Tree 상호작용
                Crop();
            }
            else if (targetTrigger.CompareTag("Chest"))
            {
                // Chest 상호작용
                manager.userInterfaceManager.StorageOpen();
            }
        }
    }
}

