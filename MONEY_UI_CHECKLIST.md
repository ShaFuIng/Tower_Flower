# ✅ 金錢系統 - 快速檢查清單

## 程式側 (100% ✅ 完成)

- [x] MoneyManager.cs - 創建完成
- [x] MoneyUIController.cs - 創建完成 + 新增 ShowMoneyUI/HideMoneyUI 方法
- [x] PlacementCostValidator.cs - 創建完成
- [x] IconDragHandler.cs - 集成成本驗證
- [x] SpawnGoalPlacementManager.cs - 集成成本扣除
- [x] PlaceableDefinition.cs - 添加 cost 字段
- [x] TowerDefenseUIManager.cs - 集成 MoneyUI 顯示/隱藏
- [x] 編譯成功 - 零錯誤

---

## 場景設置 (⚠️ 需要你手動)

### Hierarchy 結構 (需要你建立)

```
Canvas (你現有的)
├─ MoneyText (TextMeshPro)           ← 新增，預設隱藏 ❌
├─ WarningText (TextMeshPro)         ← 新增，預設隱藏 ❌
├─ [其他現有 UI]
└─ ...

GameObject: "MoneyManager"           ← 新增，根目錄
└─ Component: MoneyManager
```

### 設置檢查表

**1. MoneyManager GameObject** ⬜ TODO
```
□ 建立 Empty GameObject "MoneyManager"
□ 添加 Component: MoneyManager
□ 設置 Current Money: 1000 (可選)
```

**2. MoneyText (TextMeshPro)** ⬜ TODO
```
□ 在 Canvas 建立 TextMeshPro - Text
□ 命名為 "MoneyText"
□ 設置位置 (左上角或你想要的地方)
□ 設置字體大小: 36
□ 預設隱藏: ❌ 取消 Active 狀態
```

**3. WarningText (TextMeshPro)** ⬜ TODO
```
□ 在 Canvas 建立 TextMeshPro - Text
□ 命名為 "WarningText"
□ 設置位置 (中央上方)
□ 設置字體大小: 30
□ 設置顏色: 紅色
□ 預設隱藏: ❌ 取消 Active 狀態
```

**4. MoneyUIController 組件** ⬜ TODO
```
□ 在 Canvas (或 Panel) 添加 Component: MoneyUIController
□ 分配 Money Text: 拖拽 MoneyText
□ 分配 Warning Text: 拖拽 WarningText
□ 驗證 Fade Duration: 0.3
□ 驗證 Display Duration: 2
```

**5. TowerDefenseUIManager 連接** ⬜ TODO
```
□ 選擇 TowerDefenseUIManager GameObject
□ 找到 "Money UI" 欄位
□ 拖拽 MoneyUIController 進去
   (或拖拽包含 MoneyUIController 組件的 GameObject)
```

**6. 設置塔成本** ⬜ TODO
```
□ floor_spike Button
  └─ PlaceableDefinition.cost = 100

□ wall_laser Button
  └─ PlaceableDefinition.cost = 220

(來源: towers.json Level 1 成本)
```

---

## 測試檢查表

### 測試 1: 建築模式 - UI 隱藏 ⬜ TODO
```
□ 啟動遊戲
□ 進入建築模式
□ ✓ MoneyText 不可見 (灰色)
□ ✓ WarningText 不可見
```

### 測試 2: 遊戲開始 - UI 出現 ⬜ TODO
```
□ 放置 Spawn 和 Goal
□ 點擊 "Compute Path"
□ 點擊 "Game Start"
□ ✓ MoneyText 出現在螢幕上
□ ✓ 顯示 "Money: 1000" (或初始值)
```

### 測試 3: 金錢不足 - 警告出現 ⬜ TODO
```
□ 設置 Current Money: 50
□ 嘗試拖曳塔
□ ✓ 紅色警告出現 "Not enough money!"
□ ✓ 警告淡入、停留、淡出 (~3 秒)
□ ✓ 拖曳被阻止
□ ✓ Preview 未啟動
```

### 測試 4: 金錢足夠 - 成功放置 ⬜ TODO
```
□ 設置 Current Money: 500+
□ 拖曳塔到地板
□ ✓ Preview 正常顯示
□ ✓ 成功放置
□ ✓ 金錢扣除 (MoneyText 更新)
```

### 測試 5: 回到建築 - UI 隱藏 ⬜ TODO
```
□ 在 Gameplay 中
□ 點擊 "Reset Path"
□ ✓ MoneyUI 自動隱藏
□ ✓ 回到建築模式
```

---

## 快速命令

### 設置完成後，測試金錢系統

```csharp
// 在 Console 中執行 (選擇 Game 視圖並按 `)

// 查看目前金錢
Debug.Log(MoneyManager.Instance.GetCurrentMoney());

// 加 500 金錢
MoneyManager.Instance.AddMoney(500);

// 查看能否負擔 150
Debug.Log(MoneyManager.Instance.CanAfford(150)); // true/false

// 手動扣 100
MoneyManager.Instance.Spend(100);
```

---

## 常見問題排除

### ❌ MoneyText 在建築模式時可見
**解決**:
- 檢查 MoneyText GameObject 是否預設隱藏
- Inspector → GameObject 的勾勾應該是 ❌ (灰色)

### ❌ Game Start 時 MoneyText 不出現
**解決**:
- 檢查 TowerDefenseUIManager 是否有分配 moneyUIController
- 檢查 MoneyUIController 是否有分配 moneyText

### ❌ 警告文字不是紅色
**解決**:
- 選擇 WarningText
- Inspector → TextMeshPro UGUI → Color: 改為紅色 (255, 0, 0)

### ❌ 警告沒有淡出，一直顯示
**解決**:
- 檢查 MoneyUIController 中的 warningFadeDuration 和 warningDisplayDuration
- 預設是 0.3 秒淡入 + 2 秒停留 + 0.3 秒淡出 = 約 3 秒總時間

### ❌ 放置塔時沒有扣錢
**解決**:
- 檢查每個塔的 PlaceableDefinition.cost 是否 > 0
- 檢查金錢是否足夠 (>= cost)

---

## 時間預估

| 步驟 | 時間 |
|------|------|
| 建立 MoneyManager | 2 分 |
| 建立 MoneyText | 2 分 |
| 建立 WarningText | 2 分 |
| 添加 MoneyUIController | 1 分 |
| 連接到 TowerDefenseUIManager | 1 分 |
| 設置塔成本 | 2 分 |
| **總計** | **10 分鐘** |

---

## 完成標誌 ✨

當你看到這些現象時，就表示一切正常運作：

✅ 建築模式時，頂部沒有金錢顯示
✅ 點擊 "Game Start" 後，金錢 UI 出現在左上角
✅ 嘗試放置昂貴的塔時，紅色警告閃現
✅ 成功放置塔時，金額正確扣除
✅ 點擊 "Reset" 時，金錢 UI 消失

---

## 下一步

1. ✅ 檢查這份清單
2. ⬜ 按照場景設置步驟執行
3. ⬜ 按照測試檢查表驗證
4. 🎉 完成！

---

**問題？** 參考 `MONEY_UI_SETUP_GUIDE.md` 了解詳細步驟
