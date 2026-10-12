using System.Collections;
using UnityEngine;

public class TutorialPorPosicion : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador; // Arrastra a tu personaje
    public CanvasGroup cartelCanvasGroup; // Arrastra tu TextoCombate_TMP

    [Header("Posición de Activación")]
    public float posicionXActivacion = 5.0f; // La X del mapa donde debe aparecer el texto

    [Header("Tiempos del Fade")]
    public float tiempoAparicion = 0.8f;
    public float duracionTexto = 5.0f;
    public float tiempoDesaparicion = 0.8f;

    private bool yaSeActivo = false;

    private void Update()
    {
        // Si el jugador ya superó la posición X elegida y aún no se ha mostrado el texto
        if (!yaSeActivo && jugador != null && jugador.position.x >= posicionXActivacion)
        {
            yaSeActivo = true;
            StartCoroutine(MostrarTutorialConFade());
        }
    }

    private IEnumerator MostrarTutorialConFade()
    {
        if (cartelCanvasGroup == null) yield break;

        // 1. Fade In (Aparición suave)
        float contador = 0f;
        while (contador < tiempoAparicion)
        {
            contador += Time.deltaTime;
            cartelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, contador / tiempoAparicion);
            yield return null;
        }
        cartelCanvasGroup.alpha = 1f;

        // 2. Mantiene el texto visible
        yield return new WaitForSeconds(duracionTexto);

        // 3. Fade Out (Desaparición suave)
        contador = 0f;
        while (contador < tiempoDesaparicion)
        {
            contador += Time.deltaTime;
            cartelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, contador / tiempoDesaparicion);
            yield return null;
        }
        cartelCanvasGroup.alpha = 0f;
        cartelCanvasGroup.gameObject.SetActive(false);
    }
}