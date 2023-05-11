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

        public void UpdateGame()
        {
            this.cntx.IsPlayerDropDown(this.cntx.Players[0]);
        }

    }

}