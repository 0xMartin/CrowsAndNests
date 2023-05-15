using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Game.MiniGameUtils;
using Enity;

namespace Game.MiniGame
{

    /// <summary>
    /// Implemetace hlavni herni logiky teto minihry:
    /// 1. Nahodne se vybere jedno hnizdo
    /// 2. Vybrane hnizdo se zvyrazni
    /// 3. Po chvili se spawne nahore predmet ktery pada na hnizdo
    /// 4. Po dopadu znici hnizdo
    /// 5. Proces se opakuje dokud nezustane jen jedno hnizdo
    /// </summary>
    public class MG_DontLookUp : MiniGameObj
    {
        [Header("Config")]
        public int MIN_TIME = 2; /** Minimalni delka cekani nez bude padaji predmet spawnut */
        public int MAX_TIME = 6; /** Maximalni delka cekani nez bude padaji predmet spawnut */

        [Header("Prefabs")]
        public GameObject fallingGear;  /** Padajici ozubene kolo */
        public GameObject signal;       /** Signalizacni objekt */

        [Header("Sounds")]
        public GameObject hitSoundObj;  /** Zvuk srazky padajiciho predmetu */


        private enum State {
            RandomSelect,       /** Nahodne vybere hnizdo */
            ShowAndWait,        /** Zobrazi signaliaci + ceka nahodny cas v definovanem rozsahu */
            SpawnGear,          /** Spawn gear */
            WaitForCollision,   /** Ceka dokud nedojde ke kolizi */    
            Hit                 /** Zasah -> odstraneni hnizd */
        }
        private State state;

        private int randomPosition; /** Nahodne vygenerovana pozice */ 

        private List<int> existingNests; /** Saznam ID hnizd ktere jeste existuji ve hre */

        private GameObject objToRemove; /** Reference na objekt = pro pozdejsi odstraneni */

        private float timeToWait; /** Cas cekani nez spawne objekt */

        private float startTime; /** Prommena pro casovani stavu hry */

        public override bool EndGame()
        {
            for(int i = 0; i < this.cntx.Nests.Length; ++i) {
                this.cntx.ShowNest(i);
            }
            return false;
        }

        public override string GetName()
        {
            return "Dont look up";
        }

        public override void ReinitGame(MiniGameContext cntx)
        {
            this.cntx = cntx;
            this.existingNests = new List<int>();
            for(int i = 0; i < this.cntx.Nests.Length; ++i) {
                this.existingNests.Add(i);
            }
        }

        public override bool IsGameOver()
        {
            // konec hry pokud uz zbyva jen jedno hnizdo
            return existingNests.Count <= 1;
        }

        public override void RunGame()
        {
            this.state = State.RandomSelect;
        }

        public override void UpdateGame()
        {
            // logika minihry
            switch(this.state) {
                case State.RandomSelect:
                    // nahodny vyber
                    if(SelectRandomNest()) {
                        // signalizace
                        this.objToRemove = Instantiate(this.signal);
                        this.objToRemove.transform.parent = this.cntx.Nests[this.randomPosition].transform;
                        this.objToRemove.transform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);
                        
                        // vygeneruje nahodne cas cekani v urcitem rozsahu
                        this.timeToWait = Random.Range(this.MIN_TIME, this.MAX_TIME);

                        // prechod na dalsi stav
                        this.state = State.ShowAndWait;
                        startTime = GameGlobal.Util.TimeStart();
                    }
                    break;

                case State.ShowAndWait:
                    if (GameGlobal.Util.TimePassed(startTime, this.timeToWait)) {
                        // odstraneni signalizace
                        Destroy(this.objToRemove);
                        this.objToRemove = null;

                        // prechod na dalsi stav
                        this.state = State.SpawnGear;     
                    }
                    break;

                case State.SpawnGear:
                    // spawn ozubeneho kola
                    this.objToRemove = Instantiate(this.fallingGear);
                    this.objToRemove.GetComponent<CollisionDetection>().OnCollisionDetected += HandleCollision;
                    this.objToRemove.transform.parent = this.cntx.Nests[this.randomPosition].transform;
                    this.objToRemove.transform.localPosition = new Vector3(0.0f, 60.0f, 0.0f);

                    // prechod na dalsi stav
                    this.state = State.WaitForCollision;    
                    break;

                case State.WaitForCollision:
                    // nic - k prechodu stavu dochazi pri detekci kolize
                    break;

                case State.Hit:
                    Destroy(this.objToRemove);
                    this.cntx.HideNest(this.randomPosition);

                    // prechod na dalsi stav
                    this.state = State.RandomSelect;   
                    break;
            }

            // time info label refresh
            this.cntx.showTime("/" + (this.cntx.Nests.Length - 1));

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
        /// Nahodne vybere indexe jedoho hnizda ktere se jeste nachazi v arene
        /// </summary>
        private bool SelectRandomNest() {
            if(this.existingNests.Count <= 1) return false;

            int i = Random.Range(0, this.existingNests.Count);
            this.randomPosition = this.existingNests[i];
            this.existingNests.RemoveAt(i);   

            return true;
        }

        /// <summary>
        /// Pokud je detekovana kolize spawnuteho ozubeneho kola tak je vyvolan prechod do stavu "Hit"
        /// </summary>
        private void HandleCollision() {
            if(this.state == State.WaitForCollision) {
                this.state = State.Hit;
            }
        }

    }

}