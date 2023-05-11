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
                buttonBack.onClick.AddListener(onClick_ButtonBack);
            else
                Debug.LogError(GameGlobal.Util.BuildMessage(typeof(ArenaMenuManager), "Faild to init listener for Button Back"));

            if (buttonCreate != null)
                buttonCreate.onClick.AddListener(onClick_ButtonCreate);
            else
                Debug.LogError(GameGlobal.Util.BuildMessage(typeof(ArenaMenuManager), "Faild to init listener for Button Create"));

            Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaMenuManager), "Setup Done"));
        }

        void onClick_ButtonBack()
        {
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaMenuManager), "Button Back clicked"));
            SceneManager.LoadScene(GameGlobal.Scene.MAIN_MENU);
        }

        void onClick_ButtonCreate()
        {
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(ArenaMenuManager), "Button Create clicked"));
            SceneManager.LoadScene(GameGlobal.Scene.ARENA);
        }

    }

}
