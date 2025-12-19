# 💰 金錢 UI 場景設置 - 簡潔版

## ✅ 程式部分：已完成 100%

所有代碼已寫完，包括自動控制 UI 顯示/隱藏的邏輯。

---

## ⚠️ 場景設置：需要你手動做（~5 分鐘）

### **步驟 1：創建 MoneyManager GameObject**

```
1. 在 Hierarchy 右鍵
   → Create Empty
   → 命名為: "MoneyManager"

2. 在 Inspector 添加組件:
   → Add Component → MoneyManager

3. 設置初始金錢 (可選):
   → Current Money: 1000
   
✅ 完成！
```

---

### **步驟 2：在 Canvas 創建 MoneyText（金錢顯示）**

```
1. 選擇你的 Canvas

2. 右鍵 → TextMeshPro - Text (UI)
   → 命名為: "MoneyText"

3. 設置位置:
   → 左上角 或 你想要的地方
   → 自己調整 Rect Transform

4. 設置文字:
   → Text: "Money: 1000"
   → Font Size: 36 (或適合的大小)
   → Color: 白色

5. ⚠️ 重要！預設隱藏:
   → 勾掉 GameObject 左邊的 ✅ (取消勾選 Active)
   → 現在 MoneyText 在 Hierarchy 中應該是灰色的
   
✅ 完成！
```

**為什麼要隱藏？** 因為遊戲開始時才要顯示，程式會自動控制。

---

### **步驟 3：在 Canvas 創建 WarningText（警告文字）**

```
1. 選擇你的 Canvas

2. 右鍵 → TextMeshPro - Text (UI)
   → 命名為: "WarningText"

3. 設置位置:
   → 螢幕中央上方 或 你想要的地方

4. 設置文字:
   → Text: "Not enough money!"
   → Font Size: 30
   → Color: 紅色 (255, 0, 0)

5. ⚠️ 重要！預設隱藏:
   → 勾掉 GameObject 左邊的 ✅
   → 灰色顯示

✅ 完成！
```

---

### **步驟 4：添加 MoneyUIController 組件**

```
1. 選擇 Canvas (或任何 UI Panel)

2. Add Component → MoneyUIController

3. 在 Inspector 設置:
   
   Money Display
   └─ Money Text: 拖拽 "MoneyText" 進來
   
   Warning
   └─ Warning Text: 拖拽 "WarningText" 進來
   └─ Warning Fade Duration: 0.3
   └─ Warning Display Duration: 2

✅ 完成！
```

---

### **步驟 5：連接 TowerDefenseUIManager**

```
1. 選擇 TowerDefenseUIManager GameObject (在 Hierarchy 中)

2. 在 Inspector 找到 TowerDefenseUIManager 組件

3. 找到欄位:
   
   Money UI
   └─ Money UI Controller: 拖拽 Canvas (或包含 MoneyUIController 的 GameObject)

✅ 完成！
```

---

### **步驟 6：為每個塔設置成本**

```
對每個塔的 Button:

1. 選擇 floor_spike 按鈕
   → 在 Inspector 找到 PlaceableDefinition
   → Cost: 100

2. 選擇 wall_laser 按鈕
   → 在 Inspector 找到 PlaceableDefinition
   → Cost: 220

參考: Assets/Resources/DataBase/towers.json

✅ 完成！
```

---

## 📋 檢查清單

| # | 項目 | 完成 |
|---|------|------|
| 1 | 建立 MoneyManager GameObject | ☐ |
| 2 | 建立 MoneyText (預設隱藏) | ☐ |
| 3 | 建立 WarningText (預設隱藏) | ☐ |
| 4 | 添加 MoneyUIController 組件 | ☐ |
| 5 | 連接 MoneyText 和 WarningText | ☐ |
| 6 | 連接 TowerDefenseUIManager | ☐ |
| 7 | 設置 floor_spike 成本 = 100 | ☐ |
| 8 | 設置 wall_laser 成本 = 220 | ☐ |

---

## 🎮 自動流程（程式已處理，無需手動）

當你按 **Game Start Button** 時：
```
OnGameStartButtonPressed()
  ↓
moneyUIController.ShowMoneyUI()  ← 自動啟用 MoneyText
  ↓
進入 Gameplay Phase
  ↓
玩家放置塔
  ├─ 有足夠金錢 → 正常放置
  └─ 金錢不足 → 紅色警告出現 (自動淡入淡出)

當你按 **Reset Path Button** 時：
  ↓
moneyUIController.HideMoneyUI()  ← 自動隱藏 MoneyText
  ↓
回到建築模式
```

---

## 🧪 測試流程

設置完成後測試：

### 測試 1：建築模式 - 金錢 UI 隱藏
```
1. 啟動遊戲
2. ✅ 檢查：MoneyText 不可見（灰色）
```

### 測試 2：遊戲開始 - 金錢 UI 出現
```
1. 放置 Spawn 和 Goal
2. 點擊 "Compute Path"
3. 點擊 "Game Start"
4. ✅ 檢查：MoneyText 出現，顯示 "Money: 1000"
```

### 測試 3：金錢不足 - 警告出現
```
1. 在 MoneyManager 設置 Current Money: 50
2. 嘗試拖曳塔
3. ✅ 檢查：紅色警告出現，淡入/停留/淡出
4. ✅ 檢查：拖曳被阻止
```

### 測試 4：金錢足夠 - 成功放置
```
1. 設置 Current Money: 500
2. 放置塔
3. ✅ 檢查：成功放置
4. ✅ 檢查：金錢扣除 (MoneyText 更新)
```

### 測試 5：回到建築模式 - 金錢 UI 隱藏
```
1. 在 Gameplay 中
2. 點擊 "Reset Path"
3. ✅ 檢查：MoneyText 隱藏
```

---

## 💡 關鍵重點

✅ **MoneyText 和 WarningText 預設隱藏很重要**
- 在 Hierarchy 中取消勾選 Active
- 程式會自動控制顯示/隱藏

✅ **只需要添加 UI 元素，邏輯全是自動的**
- 成本驗證 ✓
- 警告顯示 ✓
- 金錢扣除 ✓
- UI 更新 ✓
- 顯示/隱藏控制 ✓

---

## 📁 檔案位置參考

```
✅ MoneyManager.cs
   → Assets/我的/程式ㄟ/money/MoneyManager.cs

✅ MoneyUIController.cs
   → Assets/我的/程式ㄟ/money/MoneyUIController.cs

✅ TowerDefenseUIManager.cs
   → Assets/我的/程式ㄟ/塔防管理系統/TowerDefenseUIManager.cs
   (已有 ShowMoneyUI() / HideMoneyUI() 呼叫)
```

---

## ✨ 預期效果

| 階段 | 狀態 |
|------|------|
| 建築模式 | 金錢 UI 隱藏 ✅ |
| 按 Game Start | 金錢 UI 自動出現 ✅ |
| 金錢不足時 | 紅色警告淡入淡出 ✅ |
| 按 Reset Path | 金錢 UI 自動隱藏 ✅ |

---

**設置時間**: ~5 分鐘  
**難度**: ⭐⭐ 簡單（只是拖拽和配置）

準備好開始設置了嗎？😊
