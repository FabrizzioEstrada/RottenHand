using System.Collections;
using UnityEngine;

public class TutorialInicio : MonoBehaviour
{
    [Header("UI del Tutorial")]
    [Tooltip("Arrastra aquí el objeto del texto con el componente Canvas Group")]
    public CanvasGroup cartelCanvasGroup;

    [Header("Tiempos de la Secuencia")]
    public float esperaInicial = 1.5f;   // Espera antes de aparecer
    public float tiempoAparicion = 1.0f; // Tiempo que tarda el Fade In
    public float duracionTexto = 10.0f;  // Tiempo visible al 100%
    public float tiempoDesaparicion = 1.0f; // Tiempo que tarda el Fade Out

    private void Start()
    {
        if (cartelCanvasGroup != null)
        {
            // Ocultamos el texto y lo dejamos transparente al inicio
            cartelCanvasGroup.alpha = 0f;
            cartelCanvasGroup.gameObject.SetActive(true);
        }

        StartCoroutine(SecuenciaTutorialConFade());
    }

    private IEnumerator SecuenciaTutorialConFade()
    {
        // 1. Espera inicial (animación de inicio del mapa)
        yield return new WaitForSeconds(esperaInicial);

        // 2. Efecto de aparición suave (Fade In)
        float contador = 0f;
        while (contador < tiempoAparicion)
        {
            contador += Time.deltaTime;
            cartelCanvasGroup.alpha = Mathf.Lerp(0f, 1f, contador / tiempoAparicion);
            yield return null;
        }
        cartelCanvasGroup.alpha = 1f; // Aseguramos que quede al 100%

        // 3. Tiempo visible
        yield return new WaitForSeconds(duracionTexto);

        // 4. Efecto de desaparición suave (Fade Out)
        contador = 0f;
        while (contador < tiempoDesaparicion)
        {
            contador += Time.deltaTime;
            cartelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, contador / tiempoDesaparicion);
            yield return null;
        }
        cartelCanvasGroup.alpha = 0f;

        // Desactivamos el objeto completamente
        cartelCanvasGroup.gameObject.SetActive(false);
    }
}