
using UnityEngine;

public class CatController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float baseJumpForce = 10f;
    public float maxJumpForce = 20f;
    public float chargeRate = 10f;
    public float maxHorizontalSpeed = 5f;
    public float horizontalAcceleration = 5f;

    [Header("Ground Detection")]
    public LayerMask groundLayer;
    public static CatController Instance;
    private Rigidbody2D rb;
    private bool isGrounded = false;
    private bool isChargingJump = false;
    private float jumpForce;
    private float horizontalInput;
    private bool isOnIce = false;
    private bool isOnJelly = false;
    public float jellyMoveSpeed = 3f;
    private Vector2 windForceToApply;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
        rb = GetComponent<Rigidbody2D>();
    }
    public void ApplyWind(Vector2 direction, float force)
    {
        if (!isGrounded)
        {
            windForceToApply = direction.normalized * force;
        }
    }
    private void Update()
    {
        // 수평 입력 받기 (좌/우 화살표 키 또는 A/D 키)
        horizontalInput = Input.GetAxisRaw("Horizontal");

        // 점프 충전 시작
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            StartChargingJump();
        }

        // 점프 충전 유지
        if (Input.GetButton("Jump") && isChargingJump)
        {
            ChargeJump();
        }

        // 점프 실행
        if (Input.GetButtonUp("Jump") && isChargingJump)
        {
            PerformJump();
        }
    }

    private void FixedUpdate()
    {
        // 수평 이동
        MoveHorizontal();
    }

    private void MoveHorizontal()
    {
        float targetSpeed = horizontalInput * maxHorizontalSpeed;
        float speedDifference = targetSpeed - rb.velocity.x;
        float accelerationRate = horizontalAcceleration * Time.fixedDeltaTime;

        // 새로운 수평 속도 적용
        rb.velocity = new Vector2(
            rb.velocity.x + Mathf.Clamp(speedDifference, -accelerationRate, accelerationRate),
            rb.velocity.y
        );
    }

    private void StartChargingJump()
    {
        isChargingJump = true;
        jumpForce = baseJumpForce; // 점프 힘 초기화
    }

    private void ChargeJump()
    {
        // 점프 힘을 증가, 최대 점프 힘 제한
        jumpForce += chargeRate * Time.deltaTime;
        jumpForce = Mathf.Min(jumpForce, maxJumpForce);
    }

    private void PerformJump()
    {
        isChargingJump = false;

        rb.velocity = new Vector2(rb.velocity.x, jumpForce); // Y축 속도 설정
        jumpForce = baseJumpForce; // 점프 힘 초기화
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Ground와 충돌한 경우
        if (IsGroundLayer(collision.gameObject))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("IcePlatform"))
        {
            isOnIce = true;
            // 예: 슬라이딩 효과를 위해 감속력 줄이기
            rb.drag = 0.1f;
        }
        if (collision.gameObject.CompareTag("JellyPlatform"))
        {
            isOnJelly = true;
            horizontalAcceleration = jellyMoveSpeed;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Ground에서 벗어난 경우
        if (IsGroundLayer(collision.gameObject))
        {
            isGrounded = false;
        }
        if (collision.gameObject.CompareTag("IcePlatform"))
        {
            isOnIce = false;
            rb.drag = 1f; // 일반 드래그로 복귀
        }

        if (collision.gameObject.CompareTag("JellyPlatform"))
        {
            isOnJelly = false;
            horizontalAcceleration = 5f;
        }
    }

    private bool IsGroundLayer(GameObject obj)
    {
        // 충돌한 오브젝트가 Ground 레이어인지 확인
        return (groundLayer.value & (1 << obj.layer)) > 0;
    }
}
