using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject PanelPrincipal;
    public GameObject PanelOption;
    public bool panelOnOff;
    

    public void LOAD_SCENE()
    {
        SceneManager.LoadScene("Flag");
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Exit");
    }

    public void OPTION()
    {
        PanelOption.SetActive(true);
        PanelPrincipal.SetActive(false);
        panelOnOff = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        Time.timeScale = 0;
    }

    public void BUTTON_BACK()
    {
        PanelOption.SetActive(false);
        PanelPrincipal.SetActive(true);
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            panelOnOff = !panelOnOff;
            
            if (panelOnOff)
             {
                 PanelPrincipal.SetActive(false);
                 Cursor.visible = false;
                 Cursor.lockState = CursorLockMode.Locked;
                 Time.timeScale = 1;
             }
             else
             {
                PanelOption.SetActive(false);
                PanelPrincipal.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
                Time.timeScale = 0;
             }
        }

    }
}
