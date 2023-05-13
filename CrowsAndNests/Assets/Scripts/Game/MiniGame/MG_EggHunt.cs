using UnityEngine;

namespace Game.MiniGame
{

    public class MG_EggHunt : MiniGameObj
    {
        protected MiniGameContext cntx {get; private set; }

        public void EndGame()
        {
            throw new System.NotImplementedException();
        }

        public string GetName()
        {
            return "Egg Hunt";
        }

        public void InitGame(MiniGameContext cntx)
        {
            this.cntx = cntx;
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MG_EggHunt), " init done!"));
        }

        public bool IsGameOver()
        {
            return false;
        }

        public void RunGame()
        {
            throw new System.NotImplementedException();
        }

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
        public void UpdateGame()
        {
            this.cntx.IsPlayerDropDown(this.cntx.Players[0]);
        }

    }

}