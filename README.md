# GhostSpectator

## Commands

| Command | Description |
|-----------|---------------|
|.specmode|Toggles between the regular spectator mode, and ghost spectator mode|

*All commands are for the console, and not RemoteAdmin.*

## Settings

| Setting Key      | Value Type | Default Value                                                                                                                                                                                                                                                                                         | Description                                     |
|------------------|------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------|
|gs_debug			|boolean	|false		|Enables debug mode.																|
|gs_enable			|boolean	|true		|Enables or disables the plugin.													|
|gs_allow_damage	|boolean	|false		|Allows the spectators to do damage.												|
|gs_allow_pickup	|boolean	|false		|Allows the spectators to pick items up.											|
|gs_god_mode		|boolean	|true		|Enables god mode for the spectators.												|
|gs_interact		|boolean	|false		|Allows the spectators to interact with the map.									|
|gs_noclip			|boolean	|true		|Enables noclip for the spectators.													|
|gs_default_mode	|string		|"Normal"	|Sets the default spectating mode.													|
|gs_language		|string		|"en-US"	|Sets the language to be used. Language files are located in the plugin folder.		|
|gs_ghost_message	|string		|"You have been spawned as a spectator ghost.\n Drop your <color=#ff0000>7.62</color> to be <color=#ff0000>teleported</color> to the <color=#ff0000>next</color> player\n  Drop your <color=#ff0000>5.56</color> to be <color=#ff0000>teleported</color> to the <color=#ff0000>previous</color> player"|The messages displayed to spectators.|
|gs_spec_message	|string		|"This server is using <color=#ff0000>GhostSpectator</color>\nTo enable ghost mode, open your console and type <color=#ff0000>.specmode</color>"|The message displayed to spectators in normal spectator mode.|

### Spectator modes
```
Normal
Ghost
```

## Tanslations
Translation names are used in the ISO format, meaning translations can be picked by language and country.
Examples:
```json
en 			// default English
en-US 		// American Egnlish if available, otherwise default English
en-UK		// British English if available, otherwise default Enlish
```
If you make custom translation files for languages that aren't included by default, I'd be more than happy to include them in future updates if you will share them.

Current default languages:
```yml
en
da
```

## Todo
* ~~Spectator type toggle~~
* ~~No nuke interact~~
* ~~No weapon manager interact~~
* ~~Don't show door disallows~~
* ~~Add noclip~~
* ~~Fix spawn with items~~
* ~~No 914 interact~~
* ~~Fix elevators (Possibly add elevator teleport instead of activation)~~
* ~~Intercom~~
* ~~Lockers~~
* ~~Medical closets~~
* ~~Translations~~
