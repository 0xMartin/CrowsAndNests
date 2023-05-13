using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Menu
{

    /// <summary>
    /// Manager pro end game menu
    /// </summary>
    public class EndMenuManager : MonoBehaviour
    {
        public Button back;

        void Start()
        {
            if (back != null)
                back.onClick.AddListener(OnClick_Back);
            else
                Debug.LogError(GameGlobal.Util.BuildMessage(typeof(EndMenuManager), "Faild to init listener for Button back"));
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(EndMenuManager), "Setup Done"));
        }

        void Awake() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void OnClick_Back()
        {
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(EndMenuManager), "Button Back clicked"));
            SceneManager.LoadScene(GameGlobal.Scene.MAIN_MENU);
        }

    }

}
