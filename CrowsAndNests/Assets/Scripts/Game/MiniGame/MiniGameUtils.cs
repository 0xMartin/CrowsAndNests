using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

namespace MiniGameUtils
{

    // rozhrani pro minihru
    interface MiniGame
    {
        public string getName();
        public void initGame(MiniGameContext cntx);
        public void updateGame();
        public void runGame();
        public void endGame();
        public bool IsGameOver();
    }

    // struktura uchovavajici potrebne data o hraci
    public struct Player
    {
        public string Name { get; set; } /** jmeno hrace */
        // skore hrace
        public int Score { get; set; }
        // pocet zivotu hrace (i hrac ktery ma 0 zivotu muze hrat v dalsi hre, pokud ma vsak zaporny pocet zivotu je jiz ze hry vyrazen)
        public int Lives { get; set; }
        // pokud je "true" hrac je zivi, pokud "false" uz zemrel v dane minihre a jeho pohled kamery je nastaven na spectator mod
        public bool IsLiving { get; set; }
        // reference na model hrace
        public GameObject ModelRef { get; set; }
        // reference na cinemachine kontroler kamery hrace (pokud jde o hrace ktery hraje z jineho PC bude NULL)
        public CinemachineFreeLook cinemachineFreeLook { get; set; }
    }

    // statistiky hry (celkove => vsechny odehrane minihry)
    public struct GameStats
    {
        public string GameName { get; set; }
        public int GameCount { get; set; }
        public DateTime GameStart { get; private set; }

        public GameStats(string gameName)
        {
            this.GameName = gameName;
            this.GameCount = 0;
            this.GameStart = DateTime.Now;
        }
    };

    // Kontext mini hry, obsahuje vsechny data potrebne pro chod minihry v arene
    public class MiniGameContext
    {
        // pole vsech hnizd areny
        public GameObject[] Nests { get; private set; }
        // pole vsech spawnu areny
        public Transform[] Spawns { get; private set; }
        // list obsahuje indexy jiz pouzitych spawnu  
        private List<int> usedSpawns;
        // pozice pro spektatora
        public Transform SpectatorPos { get; private set; }

        // delegat pro pozadavek na vytvoreni efekty (pozadavky zpracovava hlavni ridici skript areny)
        public delegate void FxCreateRequest(string type, Vector3 pos, Quaternion rot);
        public FxCreateRequest FxCallback { get; private set; }

        // list z hraci
        public List<Player> Players { get; set; }

        // statistiky hry
        public GameStats GameStats { get; set; }

        // status o tom zda je hra spustna
        public bool IsGameRunning {get; private set;}
        // status o tom zda je hra u konce
        public bool IsGameEnd {get; private set;}

        public MiniGameContext(GameObject[] nests, Transform[] spawns, Transform spectatorPos, FxCreateRequest fxCallback)
        {
            this.Nests = nests;
            this.Spawns = spawns;
            this.SpectatorPos = SpectatorPos;
            this.FxCallback = fxCallback;
            this.IsGameRunning = false;
            this.IsGameEnd = false;
            this.Players = new List<Player>();
            this.usedSpawns = new List<int>();
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Context created. Nest count: " +
                this.Nests.Length.ToString() + ", Spawns: " + this.Spawns.Length.ToString()));
        }

        public void randomSpawnPlayer(Player player)
        {
            // pokud hrac jiz nema zivoty
            if (player.Lives < 0)
            {
                return;
            }
            // pokud z nejakeho duvodu hrac nema model nebo jiz neni volny spawn
            if (player.ModelRef == null || usedSpawns.Count >= Spawns.Length)
            {
                // selhalo
                setPlayerCameraFollowPoint(player, this.SpectatorPos, true);
                return;
            }

            // nahodne vybere spawn na kterem se jeste nenachazi zadny hrac
            int rnd = -1;
            int cnt = 50;
            do
            {
                rnd = UnityEngine.Random.Range(0, Spawns.Length);
                cnt++;
                if (cnt <= 0)
                {
                    // selhalo
                    setPlayerCameraFollowPoint(player, this.SpectatorPos, true);
                    return;
                }
            } while (usedSpawns.Contains(rnd));
            usedSpawns.Add(rnd);

            // nastavi mu pozici spawnu
            player.ModelRef.transform.position = Spawns[rnd].transform.position;
            player.ModelRef.transform.rotation = Spawns[rnd].transform.rotation;

            // aktivuje hraci model ve hre
            player.ModelRef.SetActive(true);

            // nastavi kameru hrace na sledovani jeho modelu
            setPlayerCameraFollowPoint(player, player.ModelRef.transform, false);

            // log
            Vector3 pos = Spawns[rnd].transform.position;
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Player [" + player.Name + "] spawned on: " + pos.x + ", " + pos.y + ", " + pos.z));
        }

        public void killPlayer(Player player)
        {
            // snizi hraci pocet zivotu, pokud jiz nema zivoty konci ve hre a muze se jen divat
            // hrace je bran jako vyrazeny ze hry pokud mam zaporny pocet zivotu
            if (player.Lives < 0)
            {
                return;
            }
            player.Lives = Mathf.Max(-1, player.Lives - 1);

            // deaktivuje hraci jeho model ze hry
            if (player.ModelRef != null)
            {
                player.ModelRef.SetActive(false);
            }

            // po uplinuti kratekho casu mu nastavy pohled na spectator kameru
            setPlayerCameraFollowPoint(player, this.SpectatorPos, true);

            // log
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Player [" + player.Name + "] killed"));
        }

        public void killAllPlayers()
        {
            // zabije vsechny hrace
            foreach (Player p in Players)
            {
                killPlayer(p);
            }
        }

        public void addPlayerLives(Player player, int lives)
        {
            // prida hraci definovany pocet zivotu
            player.Lives += lives;

            // log
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Player [" + player.Name + "] added " + lives.ToString() + " lives"));
        }

        public void hideNest(int id)
        {
            if (id < 0 || id >= Nests.Length)
            {
                return;
            }

            // deaktivuje/skryje hnizdo
            GameObject nest = Nests[id];
            nest.SetActive(false);

            // zobrazi efekt skryti hnizda
            if (FxCallback != null)
            {
                FxCallback("hide_nest", nest.transform.position, nest.transform.rotation);
            }

            // log
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Nest [" + id + "] is now deactivated"));
        }

        public void showNest(int id)
        {
            if (id < 0 || id >= Nests.Length)
            {
                return;
            }

            // znovu aktivuje hnizdo
            GameObject nest = Nests[id];
            nest.SetActive(true);

            // zobrazi efekt zobrazeni hnizda
            if (FxCallback != null)
            {
                FxCallback("show_nest", nest.transform.position, nest.transform.rotation);
            }

            // log
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Nest [" + id + "] is now active"));
        }

        public void resetArena()
        {
            // aktivuje vsechny hnizda
            foreach (GameObject nest in this.Nests)
            {
                nest.SetActive(true);
            }

            // deaktivuje modely vsech hracu
            foreach (Player p in this.Players)
            {
                if (p.ModelRef != null)
                {
                    p.ModelRef.SetActive(false);
                }
            }

            // uvoleni vsech spawnu
            usedSpawns.Clear();

            // log
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Arene reset done"));
        }

        public void setPlayerCameraFollowPoint(Player player, Transform position, bool spectator)
        {
            if (player.cinemachineFreeLook == null)
            {
                Debug.LogError(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Failed to set player camer look on objects. Camera is null."));
                return;
            }
            // nastavi pohled kamery hrace na zvoleny objekt a zaroven ho bude kamera nasledovat
            player.cinemachineFreeLook.LookAt = position;
            player.cinemachineFreeLook.Follow = position;
            // konfigurace cinemachine
            if (spectator)
            {
                player.cinemachineFreeLook.m_Orbits = new CinemachineFreeLook.Orbit[3];
                player.cinemachineFreeLook.m_Orbits[0] = new CinemachineFreeLook.Orbit
                {
                    m_Height = 1 * 3,
                    m_Radius = 8 * 3
                };
                player.cinemachineFreeLook.m_Orbits[1] = new CinemachineFreeLook.Orbit
                {
                    m_Height = 8 * 3,
                    m_Radius = 12 * 3
                };
                player.cinemachineFreeLook.m_Orbits[2] = new CinemachineFreeLook.Orbit
                {
                    m_Height = 17 * 3,
                    m_Radius = 4 * 3
                };
            }
            else
            {
                player.cinemachineFreeLook.m_Orbits = new CinemachineFreeLook.Orbit[3];
                player.cinemachineFreeLook.m_Orbits[0] = new CinemachineFreeLook.Orbit
                {
                    m_Height = 1,
                    m_Radius = 8
                };
                player.cinemachineFreeLook.m_Orbits[1] = new CinemachineFreeLook.Orbit
                {
                    m_Height = 8,
                    m_Radius = 12
                };
                player.cinemachineFreeLook.m_Orbits[2] = new CinemachineFreeLook.Orbit
                {
                    m_Height = 17,
                    m_Radius = 4
                };
            }
        }

        public int countLivingPlayers() {
            // spocita hrace, kteri jeste maji dostatek zivotu a mouzou pokracovat ve hre
            // hraci kteri mohou hrat maji pocet zivotu >= 0
            int cnt = 0;
            foreach(Player p in this.Players) {
                if(p.Lives >= 0) {
                    cnt++;
                }
            }
            return cnt;
        }

        public void endGame() {
            // ukonci hru
            this.IsGameRunning = false;
            this.IsGameEnd = true;
        }

        public void startGame() {
            // spusti hru
            this.IsGameRunning = true;
            this.IsGameEnd = false;
        }

    }

}