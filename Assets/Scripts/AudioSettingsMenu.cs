using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioSettingsMenu : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private string exposedParam = "MasterVolume"; 

    [Header("UI")]
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private GameObject settingsPanel;

    [Header("Игрок (необязательно, для паузы управления)")]
    [SerializeField] private Player player;

    [Header("Клавиша открытия/закрытия")]
    [SerializeField] private KeyCode toggleKey = KeyCode.Escape;

    private bool isOpen = false;
    private const string SavedVolumeKey = "MasterVolume";

    void Start()
    {
        float savedVolume = PlayerPrefs.GetFloat(SavedVolumeKey, 0.75f);
        volumeSlider.value = savedVolume;
        volumeSlider.onValueChanged.AddListener(SetVolume);

        SetVolume(savedVolume);
        settingsPanel.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleMenu();
        }
    }

    public void ToggleMenu()
    {
        isOpen = !isOpen;
        settingsPanel.SetActive(isOpen);

        if (player != null)
        {
            player.enabled = !isOpen;
        }

        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
    }

    public void SetVolume(float sliderValue)
    {
        float dB = sliderValue > 0.0001f ? Mathf.Log10(sliderValue) * 20f : -80f;
        audioMixer.SetFloat(exposedParam, dB);

        PlayerPrefs.SetFloat(SavedVolumeKey, sliderValue);
    }
}