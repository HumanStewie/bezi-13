using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetingController : MonoBehaviour
{
    public static TargetingController instance;
    private GridManager gridManager;

    public GameObject warningPrefab;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gridManager = GridManager.Instance;
    }
    public void ShowAttackWarning(List<Vector2Int> dangerZone, float duration)
    {
        StartCoroutine(ExpandWarningRoutine(dangerZone, duration));
    }

    private IEnumerator ExpandWarningRoutine(List<Vector2Int> dangerZone, float duration)
    {
        List<GameObject> activeDecals = new List<GameObject>();

        foreach (Vector2Int coord in dangerZone)
        {
            Vector3 tileCenter = GridManager.Instance.CoordToWorldPos(coord);

            Vector3 spawnPos = tileCenter + new Vector3(0, 1.01f, 0);

            GameObject decal = Instantiate(warningPrefab, spawnPos, Quaternion.Euler(90, 0, 0));
            decal.transform.localScale = Vector3.one * 0.1f;

            activeDecals.Add(decal);
        }

        float elapsed = 0f;
        Vector3 targetScale = new Vector3(2f, 2f, 2f);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percent = elapsed / duration;

            foreach (GameObject decal in activeDecals)
            {
                if (decal != null)
                {
                    decal.transform.localScale = Vector3.Lerp(Vector3.one * 0.1f, targetScale, percent);
                }
            }
            yield return null;
        }

        foreach (GameObject decal in activeDecals)
        {
            Destroy(decal);
        }
    }
}