using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Synthesizer : MonoBehaviour
{
    public Slider frequencySlider;
    public Slider volumeSlider;
    public TMP_Dropdown waveformDropdown;

    private float frequency = 440.0f;
    private float volume = 0.1f;
    private string waveformType = "sine";
    private float increment;
    private float phase;
    private float samplingFrequency = 48000.0f;

    void Start()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.Play();
        frequencySlider.onValueChanged.AddListener(OnFrequencyChanged);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        waveformDropdown.onValueChanged.AddListener(OnWaveformChanged);
        frequencySlider.value = frequency;
        volumeSlider.value = volume;
        waveformDropdown.value = 0;
    }

    void OnFrequencyChanged(float value)
    {
        frequency = value;
    }

    void OnVolumeChanged(float value)
    {
        volume = value;
    }

    void OnWaveformChanged(int index)
    {
        waveformType = waveformDropdown.options[index].text;
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        increment = frequency * 2.0f * Mathf.PI / samplingFrequency;
        for (int i = 0; i < data.Length; i += channels)
        {
            phase += increment;
            data[i] = volume * GenerateWaveform(phase, waveformType);
            if (channels == 2)
            {
                data[i + 1] = data[i];
            }
            if (phase > 2.0f * Mathf.PI)
            {
                phase -= 2.0f * Mathf.PI;
            }
        }
    }

    float GenerateWaveform(float phase, string waveformType)
    {
        switch (waveformType)
        {
            case "sine":
                return Mathf.Sin(phase);
            case "square":
                return Mathf.Sign(Mathf.Sin(phase));
            case "sawtooth":
                return 2.0f * (phase / (2.0f * Mathf.PI)) - 1.0f;
            case "triangle":
                return Mathf.PingPong(2.0f * phase / (2.0f * Mathf.PI), 1.0f) * 2.0f - 1.0f;
            default:
                return Mathf.Sin(phase);
        }
    }

    public void PlayNote(float noteFrequency)
    {
        frequency = noteFrequency;
        StartCoroutine(PlayNoteCoroutine());
    }

    private IEnumerator PlayNoteCoroutine()
    {
        yield return new WaitForSeconds(0.2f);
        frequency = 0.0f;
    }

    public void StopNote()
    {
        frequency = 0.0f;
    }
}