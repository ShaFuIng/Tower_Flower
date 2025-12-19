using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMovement : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("每秒走幾個 tile（與 tile 尺寸成正比）")]
    public float tilesPerSecond = 0.8f;   // ✅ 新：tile-relative 速度

    public float reachDistance = 0.05f;
    public float sizeRatio = 0.33f;

    private float worldMoveSpeed;          // ✅ 真正用來 MoveTowards 的速度

    private List<Vector3> path;
    private int pathIndex = 0;
    private bool hasPath = false;

    // =========================================================
    // 🔹 尺寸（你原本的，完全保留）
    // =========================================================
    public void ApplySizeFromTile(FloorTileInfo tile)
    {
        if (tile == null)
        {
            Debug.LogError("[MonsterScale] tile is null");
            return;
        }

        float tileWorldSize = GetTileWorldSize(tile);
        float targetWorldSize = tileWorldSize * sizeRatio;

        float monsterBaseWorldSize = GetMonsterBaseWorldSize_UsingMeshBounds();

        if (monsterBaseWorldSize <= 0.00001f)
        {
            StartCoroutine(ApplySizeNextFrame(targetWorldSize));
            return;
        }

        float scale = targetWorldSize / monsterBaseWorldSize;
        transform.localScale = Vector3.one * scale;

        Debug.Log($"[MonsterScale] tile={tileWorldSize:F3}, base={monsterBaseWorldSize:F3}, scale={scale:F3}");
    }

    // =========================================================
    // 🔹 速度（新增）
    // =========================================================
    public void ApplySpeedFromTile(FloorTileInfo tile)
    {
        if (tile == null)
        {
            Debug.LogError("[MonsterSpeed] tile is null");
            return;
        }

        float tileWorldSize = GetTileWorldSize(tile);
        worldMoveSpeed = tileWorldSize * tilesPerSecond;

        Debug.Log($"[MonsterSpeed] tile={tileWorldSize:F3}, speed={worldMoveSpeed:F3}");
    }

    // =========================================================
    // 🔹 Tile 世界尺寸
    // =========================================================
    private float GetTileWorldSize(FloorTileInfo tile)
    {
        var r = tile.GetComponentInChildren<Renderer>();
        if (r != null) return r.bounds.size.x;
        return tile.transform.lossyScale.x;
    }

    // =========================================================
    // 🔹 Monster mesh 基準尺寸（你原本的）
    // =========================================================
    private float GetMonsterBaseWorldSize_UsingMeshBounds()
    {
        var mf = GetComponentInChildren<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            float local = mf.sharedMesh.bounds.size.x;
            return Mathf.Abs(local * mf.transform.lossyScale.x);
        }

        var sk = GetComponentInChildren<SkinnedMeshRenderer>();
        if (sk != null && sk.sharedMesh != null)
        {
            float local = sk.sharedMesh.bounds.size.x;
            return Mathf.Abs(local * sk.transform.lossyScale.x);
        }

        return 0f;
    }

    private IEnumerator ApplySizeNextFrame(float targetWorldSize)
    {
        yield return null;

        var r = GetComponentInChildren<Renderer>();
        if (r == null) yield break;

        float baseWorld = r.bounds.size.x;
        if (baseWorld <= 0.00001f) yield break;

        float scale = targetWorldSize / baseWorld;
        transform.localScale = Vector3.one * scale;
    }

    // =========================================================
    // 🔹 Path
    // =========================================================
    public void SetPath(List<Vector3> newPath)
    {
        if (newPath == null || newPath.Count == 0)
        {
            hasPath = false;
            return;
        }

        path = newPath;
        pathIndex = FindClosestPathIndex(path);
        hasPath = true;
    }

    private void Update()
    {
        if (!hasPath || pathIndex >= path.Count) return;

        Vector3 target = path[pathIndex];
        Vector3 currentPos = transform.position;
        target.y = currentPos.y;

        Vector3 dir = target - currentPos;
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
            transform.rotation = Quaternion.LookRotation(dir);

        // ✅ 改用 tile-relative 世界速度
        transform.position = Vector3.MoveTowards(
            currentPos,
            target,
            worldMoveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target) <= reachDistance)
            pathIndex++;
    }

    private int FindClosestPathIndex(List<Vector3> path)
    {
        int closestIndex = 0;
        float minDist = float.MaxValue;
        Vector3 pos = transform.position;

        for (int i = 0; i < path.Count; i++)
        {
            float d = Vector3.Distance(pos, path[i]);
            if (d < minDist)
            {
                minDist = d;
                closestIndex = i;
            }
        }
        return closestIndex;
    }

    // =========================================================
    // 🔹 Renderer 顯示控制（你原本的）
    // =========================================================
    private Renderer[] cachedRenderers;

    private void Awake()
    {
        cachedRenderers = GetComponentsInChildren<Renderer>(true);
    }

    public void ShowAfterScale()
    {
        StartCoroutine(ShowNextFrame());
    }

    private IEnumerator ShowNextFrame()
    {
        yield return null;
        foreach (var r in cachedRenderers)
            r.enabled = true;
    }
}
