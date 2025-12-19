# 🎯 現在就做 - 10 分鐘完成指南

## ✅ 確認清單（開始前檢查）

- [x] Unity 編輯器已打開
- [x] 你的場景已打開
- [x] 可以看到 Canvas 在 Hierarchy 中
- [x] 可以看到 TowerDefenseUIManager
- [x] 準備好開始

**準備好了？開始吧！** ⏱️

---

## 🚀 7 個步驟，5 分鐘完成

### **步驟 1 - 建立 MoneyManager** (1 分鐘)

```
1. Hierarchy 中右鍵空白處
2. Create Empty
3. 命名為: MoneyManager
4. Inspector 中: Add Component > MoneyManager
5. 看到欄位 "Current Money" = 1000 ✅

⏱️ 完成第 1 步
```

---

### **步驟 2 - 建立 MoneyText** (1 分鐘)

```
1. Hierarchy 中點選 Canvas
2. 右鍵 Canvas > UI > Text - TextMeshPro
3. 命名為: MoneyText
4. Inspector 中設置:
   - Text: "Money: 1000"
   - Font Size: 36
5. 位置: 左上角 (Rect Transform 自己調)
6. ⚠️ 重要：取消勾選 Active (☑️ → ☐)
7. MoneyText 現在應該是灰色的 ✅

⏱️ 完成第 2 步
```

---

### **步驟 3 - 建立 WarningText** (1 分鐘)

```
1. Hierarchy 中點選 Canvas
2. 右鍵 Canvas > UI > Text - TextMeshPro
3. 命名為: WarningText
4. Inspector 中設置:
   - Text: "Not enough money!"
   - Font Size: 30
   - Color: 紅色 (255, 0, 0)
5. 位置: 中央上方
6. ⚠️ 重要：取消勾選 Active (☑️ → ☐)
7. WarningText 現在應該是灰色的 ✅

⏱️ 完成第 3 步
```

---

### **步驟 4 - 添加 MoneyUIController** (1 分鐘)

```
1. Hierarchy 中點選 Canvas
2. Inspector 中: Add Component > MoneyUIController
3. 看到欄位:
   - Money Display > Money Text
   - Warning > Warning Text
4. ✅

⏱️ 完成第 4 步
```

---

### **步驟 5 - 連接 UI 元素** (1 分鐘)

```
1. Hierarchy 中點選 Canvas (有 MoneyUIController)
2. Inspector 中找到 MoneyUIController 組件
3. Money Text 欄位: 拖拽 MoneyText 進去
4. Warning Text 欄位: 拖拽 WarningText 進去
5. 看到綠色對號 ✅

⏱️ 完成第 5 步
```

---

### **步驟 6 - 連接 TowerDefenseUIManager** (1 分鐘)

```
1. Hierarchy 中點選 TowerDefenseUIManager
2. Inspector 中找到 TowerDefenseUIManager 組件
3. 找到 Money UI 欄位
4. Money UI Controller: 拖拽 Canvas 進去
5. 看到綠色對號 ✅

⏱️ 完成第 6 步
```

---

### **步驟 7 - 設置塔成本** (1 分鐘)

```
設置 floor_spike:
1. Hierarchy 中找到 floor_spike Button
2. Inspector 中找到 PlaceableDefinition
3. Cost: 確認是 100 (或改成 100)

設置 wall_laser:
1. Hierarchy 中找到 wall_laser Button
2. Inspector 中找到 PlaceableDefinition
3. Cost: 改成 220

✅

⏱️ 完成第 7 步 - 全部完成！
```

---

## 🎮 測試（5 分鐘）

按照這個順序測試：

```
1. 按 Play 按鈕啟動遊戲 ▶️
   
2. 等待載入...
   
3. ✅ 檢查：MoneyText 隱藏（不應該看到）
   
4. 放置 Spawn 和 Goal
   
5. 點擊 "Compute Path"
   
6. ✅ 檢查：MoneyText 仍然隱藏
   
7. 點擊 "Game Start" Button
   
8. ✅ 檢查：MoneyText 出現了！
   (應該在左上角看到 "Money: 1000")
   
9. 拖曳一個塔到地板上
   
10. ✅ 檢查：放置成功，金錢扣除
    (MoneyText 應該更新成新數值，例如 900)
    
11. 點擊 "Reset Path"
    
12. ✅ 檢查：MoneyText 隱藏了
    
🎉 成功！所有功能都正常！
```

---

## ⚠️ 重要：如果測試失敗

### 金錢 UI 沒有隱藏

```
☐ 檢查 MoneyText 的 Active 是否取消勾選
☐ 應該看到灰色的 MoneyText
☐ 勾勾應該是空的 ☐
```

### 金錢 UI 沒有出現

```
☐ 檢查 MoneyText 是否連接到 MoneyUIController
☐ 檢查 MoneyUIController 是否連接到 TowerDefenseUIManager
☐ Inspector 中應該看到綠色對號 ✅
```

### 警告沒有出現

```
☐ 檢查 WarningText 是否連接到 MoneyUIController
☐ 將 MoneyManager.currentMoney 設置成 50
☐ 嘗試拖曳成本 > 50 的塔
☐ 應該看到紅色警告出現
```

### 成本沒有扣除

```
☐ 檢查 floor_spike 的成本是否設置成 100
☐ 檢查 wall_laser 的成本是否設置成 220
☐ 確認成功放置了塔（不是只有預覽）
```

---

## ✅ 完成檢查清單

```
☑️ MoneyManager GameObject 已建立
☑️ MoneyUIController 組件已添加
☑️ MoneyText UI 已建立且隱藏
☑️ WarningText UI 已建立且隱藏
☑️ UI 元素已連接
☑️ TowerDefenseUIManager 已連接
☑️ 塔成本已設置
☑️ 遊戲測試通過

🎉 所有步驟完成！
```

---

## 💾 保存

```
Ctrl + S (Windows)
Cmd + S (Mac)

確保你的場景已保存 ✅
```

---

## 🎊 完成！

你現在擁有一個完整的金錢系統！

### 你剛完成的事：

✅ 添加了金錢管理系統  
✅ 添加了金錢 UI 顯示  
✅ 添加了成本驗證和警告  
✅ 集成了自動化流程  
✅ 測試了所有功能  

### 系統現在支持：

✅ 檢查金錢  
✅ 扣除費用  
✅ 警告動畫  
✅ 自動 UI 更新  
✅ 自動顯示/隱藏  

---

## 🚀 下一步

現在你可以：

1. **調整數值**
   - 改變初始金錢
   - 改變塔的成本
   - 改變警告持續時間

2. **添加更多功能**
   - 敵人掉落金錢
   - 升級系統
   - 商店系統

3. **自定義 UI**
   - 改變顏色
   - 改變位置
   - 改變字體大小

---

## 📞 快速參考

### API 調用（如果你需要程式化使用）

```csharp
// 查詢金錢
int money = MoneyManager.Instance.GetCurrentMoney();

// 檢查能否負擔
if (MoneyManager.Instance.CanAfford(100)) {
    // 可以負擔
}

// 加錢（例如敵人掉落金錢）
MoneyManager.Instance.AddMoney(50);

// 訂閱變化
MoneyManager.Instance.OnMoneyChanged += (amount) => {
    Debug.Log("Money: " + amount);
};
```

---

## 🎉 恭喜！

你已經成功完成了金錢系統的場景配置！

現在你的遊戲有了完整的經濟系統！ 🎮💰

---

**花費時間**: 10-15 分鐘  
**難度**: ⭐⭐ 簡單  
**結果**: 完全可用的金錢系統 ✅

**準備好享受你的成果了嗎？Let's play! 🚀**
