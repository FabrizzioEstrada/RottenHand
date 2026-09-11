using UnityEngine;
using UnityEngine.InputSystem;

public class Movimiento2d : MonoBehaviour
{
    public Controles Controles;

    public Vector2 direccion;

    public Rigidbody2D rb2D;

    public float velocidadMovimiento;

    public float fuerzasalto;

    public LayerMask queesSuelo;

    public Transform controladorSuelo;

    public Vector3 dimensionesCaja;

    public bool enSuelo;

    public float vida = 100f;

    private bool recibiendoDanio;

    private void Awake()
    {
        Controles = new();
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
            Controles.Disable();
            Controles.Movimiento.Saltar.started -= AlSaltar;
        }
    }

    private void AlSaltar(InputAction.CallbackContext contexto)
    {
        Saltar();
    }

    private void Update()
    {
        direccion = Controles.Movimiento.Mover.ReadValue<Vector2>();

        enSuelo = Physics2D.OverlapBox(
            controladorSuelo.position,
            dimensionesCaja,
            0f,
            queesSuelo
        );
    }

    private void FixedUpdate()
    {
        rb2D.linearVelocity = new Vector2(
            direccion.x * velocidadMovimiento,
            rb2D.linearVelocity.y
        );
    }

    private void Saltar()
    {
        if (enSuelo)
        {
            rb2D.AddForce(
                new Vector2(0, fuerzasalto),
                ForceMode2D.Impulse
            );
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(
            controladorSuelo.position,
            dimensionesCaja
        );
    }

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

            Vector2 rebote = new Vector2(
                transform.position.x - direccion.x,
                1
            ).normalized;

            rb2D.AddForce(
                rebote,
                ForceMode2D.Impulse
            );

            Invoke(nameof(DesactivaDanio), 0.5f);
        }
    }

    public void DesactivaDanio()
    {
        recibiendoDanio = false;
    }
}