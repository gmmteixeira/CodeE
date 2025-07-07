using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Canvas mainMenuCanvas;
    public Slider sensitivitySlider;
    public TMP_InputField sensitivityInputField;
    public Slider fovSlider;
    public TMP_InputField fovInputField;
    public VolumeProfile volumeProfile;
    public Toggle lensDistortionToggle;
    public Toggle lensFlareToggle;
    public Toggle bloomToggle;
    public Toggle psxToggle;
    public ScriptableRendererFeature psx;

    void Start()
    {
        LoadSettings();
    }
    public void Back()
    {
        gameObject.SetActive(false);
        mainMenuCanvas.gameObject.SetActive(true);
    }

    public void SetSensitivityBySlider(float sensitivity)
    {
        sensitivityInputField.text = sensitivity.ToString("F2");
        PlayerPrefs.SetFloat("Sensitivity", sensitivity);
    }

    public void SetSensitivityByInputField(string sensitivity)
    {
        if (float.TryParse(sensitivity, out float value))
        {
            sensitivitySlider.value = value;
            PlayerPrefs.SetFloat("Sensitivity", value);
        }
        else
        {
            Debug.LogWarning("Invalid sensitivity value entered.");
        }
    }
    public void SetFOVBySlider(float fov)
    {
        PlayerPrefs.SetFloat("FOV", 80 + fov * 80);
        fovInputField.text = PlayerPrefs.GetFloat("FOV").ToString("F2");
    }
    public void SetFOVByInputField(string fov)
    {
        if (float.TryParse(fov, out float value))
        {
            fovSlider.value = (value - 80) / 80;
            PlayerPrefs.SetFloat("FOV", value);
        }
        else
        {
            Debug.LogWarning("Invalid FOV value entered.");
        }
    }
    public void SetLensDistortion(bool enabled)
    {
        PlayerPrefs.SetInt("LensDistortionEnabled", enabled ? 1 : 0);
        if (volumeProfile.TryGet<LensDistortion>(out var lensDistortion))
        {
            lensDistortion.active = enabled;
        }
    }
    public void SetLensFlare(bool enabled)
    {
        PlayerPrefs.SetInt("LensFlareEnabled", enabled ? 1 : 0);
        if (volumeProfile.TryGet<ScreenSpaceLensFlare>(out var lensFlare))
        {
            lensFlare.active = enabled;
        }
    }
    public void SetBloom(bool enabled)
    {
        PlayerPrefs.SetInt("BloomEnabled", enabled ? 1 : 0);
        if (volumeProfile.TryGet<Bloom>(out var bloom))
        {
            bloom.active = enabled;
        }
    }
    public void SetPSX(bool enabled)
    {
        PlayerPrefs.SetInt("PSXEnabled", enabled ? 1 : 0);
        if (psx != null)
        {
            psx.SetActive(enabled);
        }
    }

    private void LoadSettings()
    {

        float sensitivity = PlayerPrefs.GetFloat("Sensitivity", 0.3f);
        sensitivityInputField.text = sensitivity.ToString("F2");
        sensitivitySlider.value = sensitivity;
        float fov = PlayerPrefs.GetFloat("FOV", 125f);
        fovInputField.text = fov.ToString("F2");
        fovSlider.value = (fov - 80) / 80;
        lensDistortionToggle.isOn = PlayerPrefs.GetInt("LensDistortionEnabled", 1) == 1;
        lensFlareToggle.isOn = PlayerPrefs.GetInt("LensFlareEnabled", 1) == 1;
        bloomToggle.isOn = PlayerPrefs.GetInt("BloomEnabled", 1) == 1;
        psxToggle.isOn = PlayerPrefs.GetInt("PSXEnabled", 1) == 1;
    }
}
