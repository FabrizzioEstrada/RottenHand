using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Configuración del Enemigo")]
    public Transform player;
    public float detectionRadius = 5.0f;
    public float speed = 2.0f;
    public int cantidadDanio = 10;
    public int vida = 100;

    [Header("Ataque del Zombi")]
    public float tiempoEntreAtaques = 1.0f;
    private float tiempoSiguienteAtaque;

    [Header("Efecto de Empuje (Knockback)")]
    public float fuerzaEmpuje = 5.0f;
    public float duracionEmpuje = 0.2f;
    private bool estaSiendoEmpujado;

    private Rigidbody2D rb;
    private Vector2 movement;
    private bool enMovimiento;
    private bool muerto;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

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
        if (muerto || player == null || estaSiendoEmpujado)
        {
            if (estaSiendoEmpujado) return;
            movement = Vector2.zero;
            enMovimiento = false;
            return;
        }

        float distanciaPlayer = Vector2.Distance(transform.position, player.position);

        if (distanciaPlayer < detectionRadius)
        {
            Vector2 direction = (player.position - transform.position).normalized;

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

        if (animator != null)
        {
            animator.SetBool("enMovimiento", enMovimiento);

            if (enMovimiento && Time.time >= tiempoSiguienteAtaque)
            {
                animator.SetBool("Atacando", false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (muerto || rb == null || estaSiendoEmpujado) return;

        rb.MovePosition(rb.position + movement * speed * Time.fixedDeltaTime);
    }

    // RECIBIR DAÑO Y APLICAR EMPUJE HACIA ATRÁS
    public void RecibeDanio(Vector3 posicionAtacante, int cantidadDanioRecibido)
    {
        if (muerto) return;

        vida -= cantidadDanioRecibido;
        Debug.Log("¡El zombi recibió daño! Vida restante: " + vida);

        if (vida <= 0)
        {
            Morir();
            return;
        }

        // Aplicar fuerza de retroceso (Knockback)
        if (rb != null)
        {
            estaSiendoEmpujado = true;

            // Dirección horizontal hacia donde fue empujado (alejándolo del jugador)
            float direccionHorizontal = transform.position.x - posicionAtacante.x;
            Vector2 direccionEmpuje = new Vector2(Mathf.Sign(direccionHorizontal), 0.3f).normalized;

            // Reinicia la velocidad actual y aplica el impulso
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direccionEmpuje * fuerzaEmpuje, ForceMode2D.Impulse);

            // Detener el empuje después de un breve tiempo
            Invoke(nameof(DetenerEmpuje), duracionEmpuje);
        }
    }

    private void DetenerEmpuje()
    {
        estaSiendoEmpujado = false;
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void Morir()
    {
        muerto = true;
        movement = Vector2.zero;
        Debug.Log("¡El zombi ha muerto!");
        Destroy(gameObject);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (muerto || estaSiendoEmpujado) return;

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.name == "Jugador")
        {
            if (Time.time >= tiempoSiguienteAtaque)
            {
                if (animator != null)
                {
                    animator.SetBool("Atacando", true);
                }

                Movimiento2d jugador = collision.gameObject.GetComponent<Movimiento2d>();
                if (jugador != null)
                {
                    Vector2 direccionDanio = new Vector2(transform.position.x, 0);
                    jugador.RecibeDanio(direccionDanio, cantidadDanio);
                }

                tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
                Invoke(nameof(ResetAtacando), 0.3f);
            }
        }
    }

    private void ResetAtacando()
    {
        if (animator != null)
        {
            animator.SetBool("Atacando", false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}