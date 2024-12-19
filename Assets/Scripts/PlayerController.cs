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
    [SerializeField] private ChatBubble chatBubble;

    private Rigidbody2D rb;
    private HealthComponent healthComponent;
    private PlayerInputActions playerActions;

    private ChatManager chatManager;

    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>();
    private NetworkVariable<float> playerDirection = new NetworkVariable<float>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        rb = GetComponent<Rigidbody2D>();
        healthComponent = GetComponent<HealthComponent>();
        chatBubble = GetComponentInChildren<ChatBubble>();

        chatManager = FindObjectOfType<ChatManager>();

        playerActions = new PlayerInputActions();
        playerActions.Player.Enable();

        playerActions.Player.Jump.performed += Jump;
        playerActions.Player.Movement.performed += context =>
        {
            if (!IsOwner || !Application.isFocused || (chatManager != null && chatManager.IsChatFocused)) return;
            movementInput = context.ReadValue<float>();
        };
        playerActions.Player.Movement.canceled += context =>
        {
            if (!IsOwner || !Application.isFocused) return;
            movementInput = 0f;
        };
         
        if (IsOwner) RequestRandomColorServerRpc();

        playerColor.OnValueChanged += (oldColor, newColor) => sprite.GetComponent<SpriteRenderer>().color = newColor;

        playerDirection.OnValueChanged += (oldDir, newDir) => UpdateFlip(newDir);
    }

    private void Update()
    {
        if (!IsOwner || !Application.isFocused || (chatManager != null && chatManager.IsChatFocused))
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        rb.velocity = new Vector2(movementInput * speed, rb.velocity.y);

        if (movementInput != 0) RequestFlipServerRpc(Mathf.Sign(movementInput));
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

    private void UpdateFlip(float direction)
    {
        if (direction > 0) sprite.transform.localScale = new Vector2(Mathf.Abs(sprite.transform.localScale.x), sprite.transform.localScale.y);
        else if (direction < 0) sprite.transform.localScale = new Vector2(-Mathf.Abs(sprite.transform.localScale.x), sprite.transform.localScale.y);
    }

    [ServerRpc]
    private void RequestRandomColorServerRpc(ServerRpcParams rpcParams = default)
    {
        playerColor.Value = new Color(Random.value, Random.value, Random.value);
    }

    [ServerRpc]
    private void RequestFlipServerRpc(float direction, ServerRpcParams rpcParams = default)
    {
        if (Mathf.Sign(playerDirection.Value) != Mathf.Sign(direction)) playerDirection.Value = direction;
    }

    public void DisplayChatMessage(string message)
    {
        if (chatBubble != null) chatBubble.ShowMessage(message);
    }
}
