using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MiniGameUtils;

public class ArenaController : MonoBehaviour
{

    /*********************************************************************************************************/
    // VEREJNE PROMENNE [CONFIG]
    public int GAME_START_TIME = 30;    /** Za jak dlouho se spusti hra (v sekundach)  */
    public int MATCH_WAIT_TIME = 5;     /** Za jak dlouho zacne dalsi minihra po skonceni posledni minihry (v sekundach) */
    public int MAX_PLAYERS = 4;         /** Maximalni pocet hracu (1 - 4 & min <= max) */
    public int MIN_PLAYERS = 1;         /** Minimalni pocet hracu (1 - 4 & min <= max) */

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

    /*********************************************************************************************************/
    // LOKALNI PROMENNE
    private MiniGameUtils.MiniGameContext gameCntx; /** Kontext mini hry */

    private List<MiniGame> minigames; /** Seznam dostupnych miniher */
    private MiniGame activeMinigame; /** Aktivni minihra */

    void Start()
    {
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
        this.gameCntx = new MiniGameContext(nests, spawns);

        // vytvoreni mini her
        this.minigames = new List<MiniGame>();

        Debug.Log(GameGlobal.Util.buildMessage(typeof(ArenaController), "Init done"));  
    }

    void Update()
    {

    }

    private void randomMiniGameSelect() {
        int rng = Random.Range(0, minigames.Count);
        activeMinigame = minigames[rng];  
        Debug.Log(GameGlobal.Util.buildMessage(typeof(ArenaController), "MiniGame set on: " + activeMinigame.getName()));  
    }

}
