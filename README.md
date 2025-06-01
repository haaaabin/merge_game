# 2D Merge Puzzle Game

<a name="readme-top"></a>
<p>
  머지 게임
</p>

![Title](https://github.com/user-attachments/assets/519993ae-1d0d-4e9c-86a3-5f4618475540)
<br/>

<!-- TABLE OF CONTENTS -->

## 목차

1. [프로젝트 개요](#Intro)
2. [게임 기능](#Features)
3. [게임 플레이](#Play)
4. [핵심 기능](#CoreFeatures)
<br/>

<a name="Intro"></a>
## 프로젝트 개요
- 프로젝트 기간 : 2025.05 ~ 진행 중
- 개발 엔진 및 언어 : Unity & C#
- 플랫폼 : 모바일

<br/>

<a name="Features"></a>
## 게임 기능
1. 에너지를 소모하여 아이템을 랜덤 생성합니다.
2. 같은 종류와 레벨의 아이템 2개를 병합 시 상위 레벨 아이템이 생성됩니다.
3. 주문 시스템과 연동하여 특정 아이템을 생성하면 보상을 지급합니다.
4. 특수 아이템(거울, 망치)을 사용하면 기존 아이템의 복제 및 하위 레벨 분할이 가능합니다.
<br/>

<a name="Play"></a>
## 게임 플레이

<h5> 아이템 랜덤 생성 & 병합 </h5>
<img src="https://github.com/user-attachments/assets/01c8e384-9141-465b-b0d8-6095d21f695d" width="300">
<br/>

<h5> 에너지 충전 </h5>
<img src="https://github.com/user-attachments/assets/0c2d9940-0f0e-40dd-92fa-98fd293eb65d" width="300">
<br/>

<h5> 거울 아이템 사용 </h5>
<img src="https://github.com/user-attachments/assets/8ec0040e-4a7f-49ba-9dd4-23c01adf122e" width="300">
<br/>

<h5> 망치 아이템 사용 </h5>
<img src="https://github.com/user-attachments/assets/71b4fe42-89c4-4155-afc4-54d8d4c91498" width="300">
<br/>


<a name="CoreFeatures"></a>
## 핵심 기능
1. **객체지향 설계 및 다형성**을 적용한 아이템 관리
    - [Item 추상 클래스](https://github.com/haaaabin/merge_game/blob/9f98fc2c820e91b93ac5e43b8b3b75eb492a251d/Assets/Scripts/Item/Item.cs#L5C1-L250C2) 설계를 통해 아이템 공통 동작(드래그, 슬롯 배치, 정보 표시 등)을 정의하고 코드 중복을 최소화했습니다.
    - 각 특수 아이템([거울](https://github.com/haaaabin/merge_game/blob/9f98fc2c820e91b93ac5e43b8b3b75eb492a251d/Assets/Scripts/Item/MirrorItem.cs#L4C1-L42C2), [망치](https://github.com/haaaabin/merge_game/blob/9f98fc2c820e91b93ac5e43b8b3b75eb492a251d/Assets/Scripts/Item/HammerItem.cs#L3-L40))은 **상속과 오버라이드**를 통해 개별 동작을 추가하여 유지보수성과 확장성을 높였습니다.
  
2. **마우스 인터랙션(드래그 앤 드롭)** 기능
    - OnMouseDown, OnMouseDrag, OnMouseUp 이벤트를 활용하여 아이템의 선택, 이동, 드롭 처리를 직관적이고 부드러운 사용자 경험을 제공하도록 구현했습니다.
  
3. **아이템 병합 & 주문 관리 시스템** 기능
    - 같은 레벨의 아이템을 합쳐서 합성시켜서 높은 레벨의 아이템을 생성하도록 구현하였습니다.
    - Queue와 HashSet을 활용해 중복 아이템 주문을 방지하였습니다.

<br/>

## 진행 예정 기능
1. 경험치 기반 레벨 업 기능
2. 시각적인 재미 요소를 더할 파티클 이펙트 및 애니메이션
3. PlayerPrefs / Json을 활용하여 게임 진행 상황 및 플레이어 상태를 저장하고, 게임 재시작 시 복원 가능하도록 구현
<br/>
