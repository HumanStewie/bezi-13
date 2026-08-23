using UnityEngine;
using UnityEngine.UI; 

[RequireComponent(typeof(Image))]
public class ColorTransitor : MonoBehaviour
{
    [Header("Color Settings")]
    // Defaulted to the deep blues from your image!
    [SerializeField] private Color color1 = new Color(0.15f, 0.15f, 0.6f, 1f); 
    [SerializeField] private Color color2 = new Color(0.02f, 0.02f, 0.1f, 1f); 

    [Header("Transition Settings")]
    [SerializeField] private float transitionSpeed = 0.5f;

    private Image panelImage;

    void Start()
    {
        panelImage = GetComponent<Image>();
    }

    void Update()
    {
        float blendValue = Mathf.PingPong(Time.time * transitionSpeed, 1f);

        panelImage.color = Color.Lerp(color1, color2, blendValue);
    }
}