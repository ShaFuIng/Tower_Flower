# 🎯 金錢系統 - 即刻開始操作指南

## ✅ 確認事項

- ✅ 所有程式已完成
- ✅ 編譯成功
- ✅ 自動化流程已設置
- ✅ 準備場景配置

---

## 🚀 立即開始 (5 分鐘快速流程)

### 操作步驟順序

#### **① 建立 MoneyManager**

```
在 Hierarchy 中:
  右鍵空白處
    ↓
  Create Empty
    ↓
  命名為 "MoneyManager"
    ↓
在 Inspector 中:
  Add Component
    ↓
  搜尋 "MoneyManager"
    ↓
  點擊添加
    ↓
  設置 Current Money = 1000 (可選)
```

✅ **完成**

---

#### **② 建立 MoneyText UI**

```
在 Hierarchy 中選擇 Canvas:
  右鍵 Canvas
    ↓
  UI
    ↓
  Text - TextMeshPro
    ↓
  命名為 "MoneyText"
    ↓
在 Inspector 中設置:
  Text: "Money: 1000"
  Font Size: 36
  Color: White (255, 255, 255)
    ↓
位置:
  Rect Transform → 調整到左上角
    ↓
⚠️ 重要：
  Hierarchy 中找到 MoneyText
    ↓
  取消勾選左邊的勾勾 ☑️ → ☐
    ↓
  MoneyText 現在應該是灰色的 (Inactive)
```

✅ **完成**

---

#### **③ 建立 WarningText UI**

```
在 Hierarchy 中選擇 Canvas:
  右鍵 Canvas
    ↓
  UI
    ↓
  Text - TextMeshPro
    ↓
  命名為 "WarningText"
    ↓
在 Inspector 中設置:
  Text: "Not enough money!"
  Font Size: 30
  Color: Red (255, 0, 0)
    ↓
位置:
  Rect Transform → 調整到中央上方
    ↓
⚠️ 重要：
  取消勾選左邊的勾勾 ☑️ → ☐
    ↓
  WarningText 現在應該是灰色的 (Inactive)
```

✅ **完成**

---

#### **④ 添加 MoneyUIController 組件**

```
在 Hierarchy 中選擇 Canvas:
  
在 Inspector 中:
  Add Component
    ↓
  搜尋 "MoneyUIController"
    ↓
  點擊添加
    ↓
看到欄位:
  
  Money Display
    └─ Money Text: ← 拖拽 MoneyText 到這裡
  
  Warning
    └─ Warning Text: ← 拖拽 WarningText 到這裡
    └─ Warning Fade Duration: 0.3 (保留預設)
    └─ Warning Display Duration: 2 (保留預設)
```

✅ **完成**

---

#### **⑤ 連接 TowerDefenseUIManager**

```
在 Hierarchy 中選擇 TowerDefenseUIManager:
  
在 Inspector 中找到 TowerDefenseUIManager 組件:
  
看到欄位:
  Money UI
    └─ Money UI Controller: ← 拖拽 Canvas 到這裡
         (或任何包含 MoneyUIController 的 GameObject)
```

✅ **完成**

---

#### **⑥ 設置 floor_spike 成本**

```
在 Hierarchy 中找到 floor_spike Button:

在 Inspector 中找到 PlaceableDefinition 組件:
  
看到欄位:
  Cost: [100] ← 已經是 100，保留不變
  
或確認是 100:
  Cost: 100
```

✅ **完成**

---

#### **⑦ 設置 wall_laser 成本**

```
在 Hierarchy 中找到 wall_laser Button:

在 Inspector 中找到 PlaceableDefinition 組件:
  
看到欄位:
  Cost: [220] ← 改為 220
  
確認是 220:
  Cost: 220
```

✅ **完成**

---

## ✅ 完成檢查清單

```
☐ MoneyManager GameObject 已建立
☐ MoneyManager 組件已添加
☐ MoneyText UI 已建立
☐ MoneyText 已隱藏 (灰色)
☐ WarningText UI 已建立
☐ WarningText 已隱藏 (灰色)
☐ MoneyUIController 組件已添加到 Canvas
☐ MoneyText 已連接到 MoneyUIController
☐ WarningText 已連接到 MoneyUIController
☐ MoneyUIController 已連接到 TowerDefenseUIManager
☐ floor_spike 成本 = 100
☐ wall_laser 成本 = 220

全部完成！✅
```

---

## 🧪 快速測試

完成所有步驟後，按照這個順序測試：

```
1. 按下 Play 按鈕啟動遊戲
   
2. 等待遊戲載入
   
3. ✅ 檢查 MoneyText 是否隱藏
   (不應該在螢幕上看到)
   
4. 放置 Spawn 和 Goal
   
5. 點擊 "Compute Path" 按鈕
   
6. ✅ 檢查 MoneyText 仍然隱藏
   
7. 點擊 "Game Start" 按鈕
   
8. ✅ 檢查 MoneyText 現在出現了
   (應該在左上角看到 "Money: 1000")
   
9. 嘗試拖曳一個塔
   
10. ✅ 檢查成功放置
    (MoneyText 應該更新成新數值)
    
11. 點擊 "Reset Path" 按鈕
    
12. ✅ 檢查 MoneyText 隱藏了
```

---

## 🎮 測試場景 - 驗證成本驗證

如果想測試「金錢不足」的情況：

```
1. 打開 MoneyManager GameObject
   
2. 在 Inspector 中改成:
   Current Money: 50 (低於塔的成本)
   
3. 啟動遊戲
   
4. 進入遊戲模式 (Game Start)
   
5. 嘗試拖曳塔
   
6. ✅ 應該看到:
   • 紅色警告 "Not enough money!" 出現
   • 警告淡入、停留、淡出
   • 拖曳被阻止
   
7. 改回 Current Money: 1000
   
8. 再試一次應該能正常放置
```

---

## 💡 關鍵提醒

### ⚠️ MoneyText 和 WarningText 必須預設隱藏

```
為什麼？
  因為我們想要在建築模式時完全隱藏它們，
  只在遊戲開始時才出現。

怎麼確認？
  1. 在 Hierarchy 中看到 MoneyText 和 WarningText
  2. 它們左邊的勾勾應該是 ☐ (未勾選)
  3. 字體應該是灰色的，表示 Inactive
  4. 如果是白色且可點，則需要勾掉
```

### ✅ 所有自動化流程

你不需要做任何額外的事，下列都是自動的：

```
✅ 檢查成本 - 自動在拖曳時執行
✅ 顯示警告 - 自動在成本不足時執行
✅ 淡入淡出 - 自動執行，無需手動
✅ 扣費 - 自動在放置後執行
✅ 更新 UI - 自動在扣費後執行
✅ 顯示 UI - 自動在 Game Start 時執行
✅ 隱藏 UI - 自動在 Reset Path 時執行
```

---

## 🐛 遇到問題？

| 問題 | 解決方案 |
|------|--------|
| MoneyText 沒有隱藏 | 檢查是否取消勾選 Active ☐ |
| MoneyText 沒有出現 | 檢查是否連接到 MoneyUIController |
| 沒有看到警告 | 檢查 WarningText 是否連接到 MoneyUIController |
| 成本沒有扣除 | 檢查塔的 PlaceableDefinition.cost 是否設置 |
| 按 Game Start 沒反應 | 檢查 MoneyUIController 是否連接到 TowerDefenseUIManager |
| 編譯錯誤 | 確保所有檔案都已保存並在正確位置 |

---

## ✨ 預期最終效果

設置完成並測試後，遊戲應該有這些表現：

```
建築模式
  └─ 沒有看到金錢 UI ✅

按 "Game Start"
  └─ 金錢 UI 出現在左上角 ✅

嘗試放置塔
  ├─ 金錢不足時
  │  └─ 紅色警告出現、淡出 ✅
  │
  └─ 金錢足夠時
     ├─ 成功放置 ✅
     └─ 金錢扣除，UI 更新 ✅

按 "Reset Path"
  └─ 金錢 UI 隱藏 ✅
```

---

## 📊 設置時間表

```
① 建立 MoneyManager:          1 分鐘 ⏱️
② 建立 MoneyText:             1 分鐘 ⏱️
③ 建立 WarningText:           1 分鐘 ⏱️
④ 添加 MoneyUIController:     1 分鐘 ⏱️
⑤ 連接 UI 元素:               1 分鐘 ⏱️
⑥ 連接 TowerDefenseUIManager: 0.5 分鐘 ⏱️
⑦ 設置塔成本:                 0.5 分鐘 ⏱️
⑧ 測試:                       5 分鐘 ⏱️

總計: ~11 分鐘 ⏱️
```

---

## 🎉 現在就開始！

你已經有了所有信息，可以立即開始配置了！

```
下一步:
  1. 打開 Unity
  2. 打開你的場景
  3. 按照上面的步驟進行配置
  4. 測試
  5. 完成！🎉

預期完成時間: 10-15 分鐘
難度級別: ⭐⭐ (簡單)
```

---

**準備好開始了嗎？ Let's go! 🚀**

有任何問題就參考之前的文件，或者看看快速卡片 `MONEY_UI_QUICK_CARD.md`。

加油！😊
