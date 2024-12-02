
// IcePlatform.cs
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class IcePlatform : MonoBehaviour
{
    public PhysicsMaterial2D iceMaterial;
    public float breakTime = 3f;
    public Sprite brokenSprite;
    private SpriteRenderer spriteRenderer;

    private float timer = 0f;
    private bool isPlayerOn = false;

    private void Start()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        Collider2D collider = GetComponent<Collider2D>();
        collider.sharedMaterial = iceMaterial;
    }

    void Update()
    {
        if (isPlayerOn)
        {
            timer += Time.deltaTime;
            if (timer >= breakTime)
            {
                BreakPlatform();
            }
        }
    }

    void BreakPlatform()
    {
        spriteRenderer.sprite = brokenSprite;

        Destroy(gameObject, 1f);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOn = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerOn = false;
            timer = 0f; // 타이머 초기화
        }
    }
}

