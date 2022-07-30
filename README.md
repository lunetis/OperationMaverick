# Operation Maverick

[[한국어 Readme]](https://github.com/lunetis/OperationMaverick/blob/main/README.kr.md)

### Fan-made Top Gun: Maverick mission with Unity (with ACE COMBAT 7 style UI)

- __Editor version: 2021.3.6f1__
- __You have to install [Blender](https://www.blender.org/download/) to explore this project.__

<br>

* Based on previous project: https://github.com/lunetis/OperationZERO
* This is a personal project and currently in development.
* Since I'm testing with DualShock 4 (PlayStation 4 Controller), I'm not sure if other gamepads are working properly in this project. (XBOX Controller, etc.)
* Due to asset's license issue, commit histories can be deleted sometimes. (In that case, commit count will be very low.)
* Please check [LICENSE.md](https://github.com/lunetis/OperationMaverick/blob/main/LICENSE.md) for other used assets.

### TODO List:
- [x] Change 3D Model to F/A-18
- [x] Laser-guided Bomb Implementation
- [ ] UI for multiple enemies
- [ ] Surface-to-Air Missile Implementation
- [ ] Map Design
- [ ] Mission Implementation
- [ ] Transcript
- [ ] Mission Cutscenes
- [ ] Difficulty System
- [ ] Main/Result Screen
- [ ] Build Test

<br>

|Difficulty|Time limit|Enemy missile tracking performance|# of Enemy aircrafts|etc.|
|------|---|---|---|---|
|Easy|4:30|Normal|3|No bridges at the canyon|
|Normal|3:30|Normal|5||
|Hard|3:00|High|7|Low Player HP|
|Maverick|2:15|High|10|Additional SAM|

<br>

*This list can be changed at anytime.*

<br>
<br>

devlog : https://velog.io/@lunetis/series/maverick (Korean language only)

---

### Note: Resolving missing prefab error

![](https://github.com/lunetis/OperationMaverick/blob/main/missingerror.PNG)

If you open the project when Blender is not installed, .blend files may cause missing prefab errors.

To resolve this, you have to install [Blender](https://www.blender.org/download/), and reimport all assets.

After installing Blender, go to Unity Editor Menu - Assets - Reimport All.

<br>

If you have installed blender before you open the project first time, no error occurs.
