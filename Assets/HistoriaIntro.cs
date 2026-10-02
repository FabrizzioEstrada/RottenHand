using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class HistoriaIntro : MonoBehaviour
{
    [Header("Historia")]
    public Image imagenHistoria;
    public Sprite[] imagenes;

    [Header("Pantalla negra")]
    public Image panelNegro;

    [Header("Texto")]
    public TextMeshProUGUI textoContinuar;

    [Header("Configuración")]
    public string escenaNivel = "Nivel1";
    public float velocidadFade = 1.5f;

    private int imagenActual = 0;
    private bool cambiandoEscena = false;

    void Start()
    {
        // Mostrar la primera imagen
        imagenHistoria.sprite = imagenes[0];

        // Hacer transparente el panel negro
        Color color = panelNegro.color;
        color.a = 0f;
        panelNegro.color = color;

        // Mostrar el texto
        textoContinuar.text = "[ ESPACIO ] CONTINUAR";
    }

    void Update()
    {
        if (cambiandoEscena)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SiguienteImagen();
        }
    }

    void SiguienteImagen()
    {
        imagenActual++;

        if (imagenActual < imagenes.Length)
        {
            imagenHistoria.sprite = imagenes[imagenActual];
        }
        else
        {
            StartCoroutine(FinalizarHistoria());
        }
    }

    IEnumerator FinalizarHistoria()
    {
        cambiandoEscena = true;

        // Ocultar el texto
        textoContinuar.gameObject.SetActive(false);

        // Hacer fade a negro
        yield return StartCoroutine(FadeNegro(0f, 1f));

        // Esperar un segundo
        yield return new WaitForSeconds(1f);

        // Cargar el nivel
        SceneManager.LoadScene(escenaNivel);
    }

    IEnumerator FadeNegro(float inicio, float final)
    {
        float tiempo = 0f;

        Color color = panelNegro.color;

        while (tiempo < velocidadFade)
        {
            tiempo += Time.deltaTime;

            float alpha = Mathf.Lerp(
                inicio,
                final,
                tiempo / velocidadFade
            );

            color.a = alpha;
            panelNegro.color = color;

            yield return null;
        }

        color.a = final;
        panelNegro.color = color;
    }
}