using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] AudioMixer master;
    [SerializeField] AudioMixerGroup music, sfx;

    [SerializeField] private Slider sliderMaster, sliderMusic, sliderSFX;

    private void Start()
    {
        sliderMaster.value = 0.75f;
        var v1 = Mathf.Log10(sliderMaster.value) * 20;
        master.SetFloat("MasterVolume", v1);

        sliderMusic.value = 0.5f;
        var v2 = Mathf.Log10(sliderMusic.value) * 20;
        music.audioMixer.SetFloat("MusicVolume", v2);

        sliderSFX.value = 0.75f;
        var v3 = Mathf.Log10(sliderSFX.value) * 20;
        sfx.audioMixer.SetFloat("SFXVolume", v3);
    }

    public void SetVolumeMaster(float volume)
    {
        float v = Mathf.Log10(volume) * 20;
        master.SetFloat("MasterVolume", v);

        PlayerPrefs.SetFloat("_Master", volume);
    }

    public void SetVolumeMusic(float volume)
    {
        float v = Mathf.Log10(volume) * 20;
        music.audioMixer.SetFloat("MusicVolume", v);
    }

    public void SetVolumeSFX(float volume)
    {
        float v = Mathf.Log10(volume) * 20;
        sfx.audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
    }
}