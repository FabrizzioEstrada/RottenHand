using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float distanciaPlayer = Vector2.Distance(transform.position, player.position);

        if (distanciaPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            movement = new Vector2(direction.x, 0);
        }

        else
        {
            movement = Vector2.zero;
        }

        rb.MovePosition(rb.position + movement * speed * Time.deltaTime);

    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
