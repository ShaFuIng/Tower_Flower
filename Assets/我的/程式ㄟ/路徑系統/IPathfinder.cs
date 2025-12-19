using System.Collections.Generic;

public interface IPathfinder
{
    /// <summary>
    /// 計算從 start 到 goal 的路徑。
    /// 回傳「格子節點 PathNode 的列表」，包含起點與終點。
    /// 回傳 null 表示無法到達（被牆阻擋或沒有可走的路）。
    /// </summary>
    List<PathNode> FindPath(PathNode start, PathNode goal);
}