using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // 이동 속도
    public float moveSpeed = 5f;
    // 최대 점프 충전 시간
    public float maxChargeTime = 1f;
    // 최대 점프 힘
    public float maxJumpForce = 15f;
    // 벽에 매달릴 수 있는 시간
    public float wallHangTime = 0.5f;
    // 벽 미끄러짐 속도
    public float wallSlideSpeed = 0.5f;
    // 벽 점프 힘
    public Vector2 wallJumpForce = new Vector2(10f, 15f);

    // 내부 변수
    private float jumpCharge = 0f;
    private bool isCharging = false;
    private bool isGrounded = false;
    private bool isTouchingWall = false;
    private bool isWallHanging = false;
    private float wallHangCounter = 0f;
    private int wallDirection = 0; // -1이면 왼쪽 벽, 1이면 오른쪽 벽

    // 컴포넌트 참조
    private Rigidbody2D rb;
    private Collider2D col;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        // 바닥 체크
        isGrounded = IsGrounded();
        // 벽 체크
        isTouchingWall = IsTouchingWall();

        // 왼쪽/오른쪽 이동 (바닥에 있을 때만)
        if (isGrounded)
        {
            float moveInput = Input.GetAxisRaw("Horizontal");
            Vector2 velocity = rb.velocity;
            velocity.x = moveInput * moveSpeed;
            rb.velocity = velocity;
        }

        // 벽에 매달리기
        if (isTouchingWall && !isGrounded && rb.velocity.y <= 0)
        {
            isWallHanging = true;
            wallHangCounter = wallHangTime;
        }

        if (isWallHanging)
        {
            if (wallHangCounter > 0)
            {
                // 벽에 매달려 있는 동안 속도 제어
                rb.velocity = new Vector2(0, 0);
                wallHangCounter -= Time.deltaTime;
            }
            else
            {
                // 매달리는 시간이 끝나면 벽 미끄러짐
                rb.velocity = new Vector2(0, -wallSlideSpeed);
            }

            // 점프 충전 시작
            if (Input.GetButtonDown("Jump"))
            {
                isCharging = true;
                jumpCharge = 0f;
            }

            // 점프 충전 중
            if (isCharging)
            {
                jumpCharge += Time.deltaTime;
                if (jumpCharge >= maxChargeTime)
                {
                    jumpCharge = maxChargeTime;
                }
            }

            // 벽 점프 실행
            if (Input.GetButtonUp("Jump") && isCharging)
            {
                isCharging = false;
                float jumpForceY = (jumpCharge / maxChargeTime) * wallJumpForce.y;
                rb.velocity = new Vector2(-wallDirection * wallJumpForce.x, jumpForceY);
                jumpCharge = 0f;
                isWallHanging = false;
            }
        }
        else
        {
            // 벽에서 떨어졌을 때
            isWallHanging = false;

            // 점프 충전 시작
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                isCharging = true;
                jumpCharge = 0f;
            }

            // 점프 충전 중
            if (isCharging)
            {
                jumpCharge += Time.deltaTime;
                if (jumpCharge >= maxChargeTime)
                {
                    jumpCharge = maxChargeTime;
                }
            }

            // 점프 실행
            if (Input.GetButtonUp("Jump") && isCharging && isGrounded)
            {
                isCharging = false;
                float jumpForce = (jumpCharge / maxChargeTime) * maxJumpForce;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpCharge = 0f;
            }
        }
    }

    // 바닥 체크 함수
    bool IsGrounded()
    {
        // 콜라이더의 바닥 부분에 레이캐스트를 쏴서 바닥 여부를 확인합니다.
        RaycastHit2D hit = Physics2D.Raycast(col.bounds.center, Vector2.down, col.bounds.extents.y + 0.1f);
        return hit.collider != null;
    }

    // 벽 체크 함수 (Tag가 "wall"인 경우에만 벽으로 인식)
    bool IsTouchingWall()
    {
        RaycastHit2D hitLeft = Physics2D.Raycast(col.bounds.center, Vector2.left, col.bounds.extents.x + 0.1f);
        RaycastHit2D hitRight = Physics2D.Raycast(col.bounds.center, Vector2.right, col.bounds.extents.x + 0.1f);

        if (hitLeft.collider != null && hitLeft.collider.CompareTag("wall"))
        {
            wallDirection = -1;
            return true;
        }
        else if (hitRight.collider != null && hitRight.collider.CompareTag("wall"))
        {
            wallDirection = 1;
            return true;
        }
        else
        {
            return false;
        }
    }
}
