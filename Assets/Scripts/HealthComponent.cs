using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class HealthComponent : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField] private Slider healthBar;

    private NetworkVariable<int> networkHealth = new NetworkVariable<int>();

    public int MaxHealth => maxHealth;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        networkHealth.OnValueChanged += (oldValue, newValue) => UpdateHealthBar(newValue);
        if (IsServer)
        {
            networkHealth.Value = maxHealth;
        }
        UpdateHealthBar(networkHealth.Value);
    }

    public void ApplyDamage()
    {
        if (IsServer)
        {
            if (networkHealth.Value > 0) networkHealth.Value--;
        }
    }

    public void Heal()
    {
        if (IsServer)
        {
            if (networkHealth.Value < maxHealth) networkHealth.Value++;
        }
    }

    private void UpdateHealthBar(int currentHealth)
    {
        healthBar.value = (float)currentHealth / maxHealth;
    }
}
