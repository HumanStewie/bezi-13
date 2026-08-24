using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class BlinkingText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float timePerBlink;
    private bool visible = true;
    private void Start()
    {
        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        while (!GameManager.instance.GameStarted || !GameManager.instance.gameOver)
        {
            visible = !visible;
            textMesh.enabled = visible;
            yield return new WaitForSeconds(timePerBlink);
        }
    }

    private void Update()
    {
        if (GameManager.instance.GameStarted || GameManager.instance.gameOver)
        {
            StopCoroutine(Blink());
            visible = false;
            textMesh.enabled = visible;
            Destroy(gameObject);
        }

    }
}
