using UnityEngine;

public class CamaraJugador : MonoBehaviour
{
    public Transform objetivo;
    public Vector3 desplazamiento;

    private void LateUpdate()
    {
        transform.position = new Vector3(
            objetivo.position.x + desplazamiento.x,
            transform.position.y,
            transform.position.z
        );
    }
}