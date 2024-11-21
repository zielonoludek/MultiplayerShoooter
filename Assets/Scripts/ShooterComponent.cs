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
            if (!IsOwner || !Application.isFocused)
            {
                Bullet b = Instantiate(bulletObj);
                b.Setup(gameObject, Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()));
            }
        };
    }

    private void SpawnBullet(ushort oldValue, ushort newValue)
    {
        //Bullet b = Instantiate(bulletObj);
    }
}
