using UnityEngine;

namespace Game.MiniGameUtils {

    /// <summary>
    /// rozhrani pro minihru
    /// </summary>
    public abstract class MiniGameObj : MonoBehaviour
    {
        protected MiniGameContext cntx {get; set; }

        /// <summary>
        /// Navrati jmeno minihry
        /// </summary>
        /// <returns>Jmeno minihry</returns>
        public abstract string GetName();

        /// <summary>
        /// Inicializuje/Reincializuje minihru
        /// </summary>
        /// <param name="cntx">Reference na MiniGameContext</param>
        public abstract void ReinitGame(MiniGameContext cntx);

        /// <summary>
        /// Update metoda minihry
        /// </summary>
        public abstract void UpdateGame();

        /// <summary>
        /// Spusti minihru
        /// </summary>
        public abstract void RunGame();

        /// <summary>
        /// Ukonci minihru
        /// </summary>
        /// <returns>Navrati 'true' pokud lokalni hrac vyhral minihru</returns>
        public abstract bool EndGame();

        /// <summary>
        /// Overi jestli minihra neskoncila uz
        /// </summary>
        /// <returns>True -> minihra jiz zkoncila</returns>
        public abstract bool IsGameOver();
    }

}