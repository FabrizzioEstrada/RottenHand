using UnityEngine;
using UnityEngine.InputSystem;

public class Movimiento2d : MonoBehaviour
{
    public Controles Controles;

    public Vector2 direccion;

    public Rigidbody2D rb2D;

    public float velocidadMovimiento = 5f;

    public float fuerzasalto = 5f;

    public LayerMask queesSuelo;

    public Transform controladorSuelo;

    public Vector3 dimensionesCaja;

    public bool enSuelo;

    public float vida = 100f;

    private bool recibiendoDanio;

    public Animator animator;

    private bool atacando;

    // Variables del ataque
    public Transform controladorAtaque;

    public float radioAtaque = 0.5f;

    public int danioAtaque = 20;

    public LayerMask queEsEnemigo;

    private void Awake()
    {
        Controles = new Controles();
    }

    private void OnEnable()
    {
        Controles.Enable();
        Controles.Movimiento.Saltar.started += AlSaltar;
    }

    private void OnDisable()
    {
        if (Controles != null)
        {
            Controles.Movimiento.Saltar.started -= AlSaltar;
            Controles.Disable();
        }
    }

    private void AlSaltar(InputAction.CallbackContext contexto)
    {
        Saltar();
    }

    private void Update()
    {
        direccion = Controles.Movimiento.Mover.ReadValue<Vector2>();

        if (controladorSuelo != null)
        {
            enSuelo = Physics2D.OverlapBox(
                controladorSuelo.position,
                dimensionesCaja,
                0f,
                queesSuelo
            );
        }

        // Actualizar animaciones de movimiento
        if (animator != null)
        {
            animator.SetFloat("movement", Mathf.Abs(direccion.x));
            animator.SetBool("ensuelo", enSuelo);
        }

        // Voltear al jugador según su dirección
        if (direccion.x < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (direccion.x > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }

        // Atacar con la tecla J
        if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
        {
            Atacar();
        }
    }

    private void FixedUpdate()
    {
        if (rb2D != null)
        {
            rb2D.linearVelocity = new Vector2(
                direccion.x * velocidadMovimiento,
                rb2D.linearVelocity.y
            );
        }
    }

    private void Saltar()
    {
        if (enSuelo && rb2D != null)
        {
            rb2D.AddForce(
                new Vector2(0, fuerzasalto),
                ForceMode2D.Impulse
            );
        }
    }

    // ATAQUE DEL JUGADOR
    public void Atacar()
    {
        if (atacando)
        {
            return;
        }

        atacando = true;

        // Activa el parámetro "Atacando" (bool) del Animator
        if (animator != null)
        {
            animator.SetBool("Atacando", true);
        }

        // Detección de enemigos
        if (controladorAtaque != null)
        {
            Collider2D[] enemigos = Physics2D.OverlapCircleAll(
                controladorAtaque.position,
                radioAtaque,
                queEsEnemigo
            );

            foreach (Collider2D enemigo in enemigos)
            {
                // Buscar el script del enemigo
                EnemyController zombie = enemigo.GetComponentInParent<EnemyController>();

                if (zombie != null)
                {
                    zombie.RecibeDanio(transform.position, danioAtaque);
                }
            }
        }

        // Permitir atacar nuevamente y desactivar la animación después de un tiempo
        Invoke(nameof(FinAtaque), 0.5f);
    }

    public void FinAtaque()
    {
        atacando = false;

        // Apaga la animación de ataque
        if (animator != null)
        {
            animator.SetBool("Atacando", false);
        }
    }

    // Dibujar el área de ataque en el editor
    private void OnDrawGizmosSelected()
    {
        if (controladorAtaque != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(
                controladorAtaque.position,
                radioAtaque
            );
        }
    }

    // DIBUJAR EL ÁREA DEL SUELO
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        if (controladorSuelo != null)
        {
            Gizmos.DrawWireCube(
                controladorSuelo.position,
                dimensionesCaja
            );
        }
    }

    // DAÑO RECIBIDO POR EL JUGADOR
    public void RecibeDanio(Vector2 direccion, int cantDanio)
    {
        if (!recibiendoDanio)
        {
            recibiendoDanio = true;

            vida -= cantDanio;

            if (vida <= 0)
            {
                vida = 0;
            }

            if (rb2D != null)
            {
                Vector2 rebote = new Vector2(
                    transform.position.x - direccion.x,
                    1
                ).normalized;

                rb2D.AddForce(
                    rebote,
                    ForceMode2D.Impulse
                );
            }

            Invoke(nameof(DesactivaDanio), 0.5f);
        }
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
    }
}