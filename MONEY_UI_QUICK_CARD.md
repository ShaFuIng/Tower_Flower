# 🎯 金錢系統場景設置 - 快速操作卡

## ✅ 已完成（無需做任何事）

- ✅ MoneyManager.cs - 100% 完成
- ✅ MoneyUIController.cs - 100% 完成  
- ✅ PlacementCostValidator.cs - 100% 完成
- ✅ TowerDefenseUIManager - 已集成 ShowMoneyUI() / HideMoneyUI()
- ✅ IconDragHandler - 已集成成本驗證
- ✅ SpawnGoalPlacementManager - 已集成成本扣除
- ✅ 編譯 - 成功 ✅

---

## 🎯 需要做的事（5 分鐘）

### **1️⃣ 建立 MoneyManager**

```
Hierarchy 右鍵
  ↓
Create Empty
  ↓
命名: "MoneyManager"
  ↓
Inspector → Add Component → MoneyManager
  ↓
✅ 完成
```

---

### **2️⃣ 建立 MoneyText**

```
選擇 Canvas
  ↓
右鍵 → TextMeshPro - Text (UI)
  ↓
命名: "MoneyText"
  ↓
Inspector 設置:
  • Text: "Money: 1000"
  • Font Size: 36
  • Color: White (255, 255, 255)
  • Position: 左上角
  ↓
⚠️ 重要：取消勾選 Active (左邊勾勾)
  ↓
✅ 完成（現在是灰色的）
```

---

### **3️⃣ 建立 WarningText**

```
選擇 Canvas
  ↓
右鍵 → TextMeshPro - Text (UI)
  ↓
命名: "WarningText"
  ↓
Inspector 設置:
  • Text: "Not enough money!"
  • Font Size: 30
  • Color: Red (255, 0, 0)
  • Position: 中央上方
  ↓
⚠️ 重要：取消勾選 Active
  ↓
✅ 完成（現在是灰色的）
```

---

### **4️⃣ 添加 MoneyUIController**

```
選擇 Canvas (或任何 Panel)
  ↓
Inspector → Add Component → MoneyUIController
  ↓
設置欄位:
  
  Money Display
    └─ Money Text: 拖拽 "MoneyText" 進來
  
  Warning
    └─ Warning Text: 拖拽 "WarningText" 進來
    └─ Warning Fade Duration: 0.3
    └─ Warning Display Duration: 2
  ↓
✅ 完成
```

---

### **5️⃣ 連接 TowerDefenseUIManager**

```
選擇 TowerDefenseUIManager GameObject
  ↓
在 Inspector 找到 TowerDefenseUIManager 組件
  ↓
找到欄位 "Money UI"
  └─ Money UI Controller: 拖拽 Canvas (包含 MoneyUIController)
  ↓
✅ 完成
```

---

### **6️⃣ 設置 floor_spike 成本**

```
選擇 floor_spike Button
  ↓
在 Inspector 找到 PlaceableDefinition 組件
  ↓
設置 Cost: 100
  ↓
✅ 完成
```

---

### **7️⃣ 設置 wall_laser 成本**

```
選擇 wall_laser Button
  ↓
在 Inspector 找到 PlaceableDefinition 組件
  ↓
設置 Cost: 220
  ↓
✅ 完成
```

---

## 📋 檢查清單

```
☐ 建立 MoneyManager GameObject
☐ 建立 MoneyText (取消 Active)
☐ 建立 WarningText (取消 Active)
☐ 添加 MoneyUIController 組件
☐ 連接 MoneyText 到 MoneyUIController
☐ 連接 WarningText 到 MoneyUIController
☐ 連接 MoneyUIController 到 TowerDefenseUIManager
☐ 設置 floor_spike 成本 = 100
☐ 設置 wall_laser 成本 = 220

全部完成時打勾 ☑️
```

---

## 🧪 快速測試

設置完成後：

```
1. 啟動遊戲
   ↓
2. ✅ MoneyText 隱藏（灰色）
   ↓
3. 放置 Spawn/Goal，點 "Compute Path"
   ↓
4. ✅ MoneyText 仍然隱藏
   ↓
5. 點 "Game Start"
   ↓
6. ✅ MoneyText 出現，顯示 "Money: 1000"
   ↓
7. 拖曳塔
   ↓
8. ✅ 成功放置，金錢扣除，MoneyText 更新
   ↓
9. 點 "Reset Path"
   ↓
10. ✅ MoneyText 隱藏
```

---

## 💡 重要提醒

### ⚠️ **MoneyText 和 WarningText 必須預設隱藏**

```
為什麼？
  因為我們想在建築模式時隱藏它們，
  只在遊戲開始時才顯示。

怎麼做？
  在 Hierarchy 中找到它們，
  點擊左邊的勾勾取消勾選：
  
  ✅ (現在檢查中) → ❌ (現在取消檢查)
  
  它們應該變成灰色的。
```

---

## 📞 遇到問題？

| 問題 | 解決方案 |
|------|--------|
| MoneyText 不隱藏 | 檢查是否取消勾選 Active |
| 按下 Game Start 沒反應 | 檢查 MoneyUIController 是否連接到 TowerDefenseUIManager |
| 編譯錯誤 | 確保所有 .cs 檔案都在正確位置 |
| 警告不出現 | 檢查 WarningText 是否正確連接到 MoneyUIController |
| 成本沒有扣除 | 檢查塔的 PlaceableDefinition.cost 是否設置 |

---

## ✨ 預期效果

完成設置後：

```
建築模式
  → MoneyText 隱藏 ✅

點 "Game Start"
  → MoneyText 自動出現 ✅

嘗試放置塔
  • 金錢不足 → 紅色警告自動淡入淡出 ✅
  • 金錢足夠 → 成功放置，金錢自動扣除 ✅

點 "Reset Path"
  → MoneyText 自動隱藏 ✅
```

---

## 🎉 就這樣！

只需 5 分鐘的設置，金錢系統就完全可用了！

所有邏輯都已程式化，你只需要：
1. ✅ 建立 UI 元素
2. ✅ 拖拽連接它們
3. ✅ 設置成本數值

就完成了！ 🚀
