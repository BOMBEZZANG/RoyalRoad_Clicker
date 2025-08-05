# Unity Setup Guide - Player System

## Overview
This guide provides step-by-step instructions for setting up the Player system in Unity, including GameObject hierarchy, UI configuration, and script assignments.

## 1. Creating the ClassRequirements ScriptableObject

### Step 1: Create the Asset
1. Right-click in Project window → Create → RoyalRoad → Class Requirements
2. Name it "ClassRequirements_Default"
3. Configure the requirements in the Inspector (the default values are already set in code)

## 2. Scene Hierarchy Setup

### Root GameObject Structure
```
RoyalRoadClicker (Empty GameObject)
├── Managers (Empty GameObject)
│   └── GameManager (Empty GameObject)
├── Player (Empty GameObject)
└── UI
    └── Canvas
        ├── TopHUD
        │   ├── ResourcePanel
        │   │   ├── RiceDisplay
        │   │   ├── HonorDisplay
        │   │   └── ClassDisplay
        │   └── ProductionPanel
        │       ├── RicePerSecond
        │       └── HonorPerSecond
        ├── MiddleVisualStage
        │   ├── BackgroundImage
        │   ├── CharacterImage
        │   └── TapArea
        └── BottomInteractionZone
            ├── UpgradeTab
            ├── HonorTab
            ├── ClassTab
            └── ShopTab
```

## 3. Player GameObject Setup

### Create Player GameObject:
1. Create Empty GameObject named "Player"
2. Position: (0, 0, 0)
3. Add Components:
   - PlayerPresenter.cs
   - PlayerView.cs
   - PlayerStatus.cs

### Configure PlayerPresenter:
```
Production Update:
- Production Update Interval: 0.1
Save Settings:
- Auto Save Interval: 30
```

### Configure PlayerStatus:
```
Class Requirements: [Drag ClassRequirements_Default here]
Validation Settings:
- Max Resource Cap: 1e15
- Max Production Rate: 1e12
```

## 4. UI Canvas Setup

### Canvas Settings:
1. Create Canvas (Right-click → UI → Canvas)
2. Canvas Component:
   - Render Mode: Screen Space - Overlay
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1080 x 1920 (Portrait)
   - Screen Match Mode: 0.5

### Top HUD Setup:

#### ResourcePanel:
1. Create Empty GameObject "ResourcePanel"
2. Add Vertical Layout Group:
   - Spacing: 10
   - Child Alignment: Upper Center
   - Child Force Expand: Width ✓

#### Rice Display:
1. Create TextMeshPro - Text (UI) named "RiceText"
2. Settings:
   - Font Size: 36
   - Alignment: Center
   - Text: "0 쌀"
   - Font Asset: [Your Korean-supporting font]

#### Honor Display:
1. Create TextMeshPro - Text (UI) named "HonorText"
2. Settings:
   - Font Size: 32
   - Alignment: Center
   - Text: "0 명예"
   - Color: Gold (#FFD700)

#### Class Display:
1. Create TextMeshPro - Text (UI) named "ClassNameText"
2. Settings:
   - Font Size: 40
   - Alignment: Center
   - Text: "노비"
   - Font Style: Bold

### Middle Visual Stage Setup:

#### Background Image:
1. Create UI → Image named "BackgroundImage"
2. Rect Transform:
   - Anchor: Stretch both
   - Offset: 0 on all sides
3. Image Component:
   - Source Image: [Leave empty for now]

#### Character Image:
1. Create UI → Image named "CharacterImage"
2. Rect Transform:
   - Anchor: Center
   - Width: 400, Height: 600
3. Image Component:
   - Source Image: [Leave empty for now]
   - Preserve Aspect: ✓

#### Tap Area:
1. Create UI → Button named "TapArea"
2. Rect Transform:
   - Anchor: Stretch both
   - Cover entire middle section
3. Image Component:
   - Color: (255, 255, 255, 0) [Transparent]
4. Button Component:
   - Transition: None
   - OnClick(): Player.PlayerPresenter.OnTap()

## 5. Script Connections

### PlayerView Component Configuration:

#### Resource Displays:
```
Rice Text: [Drag RiceText]
Honor Text: [Drag HonorText]
Rice Per Second Text: [Drag RicePerSecondText]
Honor Per Second Text: [Drag HonorPerSecondText]
Rice Per Tap Text: [Drag RicePerTapText]
Koku Text: [Drag KokuText] (Initially inactive)
```

#### Class Display:
```
Class Name Text: [Drag ClassNameText]
Character Image: [Drag CharacterImage]
Background Image: [Drag BackgroundImage]
```

#### Sprites (Create placeholder sprites or import assets):
```
Character Sprites:
- Slave Sprite: [Drag sprite]
- Tenant Farmer Sprite: [Drag sprite]
- Commoner Sprite: [Drag sprite]
- Noble Sprite: [Drag sprite]
- Lord Sprite: [Drag sprite]
- King Sprite: [Drag sprite]

Background Sprites:
- Slave Background: [Drag sprite]
- Tenant Farmer Background: [Drag sprite]
- Commoner Background: [Drag sprite]
- Noble Background: [Drag sprite]
- Lord Background: [Drag sprite]
- King Background: [Drag sprite]
```

#### Formatting:
```
Rice Format: "{0:N0} 쌀"
Honor Format: "{0:N0} 명예"
Per Second Format: "{0:N1}/초"
Per Tap Format: "{0:N0}/탭"
Koku Format: "{0:N0} 석고"
```

## 6. GameManager Setup

Create a simple GameManager to initialize the player:

### GameManager.cs:
```csharp
using UnityEngine;
using RoyalRoadClicker.Gameplay.Player;
using RoyalRoadClicker.Data;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PlayerPresenter playerPresenter;
    
    private PlayerModel playerModel;
    
    private void Start()
    {
        InitializeGame();
    }
    
    private void InitializeGame()
    {
        // Load saved data or create new
        SaveData saveData = PlayerDataSerializer.LoadFromFile();
        
        if (saveData != null && saveData.IsValidSave())
        {
            playerModel = new PlayerModel(saveData.PlayerData);
        }
        else
        {
            playerModel = new PlayerModel();
        }
        
        // Initialize presenter
        playerPresenter.Initialize(playerModel);
        
        // Subscribe to save events
        playerPresenter.OnPlayerDataChanged += HandlePlayerDataChanged;
    }
    
    private void HandlePlayerDataChanged(PlayerData data)
    {
        SaveData saveData = new SaveData(data, 0);
        PlayerDataSerializer.SaveToFile(saveData);
    }
    
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            playerPresenter.SavePlayerData();
    }
    
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            playerPresenter.SavePlayerData();
    }
}
```

### GameManager GameObject Setup:
1. Add GameManager.cs to the GameManager GameObject
2. Drag the Player GameObject to the Player Presenter field

## 7. Testing Setup

### Create Test Buttons (Optional for Development):
1. Create UI → Button "AddRiceButton"
   - OnClick(): Add test method to give rice
2. Create UI → Button "AddHonorButton"
   - OnClick(): Add test method to give honor
3. Create UI → Button "ResetButton"
   - OnClick(): Call PlayerPresenter.ResetProgress()

### Test Methods (Add to GameManager):
```csharp
public void TestAddRice()
{
    playerModel.AddRice(100);
}

public void TestAddHonor()
{
    playerModel.AddHonor(10);
}
```

## 8. Font Setup for Korean Text

1. Import a Korean-supporting font (e.g., Noto Sans KR)
2. Create TextMeshPro Font Asset:
   - Window → TextMeshPro → Font Asset Creator
   - Source Font: [Your Korean font]
   - Character Set: Unicode Range (Hex)
   - Character Sequence: 0020-007F,AC00-D7AF
   - Generate Font Atlas

## 9. Build Settings

### Player Settings:
1. File → Build Settings → Player Settings
2. Other Settings:
   - Api Compatibility Level: .NET Standard 2.1
3. Publishing Settings:
   - Write Permission: External (SDCard) for save files

## 10. Prefab Creation

1. Select the Player GameObject
2. Drag to Project window to create prefab
3. Name it "PlayerSystem"
4. Do the same for the UI Canvas → "GameUI"

## Common Issues and Solutions

### Issue: Scripts not compiling
- Solution: Ensure all using statements are correct
- Check namespace declarations match

### Issue: UI not displaying Korean text
- Solution: Use TextMeshPro with Korean font
- Ensure font atlas includes Korean character range

### Issue: Save/Load not working
- Solution: Check Application.persistentDataPath permissions
- Test in actual device, not just editor

### Issue: Tap not registering
- Solution: Ensure TapArea Button covers entire middle section
- Check EventSystem exists in scene

## Final Checklist

- [ ] ClassRequirements ScriptableObject created and configured
- [ ] Player GameObject has all 3 components attached
- [ ] PlayerView has all UI references assigned
- [ ] PlayerStatus has ClassRequirements assigned
- [ ] GameManager initializes PlayerPresenter
- [ ] UI Canvas configured for portrait mobile
- [ ] TapArea button connected to OnTap method
- [ ] Korean fonts properly set up
- [ ] Save/Load tested on target platform