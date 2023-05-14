using System;
using System.Collections.Generic;

namespace Game.MiniGameUtils {

    /// <summary>
    /// statistiky hry (celkove => vsechny odehrane minihry)
    /// </summary>
    public class GameStats : ICloneable
    {
        public string GameName { get; set; } /** Jmeno hry. Volene hostitelem areny */

        public int GameCount { get; set; } /** Kolik her bylo jiz odehrani.  */

        public DateTime GameStart { get; set; } /** Cas zapoceti hry */

        public DateTime GameEnd { get; set; } /** Cas zapoceti hry */

        public List<string> NameList { get; set; } /** List jmen vsech hracu */

        public List<float> ScoreList { get; set; } /** List skore vsech hracu */

        private GameStats() 
        {
            this.NameList = new List<string>(); 
            this.ScoreList = new List<float>(); 
        }

        public GameStats(string gameName)
        {
            this.NameList = new List<string>(); 
            this.ScoreList = new List<float>(); 
            this.GameName = gameName;
            this.GameCount = 0;
            this.GameStart = DateTime.Now;
        }

        public object Clone()
        {
            GameStats stats = new GameStats() {
                GameName = this.GameName,
                GameCount = this.GameCount,
                GameStart = this.GameStart,
                GameEnd = this.GameEnd
            };

            foreach(string name in this.NameList) {
                stats.NameList.Add(name);
            }

            foreach(float score in this.ScoreList) {
                stats.ScoreList.Add(score);
            }

            return stats;
        }
    }

}