using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Menu
{

    /// <summary>
    /// Manager pro game over menu
    /// </summary>
    public class GameOverMenuManager : MonoBehaviour
    {
        public Button buttonBack;

        private void Start() {
            buttonBack.onClick.AddListener(GoToMenu);
        }

        void GoToMenu()
        {
            SceneManager.LoadScene(GameGlobal.Scene.MAIN_MENU);
        }

    }

}