using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]

public class TVSound : MonoBehaviour, IInteractable
{
    [Header("Звук")]
    [SerializeField] private AudioClip clip;
    [SerializeField] private bool startActive = false;

    [Header("визуальная обратная связь (не обязательно)")]
    [SerializeField] private Renderer objectRenderer;
    [SerializeField] private Color activeColor = new Color(0.3f, 1f, 0.3f);
    [SerializeField] private Color inactiveColor = new Color(0.4f, 0.4f, 0.4f);
    [SerializeField] private float fadeSpeed = 4f;
    [SerializeField] private GameObject TV1;
    [SerializeField] private GameObject TV2;

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
        TV1.SetActive(false);
        TV2.SetActive(true);
    }

    public string GetPrompt()
    {
        return isActive ? "выключить" : "включить";
    }

    private void SetActive(bool active, bool instant = false, bool notify = true)
    {
        isActive = active;
        targetVolume = active ? 1f : 0f;

        if (instant)
        {
            audioSource.volume = targetVolume;
        }


        if (notify)
        {
            MusicManager.Instance.NotifyLayerToggled(active);
        }
    }
}
