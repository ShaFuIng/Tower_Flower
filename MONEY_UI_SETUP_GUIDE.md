# 💰 金錢系統 - 完整場景設置指南

## ✅ 程式集成已完成

MoneyUI 現在已經完全集成到遊戲流程中：

- ✅ 建築模式 → MoneyUI **隱藏**
- ✅ 點擊 Game Start Button → MoneyUI **自動顯示**
- ✅ 點擊 Reset Path Button → MoneyUI **自動隱藏**

---

## 📋 場景設置步驟（10 分鐘）

### **步驟 1：創建 MoneyManager GameObject** (~2 分鐘)

```
1. Hierarchy 右鍵
   → Create Empty
   → 命名為: "MoneyManager"

2. 在 Inspector 添加組件:
   → Add Component → MoneyManager

3. 設置初始金錢 (可選):
   → Current Money: 1000
   
4. 完成！(會自動設為 Singleton，場景持久化)
```

---

### **步驟 2：在 Canvas 創建 MoneyUI 元素** (~5 分鐘)

#### **2A. 創建 Money 顯示文字**

```
1. 選擇 Canvas
   → 右鍵 → TextMeshPro - Text (UI)
   → 命名為: "MoneyText"

2. 設置位置:
   → 左上角 或 你想要的地方
   → Rect Transform 自己調整位置和大小

3. 設置文字:
   → Text: "Money: 1000"
   → Font Size: 36 (或適合的大小)
   → Color: 白色或你喜歡的顏色

4. 預設隱藏 (重要！):
   → GameObject 左邊的勾勾 ✅ 改為 ❌ (勾掉 Active 狀態)
   
✅ 完成：現在 MoneyText 在 Hierarchy 中是灰色的（非啟用狀態）
```

#### **2B. 創建 Warning 警告文字**

```
1. 選擇 Canvas
   → 右鍵 → TextMeshPro - Text (UI)
   → 命名為: "WarningText"

2. 設置位置:
   → 螢幕中央上方 或 你想要的地方

3. 設置文字:
   → Text: "Not enough money!"
   → Font Size: 30
   → Color: 紅色 (255, 0, 0)

4. 預設隱藏:
   → GameObject 的勾勾 ✅ 改為 ❌
```

---

### **步驟 3：添加 MoneyUIController 組件** (~2 分鐘)

```
1. 選擇 Canvas (或任何 UI Panel)

2. Add Component → MoneyUIController

3. 在 Inspector 設置欄位:
   
   ├─ Money Display
   │  └─ Money Text: 拖拽 "MoneyText" 進來
   │
   └─ Warning
      └─ Warning Text: 拖拽 "WarningText" 進來
      └─ Warning Fade Duration: 0.3 (淡入時間)
      └─ Warning Display Duration: 2 (停留時間)

4. 完成！
```

---

### **步驟 4：連接 TowerDefenseUIManager** (~1 分鐘)

```
1. 選擇 TowerDefenseUIManager GameObject

2. 在 Inspector 找到 TowerDefenseUIManager 組件

3. 找到新增的欄位:
   → Money UI
      └─ Money UI Controller: 拖拽 Canvas (或添加 MoneyUIController 的 GameObject)

4. 完成！
```

---

### **步驟 5：為每個塔設置成本** (~1 分鐘)

```
對每個塔的 Button GameObject:

1. 選擇塔的 Icon Button (在 Canvas 中)

2. 在 Inspector 找到 PlaceableDefinition 組件

3. 設置 Cost 字段:
   ├─ floor_spike (地刺陷阱)
   │  └─ Cost: 100
   │
   └─ wall_laser (雷射炮)
      └─ Cost: 220

參考: Assets/Resources/DataBase/towers.json 中的 Level 1 成本
```

---

## 🎮 UI 行為流程

```
遊戲開始
  ↓
進入建築模式 (Build Phase)
  ├─ MoneyUI: 隱藏 ❌
  └─ 玩家放置 Spawn/Goal
  ↓
點擊 "Compute Path" Button
  ├─ MoneyUI: 仍然隱藏 ❌
  └─ 路徑計算
  ↓
點擊 "Game Start" Button
  ├─ OnGameStartButtonPressed() 呼叫
  ├─ MoneyUI: 自動顯示 ✅ (ShowMoneyUI())
  ├─ 進入 Gameplay Phase
  └─ 玩家開始放置防禦塔
      ├─ 有足夠金錢 → 正常放置
      └─ 金錢不足 → 紅色警告出現
  ↓
點擊 "Reset Path" Button
  ├─ OnResetPathButtonPressed() 呼叫
  ├─ MoneyUI: 自動隱藏 ❌ (HideMoneyUI())
  └─ 回到建築模式
```

---

## 📋 完整設置清單

| # | 項目 | 地點 | 重要設置 | 狀態 |
|---|------|------|--------|------|
| 1 | 建立 MoneyManager | Hierarchy (根) | `Current Money: 1000` | ✅ |
| 2 | 建立 MoneyText | Canvas > MoneyText | **預設隱藏** ❌ | ⚠️ |
| 3 | 建立 WarningText | Canvas > WarningText | **預設隱藏** ❌ | ⚠️ |
| 4 | 添加 MoneyUIController | Canvas 或 Panel | 分配 UI 元素 | ✅ |
| 5 | 連接 TowerDefenseUIManager | TowerDefenseUIManager | 拖拽 MoneyUIController | ✅ |
| 6 | 設置塔成本 (floor_spike) | floor_spike Button | Cost: 100 | ⚠️ |
| 7 | 設置塔成本 (wall_laser) | wall_laser Button | Cost: 220 | ⚠️ |

**⚠️ = 需要手動設置**

---

## 🧪 測試流程

設置完成後，按照這個順序測試：

### 測試 1：建築模式 - MoneyUI 隱藏
```
1. 啟動遊戲
2. 進入建築模式
3. ✅ 檢查：MoneyText 不可見（灰色）
4. ✅ 檢查：WarningText 不可見
```

### 測試 2：遊戲開始 - MoneyUI 顯示
```
1. 放置 Spawn 和 Goal
2. 點擊 "Compute Path"
3. 點擊 "Game Start" Button
4. ✅ 檢查：MoneyText 出現在螢幕上
5. ✅ 檢查：顯示 "Money: 1000" (或你設置的初始值)
```

### 測試 3：金錢不足 - 警告出現
```
1. 在 MoneyManager 中設置 Current Money: 50 (低於塔的成本)
2. 嘗試拖曳塔
3. ✅ 檢查：紅色警告 "Not enough money!" 出現
4. ✅ 檢查：警告淡入、停留 2 秒、淡出
5. ✅ 檢查：拖曳被阻止，Preview 未啟動
```

### 測試 4：金錢足夠 - 成功放置
```
1. 設置 Current Money: 500+
2. 拖曳塔到地板
3. ✅ 檢查：Preview 正常顯示
4. ✅ 檢查：成功放置
5. ✅ 檢查：金錢扣除（MoneyText 更新）
```

### 測試 5：回到建築模式 - MoneyUI 隱藏
```
1. 在遊戲中 (Gameplay Phase)
2. 點擊 "Reset Path" Button
3. ✅ 檢查：MoneyUI 自動隱藏
4. ✅ 檢查：回到建築模式
```

---

## 💡 關鍵提醒

### ⚠️ **重要：預設隱藏 MoneyUI**

MoneyText 和 WarningText 必須在 Hierarchy 中預設隱藏：

```
在 Inspector 中取消勾選 GameObject 的 Active 狀態

✅ MoneyText: 
   ❌ (勾掉 Active)
   
✅ WarningText:
   ❌ (勾掉 Active)
```

**原因**: 我們希望 UI 在建築模式時完全隱藏，只在 Game Start 時才顯示。

---

## 🔄 程式流程（自動執行）

你不需要做任何事，下列流程全是自動的：

```
✅ 自動檢查成本
   → IconDragHandler.OnBeginDrag()
   → PlacementCostValidator.CanStartPlacement()

✅ 自動顯示警告
   → 金錢不足時
   → MoneyUIController.ShowNotEnoughMoneyWarning()
   → 紅色警告淡入、停留、淡出

✅ 自動扣費
   → SpawnGoalPlacementManager.PlaceObjectFromPreview()
   → PlacementCostValidator.SpendPlacementCost()
   → MoneyManager.Spend()

✅ 自動更新 UI
   → MoneyManager.OnMoneyChanged 事件
   → MoneyUIController.OnMoneyChanged()
   → MoneyText 文字更新

✅ 自動顯示隱藏
   → Game Start Button 按下
   → TowerDefenseUIManager.OnGameStartButtonPressed()
   → moneyUIController.ShowMoneyUI()
```

---

## 📞 快速參考

### 檔案位置
```
✅ 新增 MoneyManager.cs
   → Assets/我的/程式ㄟ/money/MoneyManager.cs

✅ 新增 MoneyUIController.cs
   → Assets/我的/程式ㄟ/money/MoneyUIController.cs

✅ 新增 PlacementCostValidator.cs
   → Assets/我的/程式ㄟ/money/PlacementCostValidator.cs

✅ 修改 TowerDefenseUIManager.cs
   → Assets/我的/程式ㄟ/塔防管理系統/TowerDefenseUIManager.cs
   (已添加 ShowMoneyUI / HideMoneyUI 調用)
```

### 新增 API
```csharp
// MoneyUIController
moneyUIController.ShowMoneyUI();              // 顯示金錢 UI
moneyUIController.HideMoneyUI();              // 隱藏金錢 UI
moneyUIController.ShowNotEnoughMoneyWarning(); // 手動觸發警告

// MoneyManager (如前)
MoneyManager.Instance.CanAfford(cost);       // 檢查負擔能力
MoneyManager.Instance.Spend(cost);           // 扣費
MoneyManager.Instance.AddMoney(amount);      // 加錢
MoneyManager.Instance.GetCurrentMoney();     // 查詢餘額
```

---

## ✨ 總結

| 項目 | 狀態 |
|------|------|
| **程式集成** | ✅ 完成 |
| **自動控制 UI 顯示/隱藏** | ✅ 完成 |
| **自動成本驗證** | ✅ 完成 |
| **自動扣費** | ✅ 完成 |
| **編譯** | ✅ 成功 |
| **場景設置** | ⚠️ 需要你手動做 (10 分鐘) |

**下一步**: 按照上面的 5 個步驟設置場景，然後測試！

---

**預期效果**:
1. 建築模式：沒有金錢 UI ✅
2. 遊戲開始：金錢 UI 出現 ✅
3. 金錢不足：紅色警告出現 ✅
4. 回到建築：金錢 UI 隱藏 ✅
