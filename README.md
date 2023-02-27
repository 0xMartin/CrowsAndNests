![CrowsAndNests](https://socialify.git.ci/0xMartin/CrowsAndNests/image?description=1&font=Jost&forks=1&issues=1&language=1&name=1&owner=1&pattern=Brick%20Wall&pulls=1&stargazers=1&theme=Light)

<div>
  <img alt="Made with Unity" src="https://img.shields.io/badge/Made%20with-Unity-57b9d3.svg?style=flat&logo=unity">
  <img alt="GitHub release (latest SemVer)" src="https://img.shields.io/github/v/release/0xMartin/CrowsAndNests">
  <img alt="GitHub all releases" src="https://img.shields.io/github/downloads/0xMartin/CrowsAndNests/total">
  <img alt="GitHub repo size" src="https://img.shields.io/github/repo-size/0xMartin/CrowsAndNests">
</div>

> Unity version: 2021.3.9f1

## Preview of current version

<img src="./Doc/preview.gif" width="100%" alt="Video záznam ze hry">

<img src="./Doc/preview.png">

## Game structure

<img src="./Doc/main_structure.png">

## TODO
- [ ] Herni postava
  - [X] Pohybove animace
  - [ ] Vyladit pohybovou fyziku (opravit big zpetneho odrazu hned pri vyskoku z hnizda)
  - [ ] Vytvorit skiny
- [ ] Kamera
  - [ ] Limity pohybu v ose Y (kamera se nesmi dostat pod mapu pokud hrac pada dolu)
  - [ ] Treseni kamery pri ruznych efektech
- [ ] Design
  - [ ] Vytvorit ikonu hry
  - [ ] Vytvorit baner hry (nejlip primo vyrenderovat s modelu hry v blenderu + postprodukce)
- [ ] Menu
  - [ ] 3D scena v pozadi menu
  - [ ] Styl tlacitek a textu
  - [ ] Settings menu
  - [X] Main menu
  - [ ] Game settings menu
  - [ ] Game lobby menu
  - [ ] Connect to host menu
- [ ] Zvukove efekty
  - [ ] Menu
    - [ ] Zvuk stiknuti tlacika
    - [ ] Zvuk pripojeni do lobby hry
    - [ ] Zvuk spusteni hry
  - [ ] Vrana
    - [ ] Chuze
    - [ ] Vyskok
    - [ ] Dopad
    - [ ] Utok
  - [ ] Vejce
    - [ ] Rozbiti
  - [ ] Hnizdo
    - [ ] Efekt zniceni "pokud hnizdno zmizi z areny / rozbije ho nejaka herni udalost"
  - [ ] Globalni
    - [ ] Zvuk vitezstvi
    - [ ] Zvuk porazky
    - [ ] Zvuk smrti "pokud vrana vypadne z areny"
    - [ ] Odpocet "tick hodin"
  - [ ] Soundtrack
    - [ ] Menu
    - [X] Arena - soundtrack1 
      - ? https://www.youtube.com/watch?v=FNZMAlO_gi4&list=PLlHY6e7WCV6NaJ5iNMoAWSEasT83GOaDo&index=5&ab_channel=GamesMusic
    - [ ] Arena - soundtrack2
      - ? https://www.youtube.com/watch?v=OA5oLdbvoLc&ab_channel=IsaacMoring
    - [ ] Arena - soundtrack3
- [ ] Efekty sceny
  - [X] Pridani mlhy
  - [ ] Particly v prostredi
  - [X] Nocni scena
  - [ ] Prizpusobyt osvetleni
  - [X] Skybox (noc/vesmir/...)
- [X] Postprocessing
  - [X] Vyladeni barev obrazu
- [ ] Skript pro rizeni areny
  - [X] Zakladni struktura
  - [X] Vstupy skriptu
  - [ ] Hlavni herni smycka
  - [X] Respawn handler
  - [X] Spawn handler
  - [ ] Manazer hernich modu
  - [ ] Manazer skore
  - [X] Cleaning manager
- [ ] Mini Hry
  - [ ] Fall guy Perfect match
  - [ ] Bitva o vejce
  - [ ] Padajici predmety
  - [ ] ?? napady na dalsi hry
- [ ] Multiplayer LAN
  - [ ] Lokalni server
  - [ ] Klient
  - [ ] Synchronizace dat
  - [ ] Komponanta pro prenos dat
  - [ ] Automatizovany vyhledavac serveru v lokalni siti
  
### Dodatecne vylepseni: 
* zlepsit vzhled modelu
* vytvorit vlastni soundtrack
* vytvorit vlastno font pisma pro hru
* vytvorit trailer video ke hre
* publikace na steam
* moznost hrani na verejnem serveru
