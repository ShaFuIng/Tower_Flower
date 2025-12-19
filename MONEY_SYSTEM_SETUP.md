# Money System Setup Guide

## Overview
A simple, non-invasive money system has been added to the tower defense project. It includes:

1. **MoneyManager.cs** - Core money tracking and API
2. **MoneyUIController.cs** - UI display and warning notifications
3. **PlacementCostValidator.cs** - Placement cost validation logic

## Files Modified

### New Files Created
- `Assets/我的/程式ㄟ/money/MoneyManager.cs`
- `Assets/我的/程式ㄟ/money/MoneyUIController.cs`
- `Assets/我的/程式ㄟ/money/PlacementCostValidator.cs`

### Existing Files Modified
- `Assets/我的/程式ㄟ/塔防管理系統/PlaceableDefinition.cs` - Added `cost` field
- `Assets/我的/程式ㄟ/塔防管理系統/IconDragHandler.cs` - Added cost validation in `OnBeginDrag`
- `Assets/我的/程式ㄟ/塔防管理系統/SpawnGoalPlacementManager.cs` - Added money spending after placement

## Scene Setup

### 1. Add MoneyManager to Scene
- Create a new empty GameObject named `MoneyManager` at the root level
- Add the `MoneyManager` script component
- Adjust starting money in Inspector (default: 1000)
- This GameObject will persist across scenes (DontDestroyOnLoad)

### 2. Add MoneyUIController to Canvas
- Find your Canvas in the scene
- Create a new Panel for money display
- Add the `MoneyUIController` script component
- In Inspector, assign:
  - **Money Text**: A TextMeshProUGUI element showing current money
  - **Warning Text**: A TextMeshProUGUI element for "Not enough money" warning
  - **Warning Fade Duration**: Duration for fade in/out (default: 0.3s)
  - **Warning Display Duration**: How long warning stays visible (default: 2s)

### 3. Configure PlaceableDefinition Costs
- For each tower icon button in your scene:
  - Find or attach the `PlaceableDefinition` component
  - Set the **Cost** field to the base cost from `towers.json`
  - Example: `floor_spike` level 1 costs 100

## How It Works

### Placement Flow
1. User clicks a tower icon to start dragging
2. `IconDragHandler.OnBeginDrag()` is called
3. `PlacementCostValidator.CanStartPlacement(cost)` checks affordability
4. If player can't afford:
   - Preview is blocked
   - "Not enough money" warning fades in/out
   - Drag does NOT start
5. If player can afford:
   - Preview starts normally
   - User can drag to place
6. After successful placement:
   - `PlacementCostValidator.SpendPlacementCost(cost)` deducts money
   - `MoneyManager` fires `OnMoneyChanged` event
   - `MoneyUIController` updates display

### API Usage

**MoneyManager**
```csharp
bool canAfford = MoneyManager.Instance.CanAfford(100);
bool spent = MoneyManager.Instance.Spend(100);
MoneyManager.Instance.AddMoney(50);
int current = MoneyManager.Instance.GetCurrentMoney();

// Subscribe to changes
MoneyManager.Instance.OnMoneyChanged += (amount) => Debug.Log($"Money: {amount}");
```

**PlacementCostValidator**
```csharp
// Check if placement can start (automatic in drag flow)
if (!PlacementCostValidator.CanStartPlacement(cost))
{
    return; // Block placement
}

// Spend money after placement succeeds (automatic in placement flow)
PlacementCostValidator.SpendPlacementCost(cost);
```

**MoneyUIController**
```csharp
// Trigger warning manually if needed
moneyUIController.ShowNotEnoughMoneyWarning();
```

## Integration Points

### Automatic Integration
- Cost validation happens automatically when user tries to drag a tower
- Money spending happens automatically after successful placement
- No manual calls needed in most cases

### Custom Integration
If you need to add custom behaviors:

1. **Before drag**: Check `PlacementCostValidator.CanStartPlacement(cost)`
2. **After placement**: Call `PlacementCostValidator.SpendPlacementCost(cost)`
3. **Money rewards**: Call `MoneyManager.Instance.AddMoney(amount)` when enemies die, etc.

## Design Principles

✅ **Non-Invasive**
- All changes are either new scripts or minimal hook calls
- Existing placement logic untouched
- Pure validation layer, no modifications to core systems

✅ **Event-Driven**
- UI updates via `MoneyManager.OnMoneyChanged` event
- Loose coupling between managers

✅ **Stateless Validation**
- `PlacementCostValidator` doesn't store state
- Pure static methods for checking and spending
- Easy to test and reason about

✅ **Optional UI**
- All UI elements optional (can work without MoneyUIController)
- Easy to replace with custom UI

## Notes

- Starting money is configurable in MoneyManager Inspector
- Warning fade duration is configurable in MoneyUIController
- Cost field on PlaceableDefinition defaults to 100 if not set
- System works across preparation and combat phases
- Money persists between scenes (MoneyManager has DontDestroyOnLoad)
