using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using Unity.Netcode;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] private TMP_InputField chatInputField;
    private PlayerInputActions playerActions;

    public bool IsChatFocused => chatInputField.isFocused;


    private void Awake()
    {
        playerActions = new PlayerInputActions();
        playerActions.UI.SubmitChat.performed += OnSubmitChat;
        playerActions.UI.Enable();

    }

    private void Update()
    {
        if (IsOwner && !chatInputField.isFocused)
        {
            chatInputField.DeactivateInputField();
        }
    }

    private void OnSubmitChat(InputAction.CallbackContext context)
    {
        if (!string.IsNullOrWhiteSpace(chatInputField.text)) SendChatMessage(chatInputField.text);
    }


    private void SendChatMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        chatInputField.text = string.Empty;
        chatInputField.ActivateInputField();

        SendChatMessageServerRpc(NetworkManager.LocalClientId, message);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(ulong senderId, string message)
    {
        BroadcastChatMessageClientRpc(senderId, message);
    }

    [ClientRpc]
    private void BroadcastChatMessageClientRpc(ulong senderId, string message)
    {
        foreach (var player in FindObjectsOfType<PlayerController>())
        {
            if (player.OwnerClientId == senderId)
            {
                player.DisplayChatMessage(message);
                break;
            }
        }
    }
}
