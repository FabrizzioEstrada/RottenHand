using Unity.Android.Gradle.Manifest;
using UnityEngine;

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

    private void Awake()
    {
        Controles = new();
    }

    private void OnEnable()
    {
        Controles.Enable();
        Controles.Movimiento.Saltar.started += _ => Saltar();

    }

    private void OnDisable()
    {
        Controles.Disable();
        Controles.Movimiento.Saltar.started -= _ => Saltar();
    }

    private void Update()
    {
        direccion = Controles.Movimiento.Mover.ReadValue<Vector2>();

        enSuelo = Physics2D.OverlapBox(controladorSuelo.position, dimensionesCaja, 0f, queesSuelo);
    }

    private void FixedUpdate()
    {
        rb2D.linearVelocity = new Vector2(direccion.x * velocidadMovimiento, rb2D.linearVelocity.y);
    }

    private void Saltar()
    {
        if (enSuelo)
        {
            rb2D.AddForce(new Vector2(0, fuerzasalto), ForceMode2D.Impulse);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(controladorSuelo.position,dimensionesCaja);
    }
}
