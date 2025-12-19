using System.Collections.Generic;
using UnityEngine;

public class ArrowPathRenderer : MonoBehaviour
{
    [Header("References")]
    public LineRenderer line;
    public Material flowMaterial;

    [Header("Visual Settings")]
    public float lineWidth = 0.15f;     // ✅ 線條寬度（你可自行調大調小）
    public float cornerRadius = 0.25f;  // ✅ 轉角圓弧半徑
    public int cornerSegments = 6;      // ✅ 轉角細分數（4~10 都合理）

    [Header("Height Offset")]
    public float yOffset = 0.02f;       // ✅ 路徑整體浮起高度（避免 Z-fighting）

    // ------------------------------------------------------
    // ✅ 主入口：World Path → 轉角圓滑 → 畫線
    // ------------------------------------------------------
    public void ShowWorldPoints(List<Vector3> points)
    {
        if (line == null)
        {
            Debug.LogWarning("ArrowPathRenderer aborted: LineRenderer is null");
            return;
        }

        if (points == null || points.Count < 2)
        {
            Clear();
            return;
        }

        // ✅ 套用穩定版轉角平滑
        List<Vector3> smoothPoints = SmoothPathWithRoundedCorners(points);

        // ✅ 統一高度（避免閃爍與斷線）
        float fixedY = smoothPoints[0].y + yOffset;

        Vector3[] arr = new Vector3[smoothPoints.Count];
        for (int i = 0; i < smoothPoints.Count; i++)
        {
            Vector3 p = smoothPoints[i];
            p.y = fixedY;
            arr[i] = p;
        }

        line.positionCount = arr.Length;
        line.SetPositions(arr);

        // ✅ 設定線寬（你剛剛問的寬度就在這）
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;

        if (flowMaterial != null)
            line.material = flowMaterial;
    }

    // ------------------------------------------------------
    // ✅ 穩定版：只在「單一轉角」加圓弧，不跨多個彎
    // ------------------------------------------------------
    private List<Vector3> SmoothPathWithRoundedCorners(List<Vector3> raw)
    {
        List<Vector3> result = new List<Vector3>();

        if (raw.Count < 3)
            return raw;

        result.Add(raw[0]); // 起點保留

        for (int i = 1; i < raw.Count - 1; i++)
        {
            Vector3 prev = raw[i - 1];
            Vector3 curr = raw[i];
            Vector3 next = raw[i + 1];

            Vector3 dir1 = (curr - prev).normalized;
            Vector3 dir2 = (next - curr).normalized;

            // ✅ 如果是「直線」，直接加點
            if (Vector3.Dot(dir1, dir2) > 0.999f)
            {
                result.Add(curr);
                continue;
            }

            // ✅ 計算轉角圓弧的「進點」與「出點」
            Vector3 enter = curr - dir1 * cornerRadius;
            Vector3 exit = curr + dir2 * cornerRadius;

            result.Add(enter);

            // ✅ 圓弧補點
            for (int j = 1; j <= cornerSegments; j++)
            {
                float t = j / (float)cornerSegments;
                Vector3 p = QuadraticBezier(enter, curr, exit, t);
                result.Add(p);
            }

            result.Add(exit);
        }

        result.Add(raw[^1]); // 終點保留
        return result;
    }

    // ------------------------------------------------------
    // ✅ 二次貝茲（只用在「單一轉角圓弧」）
    // ------------------------------------------------------
    private Vector3 QuadraticBezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        Vector3 p1 = Vector3.Lerp(a, b, t);
        Vector3 p2 = Vector3.Lerp(b, c, t);
        return Vector3.Lerp(p1, p2, t);
    }

    // ------------------------------------------------------
    // ✅ 清除線段
    // ------------------------------------------------------
    public void Clear()
    {
        if (line != null)
            line.positionCount = 0;
    }
}
