using UnityEngine;

public class CamaraJugador : MonoBehaviour
{
    public Transform objetivo;
    public Vector3 desplazamiento;

    [Header("Límites de la Cámara")]
    public float minX; // Límite mínimo en X (ej. el borde izquierdo de tu fondo)
    public float maxX; // Límite máximo en X (ej. el borde derecho de tu fondo)

    private void LateUpdate()
    {
        if (objetivo == null) return;

        // 1. Calculamos la posición en X a la que debería ir la cámara
        float posXDeseada = objetivo.position.x + desplazamiento.x;

        // 2. Restringimos esa posición entre minX y maxX
        float posXLimitada = Mathf.Clamp(posXDeseada, minX, maxX);

        // 3. Aplicamos la nueva posición a la cámara
        transform.position = new Vector3(
            posXLimitada,
            transform.position.y,
            transform.position.z
        );
    }
}