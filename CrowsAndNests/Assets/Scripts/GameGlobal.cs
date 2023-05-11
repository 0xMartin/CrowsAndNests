using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameGlobal
{

    /// <summary>
    /// Obsahuje ID jednotlivych scen hry a zakladni funkce
    /// </summary>
    public static class Scene {
        // aktualni scena
        public static int current_scene = -1;

        public static readonly int MAIN_MENU = 0;
        public static readonly int ARENA_MENU = 1;
        public static readonly int MULTIPLAYER_MENU = 2;
        public static readonly int SETTINGS_MENU = 3;

        public static readonly int ARENA = 4;
        public static readonly int GAME_OVEW = 5;

        /// <summary>
        /// Aplikaci naviguje do definovane sceny
        /// </summary>
        /// <param name="sceneID">ID cilove sceny</param>
        public static void NavigateTo(int sceneID) {
            Scene.current_scene = sceneID;
            SceneManager.LoadScene(sceneID);
        }
    }

    /// <summary>
    /// Tato staticka trida obsahuje globalne uzivatene nastroje
    /// </summary>
    public static class Util {


        /// <summary>
        /// Funkce pro sestaveni zpravy, ktera je uricana primarne pro vypisu do aplikacnich logu
        /// </summary>
        /// <param name="type">Typ tridy, ve ktere se tato funkce vola. Pouzit command "typeof()"</param>
        /// <param name="message">Zprava, ktera bude ve zprave obsazena</param>
        /// <returns>Sestavenou zpravu ve formatu [{TYPE_TRIDY}] >> {ZPRAVA} </returns>
        public static string BuildMessage(System.Type type, string message) {
            return "[" + type.Name + "] >> " + message;
        }

        /// <summary>
        /// Navrati aktualni aplikacni cas. Funkce se vyuziva v kombinaci s funkci 
        /// "time_passed(start, time)". Urceno pro neblokujici mereni casu, napr.: v update metodach...
        /// </summary>
        public static float Time_start() {
            return Time.time;
        }

        /// <summary>
        /// Overi zda jiz neuplinul definovany cas. Pouziva se v kombinaci s funkci "time_start()".
        /// </summary>
        /// <param name="start">Cas pocatku mereni casoveho useku (v sekundach)</param>
        /// <param name="time">Celkova doba cekani (v sekundach)</param>
        /// <returns>Navrati "true" pokud cas od priniho namereneho casu "start" bude vetsi nez cas definovany v argumentu "time"</returns>
        public static bool Time_passed(float start, float time) {
            return (Time.time - start) > time;
        }

        /// <summary>
        /// Navrati cas zbyvajici do uplynuti definovaneho casoveho useku. Pouziva se v kombinaci s funkci "time_start()".
        /// </summary>
        /// <param name="start">Cas pocatku mereni casoveho useku (v sekundach)</param>
        /// <param name="time">Celkova doba cekani (v sekundach)</param>
        /// <returns>Pocet zbyvajicich sekund</returns>
        public static float Time_remaining(float start, float time) {
            return Mathf.Max((time + start) - Time.time, 0);
        }
    }

}
