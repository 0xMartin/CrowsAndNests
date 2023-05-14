using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

namespace Game.MiniGameUtils 
{

    /// <summary>
    /// kontext mini hry, obsahuje vsechny data potrebne pro chod minihry v arene
    /// </summary>
    public class MiniGameContext
    {
        public GameObject[] Nests { get; protected set; } /** pole vsech hnizd areny */
        public Transform[] Spawns { get; protected set; } /** pole vsech spawnu areny */
        private List<int> UsedSpawns; /** list obsahuje indexy jiz pouzitych spawnu */
        public Transform SpectatorPos { get; protected set; } /** pozice pro spektatora */

        public int YMin {get; protected set; } /** Y souradnice kdy je hrac detekovan jako vypadany z areny */

        public Player LocalPlayer {get; set; } /** Instance lokalniho hrace */
        public List<Player> Players { get; set; } /** list z hraci */

        public GameStats GameStats { get; set; } /** statistiky hry */

        public bool IsGameRunning {get; protected set;} /** status o tom zda je hra spustna */
        public bool IsGameEnd {get; protected set;} /** status o tom zda je hra u konce */

        // delegat pro pozadavek na vytvoreni efekty (pozadavky zpracovava hlavni ridici skript areny)
        public delegate void FxCreateRequest(string type, Vector3 pos, Quaternion rot); 
        public FxCreateRequest FxCallback { get; protected set; }

        // delegat pro nastaveni obrazku
        public delegate void ImageShowRequest(Texture2D image); 
        public ImageShowRequest ImageCallback { get; protected set; }

        // delegat pro nastaveni casu
        public delegate void TimeShowRequest(string time); 
        public TimeShowRequest TimeCallback { get; protected set; }

        /// <summary>
        /// Vytvorit kontext pro minihry
        /// </summary>
        /// <param name="nests">Pole referenci na vsechny hnizda</param>
        /// <param name="spawns">Pole referenci na vsechny spawnpoint pro hrace</param>
        /// <param name="spectatorPos">Reference na pozici pro spectator mod</param>
        /// <param name="yMin">Y souradnice kdy je hrac detekovan jako vypadany z areny</param>
        /// <param name="fxCallback">Callback pro vytvareni efektu ve scene hry</param>
        /// <param name="imageCallback">Callback pro nastavovani zobrazovaneho obrazku</param>
        /// <param name="timeCallback">Callback pro nastaveni zobrazovaneho casu</param>
        public MiniGameContext(GameObject[] nests, Transform[] spawns, 
                               Transform spectatorPos, int yMin, 
                               FxCreateRequest fxCallback, ImageShowRequest imageCallback, 
                               TimeShowRequest timeCallback)
        {
            this.Nests = nests;
            this.Spawns = spawns;
            this.SpectatorPos = spectatorPos;
            this.FxCallback = fxCallback;
            this.ImageCallback = imageCallback;
            this.TimeCallback = timeCallback;
            this.YMin = yMin;

            this.IsGameRunning = false;
            this.IsGameEnd = false;
            this.Players = new List<Player>();
            this.UsedSpawns = new List<int>();
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MiniGameContext), "Context created. Nest count: " +
                this.Nests.Length.ToString() + ", Spawns: " + this.Spawns.Length.ToString()));
        }

        /// <summary>
        /// Nahodne spawne zvoleneho hrace do areny. Vybrany spawn bude mozne pouzit az po resetu areny, funkce "resetArena()"
        /// </summary>
        /// <param name="player">Reference na hrace, ktery ma byt v arene spawnut</param>
        public void RandomSpawnPlayer(Player player)
        {
            // pokud hrac jiz nema zivoty
            if (player.Lives < 0)
            {
                return;
            }
            // pokud z nejakeho duvodu hrac nema model nebo jiz neni volny spawn
            if (player.ModelRef == null || UsedSpawns.Count >= Spawns.Length)
            {
                // selhalo
                SetPlayerCameraFollowPoint(player, this.SpectatorPos, true);
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
                    SetPlayerCameraFollowPoint(player, this.SpectatorPos, true);
                    return;
                }
            } while (UsedSpawns.Contains(rnd));
            UsedSpawns.Add(rnd);

            // nastavi mu pozici spawnu
            player.ModelRef.transform.position = Spawns[rnd].transform.position;
            player.ModelRef.transform.rotation = Spawns[rnd].transform.rotation;

            // aktivuje hraci model ve hre
            player.ModelRef.SetActive(true);
            Animator animator = player.ModelRef.GetComponent<Animator>();
            animator.Update(0f);

            // nastavi kameru hrace na sledovani jeho modelu
            SetPlayerCameraFollowPoint(player, player.ModelRef.transform, false);

            // log
            Vector3 pos = Spawns[rnd].transform.position;
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MiniGameContext), "Player [" + player.Name + "] spawned on: " + pos.x + ", " + pos.y + ", " + pos.z));
        }

        /// <summary>
        /// Zabije hrace a skryje jeho model. Po jeho smrti bude po chvili jeho pohled premisten na spectator pozici.
        /// </summary>
        /// <param name="player">Reference na hrace ktery bude zapit/odstranen.</param>
        public void KillPlayer(Player player)
        {
            // snizi hraci pocet zivotu, pokud jiz nema zivoty konci ve hre a muze se jen divat
            // hrace je bran jako vyrazeny ze hry pokud mam zaporny pocet zivotu
            if (player.Lives < 0)
            {
                return;
            }
            player.Lives = Mathf.Max(-1, player.Lives - 1);
            if(player.Lives < 0) {
                player.IsLiving = false;
            }

            // deaktivuje hraci jeho model ze hry
            if (player.ModelRef != null)
            {
                Animator animator = player.ModelRef.GetComponent<Animator>();
                animator.StopPlayback();
                animator.Rebind();
                player.ModelRef.SetActive(false);
            }

            // po uplinuti kratekho casu mu nastavy pohled na spectator kameru
            SetPlayerCameraFollowPoint(player, this.SpectatorPos, true);

            // log
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MiniGameContext), "Player [" + player.Name + "] killed"));
        }

        /// <summary>
        /// Zabiju vsechny hrace ve hre
        /// </summary>
        public void KillAllPlayers()
        {
            // zabije vsechny hrace
            foreach (Player p in Players)
            {
                KillPlayer(p);
            }
        }

        /// <summary>
        /// Prida hraci definovany pocet zivotu
        /// </summary>
        /// <param name="player">Reference na hrace, kteremu budou pridany zivoty</param>
        /// <param name="lives">Pocet pridanych zivotu</param>
        public void AddPlayerLives(Player player, int lives)
        {
            // prida hraci definovany pocet zivotu
            player.Lives += lives;

            // log
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MiniGameContext), "Player [" + player.Name + "] added " + lives.ToString() + " lives"));
        }

        /// <summary>
        /// Skryje vybrane hnizdu.
        /// </summary>
        /// <param name="id">ID vybraneho hnizda. ID je index v poli.</param>
        public void HideNest(int id)
        {
            if (id < 0 || id >= Nests.Length)
            {
                return;
            }

            // deaktivuje/skryje hnizdo
            GameObject nest = Nests[id];
            if(!nest.activeSelf) {
                return;
            }

            nest.SetActive(false);

            // zobrazi efekt skryti hnizda
            if (FxCallback != null)
            {
                FxCallback("hide_nest", nest.transform.position, nest.transform.rotation);
            }

            // log
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MiniGameContext), "Nest [" + id + "] is now deactivated"));
        }

        /// <summary>
        /// Zobrazi hnizdo.
        /// </summary>
        /// <param name="id">ID vybraneho hnizda. ID je index v poli.</param>
        public void ShowNest(int id)
        {
            if (id < 0 || id >= Nests.Length)
            {
                return;
            }

            // znovu aktivuje hnizdo
            GameObject nest = Nests[id];
            if(nest.activeSelf) {
                return;
            }

            nest.SetActive(true);

            // zobrazi efekt zobrazeni hnizda
            if (FxCallback != null)
            {
                FxCallback("show_nest", nest.transform.position, nest.transform.rotation);
            }

            // log
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MiniGameContext), "Nest [" + id + "] is now active"));
        }

        /// <summary>
        /// Resetuje celou arenu do puvodniho stavu. Zobrazi vsechny hnizda. Deaktivuje model vsech hracu. Resetuje blokovani spawnu.
        /// </summary>
        public void ResetArena()
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
            UsedSpawns.Clear();

            // log
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MiniGameContext), "Arene reset done"));
        }

        /// <summary>
        /// Vycisti statusi vsech zabranych hnizd. Je nutne provest pokud 
        /// </summary>
        public void ClearUsedNestsStatus() {
            UsedSpawns.Clear();
        }

        /// <summary>
        /// Nastavi pohled a nasledovani kamery hrace na definovany objekt. Pokud hrac kameru nema nic se neprovede.
        /// To nastene jen v pripade poku pujde o hrace ktery hraje na jinem zarizeni pres LAN mod.
        /// </summary>
        /// <param name="player">Reference na hrace</param>
        /// <param name="position">Reference na pozici objektu, kterou bude kamera sledovat</param>
        /// <param name="spectator">True -> jde o spectator mod. False -> normalni mod ovladani postavy</param>
        public void SetPlayerCameraFollowPoint(Player player, Transform position, bool spectator)
        {
            if (player.CinemachineFreeLook == null)
            {
                Debug.LogError(GameGlobal.Util.BuildMessage(typeof(MiniGameContext), "Failed to set player camera look on objects. Camera is null."));
                return;
            }
            // nastavi pohled kamery hrace na zvoleny objekt a zaroven ho bude kamera nasledovat
            player.CinemachineFreeLook.LookAt = position;
            player.CinemachineFreeLook.Follow = position;
            // konfigurace cinemachine
            if (spectator)
            {
                player.CinemachineFreeLook.m_Orbits = new CinemachineFreeLook.Orbit[3];
                player.CinemachineFreeLook.m_Orbits[0] = new CinemachineFreeLook.Orbit
                {
                    m_Height = 1 * 3,
                    m_Radius = 8 * 3
                };
                player.CinemachineFreeLook.m_Orbits[1] = new CinemachineFreeLook.Orbit
                {
                    m_Height = 8 * 3,
                    m_Radius = 12 * 3
                };
                player.CinemachineFreeLook.m_Orbits[2] = new CinemachineFreeLook.Orbit
                {
                    m_Height = 17 * 3,
                    m_Radius = 4 * 3
                };
            }
            else
            {
                player.CinemachineFreeLook.m_Orbits = new CinemachineFreeLook.Orbit[3];
                player.CinemachineFreeLook.m_Orbits[0] = new CinemachineFreeLook.Orbit
                {
                    m_Height = 1,
                    m_Radius = 8
                };
                player.CinemachineFreeLook.m_Orbits[1] = new CinemachineFreeLook.Orbit
                {
                    m_Height = 8,
                    m_Radius = 12
                };
                player.CinemachineFreeLook.m_Orbits[2] = new CinemachineFreeLook.Orbit
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
        public int CountLivingPlayers() {
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
        public void EndGame() {
            this.IsGameRunning = false;
            this.IsGameEnd = true;
            this.GameStats.GameEnd = DateTime.Now;
            foreach(Player p in this.Players) {
                this.GameStats.NameList.Add(p.Name);
                this.GameStats.ScoreList.Add(Mathf.Round(p.Score));
            }
        }

        /// <summary>
        /// spusti hru
        /// </summary>
        public void StartGame() {
            this.IsGameRunning = true;
            this.IsGameEnd = false;
            this.GameStats.GameStart = DateTime.Now;
        }

        /// <summary>
        /// Detekuje zda hrac vypadl dolu z areny. Pokud je hraci jiz mrtev nebude navracet true
        /// </summary>
        /// <returns>True -> hrac vypadnul</returns>
        public bool IsPlayerDropDown(Player player) {
            return player.ModelRef.transform.position.y < this.YMin && player.IsLiving;
        }

        /// <summary>
        /// Zobrazi ve hre obrazek. Slouzi pro zobrazeni libovolneho obrazku v horni casti canvas plochy.
        /// </summary>
        public void ShowImage(Texture2D image) {
            this.ImageCallback.Invoke(image);
        }

        /// <summary>
        /// Zobrazi cas/text v horni casti canvasu
        /// </summary>
        /// <param name="time">Cas/string</param>
        public void showTime(string time) {
            this.TimeCallback.Invoke(time);
        }

    }

}