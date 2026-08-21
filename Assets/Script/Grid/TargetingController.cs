using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetingController : MonoBehaviour
{
    public static TargetingController instance;
    private GridManager gridManager;

    public Color warnColor = Color.red;

    public float flashInterval = 0.2f;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gridManager = GridManager.Instance;
    }
    public void ShowAttackWarning(List<Vector2Int> targetTiles, float duration)
    {
        StartCoroutine(WarningRoutine(targetTiles, duration));
    }

    private IEnumerator WarningRoutine(List<Vector2Int> targetTiles, float duration)
    {
        float timer = 0f;
        bool isColored = false;

        while (timer < duration)
        {
            isColored = !isColored;

            foreach (Vector2Int coord in targetTiles)
            {
                if (gridManager.Grid.ContainsKey(coord))
                {
                    if (isColored)
                        gridManager.ChangeTileColor(coord, warnColor);
                    else
                        gridManager.Grid[coord].ResetVisuals();
                }
            }

            float waitTime = Mathf.Min(flashInterval, duration - timer);
            yield return new WaitForSeconds(waitTime);
            timer += waitTime;
        }

        foreach (Vector2Int coord in targetTiles)
        {
            if (gridManager.Grid.ContainsKey(coord))
            {
                gridManager.Grid[coord].ResetVisuals();
            }
        }
    }
}