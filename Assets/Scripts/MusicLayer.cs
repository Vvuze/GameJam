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
        MusicManager.Insstance.RegisterLayer(audioSource);
        SetActive(startActive, instant: true);
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

    private void SetActive(bool active, bool instant = false)
    {
        isActive = active;
        targetVolume = active ? 1f : 0f;

        if(instant)
        {
            audioSource.volume = targetVolume;
        }
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        if (objectRenderer != null)
        {
            objectRenderer.material.color = isActive ? activeColor : inactiveColor;
        }
    }
    
}
