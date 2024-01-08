using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndDrop : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Canvas canvas;
    private Vector2 initialPosition;
    private Transform originalParent; // Stocke le parent d'origine de l'objet
    private bool isDragging = false;
    private float scaleFactor = 0.25f; // Facteur de réduction

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        initialPosition = rectTransform.anchoredPosition;
        originalParent = transform.parent;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        // Déplacer l'objet vers le haut de la hiérarchie pour le sortir de son parent
        transform.SetParent(canvas.transform);
        // Réduire la taille de l'image
        rectTransform.localScale = new Vector3(scaleFactor, scaleFactor, 1f);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            isDragging = false;

            // Restaurer le parent d'origine
            transform.SetParent(originalParent);

            // Réinitialiser à la position d'origine et restaurer la taille
            rectTransform.anchoredPosition = initialPosition;
            rectTransform.localScale = Vector3.one;
        }
    }
}
