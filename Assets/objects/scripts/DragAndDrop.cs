using System.Collections;
using UnityEngine;
using TMPro;

public class HammerDragWorld : MonoBehaviour
{
    [Header("Sprites")]
    [SerializeField] private Sprite normalHammerSprite;
    [SerializeField] private Sprite heldHammerSprite;

    [Header("Highlight")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("Target")]
    [SerializeField] private GameObject targetSprite;
    [SerializeField] private GameObject crackPrefab;

    [Header("Target Offset")]
    [SerializeField] private Vector3 targetOffset = new Vector3(-2.5f, -1f, 0f);

    [Header("Hit Settings")]
    [SerializeField] private LayerMask breakableLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float hitRadius = 0.5f;
    [SerializeField] private int hitsMade = 0;
    [SerializeField] private int hitsMax = 4;
    public TextMeshProUGUI healthText;

    [Header("Allowed Strike Area")]
    [SerializeField] private Collider2D allowedStrikeArea;

    [Header("Strike Animation")]
    [SerializeField] private float strikeAngle = -70f;
    [SerializeField] private float strikeTime = 0.15f;
    [SerializeField] private float returnTime = 0.25f;
    [SerializeField] private float pauseAfterStrike = 0.15f;

    [Header("Hammer Hint")]
    [SerializeField] private float hintDelay = 10f;
    [SerializeField] private float hintPulseSpeed = 4f;
    [SerializeField] private float hintMaxBrightness = 2f;
    [SerializeField] private ParticleSystem sparkleEffect;
    [SerializeField] private AudioSource hintAudio;
    [SerializeField] private float maxHintVolume = 1f;
    [SerializeField] private float volumeIncreaseSpeed = 0.15f;
    [SerializeField] private GameObject mouseHint;

    private bool hasPickedUpHammer = false;
    private bool hintActive = false;
    private float hintTimer = 0f;
    private float currentHintVolume = 0f;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;
    private Collider2D hammerCollider;
    private Rigidbody2D rb;

    private bool isDragging = false;
    private bool isAnimating = false;
    private bool isHovering = false;

    private Vector3 targetPosition;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Awake()
    {
        mainCamera = Camera.main;

        spriteRenderer = GetComponent<SpriteRenderer>();
        hammerCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        startPosition = transform.position;
        startRotation = transform.rotation;

        if (spriteRenderer != null && normalHammerSprite != null)
            spriteRenderer.sprite = normalHammerSprite;

        if (spriteRenderer != null)
            spriteRenderer.color = normalColor;

        if (targetSprite != null)
            targetSprite.SetActive(false);

        AutoFindUIIfMissing();
        UpdateHealth(hitsMax - hitsMade);
        if (sparkleEffect != null)
        {
            sparkleEffect.Stop();
        }

        if (hintAudio != null)
        {
            hintAudio.loop = true;
            hintAudio.volume = 0f;
            hintAudio.Stop();
        }
        if (mouseHint != null)
        {
            mouseHint.SetActive(false);
        }
    }


    private void Update()
    {
        HandleHammerHint();

        if (isAnimating)
            return;

        CheckHover();

        if (Input.GetMouseButtonDown(0))
        {
            TryPickUpHammer();
        }

        if (isDragging)
        {
            DragHammer();
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            DropHammer();
        }
    }

    private void CheckHover()
    {
        if (isDragging || hintActive)
            return;

        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

        bool mouseIsOverHammer = hit != null && hit.gameObject == gameObject;

        if (mouseIsOverHammer && !isHovering)
        {
            isHovering = true;

            if (spriteRenderer != null)
                spriteRenderer.color = highlightColor;
        }
        else if (!mouseIsOverHammer && isHovering)
        {
            isHovering = false;

            if (spriteRenderer != null)
                spriteRenderer.color = normalColor;
        }
    }

    private void AutoFindUIIfMissing()
    {
        if (healthText == null)
        {
            GameObject healthObj = GameObject.Find("healthText");
            if (healthObj != null)
            {
                healthText = healthObj.GetComponent<TextMeshProUGUI>();
            }
        }
    }

    private void UpdateHealth(int strikesLeft)
    {
        if (healthText == null)
        {
            Debug.LogWarning("Health text missing. Drag text into box.");
            return;
        }
        strikesLeft--;

        healthText.text = "" + strikesLeft;
    }

    private void TryPickUpHammer()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos);

        if (hit != null && hit.gameObject == gameObject)
        {

            hasPickedUpHammer = true;
            StopHammerHint();
            isDragging = true;
            isHovering = false;

            if (spriteRenderer != null)
            {
                spriteRenderer.color = normalColor;

                if (heldHammerSprite != null)
                    spriteRenderer.sprite = heldHammerSprite;
            }

            if (hammerCollider != null)
                hammerCollider.enabled = false;

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.bodyType = RigidbodyType2D.Kinematic;
            }

            if (targetSprite != null)
                targetSprite.SetActive(true);

            Debug.Log("Picked up hammer");
        }
    }

    private void DragHammer()
    {
        Vector3 mouseWorldPos = GetMouseWorldPosition();

        transform.position = mouseWorldPos;

        targetPosition = mouseWorldPos + targetOffset;

        if (targetSprite != null)
        {
            targetSprite.transform.position = targetPosition;
        }
    }

    private void DropHammer()
    {
        isDragging = false;

        if (targetSprite != null)
            targetSprite.SetActive(false);

        if (!CanStrikeAtTarget())
        {
            Debug.Log("Cannot strike here. Target is outside allowed area.");
            StartCoroutine(ReturnHammerWithoutStrike());
            return;
        }

        StartCoroutine(StrikeAnimation());
    }
    private bool CanStrikeAtTarget()
    {
        if (allowedStrikeArea == null)
        {
            Debug.LogWarning("No allowed strike area assigned.");
            return true;
        }

        return allowedStrikeArea.OverlapPoint(targetPosition);
    }

    private IEnumerator StrikeAnimation()
    {
        isAnimating = true;

        Vector3 strikePosition = transform.position;
        Quaternion beforeStrikeRotation = transform.rotation;
        Quaternion strikeRotation = Quaternion.Euler(0f, 0f, strikeAngle);

        float timer = 0f;

        while (timer < strikeTime)
        {
            timer += Time.deltaTime;
            float t = timer / strikeTime;

            transform.rotation = Quaternion.Lerp(beforeStrikeRotation, strikeRotation, t);

            yield return null;
        }

        transform.rotation = strikeRotation;

        Strike(targetPosition);

        yield return new WaitForSeconds(pauseAfterStrike);

        timer = 0f;

        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        while (timer < returnTime)
        {
            timer += Time.deltaTime;
            float t = timer / returnTime;

            transform.position = Vector3.Lerp(currentPosition, startPosition, t);
            transform.rotation = Quaternion.Lerp(currentRotation, startRotation, t);

            yield return null;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;

        if (spriteRenderer != null)
        {
            if (normalHammerSprite != null)
                spriteRenderer.sprite = normalHammerSprite;

            spriteRenderer.color = normalColor;
        }

        if (hammerCollider != null)
            hammerCollider.enabled = true;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        isAnimating = false;

        Debug.Log("Hammer returned to table");
    }

    private IEnumerator ReturnHammerWithoutStrike()
    {
        isAnimating = true;

        float timer = 0f;

        Vector3 currentPosition = transform.position;
        Quaternion currentRotation = transform.rotation;

        while (timer < returnTime)
        {
            timer += Time.deltaTime;
            float t = timer / returnTime;

            transform.position = Vector3.Lerp(currentPosition, startPosition, t);
            transform.rotation = Quaternion.Lerp(currentRotation, startRotation, t);

            yield return null;
        }

        transform.position = startPosition;
        transform.rotation = startRotation;

        if (spriteRenderer != null)
        {
            if (normalHammerSprite != null)
                spriteRenderer.sprite = normalHammerSprite;

            spriteRenderer.color = normalColor;
        }

        if (hammerCollider != null)
            hammerCollider.enabled = true;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        isAnimating = false;

        Debug.Log("Hammer returned without striking");
    }

    private void Strike(Vector3 position)
    {
        Debug.Log("Hammer hit at: " + position);

        hitsMade++;

        int strikesLeft = hitsMax - hitsMade;
        UpdateHealth(strikesLeft);

        if (hitsMade >= hitsMax)
        {
            Debug.Log("Hammer has no strikes left. Ending game.");
            EndGame();
            return;
        }



        if (crackPrefab != null)
        {
            Instantiate(crackPrefab, position, Quaternion.identity);
        }

        // Check if player was hit
        Collider2D playerHit = Physics2D.OverlapCircle(position, hitRadius, playerLayer);

        if (playerHit != null)
        {
            Debug.Log("Player was hit by hammer. Game Over.");
            EndGame();
            return;
        }

        // Check if hammer hit an interactable object like the falling rock
        Collider2D hitObject = Physics2D.OverlapCircle(position, hitRadius, breakableLayer);

        if (hitObject != null)
        {
            Debug.Log("object hit: " + hitObject.name);

            BreakableObject breakable = hitObject.GetComponent<BreakableObject>();

            if (breakable != null)
            {
                breakable.TakeHit();
            }
            else
            {
                canHammer hammerObject = hitObject.GetComponent<canHammer>();

                if (hammerObject != null && hammerObject.CanInteract())
                {
                    hammerObject.Interact();
                }
            }
        }
    }

    private void EndGame()
    {
        Debug.Log("GAME OVER");
        GameObject player;
        if (hitsMade < hitsMax)
        {
            player = GameObject.FindGameObjectWithTag("Player");
            Debug.Log("Player found: " + (player != null));
        }
        else
        {
            GameObject parentObj = GameObject.Find("UI");
            Transform gameOverScreen = parentObj.transform.Find("mainGameOver");
            player = gameOverScreen != null ? gameOverScreen.gameObject : null;
            Debug.Log("Game Over object found: " + (player != null));
        }
        if (player != null)
        {
            if (!player.activeSelf)
            {
                player.SetActive(true);
            }
            HealthController health = player.GetComponent<HealthController>();
            health.SetAlive(false);
        }
        // Later connect this to your real GameManager:
        // GameManager.Instance.GameOver();
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(mainCamera.transform.position.z);

        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        return worldPos;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetPosition, hitRadius);
    }
    private void HandleHammerHint()
    {
        if (hasPickedUpHammer)
            return;

        if (isDragging || isAnimating)
            return;

        hintTimer += Time.deltaTime;

        if (hintTimer >= hintDelay && !hintActive)
        {
            StartHammerHint();
        }

        if (hintActive)
        {
            PulseHammerBrightness();
            IncreaseHintVolume();
        }
    }

    private void StartHammerHint()
    {
        hintActive = true;

        if (sparkleEffect != null)
        {
            sparkleEffect.Play();
        }

        if (hintAudio != null)
        {
            hintAudio.volume = 0f;
            hintAudio.Play();
        }

        if (mouseHint != null)
        {
            mouseHint.SetActive(true);
        }

        Debug.Log("Hammer hint started");
    }

    private void StopHammerHint()
    {
        hintActive = false;

        if (sparkleEffect != null)
        {
            sparkleEffect.Stop();
        }

        if (hintAudio != null)
        {
            hintAudio.Stop();
            hintAudio.volume = 0f;
        }

        currentHintVolume = 0f;

        if (spriteRenderer != null)
        {
            spriteRenderer.color = normalColor;
        }
        if (mouseHint != null)
        {
            mouseHint.SetActive(false);
        }

        Debug.Log("Hammer hint stopped");
    }

    private void PulseHammerBrightness()
    {
        if (spriteRenderer == null)
            return;

        float pulse = (Mathf.Sin(Time.time * hintPulseSpeed) + 1f) / 2f;

        Color brightColor = normalColor * hintMaxBrightness;
        brightColor.a = normalColor.a;

        spriteRenderer.color = Color.Lerp(normalColor, brightColor, pulse);
    }

    private void IncreaseHintVolume()
    {
        if (hintAudio == null)
            return;

        currentHintVolume += Time.deltaTime * volumeIncreaseSpeed;
        currentHintVolume = Mathf.Clamp(currentHintVolume, 0f, maxHintVolume);

        hintAudio.volume = currentHintVolume;
    }
}