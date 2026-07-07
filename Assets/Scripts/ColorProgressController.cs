using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class ColorProgressController : MonoBehaviour
{
    public static ColorProgressController Instance { get; private set; }

    [Header("Диапазон насыщенности")]
    [SerializeField] private float minSaturation = -100f; 
    [SerializeField] private float maxSaturation = 0f;     
    [Header("Скорость перехода")]
    [SerializeField] private float transitionSpeed = 40f; 

    private Volume volume;
    private ColorAdjustments colorAdjustments;
    private float targetSaturation;

    void Awake()
    {
        Instance = this;
        volume = GetComponent<Volume>();

        if (volume.profile == null)
        {
            Debug.LogError("[ColorProgressController] У Volume не назначен Profile!");
            return;
        }

        if (!volume.profile.TryGet(out colorAdjustments))
        {
            colorAdjustments = volume.profile.Add<ColorAdjustments>(true);
        }

        colorAdjustments.saturation.overrideState = true;
        targetSaturation = minSaturation;
        colorAdjustments.saturation.value = minSaturation; 
    }

    void Update()
    {
        if (colorAdjustments == null) return;

        colorAdjustments.saturation.value = Mathf.MoveTowards(colorAdjustments.saturation.value,targetSaturation,transitionSpeed * Time.deltaTime);
    }

    public void SetProgress(float progress01)
    {
        progress01 = Mathf.Clamp01(progress01);
        targetSaturation = Mathf.Lerp(minSaturation, maxSaturation, progress01);
    }
}
