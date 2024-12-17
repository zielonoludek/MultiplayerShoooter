using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;


public class ShooterComponent : NetworkBehaviour
{
    private PlayerInputActions playerActions;
    private NetworkVariable<ushort> ammoNum = new NetworkVariable<ushort>(5);
    [SerializeField] private Bullet bulletObj;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        playerActions = new PlayerInputActions();
        playerActions.Player.Enable();
        if (!IsServer) ammoNum.OnValueChanged += SpawnBullet;

        playerActions.Player.Shoot.performed += context =>
        {
            SpawnBullet(0,0);
        };
    }

    private void SpawnBullet(ushort oldValue, ushort newValue)
    {
        if (!IsOwner || !Application.isFocused) return;
        Bullet b = Instantiate(bulletObj);
        b.Setup(gameObject, Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
    }
}
