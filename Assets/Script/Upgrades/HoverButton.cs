using UnityEngine;
using UnityEngine.EventSystems;

public class HoverOscillator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    [SerializeField] private float moveSpeed = 10f;     
    [SerializeField] private float moveDistance = 15f;  
    [SerializeField] private Vector2 moveDirection = new Vector2(-1, 0);

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private bool isHovered = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        originalPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (isHovered)
        {
            float offset = Mathf.Abs(Mathf.Sin(Time.time * moveSpeed)) * moveDistance;

            rectTransform.anchoredPosition = originalPosition + (moveDirection.normalized * offset);
        }
        else
        {
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, originalPosition, Time.deltaTime * 15f);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        MusicManager.Instance.PlayHoverSound();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}