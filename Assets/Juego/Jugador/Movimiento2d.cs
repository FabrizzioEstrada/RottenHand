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
    private bool atacandoCuchillo;

    // Variables del ataque Melee Puño (Tecla J)
    [Header("Ataque Melee Puño (Tecla J)")]
    public Transform controladorAtaque;

    public float radioAtaque = 0.5f;

    public int danioAtaque = 20;

    public LayerMask queEsEnemigo;

    // Variables del ataque Melee Cuchillo (Tecla 3)
    [Header("Ataque Cuchillo (Tecla 3)")]
    public float radioAtaqueCuchillo = 0.8f; // Algo más de alcance que el puño
    public int danioCuchillo = 35;          // Mayor daño que el puño

    // Variables del disparo con AR por área (Tecla 2)
    [Header("Configuración Disparo AR (Tecla 2)")]
    public float cadenciaDisparoAR = 0.15f;
    private float tiempoSiguienteDisparoAR = 0f;
    public Vector2 areaDisparoAR = new Vector2(3.0f, 1.2f);
    public int danioDisparoAR = 10;

    // Variables del disparo con Pistola por área (Tecla 1)
    [Header("Configuración Disparo Pistola (Tecla 1)")]
    public float cadenciaDisparoPistola = 0.35f;
    private float tiempoSiguienteDisparoPistola = 0f;
    public Vector2 areaDisparoPistola = new Vector2(2.0f, 1.0f);
    public int danioDisparoPistola = 15;

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

        // Atacar Puño con la tecla J
        if (Keyboard.current != null && Keyboard.current.jKey.wasPressedThisFrame)
        {
            Atacar();
        }

        // Atacar Cuchillo con la tecla 3
        if (Keyboard.current != null && (Keyboard.current.digit3Key.wasPressedThisFrame || Keyboard.current.numpad3Key.wasPressedThisFrame))
        {
            AtacarCuchillo();
        }

        // CONTROL DE DISPAROS CONTINUOS
        ManejarDisparoPistola(); // Tecla 1
        ManejarDisparoAR();      // Tecla 2
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

    // ATAQUE MELEE PUÑO (TECLA J)
    public void Atacar()
    {
        if (atacando || atacandoCuchillo) return;

        atacando = true;

        if (animator != null)
        {
            animator.SetBool("Atacando", true);
        }

        AplicarDanioMelee(radioAtaque, danioAtaque);

        Invoke(nameof(FinAtaque), 0.5f);
    }

    public void FinAtaque()
    {
        atacando = false;

        if (animator != null)
        {
            animator.SetBool("Atacando", false);
        }
    }

    // ATAQUE CUCHILLO (TECLA 3)
    public void AtacarCuchillo()
    {
        if (atacando || atacandoCuchillo) return;

        atacandoCuchillo = true;

        if (animator != null)
        {
            animator.SetBool("atacandoCuchillo", true);
        }

        AplicarDanioMelee(radioAtaqueCuchillo, danioCuchillo);

        Invoke(nameof(FinAtaqueCuchillo), 0.4f);
    }

    public void FinAtaqueCuchillo()
    {
        atacandoCuchillo = false;

        if (animator != null)
        {
            animator.SetBool("atacandoCuchillo", false);
        }
    }

    private void AplicarDanioMelee(float radio, int danio)
    {
        if (controladorAtaque == null) return;

        Collider2D[] enemigos = Physics2D.OverlapCircleAll(
            controladorAtaque.position,
            radio,
            queEsEnemigo
        );

        foreach (Collider2D enemigo in enemigos)
        {
            EnemyController zombie = enemigo.GetComponentInParent<EnemyController>();

            if (zombie != null)
            {
                zombie.RecibeDanio(transform.position, danio);
            }
        }
    }

    // CONTROL DISPARO PISTOLA (TECLA 1)
    private void ManejarDisparoPistola()
    {
        bool presionandoTecla1 = false;

        if (Keyboard.current != null)
        {
            presionandoTecla1 = Keyboard.current.digit1Key.isPressed || Keyboard.current.numpad1Key.isPressed;
        }

        if (presionandoTecla1)
        {
            if (animator != null)
            {
                animator.SetBool("disparandoPistola", true);
            }

            if (Time.time >= tiempoSiguienteDisparoPistola)
            {
                ProcesarDisparoArea(areaDisparoPistola, danioDisparoPistola);
                tiempoSiguienteDisparoPistola = Time.time + cadenciaDisparoPistola;
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("disparandoPistola", false);
            }
        }
    }

    // CONTROL DISPARO AR (TECLA 2)
    private void ManejarDisparoAR()
    {
        bool presionandoTecla2 = false;

        if (Keyboard.current != null)
        {
            presionandoTecla2 = Keyboard.current.digit2Key.isPressed || Keyboard.current.numpad2Key.isPressed;
        }

        if (presionandoTecla2)
        {
            if (animator != null)
            {
                animator.SetBool("disparando", true);
            }

            if (Time.time >= tiempoSiguienteDisparoAR)
            {
                ProcesarDisparoArea(areaDisparoAR, danioDisparoAR);
                tiempoSiguienteDisparoAR = Time.time + cadenciaDisparoAR;
            }
        }
        else
        {
            if (animator != null)
            {
                animator.SetBool("disparando", false);
            }
        }
    }

    private void ProcesarDisparoArea(Vector2 area, int danio)
    {
        if (controladorAtaque == null) return;

        Collider2D[] enemigosImpactados = Physics2D.OverlapBoxAll(
            controladorAtaque.position,
            area,
            0f,
            queEsEnemigo
        );

        foreach (Collider2D enemigo in enemigosImpactados)
        {
            EnemyController zombie = enemigo.GetComponentInParent<EnemyController>();

            if (zombie != null)
            {
                zombie.RecibeDanio(transform.position, danio);
            }
        }
    }

    // DIBUJAR ÁREAS EN EL EDITOR DE SCENE
    private void OnDrawGizmosSelected()
    {
        if (controladorAtaque != null)
        {
            // Círculo Melee Puño (Rojo)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(controladorAtaque.position, radioAtaque);

            // Círculo Melee Cuchillo (Amarillo)
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(controladorAtaque.position, radioAtaqueCuchillo);

            // Caja AR (Cian)
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(controladorAtaque.position, areaDisparoAR);

            // Caja Pistola (Verde)
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(controladorAtaque.position, areaDisparoPistola);
        }
    }

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