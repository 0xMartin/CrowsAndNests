using System.Collections.Generic;
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
        public static readonly int ARENA = 1;
        public static readonly int GAME_OVER = 2;

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
        public static float TimeStart() {
            return Time.time;
        }

        /// <summary>
        /// Overi zda jiz neuplinul definovany cas. Pouziva se v kombinaci s funkci "time_start()".
        /// </summary>
        /// <param name="start">Cas pocatku mereni casoveho useku (v sekundach)</param>
        /// <param name="time">Celkova doba cekani (v sekundach)</param>
        /// <returns>Navrati "true" pokud cas od priniho namereneho casu "start" bude vetsi nez cas definovany v argumentu "time"</returns>
        public static bool TimePassed(float start, float time) {
            return (Time.time - start) > time;
        }

        /// <summary>
        /// Navrati cas zbyvajici do uplynuti definovaneho casoveho useku. Pouziva se v kombinaci s funkci "time_start()".
        /// </summary>
        /// <param name="start">Cas pocatku mereni casoveho useku (v sekundach)</param>
        /// <param name="time">Celkova doba cekani (v sekundach)</param>
        /// <returns>Pocet zbyvajicich sekund</returns>
        public static float TimeRemaining(float start, float time) {
            return Mathf.Max((time + start) - Time.time, 0);
        }

        /// <summary>
        /// Prevede sekundy na formatovany cas "00:00"
        /// </summary>
        /// <param name="seconds">Cas v sekundach</param>
        /// <returns>Formatovani string obsahuji cas</returns>
        public static string FormatTime(float seconds) {
            int min = Mathf.FloorToInt(seconds / 60f);
            int sec = Mathf.FloorToInt(seconds % 60f);
            return string.Format("{0:00}:{1:00}", min, sec);
        }
    }

    /// <summary>
    /// Trida umoznujici prenosy dat mezi moduly
    /// </summary>
    public class DataTransmissions {

        private static DataTransmissions _instance;

        public static DataTransmissions Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DataTransmissions();
                    _instance.dict = new Dictionary<string, object>();
                }
                return _instance;
            }
        }

        private Dictionary<string, object> dict;

        /// <summary>
        /// Generická metoda pro ukládání dat
        /// </summary>
        /// <param name="key">Klic dat</param>
        /// <param name="data">Data</param>
        /// <typeparam name="T">Typ dat</typeparam>
        public void SaveData(string key, object data)
        {
            this.dict[key] = data;
        }

        /// <summary>
        /// Generická metoda pro získávání dat
        /// </summary>
        /// <param name="key">Klic dat</param>
        /// <typeparam name="T">Typ dat</typeparam>
        /// <returns>Ulozena data</returns>
        public T LoadData<T>(string key)
        {
            object data = null;
            this.dict.TryGetValue(key, out data);
            return (T) data;
        }

    }

}
