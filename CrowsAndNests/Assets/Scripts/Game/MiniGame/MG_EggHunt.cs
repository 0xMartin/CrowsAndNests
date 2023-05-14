using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.MiniGameUtils;

namespace Game.MiniGame
{

    /// <summary>
    /// Implemetace hlavni herni logiky teto minihry:
    /// 1. nahodne se spawnuji vejce na vnizdech
    /// 2. je spawnuto vzdy jen definovany pocet vejci
    /// 3. hrac/hraci rozbijeji vejce a za to ziskavaji body
    /// 4. vyhrava ten kdo ziska nejvice bodu (single player = ziska definovany pocet bodu) za definovany cas
    /// 5. hraci ktery vyhral je pridelan jeden zivot
    /// + hraci pri padu dolu ztraceji jeden svuj zivot
    /// + hra je na cas
    /// </summary>
    public class MG_EggHunt : MiniGameObj
    {
        [Header("Config")]
        public int MAX_TIME_LIMIT = 70; /** Maximalni cas */
        public int MIN_EGGS_TO_BREAK = 5; /** Maximalni pozadovany pocet rozbitych vajec */
        public int EGGS_TO_SPAWN = 4;   /** Maximalni pocet spavnovanych vajce */

        [Header("Prefabs")]
        public GameObject breakableEgg; /** Vejce ktere jde rozbit*/

        /** 
         * Hashmap spawnutych vajec, pokud je vejce zniceno je odstraneno z mapy 
         */
        private Dictionary<BreakableEgg, int> eggMap = new Dictionary<BreakableEgg, int>(); 

        private int lastBreakID; /** ID pozice posledniho rozbiteho vejce */

        /** 
         * Seznam objektu ktere budou odstraneni pri konci 
         */
        private List<GameObject> toRemove = new List<GameObject>();

        private int score; /** Skore minihry = pocet rozbitych vajec */
        private int winScore; /** Skore pozadovane pro vizezstvi */
        private float time; /** Cas ktery ma hrac k dispozici */

        public override bool EndGame()
        {
            Invoke(nameof(Clear), 3.0f);

            // vitez minihry?
            if(this.score >= this.winScore) {
                // prida zivot
                this.cntx.AddPlayerLives(this.cntx.LocalPlayer, 1);
                // globalni skore +15
                this.cntx.LocalPlayer.Score += 15f;
                return true;
            }

            return false;
        }

        private void Clear() {
            //  vycisteni
            foreach(GameObject obj in toRemove) {
                if(obj != null) {
                    Destroy(obj);
                }
            }
        }

        public override string GetName()
        {
            return "Egg Hunt";
        }

        public override void ReinitGame(MiniGameContext cntx)
        {
            base.cntx = cntx;
            this.score = 0;
            this.winScore = this.MIN_EGGS_TO_BREAK + (int) Mathf.Floor((cntx.GameStats.GameCount - 1) / 2);
            this.time = this.MAX_TIME_LIMIT;
            this.eggMap.Clear();
            this.toRemove.Clear();
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggHunt), " init done!"));
        }

        public override bool IsGameOver()
        {
            // cas vyprsel nebo bylo dosazeno vitezne skore
            return this.time == 0 || this.score >= this.winScore;
        }

        public override void RunGame()
        {
            this.cntx.showTime("00:00<br>" + "0/" + this.winScore);
            this.SpawnRandomEgg(this.EGGS_TO_SPAWN);
        }

        public override void UpdateGame()
        {
            // cas
            this.time -= Time.deltaTime;
            this.time = this.time < 0.0f ? 0.0f : this.time;

            // time info label refresh
            this.cntx.showTime(GameGlobal.Util.FormatTime(this.time) + "<br>" + score + "/" + this.winScore);

            // drop down
            foreach(Player p in base.cntx.Players) 
            {
                if(base.cntx.IsPlayerDropDown(p)) 
                {
                    Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaController), "KILLED: " + p.ToString()));
                    base.cntx.KillPlayer(p);
                    base.cntx.RandomSpawnPlayer(p);
                    base.cntx.ClearUsedNestsStatus();    
                }    
            }
        }

        /// <summary>
        /// Nahodne spawne vejce
        /// </summary>
        /// <param name="count">Pocet spawnutych vajec (default = 1)</param>
        private void SpawnRandomEgg(int count = 1) {
            for(int i = 0; i < count && this.eggMap.Count < this.EGGS_TO_SPAWN; ++i) {
                int id = -1, j = 0;
                // nahodne vygeneruje pozici (takove hnizdo ktere je volne a zaroven nejde o hnizdo na ktere bylo vejce rozbito naposled)
                do {
                    id = (int) UnityEngine.Random.Range(0, this.cntx.Nests.Length);
                } while((this.eggMap.Values.Contains(id) || this.lastBreakID == id) && j++ < 100);

                if(id >= 0) {
                    // vytvori instanci noveho vejce a jako parent object mu priradi hnizdo
                    GameObject newEgg = Instantiate(this.breakableEgg);
                    this.toRemove.Add(newEgg);
                    newEgg.transform.parent = this.cntx.Nests[id].transform;
                    newEgg.transform.localPosition = new Vector3(0.0f, 0.32f, 0.0f);

                    // prida zaznam do egg mapy (pri rozbiti vejce bude hned odebran) a nastavi callback na rozbiti
                    BreakableEgg bec = newEgg.GetComponentInChildren<BreakableEgg>();
                    this.eggMap.Add(bec, id);
                    // nakonfiguruje skript pro rozbiti vejce
                    bec.playerTransform = this.cntx.LocalPlayer.ModelRef.transform;
                    bec.EggBreakCallback = new BreakableEgg.EggBreakAction(this.EggBreakCallback);
                }
            }
        }

        /// <summary>
        /// Callback aktivovan ve chvili kdy je vejce rozbite
        /// </summary>
        /// <param name="egg">Rozbite vejce</param>
        private void EggBreakCallback(BreakableEgg egg) {
            // globalni skore +0.5
            this.cntx.LocalPlayer.Score += 0.5f;

            // lokalni 
            int id;
            if(this.eggMap.TryGetValue(egg, out id)) {
                this.lastBreakID = id;
            }
            this.score++;
            this.eggMap.Remove(egg);  
            this.SpawnRandomEgg(); 
        }

    }

}