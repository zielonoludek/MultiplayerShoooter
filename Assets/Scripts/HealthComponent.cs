using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class HealthComponent : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 5;
    [SerializeField]  private int health;

    public int MaxHealth { get { return maxHealth; } }
    public int Health { get { return health; } }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        SetMaxHealth();
    }
    public void SetMaxHealth() => health = MaxHealth;
    public void ApplyDamage()
    {
        health--;
    }
    public void Heal()
    {
        health++;
    }
}