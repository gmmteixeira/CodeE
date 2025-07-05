using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Canvas mainMenuCanvas;
    public Slider sensitivitySlider;
    public TMP_InputField sensitivityInputField;
    public Slider volumeSlider;
    public TMP_InputField volumeInputField;

    void Start()
    {
        LoadSettings();
    }
    public void Back()
    {
        gameObject.SetActive(false);
        mainMenuCanvas.gameObject.SetActive(true);
    }

    public void SetSensitivityBySlider()
    {
        float sensitivity = sensitivitySlider.value;
        sensitivityInputField.text = sensitivity.ToString("F2");
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
    }

    public void SetSensitivityByInputField()
    {
        float sensitivity;
        if (float.TryParse(sensitivityInputField.text, out sensitivity))
        {
            sensitivitySlider.value = sensitivity;
            PlayerPrefs.SetFloat("Sensitivity", sensitivity);
        }
        else
        {
            Debug.LogWarning("Invalid sensitivity value entered.");
        }
        
    }

    public void SetVolumeBySlider()
    {
        float volume = volumeSlider.value;
        volumeInputField.text = volume.ToString("F2");
        PlayerPrefs.SetFloat("Volume", volume);
        audioMixer.SetFloat("Volume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("Volume", 1f), 0.0001f, 1f)) * 20f);
    }

    public void SetVolumeByInputField()
    {
        float volume;
        if (float.TryParse(volumeInputField.text, out volume))
        {
            volumeSlider.value = volume;
            PlayerPrefs.SetFloat("Volume", volume);
            audioMixer.SetFloat("Volume", Mathf.Log10(Mathf.Clamp(PlayerPrefs.GetFloat("Volume", 1f), 0.0001f, 1f)) * 20f);
        }
        else
        {
            Debug.LogWarning("Invalid volume value entered.");
        }
    }

    private void LoadSettings()
    {
        
        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.3f);
        float volume = PlayerPrefs.GetFloat("Volume", 1f);
        sensitivitySlider.value = sensitivity;
        sensitivityInputField.text = sensitivity.ToString("F2");
        volumeSlider.value = volume;
        volumeInputField.text = volume.ToString("F2");
        audioMixer.SetFloat("Volume", -80 + PlayerPrefs.GetFloat("Volume", 1f) * 80f);
    }
}
