using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Image rellenoBarraVida;
    private Movimiento2d playerController;
    private float vidaMaxima;

    void Start()
    {
        playerController = GameObject.Find("Jugador").GetComponent<Movimiento2d>();
        vidaMaxima = playerController.vida;
    }

    void Update()
    {
        rellenoBarraVida.fillAmount = playerController.vida / vidaMaxima;
    }    

}
