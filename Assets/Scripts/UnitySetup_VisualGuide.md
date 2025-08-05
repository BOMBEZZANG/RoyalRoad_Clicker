# Unity Visual Setup Guide - Step by Step

## Step 1: Create Base Scene Structure

### 1.1 Create Root GameObjects
```
Hierarchy Window:
┌─────────────────────────┐
│ SampleScene             │
│ ├── RoyalRoadClicker    │  <- Create Empty (Position: 0,0,0)
│ ├── EventSystem         │  <- Auto-created with Canvas
│ └── Main Camera         │  <- Default camera
└─────────────────────────┘
```

### 1.2 Create Manager Structure
```
Right-click RoyalRoadClicker → Create Empty → Name it "Managers"
┌─────────────────────────┐
│ RoyalRoadClicker        │
│ └── Managers            │
│     └── GameManager     │  <- Create Empty inside Managers
└─────────────────────────┘
```

## Step 2: Create Player GameObject

### 2.1 Create Player Object
```
Right-click RoyalRoadClicker → Create Empty → Name it "Player"
┌─────────────────────────┐
│ RoyalRoadClicker        │
│ ├── Managers            │
│ │   └── GameManager     │
│ └── Player              │  <- New GameObject
└─────────────────────────┘
```

### 2.2 Add Components to Player
```
Inspector Window (Player Selected):
┌─────────────────────────────────────┐
│ Player                              │
│ ─────────────────────────────────── │
│ Transform                           │
│   Position: (0, 0, 0)               │
│   Rotation: (0, 0, 0)               │
│   Scale: (1, 1, 1)                  │
│ ─────────────────────────────────── │
│ [Add Component]                     │
│   → Scripts → Gameplay → Player →   │
│     • PlayerPresenter               │
│     • PlayerView                    │
│     • PlayerStatus                  │
└─────────────────────────────────────┘
```

## Step 3: Create UI Canvas

### 3.1 Create Canvas
```
Right-click Hierarchy → UI → Canvas
┌─────────────────────────┐
│ RoyalRoadClicker        │
│ ├── Managers            │
│ ├── Player              │
│ └── Canvas              │  <- New UI Canvas
└─────────────────────────┘
```

### 3.2 Configure Canvas for Mobile Portrait
```
Inspector (Canvas):
┌─────────────────────────────────────┐
│ Canvas                              │
│ ─────────────────────────────────── │
│ Canvas                              │
│   Render Mode: Screen Space-Overlay │
│ ─────────────────────────────────── │
│ Canvas Scaler                       │
│   UI Scale Mode: Scale With Screen  │
│   Reference Resolution:             │
│     X: 1080  Y: 1920               │
│   Screen Match Mode: 0.5            │
└─────────────────────────────────────┘
```

## Step 4: Create UI Structure

### 4.1 Create Top HUD
```
Canvas Structure:
├── Canvas
│   ├── TopHUD (Empty GameObject)
│   │   ├── ResourcePanel
│   │   └── ProductionPanel
```

### 4.2 TopHUD Setup
```
TopHUD RectTransform:
┌─────────────────────────────────────┐
│ Anchors: Top Stretch                │
│ ┌─────────────────────────────────┐ │
│ │          TOP HUD AREA            │ │
│ │ Height: 200                      │ │
│ └─────────────────────────────────┘ │
│                                     │
│          [Middle Area]              │
│                                     │
│          [Bottom Area]              │
└─────────────────────────────────────┘
```

### 4.3 Create Resource Display
```
ResourcePanel → Right-click → UI → Text - TextMeshPro
├── ResourcePanel
│   ├── RiceText (TextMeshPro)
│   ├── HonorText (TextMeshPro)
│   └── ClassNameText (TextMeshPro)
```

## Step 5: Create Middle Visual Stage

### 5.1 Create Visual Area
```
Canvas:
├── TopHUD
├── MiddleVisualStage (Empty GameObject)
│   ├── BackgroundImage (UI → Image)
│   ├── CharacterImage (UI → Image)
│   └── TapArea (UI → Button)
```

### 5.2 Configure Tap Area
```
TapArea Button Setup:
┌─────────────────────────────────────┐
│ TapArea                             │
│ ─────────────────────────────────── │
│ Rect Transform                      │
│   Anchors: Stretch All              │
│   Left: 0, Right: 0                 │
│   Top: 0, Bottom: 0                 │
│ ─────────────────────────────────── │
│ Image                               │
│   Color: (255,255,255,0)            │
│ ─────────────────────────────────── │
│ Button                              │
│   Transition: None                  │
│   OnClick():                        │
│     [+] → Player → PlayerPresenter  │
│         → OnTap()                   │
└─────────────────────────────────────┘
```

## Step 6: Connect PlayerView References

### 6.1 PlayerView Inspector Setup
```
Inspector (Player GameObject):
┌─────────────────────────────────────┐
│ Player View (Script)                │
│ ─────────────────────────────────── │
│ Resource Displays                   │
│   Rice Text: ⊡ → [RiceText]        │
│   Honor Text: ⊡ → [HonorText]      │
│   Rice Per Second: ⊡ → [RpsText]   │
│   Honor Per Second: ⊡ → [HpsText]  │
│   Class Name Text: ⊡ → [ClassText] │
│ ─────────────────────────────────── │
│ Class Display                       │
│   Character Image: ⊡ → [CharImage] │
│   Background Image: ⊡ → [BgImage]  │
└─────────────────────────────────────┘
```

## Step 7: Create ClassRequirements Asset

### 7.1 Create ScriptableObject
```
Project Window:
1. Right-click in Assets folder
2. Create → RoyalRoad → Class Requirements
3. Name: "ClassRequirements_Default"
```

### 7.2 Configure in Inspector
```
Inspector (ClassRequirements_Default):
┌─────────────────────────────────────┐
│ Class Requirements                  │
│ ─────────────────────────────────── │
│ Requirements                        │
│   Size: 5                           │
│ ─────────────────────────────────── │
│ Element 0                           │
│   Target Class: Tenant Farmer      │
│   Honor Required: 100              │
│   Rice Multiplier: 1.5             │
│ ─────────────────────────────────── │
│ Element 1                           │
│   Target Class: Commoner           │
│   Honor Required: 1000             │
│   Rice Multiplier: 2               │
└─────────────────────────────────────┘
```

## Step 8: Configure GameManager

### 8.1 Add GameManager Script
```
Inspector (GameManager GameObject):
┌─────────────────────────────────────┐
│ Game Manager (Script)               │
│ ─────────────────────────────────── │
│ Core References                     │
│   Player Presenter: ⊡ → [Player]   │
│ ─────────────────────────────────── │
│ Game Settings                       │
│   ☑ Load Save On Start             │
│   ☐ Debug Mode                     │
└─────────────────────────────────────┘
```

## Step 9: Configure PlayerStatus

### 9.1 Assign ClassRequirements
```
Inspector (Player → PlayerStatus):
┌─────────────────────────────────────┐
│ Player Status (Script)              │
│ ─────────────────────────────────── │
│ Class Requirements: ⊡ →             │
│   [ClassRequirements_Default]       │
│ ─────────────────────────────────── │
│ Validation Settings                 │
│   Max Resource Cap: 1e+15          │
│   Max Production Rate: 1e+12       │
└─────────────────────────────────────┘
```

## Step 10: Test Setup

### 10.1 Enter Play Mode Checklist
```
Before pressing Play:
☑ GameManager has Player reference
☑ Player has all 3 components
☑ PlayerView has UI references
☑ PlayerStatus has ClassRequirements
☑ TapArea Button → OnClick → Player.OnTap()
☑ Canvas set to portrait (1080x1920)
```

### 10.2 Runtime Debug
```
During Play Mode:
1. Click in middle area → Rice should increase
2. Check Console for any errors
3. Game Manager → Context Menu → Debug methods
   - Add Debug Rice
   - Add Debug Honor
   - Force Class Ascension
```

## Common Setup Mistakes to Avoid

### ❌ Missing References
```
NullReferenceException: Object reference not set
→ Check all ⊡ fields in Inspector are assigned
```

### ❌ Wrong Canvas Setup
```
UI not scaling properly on mobile
→ Canvas Scaler must be "Scale With Screen Size"
→ Reference Resolution: 1080x1920
```

### ❌ Tap Not Working
```
Taps not registering
→ Check TapArea has Button component
→ Check EventSystem exists in scene
→ TapArea Image alpha = 0 (transparent)
```

### ❌ Korean Text Not Showing
```
Text shows □□□ instead of Korean
→ Use TextMeshPro, not legacy Text
→ Import Korean font (Noto Sans KR)
→ Create TMP font with Korean range
```

## Final Scene Hierarchy
```
┌─────────────────────────────────────┐
│ SampleScene                         │
│ ├── RoyalRoadClicker               │
│ │   ├── Managers                   │
│ │   │   └── GameManager            │
│ │   └── Player                     │
│ ├── Canvas                         │
│ │   ├── TopHUD                     │
│ │   │   ├── ResourcePanel          │
│ │   │   │   ├── RiceText           │
│ │   │   │   ├── HonorText          │
│ │   │   │   └── ClassNameText      │
│ │   │   └── ProductionPanel        │
│ │   │       ├── RicePerSecondText  │
│ │   │       └── HonorPerSecondText │
│ │   ├── MiddleVisualStage          │
│ │   │   ├── BackgroundImage        │
│ │   │   ├── CharacterImage         │
│ │   │   └── TapArea                │
│ │   └── BottomInteractionZone      │
│ ├── EventSystem                    │
│ └── Main Camera                    │
└─────────────────────────────────────┘
```