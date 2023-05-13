using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Menu
{

    /// <summary>
    /// Manager pro menu areny
    /// </summary>
    public class ArenaMenuManager : MonoBehaviour
    {
        public Button buttonBack, buttonCreate;

        void Start()
        {
            if (buttonBack != null)
                buttonBack.onClick.AddListener(OnClick_ButtonBack);
            else
                Debug.LogError(GameGlobal.Util.BuildMessage(typeof(ArenaMenuManager), "Faild to init listener for Button Back"));

            if (buttonCreate != null)
                buttonCreate.onClick.AddListener(OnClick_ButtonCreate);
            else
                Debug.LogError(GameGlobal.Util.BuildMessage(typeof(ArenaMenuManager), "Faild to init listener for Button Create"));

            Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaMenuManager), "Setup Done"));
        }
        
        void Awake() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void OnClick_ButtonBack()
        {
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaMenuManager), "Button Back clicked"));
            SceneManager.LoadScene(GameGlobal.Scene.MAIN_MENU);
        }

        void OnClick_ButtonCreate()
        {
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaMenuManager), "Button Create clicked"));
            SceneManager.LoadScene(GameGlobal.Scene.ARENA);
        }

    }

}
