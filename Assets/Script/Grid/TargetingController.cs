using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetingController : MonoBehaviour
{
    public static TargetingController instance;
    private GridManager gridManager;

    public GameObject warningPrefab;

    public float targetscale = 1f;

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
            if (gridManager.Grid.TryGetValue(coord, out Node node))
            {
                Vector3 spawnPos = node.transform.position + (node.transform.up * 1.01f);

                GameObject decal = Instantiate(warningPrefab, spawnPos, Quaternion.identity, node.transform);

                decal.transform.localScale = Vector3.one * 0.1f;
                decal.transform.localRotation = Quaternion.Euler(90, 0, 0);

                activeDecals.Add(decal);
            }
        }

        float elapsed = 0f;
        Vector3 targetScale = Vector3.one * targetscale;

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