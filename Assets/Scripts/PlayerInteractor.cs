using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Настройки взаимодействия")]
    [SerializeField] private float interactDistance = 3f;
    [SerializeField] private LayerMask interactableLayer = -0;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private IInteractable currentTarget;

    void Update()
    {
        CheckForInteractable();
        
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * interactDistance);
    }
}
