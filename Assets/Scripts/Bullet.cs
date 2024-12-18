using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    private Rigidbody2D rb;
    public ShooterComponent parent;
    private float speed = 1000;

    public void Setup(Vector3 target)
    {
        rb = GetComponent<Rigidbody2D>();
        transform.position = parent.transform.position + new Vector3(0, 0.5f, 0);

        Vector3 force = (target - transform.position).normalized;
        rb.AddForce(force * speed);

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), parent.GetComponent<Collider2D>());

        float angle = Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsOwner) return;
        parent.RequestDestroy(gameObject);
    }
}