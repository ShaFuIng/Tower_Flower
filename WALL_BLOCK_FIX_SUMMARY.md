# Wall Block Fix Summary

## Problem Found
你的 **blocks 全是 false** 是因為：

### Root Cause
在 `FloorBuildManager.cs` 中，有两个地方可以放置墙：
1. ✅ **Floor → Wall** (`TryPlaceWallFromFloor`) - **有调用** `RegisterWallBlockBetweenTiles()`
2. ❌ **Wall → Wall** (`TryPlaceWallFromWall`) - **完全没有调用**！

所以当你在墙上再建墙时，BFS 阻挡标记（block flags）完全没被设置。

## Changes Made

### 1. **Fixed `TryPlaceWallFromWall()`**
- 添加了 `TryFindAdjacentFloor()` 辅助方法来找到新墙两侧的 floor tiles
- 新墙两侧如果都有 floor tiles，就调用 `RegisterWallBlockBetweenTiles()` 来设置阻挡

**关键逻辑：**
```csharp
Vector3 wallDir = pivotWall.forward;
Vector3 offset = wallDir * (scale.x * 0.5f + 0.01f);

FloorTileInfo floorBefore = TryFindAdjacentFloor(pos + offset);
FloorTileInfo floorAfter = TryFindAdjacentFloor(pos - offset);

if (floorBefore != null && floorAfter != null)
{
    RegisterWallBlockBetweenTiles(floorBefore, floorAfter);
}
```

### 2. **Added Undo Cleanup**
- 在 `OnUndoButtonPressed()` 中添加了 `ClearWallBlocks()` 调用
- 撤销墙时会正确清理所有相关的 block 标记

### 3. **Added Debug Logging**
- 在 `RegisterWallBlockBetweenTiles()` 中添加了调试日志
- 如果你开启 `debugLogs` 开关，会看到每次设置 block 时的详细信息

```csharp
if (debugLogs) Debug.Log($"[WallBlock] {a.name}({a.gridX},{a.gridY}) <-> {b.name}({b.gridX},{b.gridY}), dx=1 dy=0, blockEast/blockWest = true");
```

## How to Test

1. **打开 debugLogs 开关**
   - 在 Inspector 中找到 `FloorBuildManager`
   - 勾选 `Debug Logs` 复选框

2. **建造测试场景**
   - 放一些 floor tiles
   - 在 floor tile 边缘放墙（Floor → Wall）- **会看到日志**
   - 在现有的墙上方继续放墙（Wall → Wall） - **现在也会看到日志了**
   - 在现有的墙旁放墙（Wall → Wall 横向） - **也会记录**

3. **检查结果**
   - 查看 console 中的 `[WallBlock]` 日志
   - 进行路径查找，确认怪物不会穿过墙

## Important Notes

⚠️ **关于坐标系和方向映射**：
- 代码假设 `pivotWall.forward` 是墙面向外的法线方向
- `TryFindAdjacentFloor()` 使用 1.5f 倍的 tile 宽度作为搜索范围
- 如果你的 floor tiles 间距不规则，可能需要调整这个距离

⚠️ **关于 Wall-on-Wall 放置**：
- 向上放墙 (Up) - 不会创建新的 floor tile 连接，所以不会记录 block
- 向下放墙 (Down) - 同样不会创建新的 floor tile 连接
- 左右方向放墙 (Left/Right) - 会找到相邻的 floor tiles 并记录 block ✅

⚠️ **与 SpawnGoalPlacementManager 的差异**：
- 建造模式用的是 `FloorBuildManager` 系统
- 塔放置用的是 `SpawnGoalPlacementManager` 系统（有独立的 wall block 机制）
- 这两套系统目前**各自独立**，暂不同步

## Debugging Commands

查看当前所有 tiles 的状态：
```csharp
Debug.Log($"[GRID] Tile {tile.name} => ({tile.gridX},{tile.gridY}) " +
          $"Blocks(N,S,E,W)=({tile.blockNorth},{tile.blockSouth},{tile.blockEast},{tile.blockWest})");
```

查看单条边是否被阻挡：
```csharp
bool blocked = tile.IsBlockedTowards(neighbourTile);
```
