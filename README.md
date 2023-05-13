<div align="center">
  <img src="./Doc/logo1.png" width="25%">
  <h1>Crows and Nests</h1>
  <div>
    <img alt="Made with Unity" src="https://img.shields.io/badge/Made%20with-Unity-57b9d3.svg?style=flat&logo=unity">
    <img alt="GitHub release (latest SemVer)" src="https://img.shields.io/github/v/release/0xMartin/CrowsAndNests">
    <img alt="GitHub all releases" src="https://img.shields.io/github/downloads/0xMartin/CrowsAndNests/total">
    <img alt="GitHub repo size" src="https://img.shields.io/github/repo-size/0xMartin/CrowsAndNests">
  </div>
</div>

> Unity version: 2021.3.9f1
> Lowpoly game

## Preview of current version

<img src="./Doc/preview.png">

## Video

<img src="./Doc/preview.gif" width="100%" alt="Video záznam ze hry">

## Game structure

<img src="./Doc/main_structure.png">

## TODO
- [ ] Prostredi
  - [ ] Vymodelovat lepe vypadajici vez z hodinama (aktualni verze: nic moc obarveni, nakvalitni tvary, malo komplexni, nejlepe pouziti textur ruznych materualu pro obarveni)
  - [ ] Dalsi dekorativni material do prostredi, kolem areny je zatim relativne prazdno, bylo by dobre prostor nejaky vhodnym zpusoben zaplnit
  - [ ] Animace pohypu palicek v ptacim hnizde
- [ ] Herni postava
  - [X] Pohybove animace
  - [ ] Vyladit pohybovou fyziku (opravit big zpetneho odrazu hned pri vyskoku z hnizda)
  - [ ] Vytvorit skiny
- [ ] Kamera
  - [ ] Limity pohybu v ose Y (kamera se nesmi dostat pod mapu pokud hrac pada dolu)
  - [ ] Treseni kamery pri ruznych efektech
- [ ] Design - ELISKA
  - [ ] Vytvorit ikonu hry
  - [ ] Vytvorit baner hry (nejlip primo vyrenderovat s modelu hry v blenderu + postprodukce)
- [ ] Menu - PAVEL
  - [ ] 3D scena v pozadi menu
  - [ ] Styl tlacitek a textu
  - [ ] Settings menu
  - [X] Main menu
  - [ ] Game settings menu
  - [ ] Game lobby menu
  - [ ] Connect to host menu
- [ ] Zvukove efekty - PAVEL
  - [ ] Menu - PAVEL
    - [ ] Zvuk stiknuti tlacika - PAVEL
    - [ ] Zvuk pripojeni do lobby hry - PAVEL
    - [ ] Zvuk spusteni hry - PAVEL
  - [ ] Vrana - PAVEL
    - [ ] Chuze - PAVEL
    - [ ] Vyskok - PAVEL
    - [ ] Dopad - PAVEL
    - [ ] Utok - PAVEL
  - [ ] Vejce - PAVEL
    - [ ] Rozbiti - PAVEL
  - [ ] Hnizdo - PAVEL
    - [ ] Efekt zniceni "pokud hnizdno zmizi z areny / rozbije ho nejaka herni udalost" - PAVEL
  - [ ] Globalni - PAVEL
    - [ ] Zvuk vitezstvi- PAVEL
    - [ ] Zvuk porazky- PAVEL
    - [ ] Zvuk smrti "pokud vrana vypadne z areny"- PAVEL
    - [ ] Odpocet "tick hodin"- PAVEL
  - [ ] Soundtrack - 
    - [ ] Menu
    - [X] Arena - soundtrack1 
      - ? https://www.youtube.com/watch?v=FNZMAlO_gi4&list=PLlHY6e7WCV6NaJ5iNMoAWSEasT83GOaDo&index=5&ab_channel=GamesMusic
    - [ ] Arena - soundtrack2
      - ? https://www.youtube.com/watch?v=OA5oLdbvoLc&ab_channel=IsaacMoring
    - [ ] Arena - soundtrack3
- [ ] Efekty sceny - ELISKA
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
  - [X] Hlavni herni smycka
  - [X] Respawn handler
  - [X] Spawn handler
  - [X] Manazer hernich modu
  - [ ] Manazer skore
  - [X] Cleaning manager
- [ ] Mini Hry
  - [ ] Fall guy Perfect match
  - [ ] Bitva o vejce
  - [ ] Padajici predmety
  - [ ] ?? napady na dalsi hry

  
### Dodatecne vylepseni: 
* zlepsit vzhled modelu 
* vytvorit vlastni soundtrack (FL Studio)
* vytvorit vlastno font pisma pro hru (Adobe Ilustrator)
* vytvorit trailer video ke hre (Premiera + After Effects)
* publikace na steam 
* moznost hrani na verejnem serveru ()
- [ ] Multiplayer LAN
  - [ ] Lokalni server
  - [ ] Klient
  - [ ] Synchronizace dat
  - [ ] Komponanta pro prenos dat
  - [ ] Automatizovany vyhledavac serveru v lokalni siti
