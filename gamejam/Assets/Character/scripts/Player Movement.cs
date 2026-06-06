using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 8f;

    [Header("Jump")]
    [SerializeField] private float jumpPower = 12f;
    [SerializeField] private float fallMultiplier = 3f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private Animator anim;
    private bool facingRight = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

        if (horizontalInput > 0.01f && !facingRight)
        {
            Flip();
        }
        else if (horizontalInput < -0.01f && facingRight)
        {
            Flip();
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Jump();
        }

        BetterJump();

        anim.SetBool("Grounded", IsGrounded());
    }

    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
        anim.SetTrigger("jump");
    }

    private void BetterJump()
    {
        // Falling faster makes the jump feel less floaty
        if (body.linearVelocity.y < 0)
        {
            body.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Releasing space early makes the jump shorter
        else if (body.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            body.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            Vector2.down,
            0.1f,
            groundLayer
        );

        return raycastHit.collider != null;
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float speed = 8f;

    [Header("Jump")]
    [SerializeField] private float jumpPower = 12f;
    [SerializeField] private float fallMultiplier = 3f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D body;
    private BoxCollider2D boxCollider;
    private Animator anim;
    private bool facingRight = true;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);

        if (horizontalInput > 0.01f && !facingRight)
        {
            Flip();
        }
        else if (horizontalInput < -0.01f && facingRight)
        {
            Flip();
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            Jump();
        }

        BetterJump();

        anim.SetBool("Grounded", IsGrounded());
    }

    private void Jump()
    {
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpPower);
        anim.SetTrigger("jump");
    }

    private void BetterJump()
    {
        // Falling faster makes the jump feel less floaty
        if (body.linearVelocity.y < 0)
        {
            body.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // Releasing space early makes the jump shorter
        else if (body.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            body.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0,
            Vector2.down,
            0.1f,
            groundLayer
        );

        return raycastHit.collider != null;
    }

    private void Flip()
    {
        facingRight = !facingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}