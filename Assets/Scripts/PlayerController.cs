using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] [Range(0.8f, 0.95f)] private float screenBorder = 0.9f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string runAnimParam = "isRunning";

    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = true;
    private float screenWidthInWorldUnits;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        CalculateScreenBounds();
    }

    private void CalculateScreenBounds()
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = Camera.main.orthographicSize * 2;
        screenWidthInWorldUnits = cameraHeight * screenAspect;
    }

    private void Update()
    {
        HandleMovement();
        HandleAnimation();
    }

    private void HandleMovement()
    {
        float moveInput = Input.GetAxis("Horizontal");
        
        Vector3 movement = new Vector3(moveInput, 0f, 0f) * moveSpeed * Time.deltaTime;
        Vector3 newPosition = transform.position + movement;
        
        float halfPlayerWidth = spriteRenderer.bounds.size.x / 2;
        float screenEdge = (screenWidthInWorldUnits / 2) * screenBorder;
        
        newPosition.x = Mathf.Clamp(
            newPosition.x,
            -screenEdge + halfPlayerWidth,
            screenEdge - halfPlayerWidth
        );

        transform.position = newPosition;
        
        if (moveInput > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void HandleAnimation()
    {
        bool isMoving = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.1f;
        animator.SetBool(runAnimParam, isMoving);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        spriteRenderer.flipX = !spriteRenderer.flipX;
    }
}