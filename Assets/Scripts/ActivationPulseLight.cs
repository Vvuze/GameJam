using System.Collections;
using UnityEngine;
[RequireComponent(typeof(Light))]
public class ActivationPulseLight : MonoBehaviour
{
    [Header("Свечение в состоянии \"включено\"")]
    [SerializeField] private float restingIntensity = 1.2f; 

    [Header("Вспышка в момент переключения")]
    [SerializeField] private float pulseIntensity = 3.5f; 
    [SerializeField] private float pulseDuration = 0.6f;   

    private Light lightSource;
    private Coroutine pulseRoutine;

    void Awake()
    {
        lightSource = GetComponent<Light>();
        lightSource.enabled = false;
        lightSource.intensity = 0f;
    }

    public void Play(Color color, bool turningOn)
    {
        lightSource.color = color;

        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
        }
        pulseRoutine = StartCoroutine(PulseRoutine(turningOn));
    }

    private IEnumerator PulseRoutine(bool turningOn)
    {
        lightSource.enabled = true;

        float restingTarget = turningOn ? restingIntensity : 0f;
        float halfDuration = pulseDuration * 0.5f;
        float startIntensity = lightSource.intensity;

        float elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);
            lightSource.intensity = Mathf.Lerp(startIntensity, pulseIntensity, t);
            yield return null;
        }
        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / halfDuration);
            lightSource.intensity = Mathf.Lerp(pulseIntensity, restingTarget, t);
            yield return null;
        }

        lightSource.intensity = restingTarget;
        lightSource.enabled = restingTarget > 0f;
    }
}