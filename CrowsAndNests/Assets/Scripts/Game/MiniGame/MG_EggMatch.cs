using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.MiniGameUtils;

namespace Game.MiniGame
{

    /// <summary>
    /// Implemetace hlavni herni logiky teto minihry:
    /// 1. 
    /// </summary>
    public class MG_EggMatch : MiniGameObj
    {

        /************************************************************************************************************************/
        [Header("Config")]
        public int ROUND_COUNT = 3;         /** Pocet kol */
        public int SHOW_COUNT = 3;          /** Kolikrat bude ukazana kazda skupina kombinaci */
        public int INCREMENT_EGG_PER = 4;   /** Po kolika kolech ma inkrementovat pocet generovanych vajec na jedno hnizdo */
        public int INCREMENT_COMB_PER = 2;  /** Po kolika kolech ma inkrementovat pocet kombinaci */
        public int TIME_FOR_FINDING = 10;   /** Cas na hledani spravneho hnizda [sec] */
        public int TIME_FOR_SHOW = 3;       /** Cas na zobrazeni jedne skupiny [sec] */
        public int TIME_FOR_REMEMBER = 5;   /** Cas na zapamatovani [sec] */
        public int TIME_FOR_FINAL = 5;      /** Cas cekani ve finalni fazi [sec] */

        [Header("Prefabs")]
        public GameObject classicEgg; /** Vejce nejde rozbit */

        [Header("Egg Colors")]
        public Material whiteColor;
        public Material redColor;
        public Material greenColor;
        public Material blueColor;
        public Material yellowColor;
        public Material magentaColor;
        /************************************************************************************************************************/

        private int current_round;  /** Aktualni kolo */
        private int comb_count;     /** aktualni pocet kombinaci */
        private int egg_count;      /** aktualni pocet vajec */

        private enum State {
            Generate,   /** vygenerovani kombinaci + text "Dalsi kolo" */
            Show,       /** zobrazovani kombinaci (stridani 2 skupin - 3x 1., 3x 2.) */
            Wait,       /** cekani (definovany cas) */
            Finding,    /** zobrazeni hledaneho obrazku + cekani */
            Remove,     /** odstraneni neplatnych hnizd + cekani */
            Reset       /** clear & reset */
        }
        private State state; /** Aktualni stav minihry */

        private enum EggColor {
            White, Red, Green, Blue, Yellow, Magenta
        };
        private List<List<EggColor>> combinations; /** Seznam vsech kombinaci */

        private List<bool> showGroups; /** Zobrazovane skupiny vajec */
        private int showGroupCounter; /** Citac pro rizeni zobrazovani skupin */

        private List<int> combMap; /** Map kombinaci obsahujici informace o tom na jakem hnizde se nachazi jaka kombinace */

        private List<GameObject> eggs; /** Seznam game objectu vajec aktualne spawnutych ve hre */

        private float startTime;

        public override bool EndGame()
        {
            return false;
        }

        public override string GetName()
        {
            return "Egg Match";
        }

        public override bool IsGameOver()
        {
            // konec hry kdyz jsou vsichni hraci mrtvi (bez dalsich zivotu) nebo pokud byli odehrany  vsechny kola
            return this.cntx.Players.All(p => p.Lives < 0) || this.current_round > this.ROUND_COUNT;
        }

        public override void ReinitGame(MiniGameContext cntx)
        {
            base.cntx = cntx;
            this.current_round = 0;
            this.eggs = new List<GameObject>();
            this.combinations = new List<List<EggColor>>();
            this.showGroups = new List<bool>();
            this.combMap = new List<int>();
        }

        public override void RunGame()
        {
            this.state = State.Generate;
            this.comb_count = 2 + (int) Mathf.Floor(this.cntx.GameStats.GameCount / this.INCREMENT_COMB_PER);
            this.comb_count = this.comb_count > 16 ? 16 : this.comb_count;
            this.egg_count = 1 + (int) Mathf.Floor(this.cntx.GameStats.GameCount / this.INCREMENT_EGG_PER);
            this.egg_count = this.egg_count > 4 ? 4 : this.egg_count;
        }

        public override void UpdateGame()
        {
            // hlavni logika
            switch(this.state) {
                case State.Generate:
                    Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggMatch), "Combination Generating ..."));

                    // dalsi kolo
                    this.current_round += 1;

                    // vygeneruje kombinace
                    this.GenerateComb(this.comb_count, this.egg_count);

                    // vygeneruje zobrazovaci masku
                    this.GenerateGroupMask();

                    // vygeneruje mapu kombinaci 
                    this.GenerateCombinationMap();

                    this.state = State.Show;
                    startTime = GameGlobal.Util.TimeStart();
                    break;

                case State.Show:
                    if (GameGlobal.Util.TimePassed(startTime, TIME_FOR_SHOW)) {
                        Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggMatch), $"Show combination [{this.showGroupCounter}] ..."));

                        // zobrazeni skupiny
                        ShowEggGroup(this.showGroupCounter % 2 == 0);

                        // konec zobrazovani
                        if(this.showGroupCounter >= this.SHOW_COUNT * 2) {
                            this.state = State.Wait;
                            startTime = GameGlobal.Util.TimeStart();       
                        }
                    }
                    break;

                case State.Wait:
                    //Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggMatch), "Wait time ..."));
                    break;

                case State.Finding:
                    Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggMatch), "Finding time ..."));
                    break;

                case State.Remove:
                    Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggMatch), "Remove incorrect nests ..."));
                    break;

                case State.Reset:
                    Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggMatch), "Reset mini game ..."));
                    break;
            }

            // time info label refresh
            this.cntx.showTime(this.current_round.ToString() + "/" + this.ROUND_COUNT.ToString() + "<br>" + "00:00");
        }

        /// <summary>
        /// Vygeneruje kombinace vajec
        /// </summary>
        /// <param name="combCount">Pozadovany pocet generovanych kombinaci (pokud to neni mozne tak jich vygeneruje min)</param>
        /// <param name="eggCount">Pocet vajec v kombinacich</param>
        private void GenerateComb(int combCount, int eggCount) {
            this.combinations = new List<List<EggColor>>();

            for (int i = 0; i < 200 && this.combinations.Count < combCount; ++i)
            {
                var colors = new List<EggColor>();
                while (colors.Count < eggCount)
                {
                    var color = (EggColor)Random.Range(0, 6);
                    colors.Add(color);
                }

                if (!this.combinations.Any(c => IdenticalCombination(c, colors)))
                {
                    this.combinations.Add(colors);
                }
            }

            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggMatch), "Generated combinations: " + this.combinations.Count.ToString()));
        }

        /// <summary>
        /// Overi zda kombinace barve vajec jsou shodne (nezalezi na poradi)
        /// </summary>
        /// <param name="comb1">Kombinace 1</param>
        /// <param name="comb2">Kombinace 2</param>
        /// <returns>True = shoduji se</returns>
        private bool IdenticalCombination(List<EggColor> comb1, List<EggColor> comb2) {
            if(comb1.Count != comb2.Count) {
                return false;
            }     

            List<EggColor> comb2_copy = comb2.ToList();

            // v kombinaci 2 postupne hleda vsechny prvky z kombinace 1
            foreach(EggColor c1 in comb1) {
                if(comb2_copy.Any(c2 => c2 == c1)) {
                    // naleze v kombinaci 2 => odstrani ho
                    comb2_copy.Remove(c1);
                } else {
                    // nenaleze => nejsou shodne
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Vygeneruje masky pro 2 skupin (pri ukozavani se zobrazuje vzdy jen jedna ze skupin)
        /// </summary>
        /// <param name="groups">Pocet skupin</param>
        private void GenerateGroupMask() {
            // resetuje citac 
            this.showGroupCounter = 0;

            // vygeneruje list (true ... | false ...)
            this.showGroups = new List<bool>();
            int perGroup = this.cntx.Nests.Length / 2;
            for(int i = 0; i < this.cntx.Nests.Length; ++i) {
                this.showGroups.Add(i < perGroup);    
            }

            // Náhodně promíchat prvky v listu
            int count = this.showGroups.Count;
            while (count > 1) {
                count--;
                int index = Random.Range(0, count + 1);
                bool temp = this.showGroups[index];
                this.showGroups[index] = this.showGroups[count];
                this.showGroups[count] = temp;
            }       

            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggMatch), "Show group mask generated"));
        }

        /// <summary>
        /// Vygeneruje mapu kombinaci
        /// </summary>
        private void GenerateCombinationMap() {
            this.combMap = new List<int>();
            int length = this.cntx.Nests.Length;

            // naplni list volnymi indexy
            List<int> freeIndexList = new List<int>();
            for(int i = 0; i < length; ++i) {
                freeIndexList.Add(i);    
            }

            // nahradi vsechny volne index nahodnym cislem kombinace
            while(freeIndexList.Count > 0) {
                for (int comb_id = 0; comb_id < Mathf.Min(this.combinations.Count, length); comb_id++) {
                    int j = Random.Range(0, freeIndexList.Count);
                    this.combMap[freeIndexList[j]] = comb_id; // id kombinace zapise do mapy kombinace na nahodny index
                    freeIndexList.Remove(j);
                }
            }

            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggMatch), "Combination map generated"));
        }

        /// <summary>
        /// Zobrazi/spawne skupinu vajce (prvni/druhy)
        /// </summary>
        /// <param name="group">Selektor skupiny</param>
        private void ShowEggGroup(bool group) {
            int length = this.cntx.Nests.Length;

            // odstraneni predchozich kombinaci ze sceny
            foreach(GameObject obj in this.eggs) {
                Destroy(obj);
            }

            // zobrazeni kombinaci dane skupiny 
            for(int i = 0; i < length; ++i) {
                if(this.showGroups[i] == group) {
                    // kombinace na pozici "i"
                    List<EggColor> comb = this.combinations[i];

                    // vytvoreni instance kombinace
                    switch(comb.Count) {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                    }
                }
            }
        }

    }

}