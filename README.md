### Archer's Retirement Garden
Image

## About Game
***Archer's Retirement Garden*** is a 2.5D game your are a Archer that retire from the Hero Party and buys a floating island to build the garden. 
The mysterious seed he brought back turns out to be a sapling of the Life Tree, quietly drawing swarms of dark monsters every night. 
By day, plant and grow your own garden on your floating island. 
By night, defend the Life Tree from waves of monster attacks, fight, strategize, and grow stronger with every night you survive.

## Key Features
1. ***Planting / Placement Garden*** : <br> Select, Drag, and confirm of the plants and garden object.
2. ***Skill & Progression System*** : <br> Win the night mode and get the earn status upgrage or purchase new skills.
3. ***Dynamic Economy Changes*** : <br> Plant and upgrade prices will scale dynamically with each purchase.
4. ***Custom Dialogue System*** : <br> Features custom dialogue scripts with typing effects powered by Text Animator (Febucci).

## Contribution (AndhikaAtmaja)
- Created most of the code for game systems and features.
- Created most UI and Scene in game.

## Layer / Module Design

## Modules and Features
| 📂 Name | 🎬 Scene | 📋 Responsibility |
|---------|----------|-------------------|
| Battle System   | Gameplay     | - Manage All Battle Stage <br>- Calculate Damage Player or Enemy|
| Currency System | Gameplay     | - Manage All Player Currency <br>- Broadcast Currency Changes|
| Day & Night System | Gameplay  | - Changing the sky and light between day and night cycle <br>- Changing Gameplay from planting to defending|
| Dialogue System | Gameplay     | Manage all dialogue |
| Grid System | Gameplay         | - Generate Grid <br>- Handle all location on grid <br>- Store Grid that has been used |
| Grid Place System | Gameplay   | - Manage and store plants that planted <br>- Handel all placement of the plant on grid |
| Input System | Gameplay        | - Manage and Handle all switching Action Map  <br>- Store previous ActionMap for overlay|
| Panel System | Gameplay        | - Handle all switching Show / Hide Panels |
| Pause System | Gameplay        | Handle pause game |
| Skill Card System   | Gameplay | - Handle All skill card UI   <br> - Manage Run Time Data skill card    <br> - Handle skill activation and usage |
| Upgrade Card System | Gameplay | - Handle All Upgrade card UI <br> - Manage Run Time Data Upgrade card  <br> - Broadcast selected upgrade card effect |
| Status System | Gameplay       | - Handle Player Status <br> - Broadcast Status Changes <br> - Handle Status Change |
| Health System | Gameplay       | - Handle Player Health <br> - Broadcast Health Changes |
| Spawener System | Gameplay     | - Handle Enemy Spawn <br> - Manage Run Time Data Enemy each Day |
| Character System | Gameplay    | - Handle Character/Enemy Logic |
| Transition System | Gameplay <br> MainMenu |   Handle All Transition UI and Scene |

## Game Flow

## Plugin / Unity Asset
Went developed this game we use some Plugin / Unity Asset for polishing and juicy
- Unity Package FEEL - More Mountains <br>
- Unity Package Text Animator - Febucci
