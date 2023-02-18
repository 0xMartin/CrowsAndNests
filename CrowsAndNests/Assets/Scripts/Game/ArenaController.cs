using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniGameUtils;

/// <summary>
/// Controller areny. Jde o hlavni komponentu hry (veskere rozhodovani).
/// </summary>
public class ArenaController : MonoBehaviour
{

    /*********************************************************************************************************/
    // VEREJNE PROMENNE [CONFIG]
    public int GAME_START_TIME = 30;        /** Za jak dlouho se spusti hra (v sekundach)  */
    public int GAME_END_TIME = 10;          /** Jak dloho ma hra cekat jeste po jejim ukonceni (v sekundach)  */
    public int MINIGAME_START_TIME = 10;    /** Za jak dlouho zacne dalsi minihra po zkonceni posledni minihry (v sekundach) */
    public int MINIGAME_END_TIME = 5;       /** Jak dlouho bude minihrace cekat po jejim konci (v sekundach) */
    public int MAX_PLAYERS = 4;             /** Maximalni pocet hracu (1 - 4 & min <= max) */
    public int MIN_PLAYERS = 1;             /** Minimalni pocet hracu pro hru (1 - 4 & min <= max) [pro single player=1, pro multiplayer=2, jine konfigurace jsou taktez mozne] */

    /*********************************************************************************************************/
    // VEREJNE PROMENNE [REFERENCES]

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

    /*********************************************************************************************************/
    // LOKALNI PROMENNE
    private MiniGameUtils.MiniGameContext gameCntx; /** Kontext mini hry */

    private List<MiniGame> minigames; /** Seznam dostupnych miniher */
    private MiniGame activeMinigame; /** Aktivni minihra */

    // aktualni stav ve kterem se hra nachazi
    private enum GameState
    {
        Not_Running, /** Hra jeste neni spustena/nakonfigurovana */
        Game_Starting, /** Hra je jiz spustene. Bezi odpocet do spusteni hry*/

        // tyto 4 faze se toci porad dokola dokud je dostatek hracu
        MiniGame_Selecting, /** V teto fazi se vybira minihra ktera se bude hrat */
        MiniGame_Starting, /** Minihra byla vybrana. Bezi odpocet nez se minihra spusti */
        MiniGame_Running, /** Minihra bezi. V teto fazi hraci uz hraji/muzou umirat. */
        MiniGame_Ending, /** Minihra skoncila, kratky cas pro rekapitulaci vitezu teto minihry a pricteni skore. */

        Game_Ending /** Hra skoncila. Kratky cas a pak presmerovani do final lobby se zobrazenymi vysledky */
    };
    private GameState state;

    private float startTime; /** promenna pro uchovavani casu */

    void Start()
    {
        // defaultni stav hry
        this.state = GameState.Not_Running;

        // vytvoreni mini her
        this.minigames = new List<MiniGame>();

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
            new MiniGameContext.FxCreateRequest(fxCallback)
        );

        Debug.Log(GameGlobal.Util.buildMessage(typeof(ArenaController), "Init done"));
    }

    void Update()
    {
        if (this.gameCntx == null) return;

        float remaining;

        // stavovy automat (hlavni herni rozhodovaci smycka)
        switch (this.state)
        {
            case GameState.Not_Running:
                // pokud je hra spusta prejde do statvu "Game_Starting"
                if (this.gameCntx.IsGameRunning)
                {
                    this.state = GameState.Game_Starting;
                }

                // zaznamenani startovniho casu pro odpocet startu cele hry
                startTime = GameGlobal.Util.time_start();
                break;


            case GameState.Game_Starting:
                // odpocet do spusteni hry
                remaining = GameGlobal.Util.time_remaining(startTime, GAME_START_TIME);
                showCountDown((int) remaining, true);

                // cas uplynul
                if(GameGlobal.Util.time_passed(startTime, GAME_START_TIME)) {
                    showCountDown(0, false);
                    this.state = GameState.MiniGame_Selecting;   
                }
                break;


            case GameState.MiniGame_Selecting:
                // vyber dalsi mini hry
                randomMiniGameSelect();
                if (this.activeMinigame == null)
                {
                    Debug.LogError(GameGlobal.Util.buildMessage(typeof(ArenaController), "Failed select random minigame"));
                    break;
                }

                // inicializace vybrane minihry
                this.activeMinigame.initGame(this.gameCntx);

                // hned prejde do stavu "MiniGame_Starting"
                this.state = GameState.MiniGame_Starting;

                // zaznamenani startovniho casu pro odpocet mini hry
                startTime = GameGlobal.Util.time_start();
                break;


            case GameState.MiniGame_Starting:
                // zobrazeni odpoctu do zacatku minihry
                remaining = GameGlobal.Util.time_remaining(startTime, MINIGAME_START_TIME);
                showCountDown((int) remaining, true);

                // pokud je odpocet u konce prejde do stavu "MiniGame_Running"
                if (GameGlobal.Util.time_passed(startTime, MINIGAME_START_TIME))
                {
                    showCountDown(0, false);
                    this.state = GameState.MiniGame_Running;
                }
                break;


            case GameState.MiniGame_Running:
                // vykonavani smycky aktualne spustene minihry
                this.activeMinigame.updateGame();

                // pokud je minihra u konce prejde do stavu "MiniGame_Ending
                if (this.activeMinigame.IsGameOver())
                {
                    this.state = GameState.MiniGame_Ending;
                    // zaznamenani startovniho casu pro odmereni minigame end timu
                    startTime = GameGlobal.Util.time_start();
                }
                break;


            case GameState.MiniGame_Ending:
                // pocka definovany cas a zobrazi viteze minihry
                if(GameGlobal.Util.time_passed(startTime, MINIGAME_END_TIME)) {
                    // spocita pocet zijicich hracu
                    if (this.gameCntx.countLivingPlayers() >= MIN_PLAYERS)
                    {
                        // pokud je dostatek hracu smycky se opakuje a jde znovu vybrat dalsi minihru
                        this.state = GameState.MiniGame_Selecting;
                    }
                    else
                    {
                        // pokud uz neni dostatek hracu je hra ukoncena
                        this.gameCntx.endGame();
                    }
                }
                break;


            case GameState.Game_Ending:
                // odpocet do ukonceni areny
                if(GameGlobal.Util.time_passed(startTime, GAME_END_TIME)) {
                    // odstrani kontext
                    this.gameCntx = null;
                    // prechod do sceny a zobrazeni vitezu celkove hry
                    //TODO>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>TODO
                }
                break;
        }

        // dodatecne prechodove pravidla
        if(this.gameCntx.IsGameEnd) {
            this.state = GameState.Game_Ending;   
            // zaznamenani startovniho casu pro odmereni game end timu
            startTime = GameGlobal.Util.time_start();
        }
    }

    private void createGame(string gameName)
    {
        if (this.gameCntx == null)
        {
            Debug.LogError(GameGlobal.Util.buildMessage(typeof(ArenaController), "Failed to create game. Context is null!"));
            return;
        }
        this.gameCntx.GameStats = new GameStats(gameName);
    }

    private void randomMiniGameSelect()
    {
        int rng = Random.Range(0, minigames.Count);
        activeMinigame = minigames[rng];
        Debug.Log(GameGlobal.Util.buildMessage(typeof(ArenaController), "MiniGame set on: " + activeMinigame.getName()));
    }

    private void showCountDown(int number, bool show) {
        // zobrazeni cisla odpoctu
        //TODO>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>TODO
    }

    private void showMiniGameInfo(string info, bool show) {
        // zobrazi info o mini hre (kratky popis jak ji hrat)
        //TODO>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>TODO
    }

    private void fxCallback(string type, Vector3 pos, Quaternion rot)
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

}
