using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Configuración del Enemigo")]
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    public int cantidadDanio = 10;
    public int vida = 100;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool enMovimiento;
    private bool muerto;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Si no asignaste el Jugador desde el Inspector, lo busca automáticamente por Tag
        if (player == null)
        {
            GameObject jugadorObj = GameObject.FindGameObjectWithTag("Player");
            if (jugadorObj != null)
            {
                player = jugadorObj.transform;
            }
        }
    }

    void Update()
    {
        if (muerto || player == null)
        {
            movement = Vector2.zero;
            enMovimiento = false;
            return;
        }

        float distanciaPlayer = Vector2.Distance(transform.position, player.position);

        if (distanciaPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

            // Voltear el zombi según su orientación
            if (direction.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (direction.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }

            movement = new Vector2(direction.x, 0);
            enMovimiento = true;
        }
        else
        {
            movement = Vector2.zero;
            enMovimiento = false;
        }

        // Actualizar parámetros de animación
        if (animator != null)
        {
            animator.SetBool("enMovimiento", enMovimiento);
        }
    }

    private void FixedUpdate()
    {
        if (muerto || rb == null) return;

        // El movimiento físico se ejecuta de manera fluida en FixedUpdate
        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    // RECIBIR DAÑO DEL JUGADOR
    public void RecibeDanio(int cantidadDanioRecibido)
    {
        if (muerto) return;

        vida -= cantidadDanioRecibido;
        Debug.Log("¡El zombi recibió daño! Vida restante: " + vida);

        if (vida <= 0)
        {
            Morir();
        }
    }

    // MUERTE DEL ZOMBI
    private void Morir()
    {
        muerto = true;
        movement = Vector2.zero;

        Debug.Log("¡El zombi ha muerto!");

        // Destruir el objeto enemigo
        Destroy(gameObject);
    }

    // EL ZOMBI HACE DAÑO AL JUGADOR
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (muerto) return;

        // Verifica si colisionó con el jugador por Tag o por nombre
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name == "Jugador")
        {
            Movimiento2d jugador = collision.gameObject.GetComponent<Movimiento2d>();

            if (jugador != null)
            {
                Vector2 direccionDanio = new Vector2(transform.position.x, 0);
                jugador.RecibeDanio(direccionDanio, cantidadDanio);
            }
        }
    }

    // MOSTRAR RADIO DE DETECCIÓN EN ESCENA
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}