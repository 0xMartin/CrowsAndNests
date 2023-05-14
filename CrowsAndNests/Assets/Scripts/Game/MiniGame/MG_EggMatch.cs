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
        public int ROUND_COUNT = 3;          /** Pocet kol */
        public int SHOW_COUNT = 3;          /** Kolikrat bude ukazana kazda skupina kombinaci */
        public int INCREPENT_EGG_PER = 4;   /** Po kolika kolech ma inkrementovat pocet generovanych vajec na jedno hnizdo */
        public int INCREMENT_COMB_PER = 2;  /** Po kolika kolech ma inkrementovat pocet kombinaci */
        public int TIME_FOR_FINDING = 10;   /** Cas na hledani spravneho hnizda [sec] */
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

        private List<GameObject> eggs; /** Seznam game objectu vajec spawnutych ve hre */

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
        }

        public override void RunGame()
        {
            this.state = State.Generate;
        }

        public override void UpdateGame()
        {
            // time info label refresh
            this.cntx.showTime(this.current_round.ToString() + "/" + this.ROUND_COUNT.ToString() + "<br>" + "00:00");
            
            // hlavni logika
            switch(this.state) {
                case State.Generate:
                    Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggMatch), "Combination Generating ..."));

                    this.current_round += 1;
                    this.GenerateComb(this.comb_count, this.egg_count);
                    this.state = State.Show;

                    foreach(List<EggColor> comb in this.combinations) {
                        string str = "";
                        foreach(EggColor c in comb)
                            str += c.ToString() + ", ";
                        Debug.Log("-> " + str);
                    }

                    break;

                case State.Show:
                    //Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggMatch), "Show combination ..."));
                    break;

                case State.Wait:
                    Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggMatch), "Wait time ..."));
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

    }

}