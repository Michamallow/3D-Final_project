using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SettingMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    private static SettingMenu setting;
    public AudioClip sonJeu;

    private void Awake()
    {
        if (setting == null)
        {

            // DontDestroyOnLoad(this.gameObject);
            setting = this;
        }
        else if (setting != this)
        {
            Destroy(this.gameObject);
        }
    }
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("fx", volume);
    }

    public void SetMusique(float musique)
    {
        audioMixer.SetFloat("musique", musique);
    }

    public float GetVolume()
    {
        float vol;
        bool res = audioMixer.GetFloat("fx", out vol);

        if (res)
        {
            return vol;
        }
        else
        {
            return 0f;
        }
    }
    public float GetMusique()
    {
        float mus;
        bool res = audioMixer.GetFloat("musique", out mus);

        if (res)
        {
            return mus;
        }
        else
        {
            return 0f;
        }
    }

    public void ChangeMusic()
    {
        AudioSource audio = GetComponent<AudioSource>();
        audio.clip = sonJeu;
        audio.Play();
    }

    public void ToggleFlagName(bool flag)
    {
        PlayerPrefs.SetInt("ShowFlagNames", flag ? 1 : 0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
