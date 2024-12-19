using TMPro;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText; 
    [SerializeField] private GameObject chatBubble; 
    [SerializeField] private float displayDuration = 5f; 

    private void Awake()
    {
        ClearBubble();
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        chatBubble.SetActive(true);
        CancelInvoke(nameof(ClearBubble)); 
        Invoke(nameof(ClearBubble), displayDuration);
    }

    private void ClearBubble()
    {
        messageText.text = "";
    }
}
