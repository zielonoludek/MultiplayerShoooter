using System.Globalization;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float speed = 3;
    [SerializeField] private float jumpForce = 3;

    private bool canJump = false;
    private Rigidbody2D rb;

    private PlayerInputActions playerActions;
    private float movementInput;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody2D>();

        playerActions = new PlayerInputActions();
        playerActions.Player.Enable();

        playerActions.Player.Jump.performed += Jump;
        playerActions.Player.Movement.performed += context => movementInput = context.ReadValue<float>();
        playerActions.Player.Movement.canceled += context => movementInput = 0f;
    }

    private void Update()
    {
        if (!IsOwner || !Application.isFocused) return;
        rb.velocity = new Vector2(movementInput * speed, rb.velocity.y);
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!canJump) return;

        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        canJump = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        canJump = true;
    }
}
