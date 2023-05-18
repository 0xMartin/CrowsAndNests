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

Crows And Nests is a game where the player takes on the role of a crow and engages in various mini-games in the arena. Completing these minigames earns the player scores and lives. Falling from the arena subtracts life. The game is currently designed for single-player mode, with plans to introduce multiplayer support in future versions. This is the initial version of the game, which includes three mini-games: "Egg Hunt", a memory game that involves memorizing combinations of eggs in nests and dodging falling objects.

## Preview of current version

<img src="./Doc/img1.png">

## Menu

The game features a basic menu that allows players to start the game and access game settings. In the current version, players can adjust the rendering quality of the scene.

<img src="./Doc/img2.png">

## Spectator mode

If the player dies during a mini-game, they will enter spectator mode until the next mini-game begins. In spectator mode, players can observe the ongoing game without active participation.

<img src="./Doc/img3.png">

## TODO
* Fix timing of egg breaking, not based on time (activated directly in a certain key frame).
* Egg breaking using a delegate.
* Fix player physics.
* Completely rigid body for the player, more plastic movement, point-based falling, lifted [HARD]...
* Simplify nest model.
* Improve tower model, enhance scene appearance, simple particles for nests, optimize lower fog effect.
* Multiplayer menu (find server, host) -> server finder menu, host server menu -> lobby menu.
