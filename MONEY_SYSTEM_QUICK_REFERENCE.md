# Money System - Quick Reference

## Three New Scripts Created

| Script | Purpose | Key Methods |
|--------|---------|-------------|
| **MoneyManager.cs** | Core money tracking | `CanAfford()`, `Spend()`, `AddMoney()`, `GetCurrentMoney()` |
| **MoneyUIController.cs** | UI display & warnings | `ShowNotEnoughMoneyWarning()` |
| **PlacementCostValidator.cs** | Validation glue | `CanStartPlacement()`, `SpendPlacementCost()` |

## Quick Setup (5 minutes)

### Step 1: Scene Setup
```
1. Create empty GameObject "MoneyManager"
   - Add MoneyManager component
   - Set starting money in Inspector (default: 1000)

2. Add MoneyUIController to Canvas
   - Assign Money Text (TextMeshProUGUI)
   - Assign Warning Text (TextMeshProUGUI)
```

### Step 2: Tower Configuration
```
For each tower button:
1. Find PlaceableDefinition component
2. Set Cost field = tower's base cost (from towers.json)
   - floor_spike: 100
   - wall_laser: 220
```

### Step 3: Done!
- Cost validation happens automatically
- Money deduction happens automatically
- UI updates happen automatically

## API Cheat Sheet

```csharp
// Check money (read-only, no side effects)
if (MoneyManager.Instance.CanAfford(100))
{
    Debug.Log("Can afford!");
}

// Add money (e.g., enemy defeated)
MoneyManager.Instance.AddMoney(50);

// Get balance
int balance = MoneyManager.Instance.GetCurrentMoney();

// Listen for money changes
MoneyManager.Instance.OnMoneyChanged += (amount) =>
{
    Debug.Log($"Money updated to: {amount}");
};

// Validate placement (called automatically in drag)
if (!PlacementCostValidator.CanStartPlacement(cost))
{
    // Show warning, block preview
    return;
}

// Spend money (called automatically after placement)
if (PlacementCostValidator.SpendPlacementCost(100))
{
    Debug.Log("Money spent successfully!");
}

// Show warning manually
moneyUIController.ShowNotEnoughMoneyWarning();
```

## What Happens When...

### User clicks a tower icon
```
✓ IconDragHandler.OnBeginDrag() called
✓ PlacementCostValidator.CanStartPlacement() checks affordability
✗ If no money: warning appears, drag blocked
✓ If enough money: preview starts normally
```

### Tower is successfully placed
```
✓ SpawnGoalPlacementManager.PlaceObjectFromPreview() executes
✓ PlacementCostValidator.SpendPlacementCost() deducts money
✓ MoneyManager fires OnMoneyChanged event
✓ MoneyUIController updates display
```

### Warning appears
```
✓ MoneyUIController.ShowNotEnoughMoneyWarning() called
✓ Text fades in (0.3s default)
✓ Text stays visible (2s default)
✓ Text fades out (0.3s default)
```

## Minimal Changes to Existing Code

| File | Change | Lines |
|------|--------|-------|
| PlaceableDefinition.cs | Added cost field | +3 |
| IconDragHandler.cs | Added validation check | +3 |
| SpawnGoalPlacementManager.cs | Added spending call | +1 |
| **Total Impact** | | **+7 lines** |

## Files to Know

**New Files:**
- `Assets/我的/程式ㄟ/money/MoneyManager.cs`
- `Assets/我的/程式ㄟ/money/MoneyUIController.cs`
- `Assets/我的/程式ㄟ/money/PlacementCostValidator.cs`

**Modified Files:**
- `Assets/我的/程式ㄟ/塔防管理系統/PlaceableDefinition.cs`
- `Assets/我的/程式ㄟ/塔防管理系統/IconDragHandler.cs`
- `Assets/我的/程式ㄟ/塔防管理系統/SpawnGoalPlacementManager.cs`

**Documentation:**
- `MONEY_SYSTEM_SETUP.md` (detailed setup guide)
- `MONEY_SYSTEM_IMPLEMENTATION.md` (architecture & design)

## Troubleshooting

| Issue | Solution |
|-------|----------|
| "MoneyManager not found" error | Create MoneyManager GameObject in scene |
| Warning not showing | Assign Warning Text in MoneyUIController inspector |
| Money not deducting | Check PlaceableDefinition.cost is set > 0 |
| UI not updating | Ensure MoneyUIController is in Canvas |
| Cost always blocks placement | Check starting money in MoneyManager |

## Ready to Use! ✅

All three systems are implemented, integrated, and tested.
Build: **Successful**
Errors: **0**
Warnings: **0** (relevant to money system)
