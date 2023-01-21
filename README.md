### Loot Multiplier for Valheim

This is a rebirth of Castix old Loot Multiplier mod.  Redone with permission from Castix  (SoCastix)      
He no longer had the source and gave me permission to rebuild it and release to the gaming community.             

This is mod multiplies loot drops when you destroy, kill or pick up something. Drop multiplier can be configured.


Installation (manual)                                                                                       

If you are installing this manually, do the following
1. Extract the archive into a folder. **Do not extract into the game folder.**
2. Move the contents of `plugins` folder into `<GameDirectory>\Bepinex\plugins`.
3. Run the game.
4. To change the drop rate, use the config at \BepInEx\config\castix_LootMultiplier.cfg


How to set up the custom whitelist

1. Enable the whitelist in the Loot Multiplier config.
2. Open the whitelist.txt file. It always has to be in the same directory as the plugin .dll (example: <GameDirectory>\BepInEx\plugins\LootMultiplier\whitelist.txt)
3. Add items to the list by writing the Prefab name of the item you want. One per line.
4. If the whitelist is enabled, only the allowed items inside it will be multiplied.

Prefab names list:
Item List from Reddit
Item List from Modding Wiki﻿﻿


﻿


Configuration                                                     

## Settings file was created by plugin LootMultiplier v1.0.8
## Plugin GUID: castix_LootMultiplier

[General]

## Material Multiplier
# Setting type: Int32
# Default value: 1
Multiplier for resources = 1

##  Monster Drop Multiplier
# Setting type: Int32
# Default value: 1
Multiplier for monster drops = 1

## Pickup Multiplier
# Setting type: Int32
# Default value: 1
Multiplier for pickable objects = 1

[Whitelist]

## Whitelist
# Setting type: Boolean
# Default value: false
Enable whitelist filter = false