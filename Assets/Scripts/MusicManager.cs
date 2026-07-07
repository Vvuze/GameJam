using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public static MusicManager Instance {  get; private set; }
    [SerializeField] private double scheduleDelay = 0.2;
    private readonly List<AudioSource> layers = new List<AudioSource>();
    private bool hasStarted = false;
    private int activeLayers = 0;

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
        ColorProgressController.Instance.SetProgress(progress);
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
    }
}
