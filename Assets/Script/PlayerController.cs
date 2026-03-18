using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Health")]
    [SerializeField] private int maxHP = 3;
    [SerializeField] private float invincibleTime = 1f;

    private int currentHP;
    private bool isInvincible = false;

    private Rigidbody2D rb;
    private Vector2 moveInput;

    // Input System
    private NEW_Input inputActions;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        inputActions = new NEW_Input();
    }

    private void OnEnable()
    {
        inputActions.Enable();

        // WASD / Keyboard
        inputActions.Player.Move.performed += OnMove;
        inputActions.Player.Move.canceled += OnMove;
    }

    private void OnDisable()
    {
        inputActions.Player.Move.performed -= OnMove;
        inputActions.Player.Move.canceled -= OnMove;

        inputActions.Disable();
    }

    private void Start()
    {
        currentHP = maxHP;
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    // =========================
    // INPUT SYSTEM (WASD)
    // =========================
    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // =========================
    // SWIPE CONTROL (MOBILE)
    // =========================
    public void SwipeMoveLeft()
    {
        moveInput = Vector2.left;
    }

    public void SwipeMoveRight()
    {
        moveInput = Vector2.right;
    }

    public void SwipeMoveUp()
    {
        moveInput = Vector2.up;
    }

    public void SwipeMoveDown()
    {
        moveInput = Vector2.down;
    }

    public void SwipeStop()
    {
        moveInput = Vector2.zero;
    }

    // =========================
    // DAMAGE SYSTEM (IDamageable)
    // =========================
    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHP -= damage;
        Debug.Log("Player HP: " + currentHP);

        if (currentHP <= 0)
        {
            Die();
        }

        StartCoroutine(InvincibleCoroutine());
    }

    private void Die()
    {
        Debug.Log("Player Dead");
        Destroy(gameObject);
    }

    private IEnumerator InvincibleCoroutine()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibleTime);
        isInvincible = false;
    }
}