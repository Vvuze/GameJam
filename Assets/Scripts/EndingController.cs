using System.Collections;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    [Header("Ссылки")]
    [SerializeField] private Player player; 
    [SerializeField] private PlayerInteractor playerInteractor; 
    [SerializeField] private Transform playerCameraTransform; 
    [SerializeField] private MusicManager musicManager;

    [Header("Сцена концовки (окно)")]
    [SerializeField] private Transform endingFocusPoint;
    [SerializeField] private float endingDuration = 4f;

    [Header("Текст эпилога (необязательно)")]
    [SerializeField] private CanvasGroup epilogueTextGroup; 
    [SerializeField] private float textFadeDuration = 2f;

    private bool endingTriggered = false;

    void OnEnable()
    {
        if (musicManager != null)
        {
            musicManager.OnFullHarmonyReached += HandleEndingTriggered;
            musicManager.OnSecretHarmonyReached += HandleEndingTriggered;
        }
    }

    void OnDisable()
    {
        if (musicManager != null)
        {
            musicManager.OnFullHarmonyReached -= HandleEndingTriggered;
            musicManager.OnSecretHarmonyReached -= HandleEndingTriggered;
        }
    }

    private void HandleEndingTriggered()
    {
        if (endingTriggered || endingFocusPoint == null) return;
        endingTriggered = true;

        StartCoroutine(PlayEndingSequence());
    }

    private IEnumerator PlayEndingSequence()
    {
        // 1. Полностью отбираем управление — включая взаимодействие с объектами
        if (player != null) player.enabled = false;
        if (playerInteractor != null) playerInteractor.enabled = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 2. Плавно ведём камеру к окну
        Vector3 startPos = playerCameraTransform.position;
        Quaternion startRot = playerCameraTransform.rotation;

        float elapsed = 0f;
        while (elapsed < endingDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / endingDuration);

            playerCameraTransform.position = Vector3.Lerp(startPos, endingFocusPoint.position, t);
            playerCameraTransform.rotation = Quaternion.Slerp(startRot, endingFocusPoint.rotation, t);

            yield return null;
        }

        if (epilogueTextGroup != null)
        {
            yield return StartCoroutine(FadeCanvasGroup(epilogueTextGroup, 0f, 1f, textFadeDuration));
        }
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float from, float to, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }
        group.alpha = to;
    }
}