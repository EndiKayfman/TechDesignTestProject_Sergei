using UnityEngine;
using UnityEngine.U2D.Animation; // Добавляем пространство имен для костной анимации

public class BoneAnimatedPlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] [Range(0.8f, 0.95f)] private float screenBorder = 0.9f;

    [Header("Animation")]
    [SerializeField] private SpriteResolver spriteResolver; // Для управления анимациями
    [SerializeField] private string runAnimationLabel = "Run";
    [SerializeField] private string idleAnimationLabel = "Idle";
    
    private SpriteRenderer spriteRenderer;
    private bool isFacingRight = true;
    private float screenWidthInWorldUnits;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteResolver == null)
        {
            spriteResolver = GetComponent<SpriteResolver>();
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
        
        // Переключаем анимации через SpriteResolver
        if (spriteResolver != null)
        {
            spriteResolver.SetCategoryAndLabel("CharacterState", 
                isMoving ? runAnimationLabel : idleAnimationLabel);
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.localScale = new Vector3(
            isFacingRight ? 1 : -1, 
            1, 
            1);
    }
}