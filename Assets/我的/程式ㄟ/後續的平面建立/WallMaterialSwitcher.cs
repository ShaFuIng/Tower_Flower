using UnityEngine;
using System.Collections.Generic;

public class WallMaterialSwitcher : MonoBehaviour
{
    public Material defaultMat;       // 原本橘色牆壁
    public Material semiMat;          // 半透明材質（可遮擋）
    public Material transparentMat;   // 完全透明材質（可遮擋）

    private int currentMode = 0;      // 0=預設, 1=半透明, 2=透明

    public int SwitchMode(List<Transform> allWalls)
    {
        currentMode++;
        if (currentMode > 2)
            currentMode = 0;

        Apply(allWalls);

        return currentMode;
    }

    private void Apply(List<Transform> allWalls)
    {
        Material mat = currentMode switch
        {
            0 => defaultMat,
            1 => semiMat,
            2 => transparentMat,
            _ => defaultMat
        };

        foreach (Transform w in allWalls)
        {
            var r = w.GetComponent<Renderer>();
            if (r != null)
                r.material = mat;
        }
    }
}
