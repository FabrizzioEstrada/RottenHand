using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    public int cantidadDanio = 10;

    private Rigidbody2D rb;
    private Vector2 movement;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float distanciaPlayer = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distanciaPlayer < detectionRadius)
        {
            Vector2 direction = (
                player.position - transform.position
            ).normalized;

            movement = new Vector2(direction.x, 0);
        }

        else
        {
            movement = Vector2.zero;
        }

        rb.MovePosition(
            rb.position + movement * speed * Time.deltaTime
        );
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Jugador")
        {
            Vector2 direccionDanio = new Vector2(transform.position.x, 0);

            collision.gameObject
                .GetComponent<Movimiento2d>()
                .RecibeDanio(direccionDanio, cantidadDanio);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            detectionRadius
        );
    }
}