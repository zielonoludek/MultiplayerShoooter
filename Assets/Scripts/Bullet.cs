using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class Bullet : NetworkBehaviour
{
    private Vector3 target;
    private Rigidbody2D rb;
    private GameObject parent;
    private float speed = 1000;

    public void Setup(GameObject parent, Vector3 targer)
    {
        rb = GetComponent<Rigidbody2D>();
        this.parent = parent;
        transform.position = parent.transform.position + new Vector3(0, 0.5f, 0);

        Vector3 force = (target - transform.position).normalized;
        rb.AddForce(force * speed);

        float angle = Mathf.Atan2(force.y, force.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (parent != collision.gameObject) Destroy(gameObject);
    }
}