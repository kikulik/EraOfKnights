# Unity Client

This Unity project targets Unity 2022 LTS (2D) with the new Input System.

## Packages

- TextMeshPro (install via Unity Package Manager when prompted)
- Colyseus Unity SDK (add via Package Manager or git URL)

## Scenes

- `Boot.unity`: GPS permission gate
- `Connect.unity`: name/faction selection and server connect
- `World.unity`: POI list and map placeholder
- `Instance.unity`: multiplayer instance with movement

## Pixel Look Notes

- Set the main camera to Orthographic.
- Add the Pixel Perfect Camera component (2D Pixel Perfect package) if desired.
- Keep sprites at 16x16 with `Filter Mode = Point`.

## Configuration

Edit `Assets/Config/AppConfig.json` to change server endpoint, GPS requirements, and simulated GPS.

## Scene Setup TODOs

The scenes are placeholders; wire the following in Unity:

- Boot: Canvas with TMP text for status, and a blocking panel for GPS denied.
- Connect: TMP input field for name, dropdown for faction, button wired to `ConnectUIController.OnConnectPressed`.
- World: TMP text list for POIs, optional buttons to call `WorldUIController.SelectPoiByIndex`.
- Instance: Sprite renderer prefab using `PixelCharacterView`, and TMP text for status/chat.
