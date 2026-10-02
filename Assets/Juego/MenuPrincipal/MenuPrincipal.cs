using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuPrincipal : MonoBehaviour
{
    public void Jugar()
    {
        SceneManager.LoadScene("Intro");
    }

    public void Salir()
    {
        Debug.Log("Salir...");
        Application.Quit();
    }
}