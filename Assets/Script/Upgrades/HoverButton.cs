using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Settings")]
    [SerializeField] private float moveSpeed = 10f;     
    [SerializeField] private float moveDistance = 15f;  
    [SerializeField] private Vector2 moveDirection = new Vector2(-1, 0);
    [SerializeField] private Animator animator;
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private bool isHovered = false;
    [SerializeField] private bool isAttackButton;
    [SerializeField] private bool isTippingButton;

    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float targetFontSize;
    private float originalFontSize;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalFontSize = text.fontSize;
        originalPosition = rectTransform.anchoredPosition;
    }

    private void Start()
    {

    }

    private void Update()
    {
        if (isHovered)
        {
            text.fontSize = Mathf.Lerp(text.fontSize, targetFontSize, 1f - Mathf.Exp(-moveSpeed * Time.deltaTime));
        }
        else
        {
            text.fontSize = Mathf.Lerp(text.fontSize, originalFontSize, 1f - Mathf.Exp(-moveSpeed * Time.deltaTime));
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        MusicManager.Instance.PlayHoverSound();
        if (isAttackButton)
        {
            Debug.Log("plug");
            animator.SetInteger("State", 1);
            animator.SetFloat("SpeedAttack", 1);
        }        
        if (isTippingButton)
        {
            animator.SetInteger("State", 2);
            animator.SetFloat("SpeedTipping", 1);
        }    
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        animator.SetFloat("SpeedAttack", -1);
        animator.SetFloat("SpeedTipping", -1);
        animator.SetInteger("State", 0);
    }
}