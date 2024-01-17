using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject PanelPrincipal;
    public GameObject PanelOption;
    public bool panelOnOff;
    private Button btnOption;
    private Button btnNewGame;
    private Button btnQuitter;
    private Button btnBack;

    // Ajoutez des champs privés pour stocker les valeurs de l'InputField et du Toggle
    private InputField inputNumbDrap;
    private Toggle toggleFlagNames;

    public void LOAD_SCENE()
    {
        SceneManager.LoadScene("EarthScene");
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

        // Récupérez les références des éléments de l'UI
        inputNumbDrap = GameObject.Find("InputNumFlag").GetComponent<InputField>();
        toggleFlagNames = GameObject.Find("ToggleFlagNames").GetComponent<Toggle>();

        // Ajoutez un auditeur pour le bouton de retour
        btnBack = GameObject.Find("Button Retour").GetComponent<Button>();
        btnBack.onClick.AddListener(BUTTON_BACK);
    }

    public void BUTTON_BACK()
    {
        // Enregistrez les valeurs des éléments dans des variables
        int numbDrapValue = 10;
        if (inputNumbDrap.text != "")
        {
            Debug.Log(inputNumbDrap.text);
            numbDrapValue = int.Parse(inputNumbDrap.text);
        }
        bool flagNamesEnabled = toggleFlagNames.isOn;

        PanelOption.SetActive(false);
        PanelPrincipal.SetActive(true);

        PlayerPrefs.SetInt("NumberOfFlags", numbDrapValue);
        PlayerPrefs.SetInt("ShowFlagNames", flagNamesEnabled ? 1 : 0);
        PlayerPrefs.Save();

    }

    void Start()
    {
        btnOption = GameObject.Find("Button option").GetComponent<Button>();
        btnOption.onClick.AddListener(OPTION);

        btnNewGame = GameObject.Find("Button New Game").GetComponent<Button>();
        btnNewGame.onClick.AddListener(LOAD_SCENE);

        btnQuitter = GameObject.Find("Button Quiter").GetComponent<Button>();
        btnQuitter.onClick.AddListener(ExitGame);
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
