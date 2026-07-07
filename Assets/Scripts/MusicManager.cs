using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public static MusicManager Insstance {  get; private set; }
    [SerializeField] private double scheduleDelay = 0.2;
    private readonly List<AudioSource> layers = new List<AudioSource>();
    private bool hasStarted = false;

    private void Awake()
    {
        if (Insstance != null && Insstance != this)
        {
            Destroy(gameObject);
            return;
        }
        Insstance = this;
    }

    public void RegisterLayer(AudioSource source)
    {
        layers.Add(source);
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
    }
}
