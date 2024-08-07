using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class ManagementMenu : MonoBehaviour
{
    [SerializeField] private GameObject botonPausa;
    [SerializeField] private GameObject menuPausa;
    [SerializeField] private GameObject panelConfirmacion;
    string filePath = Application.dataPath + "/data.mrmenu";

    public void Tutorial()
    {
        SceneManager.LoadScene(1);
    }
  
    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }

    public void TerminarPartida()
    {
        SceneManager.LoadScene(0);
    }

    public void VolverJugar()
    {
        SceneManager.LoadScene(4);
    }

    public void NuevoJuego()
    {

        if (File.Exists(filePath))
        {
            panelConfirmacion.SetActive(true);

        }
        else
        {
            SceneManager.LoadScene(5);
        }
    }

    public void PartidaGuardada()
    {

        SceneManager.LoadScene(4);
    }

    public void PausarPartida()
    {
        Time.timeScale = 0f;
        botonPausa.SetActive(false);
        menuPausa.SetActive(true);
    }

    public void ReanudarPartida()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }

    public void Reiniciar()
    {
        Time.timeScale = 1f;
        PartidaGuardada();
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
    }

    public void Cerrar()
    {
        Time.timeScale = 1f;
        botonPausa.SetActive(true);
        menuPausa.SetActive(false);
        TerminarPartida();
    }

    public void Guardar()
    {
        Debug.Log("Aquí va el proceso de guardado");
        DataJuego.data.GuardarData();
    }

    public void ConfirmarPartida()
    {
        File.Delete(filePath);
        Debug.Log("Archivo DataJuego.data eliminado correctamente.");
        SceneManager.LoadScene(5);

    }

    public void DenegarPartida()
    {
        panelConfirmacion.SetActive(false);
    }
}
