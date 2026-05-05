using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;

public class ShadowSystem : MonoBehaviour
{
    [Header("Referencias")]
    public Transform player;
    public GameObject shadowPrefab;

    [Header("Spawn")]
    public float spawnRadius = 12f;
    public float minDistance = 6f;

    List<GameObject> activeShadows = new List<GameObject>();

    public void SpawnShadow()
    {
        Vector3 spawnPos = GetSpawnPosition();

        GameObject shadow = Instantiate(shadowPrefab, spawnPos, Quaternion.identity);

        activeShadows.Add(shadow);
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 dir;
        float angle = Random.Range(-120f, 120f);

        dir = Quaternion.Euler(0, angle, 0) * player.forward;

        float distance = Random.Range(minDistance, spawnRadius);

        Vector3 pos = player.position + dir.normalized * distance;
        pos.y = 1f;

        return pos;
    }

    public void ClearShadows()
    {
        foreach (var shadow in activeShadows)
        {
            if (shadow != null)
                Destroy(shadow);
        }

        activeShadows.Clear();
    }
}
