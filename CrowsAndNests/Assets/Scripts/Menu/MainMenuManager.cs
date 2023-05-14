using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Menu
{

    /// <summary>
    /// Manager pro hlavni menu hry
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        public Button buttonSingle, buttonMulti, buttonSettings, buttonExit;

        void Start()
        {
            if (buttonSingle != null)
                buttonSingle.onClick.AddListener(OnClick_ButtonSingle);
            else
                Debug.LogError(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Faild to init listener for Button Singleplayer"));

            if (buttonMulti != null)
                buttonMulti.onClick.AddListener(OnClick_ButtonMulti);
            else
                Debug.LogError(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Faild to init listener for Button Multiplayer"));

            if (buttonSettings != null)
                buttonSettings.onClick.AddListener(OnClick_ButtonSettings);
            else
                Debug.LogError(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Faild to init listener for Button Settings"));

            if (buttonExit != null)
                buttonExit.onClick.AddListener(OnClick_ButtonExit);
            else
                Debug.LogError(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Faild to init listener for Button Exit"));

            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Setup Done"));
        }

        void Awake() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        void OnClick_ButtonSingle()
        {
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Button Singleplayer clicked"));
            SceneManager.LoadScene(GameGlobal.Scene.ARENA);
        }

        void OnClick_ButtonMulti()
        {
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Button Multiplayer clicked"));
            SceneManager.LoadScene(GameGlobal.Scene.MULTIPLAYER_MENU);
        }

        void OnClick_ButtonSettings()
        {
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Button Settings clicked"));
            SceneManager.LoadScene(GameGlobal.Scene.SETTINGS_MENU);
        }

        void OnClick_ButtonExit()
        {
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Button Exit clicked"));
            Application.Quit();
        }

    }

}
