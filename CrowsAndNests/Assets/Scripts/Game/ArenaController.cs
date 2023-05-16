using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Game.MiniGameUtils;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Menu;

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
        public List<GameObject> nests;

        /**
         * Znaroreni pozic spawnu v arene
         * [1] [*] [*] [0]
         * [*] [*] [*] [*]
         * [*] [*] [*] [*]
         * [3] [*] [*] [2]
         **/
        [Header("Spawns")]
        public List<Transform> spawns;

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
        public GameObject timeObj;
        public GameObject imageDisplayObj;

        [Header("Sounds")]
        public GameObject tickSoundObj;  /** Zvuk odpoctu "tick" */
        public GameObject nestBreakSoundObj;  /** Zvuk rozbiti vejce */
        public GameObject musicController; /** Reference na music controller */

        [Header("Pause Menu")]
        public GameObject pauseMenuObj; /** Canvas vrstva s pause menu, musi mit komponentu <PauseMenu> */

        [Header("MiniGames")]
        public List<GameObject> gamesList;  /** Seznam vsech miniher*/

        /*********************************************************************************************************/
        // LOKALNI PROMENNE
        private MiniGameContext gameCntx; /** Kontext mini hry */

        private List<MiniGameObj> minigames; /** Seznam dostupnych miniher */
        private MiniGameObj activeMinigame; /** Aktivni minihra */

        private int sameGameCounter = 0; /** Citac stejnych miniher v serii */

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

        private TextMeshProUGUI countdownText;      /** Hlavni count down text [veliky] */
        private TextMeshProUGUI playerLiveText;     /** Text poctem zivotu hrace */
        private TextMeshProUGUI playerScoreText;    /** Text poctem skore hrace */
        private TextMeshProUGUI gameNameText;       /** Text s nazvem aktualni minihry */
        private TextMeshProUGUI timeText;           /** Text s aktualni zbyvajicim casem do konce minihry */
        private RawImage imageDisplay;               /** Objekt pro zobrazovani libovolniych obrazku */
        private AudioSource tickSound;              /** Zvuk odpoctu*/
        private AudioSource nestBreakSound;              /** Zvuk rozbiti vejce*/

        void Start()
        {
            // init UI v canvas
            this.countdownText = this.countdownObj.GetComponent<TextMeshProUGUI>();
            this.countdownText.enabled = false;

            this.playerLiveText = this.playerLiveObj.GetComponent<TextMeshProUGUI>();
            this.playerScoreText = this.playerScoreObj.GetComponent<TextMeshProUGUI>();
            this.gameNameText = this.gameNameObj.GetComponent<TextMeshProUGUI>();

            this.timeText = this.timeObj.GetComponent<TextMeshProUGUI>();
            this.TimeCallBack("");
            this.imageDisplay = this.imageDisplayObj.GetComponent<RawImage>();
            this.imageDisplayObj.SetActive(false);

            this.tickSound = this.tickSoundObj.GetComponent<AudioSource>();
            this.nestBreakSound = this.nestBreakSoundObj.GetComponent<AudioSource>();
        }

        void Awake() {
            // skryje a uzamkne kurzor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // defaultni stav hry
            this.state = GameState.NotRunning;

            // vytvoreni seznamu mini her z predanych referenci objaktu obsahujicih minihry
            this.minigames = new List<MiniGameObj>();
            foreach(GameObject g in gamesList) {
                MiniGameObj mg = g.GetComponent<MiniGameObj>();
                this.minigames.Add(mg);    
            }

            // <KONTEXT MINI HERNI ARENY>
            // vytvoreni kontextu
            this.gameCntx = new MiniGameContext(
                nests.ToArray(),
                spawns.ToArray(),
                spectator,
                this.Y_MIN,
                new MiniGameContext.FxCreateRequest(FxCallback),
                new MiniGameContext.ImageShowRequest(ImageCallback),
                new MiniGameContext.TimeShowRequest(TimeCallBack)
            );
            this.gameCntx.MusicController = this.musicController.GetComponent<AudioCrossfade>();

            // spusteni defaultni music "index: 0"
            this.gameCntx.MusicController.StartCrossfade(0);

            // vytvoreni hry
            this.CreateGame("Game 1");

            // vytvoreni lokalniho hrace (hrace se jmenem = "you" + jako jediny hrac v lokalni arene bude mit kameru, ostatni hraci ze site budou pridavani postupne)
            this.gameCntx.LocalPlayer = new Player() {
                    Name = "You",
                    Score = 0,
                    Lives = 0, 
                    IsLiving = true,
                    ModelRef = this.playerRef,
                    CinemachineFreeLook = this.cinemachineCam 
            };
            this.gameCntx.Players = new List<Player>() {
                this.gameCntx.LocalPlayer     
            };

            // nahodny spawn lokalniho hrace
            this.gameCntx.RandomSpawnPlayer(this.gameCntx.LocalPlayer);
            this.gameCntx.ClearUsedNestsStatus();    

            Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "Init done"));

            // automaticke spusteni (pak predelat => pokud pujde o multiplayer => spusti hru az se pripoji hraci)
            this.gameCntx.StartGame();
        }

        void Update()
        {
            if (this.gameCntx == null) return;

            // pause menu
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseMenu pauseMenu = this.pauseMenuObj.GetComponent<PauseMenu>();
                if (pauseMenu != null)
                {
                    pauseMenu.PauseGame();
                }
            }

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
                    startTime = GameGlobal.Util.TimeStart();
                    break;


                case GameState.GameStarting:
                    // odpocet do spusteni hry
                    remaining = GameGlobal.Util.TimeRemaining(startTime, GAME_START_TIME);
                    ShowCountDown((int)remaining, true, "");
                    if(remaining == 0) {
                        ShowCountDown(0, false, "");
                    }

                    // cas uplynul
                    if (GameGlobal.Util.TimePassed(startTime, GAME_START_TIME))
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

                    // dalsi kolo
                    this.gameCntx.GameStats.GameCount += 1;

                    // inicializace vybrane minihry
                    this.activeMinigame.ReinitGame(this.gameCntx);

                    // hned prejde do stavu "MiniGame_ShowName"
                    this.state = GameState.MiniGameShowName;
                    Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "GO TO MiniGameShowName"));

                    // zobrazi jmeno zvolene minihry
                    ShowMinigameName(true);

                    // zaznamenani startovniho casu pro zobrazeni jmena minihry
                    startTime = GameGlobal.Util.TimeStart();
                    break;

                case GameState.MiniGameShowName:
                    if (GameGlobal.Util.TimePassed(startTime, MINIGAME_NAME_TIME))
                    {
                        // hned prejde do stavu "MiniGame_Starting"
                        this.state = GameState.MiniGameStarting;
                        Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "GO TO MiniGameStarting"));

                        // zaznamenani startovniho casu pro odpocet mini hry
                        startTime = GameGlobal.Util.TimeStart();
                    }
                    break;

                case GameState.MiniGameStarting:
                    // zobrazeni odpoctu do zacatku minihry
                    remaining = GameGlobal.Util.TimeRemaining(startTime, MINIGAME_START_TIME);
                    ShowCountDown((int)remaining, true, "Start");

                    // pokud je odpocet u konce prejde do stavu "MiniGame_Running"
                    if (GameGlobal.Util.TimePassed(startTime, MINIGAME_START_TIME))
                    {
                        // skryje cas
                        ShowCountDown(0, false, "Start");

                        // spusti mini hru
                        this.activeMinigame.RunGame();

                        // prejde do stavu "MiniGame_Running"
                        this.state = GameState.MiniGameRunning;
                        Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "GO TO MiniGameRunning"));
                    }
                    break;


                case GameState.MiniGameRunning:
                    // vykonavani smycky aktualne spustene minihry
                    this.activeMinigame.UpdateGame();

                    // overi zda muze hra pokracovat (single - hrac zije)
                    if(this.gameCntx.LocalPlayer.Lives < 0) 
                    {
                        this.state = GameState.GameEnding;
                        startTime = GameGlobal.Util.TimeStart();
                        this.activeMinigame.EndGame();
                        this.gameCntx.EndGame();
                    }

                    // pokud je minihra u konce prejde do stavu "MiniGame_Ending
                    if (this.activeMinigame.IsGameOver())
                    {
                        // ukonci mini hru
                        if(this.activeMinigame.EndGame()) {
                            ShowCountDown(0, true, "Win");   
                        } else {
                            ShowCountDown(0, true, "Lose");    
                        }

                        this.state = GameState.MiniGameEnding;
                        Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "GO TO MiniGameEnding"));
                        // zaznamenani startovniho casu pro odmereni minigame end timu
                        startTime = GameGlobal.Util.TimeStart();
                    }
                    break;


                case GameState.MiniGameEnding:
                    // pocka definovany cas a zobrazi viteze minihry
                    if (GameGlobal.Util.TimePassed(startTime, MINIGAME_END_TIME))
                    {
                        // skryje time label 
                        TimeCallBack("");

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
                            this.state = GameState.GameEnding;
                            startTime = GameGlobal.Util.TimeStart();
                            this.gameCntx.EndGame();
                        }
                    }
                    break;


                case GameState.GameEnding:
                    // zobrazi info a skryje time label
                    ShowCountDown(0, true, "Game Over");
                    TimeCallBack("");

                    // odpocet do ukonceni areny
                    if (GameGlobal.Util.TimePassed(startTime, GAME_END_TIME))
                    {
                        // ulozi herni statistiku do player pref
                        GameGlobal.DataTransmissions.Instance.SaveData("final_stats", this.gameCntx.GameStats.Clone());
                        // odstrani kontext
                        this.gameCntx = null;
                        // prechod do sceny a zobrazeni vitezu celkove hry
                        SceneManager.LoadScene(GameGlobal.Scene.GAME_OVER);
                    }
                    break;
            }

            // no in minigame akce
            if(this.state != GameState.MiniGameRunning) 
            {
                // automaticky respawn hrace bez odebirani zivotu
                if(this.gameCntx != null) 
                {
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
            if(activeMinigame != minigames[rng]) {
                this.sameGameCounter = 0;
            } else {
                this.sameGameCounter++;  
            }
            if(this.sameGameCounter >= 2) {
                rng++;
                rng = rng % minigames.Count;     
            }
            activeMinigame = minigames[rng];
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "MiniGame set on: " + activeMinigame.GetName()));
        }
        
        /// <summary>
        /// Zobrazi odpocet (do startu cele hry/minihry))
        /// </summary>
        /// <param name="number">Zobrazevane cislo "odpocet"</param>
        /// <param name="show">True -> cislo bude zobrazeno, false -> cislo bude skryto</param>
        /// <param name="zeroText">Text ktery ze zobrazi misto cisla 0 v odpoctu</param>
        private int lastNum;
        private void ShowCountDown(int number, bool show, string zeroText)
        {
            // zobrazeni / skryti textu
            this.countdownText.enabled = show;

            // prehraje zvuk pokud doje ke zmene predchoziho cisla
            if(this.lastNum != number && number != 0) 
            {
                this.tickSound.Play();
            }
            this.lastNum = number;

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
                        if(!this.nestBreakSound.isPlaying) {
                            this.nestBreakSound.Play();
                        }
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

        /// <summary>
        /// Zobrazi obrazek ve hre. Obrazek zustava zobrazen dokud neni nahrazen jinym. Pokud dojde null hodnota, predchozi obrazek je skryt.
        /// </summary>
        /// <param name="image">2D Texture / obrazek ktery ma byt zobrazen</param>
        private void ImageCallback(Texture2D image) {
            if(image == null) {
                this.imageDisplayObj.SetActive(false);
            } else {
                this.imageDisplayObj.SetActive(true);
                RawImageDrawer.DrawTexture(this.imageDisplay, image);
            }
        }

        /// <summary>
        /// Nastavi cas zbyvajici do konce minihry.
        /// </summary>
        /// <param name="time">Cas/string</param>
        private void TimeCallBack(string time) {
            this.timeText.SetText(time);
        }

        /// <summary>
        /// Refresh hernich statistiky lokalniho hrace (zivoty, skore, nazev hry/stav)
        /// </summary>
        private void RefreshGameStats() {
            if(this.gameCntx == null) return;

            this.playerLiveText.SetText(Mathf.Max(0, this.gameCntx.LocalPlayer.Lives).ToString() + " " + '\u2665');
            this.playerScoreText.SetText(Mathf.Round(this.gameCntx.LocalPlayer.Score).ToString() + " " + '+');
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
