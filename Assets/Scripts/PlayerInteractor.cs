using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer = -0;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("UI (Не обязательно)")]
    [SerializeField] private GameObject promptUI;
    [SerializeField] private TMPro.TMP_Text promptText;

    private IInteractable currentTarget;

    void Update()
    {
        CheckForInteractable();
        HandleUI();
        
        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact();
        }
    }

    private void CheckForInteractable()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if(Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactableLayer))
        {
            currentTarget = hit.collider.GetComponent<IInteractable>();
        }
        else
        {
            currentTarget = null;
        }
    }

    private void HandleUI()
    {
        if (promptUI == null) return;

        bool hasTarget = currentTarget != null;
        promptUI.SetActive(hasTarget);
        if (hasTarget && promptText != null)
        {
            promptText.text = currentTarget.GetPrompt();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * interactDistance);
    }
}
