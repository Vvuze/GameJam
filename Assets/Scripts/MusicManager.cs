using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{

    public static MusicManager Instance {  get; private set; }
    [SerializeField] private double scheduleDelay = 0.2;

    [Header("Искажение звука (плохие объекты)")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerSnapshot cleanSnapshot;
    [SerializeField] private AudioMixerSnapshot corruptedSnapshot;
    [SerializeField] private AudioMixerSnapshot secretSnapshot;
    [SerializeField] private float snapshotTransitionTime = 1.5f;

    // когда все "плохие" биты активны
    public event System.Action OnSecretHarmonyReached;

    //когда все "хорошие" биты активны
    public event System.Action OnFullHarmonyReached;

    private readonly List<AudioSource> layers = new List<AudioSource>();
    private bool hasStarted = false;
    private int activeLayers = 0;

    private int totalBadLayers = 0;
    private int activeBadLayers = 0;
    private bool secretReached = false;
    private bool fullHarmonyReached = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void RegisterLayer(AudioSource source, bool startsActive)
    {
        layers.Add(source);
        if (startsActive)
        {
            activeLayers++;
        }
    }

    public void NotifyLayerToggled(bool isActive)
    {
        activeLayers += isActive ? 1 : -1;
        UpdateColorProgress();
    }

    private void UpdateColorProgress()
    {
        if (layers.Count == 0 || ColorProgressController.Instance == null) return;

        float progress = (float)activeLayers / layers.Count;
        Debug.Log($"[MusicManager] Прогресс: {activeLayers}/{layers.Count} = {progress}");
        ColorProgressController.Instance.SetProgress(progress);

        if (progress >= 1f && !fullHarmonyReached)
        {
            fullHarmonyReached = true;
            Debug.Log("[MusicManager] Полная гармония достигнута!");
            OnFullHarmonyReached?.Invoke();
        }
        else if (progress < 1f)
        {
            fullHarmonyReached = false; // на случай если игрок что то выключит обратно
        }
    }

    public void RegisterBadLayer()
    {
        totalBadLayers++;
    }

    public void NotifyBadLayerToggled(bool isActive)
    {
        activeBadLayers += isActive ? 1 : -1;
        UpdateCorruptionBlend();
    }

    private void UpdateCorruptionBlend()
    {
        if (audioMixer == null || totalBadLayers == 0) return;

        bool allBadActive = activeBadLayers == totalBadLayers;

        if (allBadActive && !secretReached)
        {
            secretReached = true;

            audioMixer.TransitionToSnapshots(
                new[] { secretSnapshot },
                new[] { 1f },
                snapshotTransitionTime);

            OnSecretHarmonyReached?.Invoke();
        }
        else if (!allBadActive)
        {
            secretReached = false;
            float corruption = (float)activeBadLayers / totalBadLayers;

            audioMixer.TransitionToSnapshots(
                new[] { cleanSnapshot, corruptedSnapshot },
                new[] { 1f - corruption, corruption },
                snapshotTransitionTime);
        }
    }

    void Update()
    {
        if(!hasStarted)
        {
            hasStarted = true;
            StartAllLayers();
        }
    }

    private void StartAllLayers()
    {
        double scheduleTime = AudioSettings.dspTime + scheduleDelay;

        foreach (var source in layers)
        {
            source.PlayScheduled(scheduleTime);
        }

        UpdateColorProgress();
        Debug.Log($"[MusicManager] Запущено дорожек: {layers.Count}");
    }
}
