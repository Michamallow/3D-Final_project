using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioMix : MonoBehaviour
{

    public Slider sliderVolume;
    public Slider sliderMusique;
    private int i = 1;

    // Start is called before the first frame update
    void Start()
    {
        SettingMenu setting = UnityEngine.Object.FindObjectOfType<SettingMenu>();
        Debug.Log(setting.GetMusique());
        sliderMusique.value = setting.GetMusique();
        sliderVolume.value = setting.GetVolume();

        
    }

    // Update is called once per frame
    void Update()
    {
        while (i < 2)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "Niveau1")
            {
                SettingMenu setting = UnityEngine.Object.FindObjectOfType<SettingMenu>();
                setting.ChangeMusic();
            }
            i++;
        }
    }

    public void SetMusique(float musique)
    {
        SettingMenu setting = UnityEngine.Object.FindObjectOfType<SettingMenu>();
        setting.SetMusique(musique);
    }

    public void SetVolume(float volume)
    {
        SettingMenu setting = UnityEngine.Object.FindObjectOfType<SettingMenu>();
        setting.SetVolume(volume);
    }


}
