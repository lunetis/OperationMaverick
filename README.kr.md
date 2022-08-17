# Operation Maverick

[[English Readme]](https://github.com/lunetis/OperationMaverick/blob/main/README.md)

탑건: 매버릭의 미션을 유니티 엔진으로 구현하는 프로젝트입니다.

- __Editor version: 2020.2.3f1__
- __프로젝트를 확인하기 위해서는 [Blender](https://www.blender.org/download/)를 설치해주세요.__

<br>

* [이전 프로젝트](https://github.com/lunetis/OperationZERO)를 기반으로 합니다.
* 개인 프로젝트이며 현재 개발중입니다.
* 듀얼쇼크 4 컨트롤러 (플레이스테이션 4 전용 컨트롤러)를 사용해서 프로젝트를 진행하고 있기 때문에, XBOX 컨트롤러를 포함한 다른 컨트롤러가 제대로 지원되는지는 아직 확인하지 못했습니다.
* 에셋의 라이센스 문제로 인해, 때때로 커밋 내역이 초기화될 수 있습니다.
* 이 프로젝트에 사용된 에셋은 [LICENSE.md](https://github.com/lunetis/OperationMaverick/blob/main/LICENSE.md) 파일을 확인해주세요.

### TODO List:
- [x] F/A-18로 플레이어 전투기 모델링 변경
- [x] 레이저 유도 폭탄 구현
- [x] 지대공 미사일(Surface-to-Air Missile, SAM) 구현
- [x] 맵 디자인
- [x] 미션 구현
- [x] 대사 추가
- [x] 메인/결과 화면
- [ ] 난이도 추가
- [ ] 빌드

<br>

- [ ] 미션 컷씬 추가 (선택)

*이 리스트는 개발자 마음대로 바뀔 수 있습니다.*

devlog : https://velog.io/@lunetis/series/maverick

---

### 참고: Missing prefab 에러 해결 방법

![](https://github.com/lunetis/AceCombatZERO/blob/main/missingerror.PNG)

Blender가 설치되어 있지 않은 상태에서 프로젝트를 열 때 위와 같은 에러가 발생할 수 있습니다.

[Blender](https://www.blender.org/download/)를 설치한 후, Unity 에디터의 상단 메뉴에서 Assets - Reimport All 을 실행해주세요.

(한 번 에러가 발생한 상태에서는 단순 설치만으로 에러가 해결되지 않으며, 에셋들을 다시 임포트해야 합니다.)

<br>

프로젝트를 최초로 열 때 Blender가 이미 설치되어 있는 상태라면 위 에러가 발생하지 않습니다.
