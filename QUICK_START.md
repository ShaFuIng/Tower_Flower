# 🚀 金錢系統 - 快速上手 (10 分鐘)

> 所有程式都已經完成，只需要在場景中設置 UI 元素即可！

---

## 📝 5 個快速步驟

### 1️⃣ 創建 MoneyManager (2 分)
```
Hierarchy 右鍵
  → Create Empty
  → 名字改為: MoneyManager
  
Inspector:
  → Add Component → MoneyManager
  → Current Money: 1000
```

### 2️⃣ 創建 MoneyText (2 分)
```
Canvas 右鍵
  → TextMeshPro - Text (UI)
  → 名字改為: MoneyText
  → 位置: 左上角
  
Inspector:
  → TextMeshPro UGUI 設置:
     - Text: "Money: 1000"
     - Font Size: 36
     - Color: 白色
  
✅ 重要：取消勾選 Active 狀態 ❌
  (我們希望它預設隱藏)
```

### 3️⃣ 創建 WarningText (2 分)
```
Canvas 右鍵
  → TextMeshPro - Text (UI)
  → 名字改為: WarningText
  → 位置: 中央上方
  
Inspector:
  → TextMeshPro UGUI 設置:
     - Text: "Not enough money!"
     - Font Size: 30
     - Color: 紅色 (255, 0, 0)
  
✅ 重要：取消勾選 Active 狀態 ❌
```

### 4️⃣ 添加 MoneyUIController (2 分)
```
選擇 Canvas

Inspector:
  → Add Component → MoneyUIController
  
設置欄位:
  ├─ Money Display
  │  └─ Money Text: 拖拽 MoneyText 進去
  │
  └─ Warning
     ├─ Warning Text: 拖拽 WarningText 進去
     ├─ Warning Fade Duration: 0.3
     └─ Warning Display Duration: 2
```

### 5️⃣ 連接到 TowerDefenseUIManager (1 分)
```
選擇 TowerDefenseUIManager GameObject

Inspector 找到 TowerDefenseUIManager 組件:
  → Money UI
     └─ Money UI Controller: 拖拽 Canvas 進去
                            (或任何有 MoneyUIController 的物件)
```

---

## ✅ 設置完成，現在測試

### 快速測試
```
1. 啟動遊戲
   ✓ MoneyText 不可見 (灰色)

2. 點擊 "Game Start"
   ✓ MoneyText 出現

3. 嘗試放置貴的塔
   ✓ 紅色警告

4. 拖曳便宜的塔
   ✓ 扣錢成功

5. 點擊 "Reset"
   ✓ MoneyUI 隱藏
```

---

## 🎮 自動化流程

你什麼都不用做，一切都自動化了！

```
建築 → Game Start → 金錢顯示 ✓
     ↓
  拖曳塔 → 檢查金錢 ✓
     ↓
  金錢足夠 → 放置 → 扣錢 → 更新 UI ✓
     ↓
  Reset → 金錢隱藏 ✓
```

---

## 💡 就這樣！

**時間**: 10 分鐘  
**複雜度**: 超簡單 (複製貼上)  
**結果**: 完整的金錢系統 🎉  

**下一步**? 啟動遊戲並享受你的金錢系統！

---

## ❓ 有問題?

| 問題 | 解決方案 |
|------|--------|
| MoneyText 一直可見 | 檢查 Active 是否取消勾選 ❌ |
| Game Start 沒有顯示 | 檢查 TowerDefenseUIManager 中的 moneyUIController 欄位 |
| 警告沒有消失 | 檢查 warningDisplayDuration 設置 (預設 2 秒) |
| 沒有扣錢 | 檢查塔的 PlaceableDefinition.cost 是否 > 0 |

更多幫助: 參考 `MONEY_UI_SETUP_GUIDE.md` 或 `MONEY_UI_CHECKLIST.md`
