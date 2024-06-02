# WizardingWorld
캐주얼 RPG 게임 [플레이 영상 링크](https://youtu.be/yKgXapyh_Bs)



 
## 🖥 프로젝트 소개
- PalWorld 게임을 모티브로 만든 게임.
- 각종 아이템과 몬스터를 이용해 전투에 활용할 수 있습니다.
- FSM과 상태 패턴을 이용해 각 캐릭터들의(플레이어, 일반 몬스터, 보스 몬스터) 다양한 상태를 정의하고 관리하는 시스템을 제공합니다.
- 플레이어 ↔ 일반 몬스터, 플레이어 ↔ 보스 몬스터, 일반 몬스터 ↔ 보스 몬스터, 일반 몬스터 ↔ 일반 몬스터 간의 상호작용과 전투 메커니즘을 제공합니다.
- 무기의 등록, 설정, 해제를 관리하며, 무기의 상태 확인할 수 있습니다.
- 아이템 추가, 드래그 앤 드롭을 통한 Swap 및 인벤토리 업데이트 기능을 제공합니다.
- 무기 장착 및 포획한 몬스터 장착 기능을 제공합니다.
- Scriptable Objects을 이용한 게임 데이터(예: 아이템, 몬스터, 무기 정보)를 효율적으로 관리하고 오브젝트 풀링을 이용한 재사용을 통해 관리했습니다. 



 
## 🛠 개발 환경
- 개인 프로젝트 
- 엔진 : Unity
- 버전 및 이슈관리 : Github

 
## 🔉 개발 기간
1개월

 
## 프로젝트 구조 
- 인벤토리에 담길 아이템 
  ![BaseObject 관련 다이어그램~](https://github.com/seoeunkong/WizardingWorld/assets/87869785/fbd1ee4e-ab59-42d1-bf7e-9071ebcdee21)


- 인벤토리에 담길 아이템 데이터
  ![ObjectData 관련 다이어그램](https://github.com/seoeunkong/WizardingWorld/assets/87869785/be33e9fd-962f-4f0f-944d-eebcda9c8a53)


- FSM 구현을 위한 상태 패턴
  ![basestate 관련 다이어그램](https://github.com/seoeunkong/WizardingWorld/assets/87869785/ccc65e33-f63c-4bf0-bbb5-f3407c9400d0)


- 플레이어와 몬스터 행동 관리
  ![CharacterController 다이어그램](https://github.com/seoeunkong/WizardingWorld/assets/87869785/c480c57b-8d95-4ac4-8b93-016b328171a7)


