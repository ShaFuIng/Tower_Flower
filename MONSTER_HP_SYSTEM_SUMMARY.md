# 怪物生命與傷害系統功能總結

這份文件總結了近期實作的怪物生命、傷害、解析（死亡或抵達終點）以及主堡受損的核心功能。

## 實作目標

建立一個事件驅動、職責分離的系統，用來處理怪物的完整生命週期，從生成移動、受到傷害、到最終從場上移除，並在怪物抵達終點時對玩家主堡造成傷害。

## 核心腳本與功能

### 1. `MonsterStats.cs` - 怪物狀態核心
- **職責**: 管理單一怪物的生命值、狀態與最終解析。
- **功能**:
    - **生命值管理**: 儲存 `maxHp` 與 `currentHp`。
    - **承受傷害**: 提供 `TakeDamage(int damage)` 方法，供外部（如炮塔）呼叫以對怪物造成傷害。
    - **死亡與解析**:
        - 當 `currentHp` <= 0 時，觸發 `Die()`，將怪物狀態解析為 `Killed`。
        - 提供 `Resolve(ResolvedType type)` 方法，統一處理怪物被擊殺 (`Killed`) 或抵達終點 (`ReachedGoal`) 的情況。
        - 怪物一旦被解析，其遊戲物件會被立即 `Destroy()`。
    - **事件廣播**:
        - `OnHpChanged(int current, int max)`: 當生命值變動時觸發，主要用於更新 UI。
        - `OnResolved(ResolvedType type)`: 當怪物被解析時觸發，供未來的系統（如分數、掉落金錢）使用。

### 2. `MonsterHpUI.cs` - 怪物血條 UI
- **職責**: 顯示怪物的生命值。
- **功能**:
    - **自動綁定**: 在 `Awake()` 中使用 `GetComponentInParent<MonsterStats>()` 自動尋找並綁定對應的怪物。
    - **事件監聽**: 在 `Start()` 中訂閱 `MonsterStats.OnHpChanged` 事件。
    - **更新顯示**: 收到事件後，根據傳入的 `current` 和 `max` 生命值，更新 `Slider` 元件的 `value`，以百分比顯示血量。
    - **自動清理**: 在 `OnDestroy()` 中取消訂閱，避免記憶體洩漏。

### 3. `GoalHealthManager.cs` - 主堡生命管理器
- **職責**: 管理玩家主堡的生命值。
- **功能**:
    - **生命值管理**: 儲存主堡的 `maxHp` 與 `currentHp`。
    - **承受傷害**: 提供 `TakeDamage(int damage)` 方法，主要由抵達終點的怪物呼叫。
    - **事件廣播**:
        - `OnHpChanged`: 當生命值變動時觸發。
        - `OnHpDelta`: 回報單次傷害的數值。
        - `OnGoalDead`: 當主堡生命值歸零時觸發，用於通知遊戲結束。

### 4. `MonsterMovement.cs` - 怪物移動（修改部分）
- **職責**: 控制怪物沿著路徑移動，並在抵達終點時觸發相應邏輯。
- **功能修改**:
    - **自動綁定**: 在 `Awake()` 中獲取自己身上的 `MonsterStats` 元件，並找到場景中的 `GoalHealthManager`。
    - **終點邏輯**: 在 `Update()` 中偵測到怪物已走完路徑 (`pathIndex >= path.Count`) 時：
        1.  呼叫 `goalHealthManager.TakeDamage(1)` 對主堡造成傷害。
        2.  呼叫 `monsterStats.Resolve(ResolvedType.ReachedGoal)` 通知 `MonsterStats` 自己已抵達終點，觸發自我銷毀流程。

## 系統互動流程

1.  **受傷流程**:
    `Tower` -> `MonsterStats.TakeDamage()` -> `MonsterStats` 觸發 `OnHpChanged` -> `MonsterHpUI` 接收事件並更新 `Slider`。

2.  **死亡流程**:
    `MonsterStats.TakeDamage()` 導致 `hp <= 0` -> `MonsterStats.Resolve(Killed)` -> 怪物物件被銷毀。

3.  **抵達終點流程**:
    `MonsterMovement` 偵測到路徑終點 -> `MonsterMovement` 呼叫 `GoalHealthManager.TakeDamage()` -> `MonsterMovement` 呼叫 `MonsterStats.Resolve(ReachedGoal)` -> 怪物物件被銷毀。
