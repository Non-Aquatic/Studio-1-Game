using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Timeline;
using UnityEngine.UI;

//Manages the audio in our game
public class AudioLoader : MonoBehaviour
{
    //Audio Mixer to control the different audio groups
    public AudioMixer audioMixer;

    //Sliders for the volume levels
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    //Variables to store the volume levels 
    float masterVolume;
    float musicVolume;
    float sfxVolume;

    void Start()
    {
        //If the players has a volume saved already
        if (PlayerPrefs.HasKey("masterVolume"))
        {
            //Set the volumes levels to previously saved master, music, and sfx volumes
            masterVolume = PlayerPrefs.GetFloat("masterVolume");
            musicVolume = PlayerPrefs.GetFloat("musicVolume");
            sfxVolume = PlayerPrefs.GetFloat("sfxVolume");
        }
        else
        {
            //If no saved volume levels where found, set to default volume levels
            Debug.Log($"No player pref keys found");
            masterVolume = 1;
            musicVolume = 1;
            sfxVolume = 1;
        }

        //Sets the initial silder values
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

        //Updates the audio mixer with the initial volume levels
        UpdateMaster(masterVolume);
        UpdateMusic(musicVolume);
        UpdateSFX(sfxVolume);

    }
    //Updates the master volume
    public void UpdateMaster(float newVolume)
    {
        masterVolume = newVolume;
        float temp = Mathf.Log10(masterVolume) * 20;
        audioMixer.SetFloat("MasterVolume", temp);
        //masterAudioGroup.audioMixer.SetFloat("Volume", temp);

        Debug.Log($"Master Volume changed to {masterVolume}, lerped to {temp}");

    }
    //Updates the music volume
    public void UpdateMusic(float newVolume)
    {
        musicVolume = newVolume;
        float temp = Mathf.Log10(musicVolume) * 20;

        audioMixer.SetFloat("MusicVolume", temp);
        //musicAudioGroup.audioMixer.SetFloat("volume", temp);
    }
    //Updates the sfx volume
    public void UpdateSFX(float newVolume)
    {
        sfxVolume = newVolume;
        float temp = Mathf.Log10(sfxVolume) * 20;
        audioMixer.SetFloat("SFXVolume",temp);
    }
    //Saves current volume levels to playerPrefs
    public void SavePrefs()
    {
        PlayerPrefs.SetFloat("masterVolume", masterVolume);
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
    }
    //Plays a click sound
    public void PlayClick()
    {
        this.transform.GetComponent<AudioSource>().Play();
    }
    //When object is destroyed, saves all the current volume levels
    public void OnDestroy()
    {
        SavePrefs();
    }
}
