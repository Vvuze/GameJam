using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class MusicLayer : MonoBehaviour, IInteractable
{
    [Header("Звук")]
    [SerializeField] private AudioClip clip;
    [SerializeField] private bool startActive = false;

    [Header("визуальная обратная связь (не обязательно)")]
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private Color activeColor = new Color(0.3f, 1f, 0.3f);
    [SerializeField] private Color inactiveColor = new Color(0.4f, 0.4f, 0.4f);
    [SerializeField] private float fadeSpeed = 4f;

    [Header("Световой пульс (необязательно)")]
    [SerializeField] private ActivationPulseLight pulseLight;
    [SerializeField] private Color pulseColor = new Color(1f, 0.9f, 0.7f); // тёплый белый

    private AudioSource audioSource;
    private bool isActive;
    private float targetVolume;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0f;
    }
    void Start()
    {
        Debug.Log($"Зарегистрирован слой: {gameObject.name}", this);

        MusicManager.Instance.RegisterLayer(audioSource, startActive);
        SetActive(startActive, instant: true, notify: false);
    }

    void Update()
    {
        if (!Mathf.Approximately(audioSource.volume, targetVolume))
        {
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVolume, fadeSpeed * Time.deltaTime);
        }
    }

    public void Interact()
    {
        SetActive(!isActive);
    }

    public string GetPrompt()
    {
        return isActive ? "выключить" : "включить";
    }

    private void SetActive(bool active, bool instant = false, bool notify = true)
    {
        isActive = active;
        targetVolume = active ? 1f : 0f;

        if(instant)
        {
            audioSource.volume = targetVolume;
        }
        UpdateVisual();

        if (pulseLight != null && !instant)
        {
            pulseLight.Play(pulseColor, active);
        }

        if (notify)
        {
            MusicManager.Instance.NotifyLayerToggled(active);
        }
    }

    private void UpdateVisual()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = isActive ? activeColor : inactiveColor;
        }
    }
    
}
