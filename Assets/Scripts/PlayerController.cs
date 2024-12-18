using System.Globalization;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private float speed = 3;
    private float jumpForce = 7.5f;
    private float movementInput;

    private bool canJump = false;

    [SerializeField] private GameObject sprite;
    [SerializeField] private Sprite player2look;

    private Rigidbody2D rb;
    private HealthComponent healthComponent;
    private PlayerInputActions playerActions;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody2D>();
        healthComponent = GetComponent<HealthComponent>();

        playerActions = new PlayerInputActions();
        playerActions.Player.Enable();

        playerActions.Player.Jump.performed += Jump;
        playerActions.Player.Movement.performed += context =>
        {
            if (!IsOwner || !Application.isFocused) return;
            movementInput = context.ReadValue<float>();
        };
        playerActions.Player.Movement.canceled += context =>
        {
            if (!IsOwner || !Application.isFocused) return;
            movementInput = 0f;
        };
    }

    private void Update()
    {
        if (!IsOwner || !Application.isFocused)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }
        Flip();
        rb.velocity = new Vector2(movementInput * speed, rb.velocity.y);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!IsOwner || !Application.isFocused) return;
        if (!canJump) return;
        
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        canJump = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall")) canJump = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("AppliesDamage")) healthComponent.ApplyDamage();
        else if (collision.gameObject.CompareTag("Heal")) healthComponent.Heal();
    }
    private void Flip()
    {
        if (rb.velocity.x > 0 ) sprite.transform.localScale = new Vector2(Mathf.Abs(sprite.transform.localScale.x), sprite.transform.localScale.y) ;
        else if (rb.velocity.x < 0) sprite.transform.localScale = new Vector2(-Mathf.Abs(sprite.transform.localScale.x), sprite.transform.localScale.y) ;
    }
}