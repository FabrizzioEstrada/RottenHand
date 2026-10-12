using UnityEngine;
using UnityEngine.UI;

public class BarraVidaEnemigo : MonoBehaviour
{
    public Slider sliderVida;

    public void InicializarVida(float vidaMaxima)
    {
        if (sliderVida == null) return;
        sliderVida.minValue = 0f;
        sliderVida.maxValue = vidaMaxima;
        sliderVida.value = vidaMaxima;
    }

    public void ActualizarVida(float vidaActual)
    {
        if (sliderVida == null) return;
        sliderVida.value = vidaActual; // Fuerza al Slider a actualizar la barra roja
    }
}