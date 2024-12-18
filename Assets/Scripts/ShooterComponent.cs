using System.Globalization;
using Unity.Netcode;
using UnityEngine.InputSystem;
using UnityEngine;

public class ShooterComponent : NetworkBehaviour
{
    private PlayerInputActions playerActions;
    private NetworkVariable<ushort> ammoNum = new NetworkVariable<ushort>(5);
    [SerializeField] private GameObject bulletObj;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        playerActions = new PlayerInputActions();
        playerActions.Player.Enable();
        playerActions.Player.Shoot.performed += context =>
        {
            if (!IsOwner || !Application.isFocused) return;

            Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mouseWorldPosition.z = 0;
            SpawnBulletServerRpc(mouseWorldPosition);
        };
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(Vector3 mouseWorldPosition)
    {
        GameObject b = Instantiate(bulletObj, transform.position, Quaternion.identity);
        var bulletNetworkObject = b.GetComponent<NetworkObject>();
        bulletNetworkObject.Spawn();

        b.GetComponent<Bullet>().parent = this;
        b.GetComponent<Bullet>().Setup(mouseWorldPosition);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyServerRpc(ulong networkObjectId)
    {
        if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out var networkObject))
        {
            networkObject.Despawn();
            Destroy(networkObject.gameObject);
        }
    }

    public void RequestDestroy(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<NetworkObject>(out var networkObject))
        {
            DestroyServerRpc(networkObject.NetworkObjectId);
        }
    }
}
