using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

namespace MiniGameUtils
{

    /// <summary>
    /// rozhrani pro minihru
    /// </summary>
    interface MiniGame
    {
        /// <summary>
        /// Navrati jmeno minihry
        /// </summary>
        /// <returns>Jmeno minihry</returns>
        public string getName();

        /// <summary>
        /// Inicializuje minihru
        /// </summary>
        /// <param name="cntx">Reference na MiniGameContext</param>
        public void initGame(MiniGameContext cntx);

        /// <summary>
        /// Update metoda minihry
        /// </summary>
        public void updateGame();

        /// <summary>
        /// Spusti minihru
        /// </summary>
        public void runGame();

        /// <summary>
        /// Ukonci minihru
        /// </summary>
        public void endGame();

        /// <summary>
        /// Overi jestli minihra neskoncila uz
        /// </summary>
        /// <returns>True -> minihra jiz zkoncila</returns>
        public bool IsGameOver();
    }

    /// <summary>
    /// struktura uchovavajici potrebne data o hraci
    /// </summary>
    public struct Player
    {
        public string Name { get; set; } /** jmeno hrace */
        public int Score { get; set; } /** skore hrace */
        public int Lives { get; set; } /** pocet zivotu hrace (i hrac ktery ma 0 zivotu muze hrat v dalsi hre, pokud ma vsak zaporny pocet zivotu je jiz ze hry vyrazen) */
        public bool IsLiving { get; set; } /** pokud je "true" hrac je zivi, pokud "false" uz zemrel v dane minihre a jeho pohled kamery je nastaven na spectator mod */
        public GameObject ModelRef { get; set; } /** reference na model hrace */
        public CinemachineFreeLook cinemachineFreeLook { get; set; } /** reference na cinemachine kontroler kamery hrace (pokud jde o hrace ktery hraje z jineho PC bude NULL) */
    }

    /// <summary>
    /// statistiky hry (celkove => vsechny odehrane minihry)
    /// </summary>
    public struct GameStats
    {
        public string GameName { get; set; } /** Jmeno hry. Volene hostitelem areny */
        public int GameCount { get; set; } /** Kolik her bylo jiz odehrani.  */
        public DateTime GameStart { get; private set; } /** Cas zapoceti hry */

        public GameStats(string gameName)
        {
            this.GameName = gameName;
            this.GameCount = 0;
            this.GameStart = DateTime.Now;
        }
    };

    /// <summary>
    /// kontext mini hry, obsahuje vsechny data potrebne pro chod minihry v arene
    /// </summary>
    public class MiniGameContext
    {
        public GameObject[] Nests { get; private set; } /** pole vsech hnizd areny */
        public Transform[] Spawns { get; private set; } /** pole vsech spawnu areny */
        private List<int> usedSpawns; /** list obsahuje indexy jiz pouzitych spawnu */
        public Transform SpectatorPos { get; private set; } /** pozice pro spektatora */

        // delegat pro pozadavek na vytvoreni efekty (pozadavky zpracovava hlavni ridici skript areny)
        public delegate void FxCreateRequest(string type, Vector3 pos, Quaternion rot); 
        public FxCreateRequest FxCallback { get; private set; }

        public List<Player> Players { get; set; } /** list z hraci */

        public GameStats GameStats { get; set; } /** statistiky hry */

        public bool IsGameRunning {get; private set;} /** status o tom zda je hra spustna */
        public bool IsGameEnd {get; private set;} /** status o tom zda je hra u konce */

        /// <summary>
        /// Vytvorit kontext pro minihry
        /// </summary>
        /// <param name="nests">Pole referenci na vsechny hnizda</param>
        /// <param name="spawns">Pole referenci na vsechny spawnpoint pro hrace</param>
        /// <param name="spectatorPos">Reference na pozici pro spectator mod</param>
        /// <param name="fxCallback">Callback pro vytvareni efektu ve scene hry</param>
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

        /// <summary>
        /// Nahodne spawne zvoleneho hrace do areny. Vybrany spawn bude mozne pouzit az po resetu areny, funkce "resetArena()"
        /// </summary>
        /// <param name="player">Reference na hrace, ktery ma byt v arene spawnut</param>
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

        /// <summary>
        /// Zabije hrace a skryje jeho model. Po jeho smrti bude po chvili jeho pohled premisten na spectator pozici.
        /// </summary>
        /// <param name="player">Reference na hrace ktery bude zapit/odstranen.</param>
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

        /// <summary>
        /// Zabiju vsechny hrace ve hre
        /// </summary>
        public void killAllPlayers()
        {
            // zabije vsechny hrace
            foreach (Player p in Players)
            {
                killPlayer(p);
            }
        }

        /// <summary>
        /// Prida hraci definovany pocet zivotu
        /// </summary>
        /// <param name="player">Reference na hrace, kteremu budou pridany zivoty</param>
        /// <param name="lives">Pocet pridanych zivotu</param>
        public void addPlayerLives(Player player, int lives)
        {
            // prida hraci definovany pocet zivotu
            player.Lives += lives;

            // log
            Debug.Log(GameGlobal.Util.buildMessage(typeof(MiniGameContext), "Player [" + player.Name + "] added " + lives.ToString() + " lives"));
        }

        /// <summary>
        /// Skryje vybrane hnizdu.
        /// </summary>
        /// <param name="id">ID vybraneho hnizda. ID je index v poli.</param>
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

        /// <summary>
        /// Zobrazi hnizdo.
        /// </summary>
        /// <param name="id">ID vybraneho hnizda. ID je index v poli.</param>
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

        /// <summary>
        /// Resetuje celou arenu do puvodniho stavu. Zobrazi vsechny hnizda. Deaktivuje model vsech hracu. Resetuje blokovani spawnu.
        /// </summary>
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

        /// <summary>
        /// Nastavi pohled a nasledovani kamery hrace na definovany objekt. Pokud hrac kameru nema nic se neprovede.
        /// To nastene jen v pripade poku pujde o hrace ktery hraje na jinem zarizeni pres LAN mod.
        /// </summary>
        /// <param name="player">Reference na hrace</param>
        /// <param name="position">Reference na pozici objektu, kterou bude kamera sledovat</param>
        /// <param name="spectator">True -> jde o spectator mod. False -> normalni mod ovladani postavy</param>
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

        /// <summary>
        /// spocita hrace, kteri jeste maji dostatek zivotu a mouzou pokracovat ve hre
        /// hraci kteri mohou hrat maji pocet zivotu >= 0
        /// </summary>
        /// <returns></returns>
        public int countLivingPlayers() {
            int cnt = 0;
            foreach(Player p in this.Players) {
                if(p.Lives >= 0) {
                    cnt++;
                }
            }
            return cnt;
        }

        /// <summary>
        /// ukonci hru
        /// </summary>
        public void endGame() {
            this.IsGameRunning = false;
            this.IsGameEnd = true;
        }

        /// <summary>
        /// spusti hru
        /// </summary>
        public void startGame() {
            this.IsGameRunning = true;
            this.IsGameEnd = false;
        }

    }

}