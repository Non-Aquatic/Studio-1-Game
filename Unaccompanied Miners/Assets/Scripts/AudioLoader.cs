using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioLoader : MonoBehaviour
{
    public AudioMixer audioMixer;

    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;


    float masterVolume;
    float musicVolume;
    float sfxVolume;



    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            masterVolume = PlayerPrefs.GetFloat("masterVolume");
            musicVolume = PlayerPrefs.GetFloat("musicVolume");
            sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        }
        else
        {
            Debug.Log($"No player pref keys found");
            masterVolume = 1;
            musicVolume = 1;
            sfxVolume = 1;
        }

        if (masterSlider != null)
        {
            masterSlider.SetValueWithoutNotify(masterVolume);
        }
        if (musicSlider != null)
        {
            musicSlider.SetValueWithoutNotify(musicVolume);
        }
        if (sfxSlider != null)
        {
            sfxSlider.SetValueWithoutNotify(sfxVolume);
        }


        UpdateMaster(masterVolume);
        UpdateMusic(musicVolume);
        UpdateSFX(sfxVolume);

    }

    public void UpdateMaster(float newVolume)
    {
        masterVolume = newVolume;
        float temp = Mathf.Log10(masterVolume) * 20;
        audioMixer.SetFloat("MasterVolume", temp);
        //masterAudioGroup.audioMixer.SetFloat("Volume", temp);

        Debug.Log($"Master Volume changed to {masterVolume}, lerped to {temp}");

    }
    public void UpdateMusic(float newVolume)
    {
        musicVolume = newVolume;
        float temp = Mathf.Log10(musicVolume) * 20;

        audioMixer.SetFloat("MusicVolume", temp);
        //musicAudioGroup.audioMixer.SetFloat("volume", temp);
    }
    public void UpdateSFX(float newVolume)
    {
        sfxVolume = newVolume;
        float temp = Mathf.Log10(sfxVolume) * 20;
        audioMixer.SetFloat("SFXVolume",temp);
    }


    public void SavePrefs()
    {
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }


    public void PlayClick()
    {
        this.transform.GetComponent<AudioSource>().Play();
    }
    public void OnDestroy()
    {
        SavePrefs();
    }

}
