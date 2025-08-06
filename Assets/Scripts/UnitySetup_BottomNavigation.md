# Unity Setup Guide - Bottom Navigation System

## Overview
This guide shows how to set up the Bottom Navigation UI system with 4 tabs: Upgrade, Honor, Class, and Shop.

## Step 1: Create the Bottom UI Structure

### 1.1 Modify Existing Canvas Hierarchy
```
Canvas (existing)
├── TopHUD (existing)
├── MiddleVisualStage (existing)
└── BottomInteractionZone (MODIFY THIS)
    ├── ContentPanel_Container (Empty GameObject)
    │   ├── UpgradePanel (GameObject)
    │   ├── HonorPanel (GameObject) 
    │   ├── ClassPanel (GameObject)
    │   └── ShopPanel (GameObject)
    └── Tab_Container (GameObject with Horizontal Layout Group)
        ├── UpgradeTabButton (Button)
        ├── HonorTabButton (Button)
        ├── ClassTabButton (Button)
        └── ShopTabButton (Button)
```

## Step 2: Configure BottomInteractionZone

### 2.1 BottomInteractionZone Setup
```
Inspector (BottomInteractionZone):
┌─────────────────────────────────────┐
│ Rect Transform                      │
│   Anchors: Bottom Stretch           │
│   Left: 0, Right: 0                 │
│   Bottom: 0, Top: -400              │ <- 400px height
│ ─────────────────────────────────── │
│ Vertical Layout Group               │
│   Spacing: 10                       │
│   Child Alignment: Lower Center     │
│   Child Control Size: ☑ Width       │
│   Child Force Expand: ☑ Width       │
└─────────────────────────────────────┘
```

### 2.2 ContentPanel_Container Setup
```
Inspector (ContentPanel_Container):
┌─────────────────────────────────────┐
│ Rect Transform                      │
│   Height: 350px                     │
│ ─────────────────────────────────── │
│ Layout Element                      │
│   Flexible Height: 1               │
└─────────────────────────────────────┘
```

### 2.3 Tab_Container Setup
```
Inspector (Tab_Container):
┌─────────────────────────────────────┐
│ Rect Transform                      │
│   Height: 60px                      │
│ ─────────────────────────────────── │
│ Horizontal Layout Group             │
│   Spacing: 5                        │
│   Child Force Expand: ☑ Width       │
│   Child Force Expand: ☑ Height      │
│ ─────────────────────────────────── │
│ Layout Element                      │
│   Min Height: 60                    │
│   Preferred Height: 60              │
└─────────────────────────────────────┘
```

## Step 3: Create Tab Buttons

### 3.1 Create Four Tab Buttons
For each tab button (Upgrade, Honor, Class, Shop):

```
Inspector (UpgradeTabButton):
┌─────────────────────────────────────┐
│ Button                              │
│   Transition: Color Tint            │
│   Normal Color: Gray                │
│   Highlighted: Light Gray           │
│   Pressed: White                    │
│   Selected: White                   │
│ ─────────────────────────────────── │
│ Image                               │
│   Source Image: [Tab Background]    │
│ ─────────────────────────────────── │
│ Text (Child)                        │
│   Text: "강화"                      │
│   Font Size: 18                     │
│   Alignment: Center                 │
└─────────────────────────────────────┘
```

### 3.2 Tab Button Texts
- UpgradeTabButton: "강화" (Upgrade)
- HonorTabButton: "명예" (Honor)  
- ClassTabButton: "승급" (Class)
- ShopTabButton: "상점" (Shop)

## Step 4: Create Content Panels

### 4.1 UpgradePanel Setup
```
UpgradePanel:
├── Header (TextMeshPro) - "업그레이드"
├── ScrollView
│   └── Viewport
│       └── Content (Vertical Layout Group)
│           ├── ClickUpgradesHeader (TextMeshPro)
│           ├── ClickUpgradesContainer (Vertical Layout Group)
│           ├── ProductionUpgradesHeader (TextMeshPro)
│           └── ProductionUpgradesContainer (Vertical Layout Group)
```

### 4.2 HonorPanel Setup
```
HonorPanel:
├── Header (TextMeshPro) - "명예 활동"
├── CurrentStatus (Horizontal Layout Group)
│   ├── CurrentHonorText (TextMeshPro)
│   └── HonorPerSecondText (TextMeshPro)
└── ScrollView
    └── Viewport
        └── Content (Vertical Layout Group)
            ├── BuildingsHeader (TextMeshPro)
            ├── BuildingsContainer (Vertical Layout Group)
            ├── ActivitiesHeader (TextMeshPro)
            └── ActivitiesContainer (Vertical Layout Group)
```

### 4.3 ClassPanel Setup
```
ClassPanel:
├── Header (TextMeshPro) - "신분 승급"
├── CurrentStatus (Vertical Layout Group)
│   ├── CurrentClassText (TextMeshPro)
│   ├── CurrentHonorText (TextMeshPro)
│   └── NextClassText (TextMeshPro)
├── ProgressSection (Vertical Layout Group)
│   ├── RequiredHonorText (TextMeshPro)
│   ├── ProgressSlider (Slider)
│   ├── ProgressText (TextMeshPro)
│   └── TimeToAscensionText (TextMeshPro)
├── AscendButton (Button)
└── BenefitsScrollView
    └── BenefitsText (TextMeshPro)
```

### 4.4 ShopPanel Setup
```
ShopPanel:
├── Header (TextMeshPro) - "상점"
└── ComingSoonPanel (GameObject)
    └── ComingSoonText (TextMeshPro)
```

## Step 5: Create Prefabs

### 5.1 UpgradeItem Prefab
Create `UpgradeItem_Prefab`:
```
UpgradeItem_Prefab:
├── Background (Image)
├── IconImage (Image)
├── NameText (TextMeshPro)
├── LevelText (TextMeshPro)
├── DescriptionText (TextMeshPro)
├── CostText (TextMeshPro)
├── EffectText (TextMeshPro)
└── PurchaseButton (Button)
    └── ButtonText (TextMeshPro)
```

### 5.2 HonorItem Prefab
Create `HonorItem_Prefab`:
```
HonorItem_Prefab:
├── Background (Image)
├── NameText (TextMeshPro)
├── DescriptionText (TextMeshPro)
├── CostText (TextMeshPro)
├── EffectText (TextMeshPro)
└── ActionButton (Button)
    └── ButtonText (TextMeshPro)
```

### 5.3 ClassNode Prefab
Create `ClassNode_Prefab`:
```
ClassNode_Prefab:
├── BackgroundImage (Image)
├── IconImage (Image)
├── ClassNameText (TextMeshPro)
├── CurrentIndicator (GameObject)
├── CompletedIndicator (GameObject)
└── LockedIndicator (GameObject)
```

## Step 6: Create ScriptableObjects

### 6.1 Create UpgradeData Asset
1. Right-click in Project → Create → RoyalRoad → Upgrade Data
2. Name: "UpgradeData_Default"
3. Configure sample upgrades:

```
Click Upgrades:
┌─────────────────────────────────────┐
│ Element 0                           │
│   Id: "hoe"                         │
│   Display Name: "괭이"              │
│   Description: "기본적인 농기구"     │
│   Base Cost: 10                     │
│   Base Effect: 1                    │
│   Upgrade Type: Click Upgrade       │
└─────────────────────────────────────┘

Production Upgrades:
┌─────────────────────────────────────┐
│ Element 0                           │
│   Id: "cow"                         │
│   Display Name: "소 구입"           │
│   Description: "소가 농사를 도와줍니다"│
│   Base Cost: 50                     │
│   Base Effect: 2                    │
│   Upgrade Type: Production Upgrade  │
└─────────────────────────────────────┘
```

## Step 7: Add Script Components

### 7.1 BottomInteractionZone Scripts
```
Inspector (BottomInteractionZone):
┌─────────────────────────────────────┐
│ Bottom Nav Presenter (Script)       │
│ ─────────────────────────────────── │
│ Tab Buttons                         │
│   Upgrade Tab Button: ⊡ → [Button] │
│   Honor Tab Button: ⊡ → [Button]   │
│   Class Tab Button: ⊡ → [Button]   │
│   Shop Tab Button: ⊡ → [Button]    │
│ ─────────────────────────────────── │
│ Content Panels                      │
│   Upgrade Panel: ⊡ → [GameObject]  │
│   Honor Panel: ⊡ → [GameObject]    │
│   Class Panel: ⊡ → [GameObject]    │
│   Shop Panel: ⊡ → [GameObject]     │
└─────────────────────────────────────┘
```

### 7.2 UpgradePanel Scripts
```
Inspector (UpgradePanel):
┌─────────────────────────────────────┐
│ Upgrade Panel (Script)              │
│ ─────────────────────────────────── │
│ UI References                       │
│   Scroll Rect: ⊡ → [ScrollRect]    │
│   Click Container: ⊡ → [Transform] │
│   Production Container: ⊡ → [...]  │
│   Upgrade Item Prefab: ⊡ → [Prefab]│
│ ─────────────────────────────────── │
│ Data                                │
│   Upgrade Data: ⊡ → [ScriptableObj]│
└─────────────────────────────────────┘
```

### 7.3 Create UpgradeManager GameObject
```
Hierarchy:
RoyalRoadClicker
├── Managers
│   ├── GameManager
│   └── UpgradeManager (New)

Inspector (UpgradeManager):
┌─────────────────────────────────────┐
│ Upgrade Manager (Script)            │
│ ─────────────────────────────────── │
│ Data                                │
│   Upgrade Data: ⊡ → [Same Asset]   │
└─────────────────────────────────────┘
```

## Step 8: Testing the System

### 8.1 Enter Play Mode Test
1. **Press Play**
2. **Click tab buttons** - should switch between panels
3. **Default tab** should be Upgrade
4. **Console** should show "Switched from X to Y tab"

### 8.2 Upgrade System Test
1. **Use GameManager context menu** → "Add Debug Rice"
2. **Switch to Upgrade tab**
3. **Click purchase button** on upgrade item
4. **Check Console** for purchase messages
5. **Rice should decrease**, **production should increase**

### 8.3 Honor System Test
1. **Get some rice** (100+ recommended)
2. **Switch to Honor tab**  
3. **Click on honor building** or activity
4. **Honor or Honor/s should increase**

### 8.4 Class System Test
1. **Add honor** via GameManager debug methods
2. **Switch to Class tab**
3. **Progress bar** should show advancement
4. **Ascend button** should activate when ready

## Step 9: Visual Polish

### 9.1 Tab Button Polish
- Add icons to tab buttons
- Use consistent colors (active/inactive)
- Add subtle animations

### 9.2 Panel Polish
- Add background images
- Use consistent spacing
- Add loading states

### 9.3 Mobile Optimization
- Test on 9:16 aspect ratio
- Ensure buttons are finger-sized (44x44 minimum)
- Test scrolling performance

## Final Scene Hierarchy
```
┌─────────────────────────────────────┐
│ Canvas                              │
│ ├── TopHUD                         │
│ ├── MiddleVisualStage              │
│ └── BottomInteractionZone          │
│     ├── ContentPanel_Container     │
│     │   ├── UpgradePanel           │
│     │   ├── HonorPanel             │
│     │   ├── ClassPanel             │
│     │   └── ShopPanel              │
│     └── Tab_Container              │
│         ├── UpgradeTabButton       │
│         ├── HonorTabButton         │
│         ├── ClassTabButton         │
│         └── ShopTabButton          │
└─────────────────────────────────────┘
```

## Debug Commands Available
- GameManager → "Add Debug Rice"
- GameManager → "Add Debug Honor"  
- GameManager → "Set Production Rates"
- UpgradeManager → "Force Recalculate Stats"
- UpgradeManager → "Log All Upgrade Levels"

The system is now ready for testing and further development!