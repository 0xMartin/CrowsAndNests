using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Game.MiniGame;
using Menu;
using Cinemachine;

namespace Game
{

    /// <summary>
    /// Controller areny. Jde o hlavni komponentu hry (veskere rozhodovani).
    /// </summary>
    public class ArenaController : MonoBehaviour
    {

        /*********************************************************************************************************/
        // VEREJNE PROMENNE [CONFIG]
        public int GAME_START_TIME = 30;        /** Za jak dlouho se spusti hra (v sekundach)  */
        public int GAME_END_TIME = 10;          /** Jak dloho ma hra cekat jeste po jejim ukonceni (v sekundach)  */
        public int MINIGAME_NAME_TIME = 3;      /** Po jakou dobu bude zobrazeno jmeno zvolene minihry */
        public int MINIGAME_START_TIME = 10;    /** Za jak dlouho zacne dalsi minihra po zkonceni posledni minihry (v sekundach) */
        public int MINIGAME_END_TIME = 5;       /** Jak dlouho bude minihrace cekat po jejim konci (v sekundach) */
        public int MAX_PLAYERS = 4;             /** Maximalni pocet hracu (1 - 4 & min <= max) */
        public int MIN_PLAYERS = 1;             /** Minimalni pocet hracu pro hru (1 - 4 & min <= max) [pro single player=1, pro multiplayer=2, jine konfigurace jsou taktez mozne] */
        public int Y_MIN = -120;                /** Y pozice za kterou je hrac detekovan jako vypadeny z areny */

        /*********************************************************************************************************/
        // VEREJNE PROMENNE [REFERENCES]

        /**
         * Reference na lokalniho hrace
         **/
        [Header("Player")]
        public GameObject playerRef;
        public CinemachineFreeLook cinemachineCam;

        /**
         * Znazorneni pozic hnizd v arene
         * [4]  [3]  [2]  [1]
         * [8]  [7]  [6]  [5]
         * [12] [11] [10] [9]
         * [16] [15] [14] [13]
         **/
        [Header("Nets")]
        public GameObject nest1;
        public GameObject nest2;
        public GameObject nest3;
        public GameObject nest4;
        public GameObject nest5;
        public GameObject nest6;
        public GameObject nest7;
        public GameObject nest8;
        public GameObject nest9;
        public GameObject nest10;
        public GameObject nest11;
        public GameObject nest12;
        public GameObject nest13;
        public GameObject nest14;
        public GameObject nest15;
        public GameObject nest16;

        /**
         * Znaroreni pozic spawnu v arene
         * [1] [*] [*] [0]
         * [*] [*] [*] [*]
         * [*] [*] [*] [*]
         * [3] [*] [*] [2]
         **/
        [Header("Spawns")]
        public Transform spawn0;
        public Transform spawn1;
        public Transform spawn2;
        public Transform spawn3;

        [Header("Spectator")]
        public Transform spectator;

        [Header("FX")]
        public GameObject nestHideFxPrefab;
        public GameObject nestShowFxPrefab;

        [Header("UI-Text")]
        public GameObject countdownObj;
        public GameObject playerLiveObj;
        public GameObject playerScoreObj;
        public GameObject gameNameObj;

        /*********************************************************************************************************/
        // LOKALNI PROMENNE
        private MiniGameContext gameCntx; /** Kontext mini hry */

        private List<MiniGameObj> minigames; /** Seznam dostupnych miniher */
        private MiniGameObj activeMinigame; /** Aktivni minihra */

        private Player localPlayer; /** Instance lokalniho hrace */

        // aktualni stav ve kterem se hra nachazi
        private enum GameState
        {
            NotRunning,        /** Hra jeste neni spustena/nakonfigurovana */
            GameStarting,      /** Hra je jiz spustene. Bezi odpocet do spusteni hry*/

            // tyto 4 faze se toci porad dokola dokud je dostatek hracu
            MiniGameSelecting, /** V teto fazi se vybira minihra ktera se bude hrat */
            MiniGameShowName,  /** Zozrazi jmeno minihry */
            MiniGameStarting,  /** Minihra byla vybrana. Bezi odpocet nez se minihra spusti */
            MiniGameRunning,   /** Minihra bezi. V teto fazi hraci uz hraji/muzou umirat. */
            MiniGameEnding,    /** Minihra skoncila, kratky cas pro rekapitulaci vitezu teto minihry a pricteni skore. */

            GameEnding         /** Hra skoncila. Kratky cas a pak presmerovani do final lobby se zobrazenymi vysledky */
        };

        private GameState state;

        private float startTime; /** promenna pro uchovavani casu */

        private TextMeshProUGUI countdownText; /** Count down text */
        private TextMeshProUGUI playerLiveText; /** Text poctem zivotu hrace */
        private TextMeshProUGUI playerScoreText; /** Text poctem skore hrace */
        private TextMeshProUGUI gameNameText; /** Text s nazvem aktualni minihry */

        void Start()
        {
            // init
            this.countdownText = this.countdownObj.GetComponent<TextMeshProUGUI>();
            this.countdownText.enabled = false;

            this.playerLiveText = this.playerLiveObj.GetComponent<TextMeshProUGUI>();
            this.playerScoreText = this.playerScoreObj.GetComponent<TextMeshProUGUI>();
            this.gameNameText = this.gameNameObj.GetComponent<TextMeshProUGUI>();

            // defaultni stav hry
            this.state = GameState.NotRunning;

            // vytvoreni mini her
            this.minigames = new List<MiniGameObj>();
            this.minigames.Add(new MG_EggHunt());

            // <KONTEXT MINI HERNI ARENY>
            // hnizda
            GameObject[] nests = new GameObject[]{
                nest1, nest2, nest3, nest4,
                nest5, nest6, nest7, nest8,
                nest9, nest10, nest11, nest12,
                nest13, nest14, nest15, nest16
            };
            // spawny
            Transform[] spawns = new Transform[] {
                spawn0, spawn1, spawn2, spawn3
            };
            // vytvoreni kontextu
            this.gameCntx = new MiniGameContext(
                nests,
                spawns,
                spectator,
                new MiniGameContext.FxCreateRequest(FxCallback),
                this.Y_MIN
            );
            // vytvoreni lokalniho hrace (hrace se jmenem = "you" + jako jediny hrac v lokalni arene bude mit kameru, ostatni hraci ze site budou pridavani postupne)
            this.localPlayer = new Player() {
                    Name = "You",
                    Score = 0,
                    Lives = 3,
                    IsLiving = true,
                    ModelRef = this.playerRef,
                    CinemachineFreeLook = this.cinemachineCam 
            };
            this.gameCntx.Players = new List<Player>() {
                this.localPlayer     
            };

            // nahodny spawn lokalniho hrace
            this.gameCntx.RandomSpawnPlayer(this.localPlayer);
            this.gameCntx.ClearUsedNestsStatus();    

            Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "Init done"));

            // automaticke spusteni (pak predelat => pokud pujde o multiplayer => spusti hru az se pripoji hraci)
            this.gameCntx.StartGame();
        }

        void Update()
        {
            if (this.gameCntx == null) return;

            float remaining;

            // stavovy automat (hlavni herni rozhodovaci logiky)
            switch (this.state)
            {
                case GameState.NotRunning:
                    // pokud je hra spusta prejde do statvu "Game_Starting"
                    if (this.gameCntx.IsGameRunning)
                    {
                        this.state = GameState.GameStarting;
                        Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "GO TO GameStarting"));
                    }

                    // zaznamenani startovniho casu pro odpocet startu cele hry
                    startTime = GameGlobal.Util.Time_start();
                    break;


                case GameState.GameStarting:
                    // odpocet do spusteni hry
                    remaining = GameGlobal.Util.Time_remaining(startTime, GAME_START_TIME);
                    ShowCountDown((int)remaining, true, "");
                    if(remaining == 0) {
                        ShowCountDown(0, false, "");
                    }

                    // cas uplynul
                    if (GameGlobal.Util.Time_passed(startTime, GAME_START_TIME))
                    {
                        this.state = GameState.MiniGameSelecting;
                        Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "GO TO MiniGameSelecting"));
                    }
                    break;


                case GameState.MiniGameSelecting:
                    // vyber dalsi mini hry
                    RandomMiniGameSelect();
                    if (this.activeMinigame == null)
                    {
                        Debug.LogError(GameGlobal.Util.BuildMessage(typeof(ArenaController), "Failed select random minigame"));
                        break;
                    }

                    // inicializace vybrane minihry
                    this.activeMinigame.InitGame(this.gameCntx);

                    // hned prejde do stavu "MiniGame_ShowName"
                    this.state = GameState.MiniGameShowName;
                    Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "GO TO MiniGameShowName"));

                    // zobrazi jmeno zvolene minihry
                    ShowMinigameName(true);

                    // zaznamenani startovniho casu pro zobrazeni jmena minihry
                    startTime = GameGlobal.Util.Time_start();
                    break;

                case GameState.MiniGameShowName:
                    if (GameGlobal.Util.Time_passed(startTime, MINIGAME_NAME_TIME))
                    {
                        // hned prejde do stavu "MiniGame_Starting"
                        this.state = GameState.MiniGameStarting;
                        Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "GO TO MiniGameStarting"));

                        // zaznamenani startovniho casu pro odpocet mini hry
                        startTime = GameGlobal.Util.Time_start();
                    }
                    break;

                case GameState.MiniGameStarting:
                    // zobrazeni odpoctu do zacatku minihry
                    remaining = GameGlobal.Util.Time_remaining(startTime, MINIGAME_START_TIME);
                    ShowCountDown((int)remaining, true, "Start");

                    // pokud je odpocet u konce prejde do stavu "MiniGame_Running"
                    if (GameGlobal.Util.Time_passed(startTime, MINIGAME_START_TIME))
                    {
                        // skryje cas
                        ShowCountDown(0, false, "Start");

                        // prejde do stavu "MiniGame_Running"
                        this.state = GameState.MiniGameRunning;
                        Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "GO TO MiniGameRunning"));
                    }
                    break;


                case GameState.MiniGameRunning:
                    // vykonavani smycky aktualne spustene minihry
                    this.activeMinigame.UpdateGame();

                    // pokud je minihra u konce prejde do stavu "MiniGame_Ending
                    if (this.activeMinigame.IsGameOver())
                    {
                        this.state = GameState.MiniGameEnding;
                        Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "GO TO MiniGameEnding"));
                        // zaznamenani startovniho casu pro odmereni minigame end timu
                        startTime = GameGlobal.Util.Time_start();
                    }
                    break;


                case GameState.MiniGameEnding:
                    // pocka definovany cas a zobrazi viteze minihry
                    if (GameGlobal.Util.Time_passed(startTime, MINIGAME_END_TIME))
                    {
                        // spocita pocet zijicich hracu
                        if (this.gameCntx.CountLivingPlayers() >= MIN_PLAYERS)
                        {
                            // pokud je dostatek hracu smycky se opakuje a jde znovu vybrat dalsi minihru
                            this.state = GameState.MiniGameSelecting;
                            Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "GO TO MiniGameSelecting"));
                        }
                        else
                        {
                            // pokud uz neni dostatek hracu je hra ukoncena
                            this.gameCntx.EndGame();
                        }
                    }
                    break;


                case GameState.GameEnding:
                    // odpocet do ukonceni areny
                    if (GameGlobal.Util.Time_passed(startTime, GAME_END_TIME))
                    {
                        // odstrani kontext
                        this.gameCntx = null;
                        // prechod do sceny a zobrazeni vitezu celkove hry
                        //TODO>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>TODO
                    }
                    break;
            }

            // dodatecne prechodove pravidla
            if (this.gameCntx.IsGameEnd)
            {
                this.state = GameState.GameEnding;
                // zaznamenani startovniho casu pro odmereni game end timu
                startTime = GameGlobal.Util.Time_start();
            }

            // no in minigame akce
            if(this.state != GameState.MiniGameRunning) 
            {
                // automaticky respawn hrace bez odebirani zivotu
                foreach(Player p in this.gameCntx.Players)
                {
                    if(this.gameCntx.IsPlayerDropDown(p)) 
                    {
                        Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "RESPAWN IN WAIT MODE: " + p.ToString()));
                        this.gameCntx.RandomSpawnPlayer(p);
                        this.gameCntx.ClearUsedNestsStatus();    
                    }    
                }
            }

            // refresh game stats
            RefreshGameStats();
        }

        /// <summary>
        /// Vytvori novou hru
        /// </summary>
        /// <param name="gameName">Jmeno hry / jmeno arena ktere hraci uvidi</param>
        private void CreateGame(string gameName)
        {
            if (this.gameCntx == null)
            {
                Debug.LogError(GameGlobal.Util.BuildMessage(typeof(ArenaController), "Failed to create game. Context is null!"));
                return;
            }
            this.gameCntx.GameStats = new GameStats(gameName);
        }

        /// <summary>
        /// Nahodne vybere minihru ze seznamu a nastavi ji jako aktivni
        /// </summary>
        private void RandomMiniGameSelect()
        {
            int rng = Random.Range(0, minigames.Count);
            activeMinigame = minigames[rng];
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "MiniGame set on: " + activeMinigame.GetName()));
        }
        
        /// <summary>
        /// Zobrazi odpocet (do startu cele hry/minihry))
        /// </summary>
        /// <param name="number">Zobrazevane cislo "odpocet"</param>
        /// <param name="show">True -> cislo bude zobrazeno, false -> cislo bude skryto</param>
        /// <param name="zeroText">Text ktery ze zobrazi misto cisla 0 v odpoctu</param>
        private void ShowCountDown(int number, bool show, string zeroText)
        {
            // zobrazeni / skryti textu
            this.countdownText.enabled = show;

            // zobrazeni cisla odpoctu / text misto 0
            if(number == 0) 
            {
                this.countdownText.SetText(zeroText);        
            } 
            else
            {
                this.countdownText.SetText(number.ToString());
            }
        }

        /// <summary>
        /// Zobrazi jmenu aktivni minihry
        /// </summary>
        /// <param name="show">Pokud je true -> text bude zobrazen, false -> text bude skryt</param>
        public void ShowMinigameName(bool show)
        {
            // zobrazeni / skryti textu
            this.countdownText.enabled = show;

            // vypise jmeno aktivni minihry
            if (this.activeMinigame != null)
            {
                this.countdownText.SetText(this.activeMinigame.GetName());
            }
        }

        /// <summary>
        /// Call back procedura na vytvareni specialnich efektu v arene. Callback je volan z minihry ktera je aktivni v kontroleru areny.
        /// </summary>
        /// <param name="type">Type efektu</param>
        /// <param name="pos">Pozice efektu</param>
        /// <param name="rot">Orientace efektu</param>
        private void FxCallback(string type, Vector3 pos, Quaternion rot)
        {
            // na vyzadani kontextu nebo i jine komponenty ktery tento callback zavola 
            // vytvori pozadovany efekt ktery je v teto funkce definovany
            switch (type)
            {
                case "hide_nest":
                    if (nestHideFxPrefab != null)
                    {
                        Instantiate(nestHideFxPrefab, pos, rot);
                    }
                    break;
                case "show_nest":
                    if (nestShowFxPrefab != null)
                    {
                        Instantiate(nestShowFxPrefab, pos, rot);
                    }
                    break;
            }
        }

        private void RefreshGameStats() {
            this.playerLiveText.SetText(Mathf.Max(0, this.localPlayer.Lives).ToString() + " " + '\u2665');
            this.playerScoreText.SetText(this.localPlayer.Score.ToString() + " " + '+');
            switch(this.state) {
                case GameState.GameStarting:
                    this.gameNameText.SetText("Starting");
                    break;
                case GameState.MiniGameEnding:
                    this.gameNameText.SetText("End");
                    break;
                case GameState.GameEnding:
                    this.gameNameText.SetText("Game over");
                    break;
                default:
                    if(this.activeMinigame != null) {
                        this.gameNameText.SetText(this.activeMinigame.GetName());
                    } else {
                        this.gameNameText.SetText("Waiting");
                    }
                    break;
            }
        }

    }

}
