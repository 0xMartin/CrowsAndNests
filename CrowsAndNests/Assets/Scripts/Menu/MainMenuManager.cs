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
        [Header("Sections")]
        public GameObject mainSection;
        public GameObject settingsSection;
        public GameObject aboutSection;

        [Header("Main Section")]
        public Button buttonSingle;
        public Button buttonSettings;
        public Button buttonAbout;
        public Button buttonExit;

        [Header("Settings Section")]
        public Button buttonAccept;
        public Button buttonReset;
        public Button buttonBack;

        [Header("About Section")]
        public Button buttonBack2;

        void Start()
        {
            // sekce main
            buttonSingle.onClick.AddListener(MainActionSingle);
            buttonSettings.onClick.AddListener(MainActionSettings);
            buttonExit.onClick.AddListener(MainActionExit);    
            buttonAbout.onClick.AddListener(MainActionAbout);

            // sekce settings
            buttonAccept.onClick.AddListener(SettingsActionAccept);
            buttonReset.onClick.AddListener(SettingsActionReset);
            buttonBack.onClick.AddListener(SettingsActionBack); 

            // sekce about
            buttonBack2.onClick.AddListener(AboutActionBack); 
        }

        void Awake() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            this.mainSection.SetActive(true);
            this.settingsSection.SetActive(false);
            this.aboutSection.SetActive(false);
        }
 
        /*MAIN***************************************************************************************************/

        void MainActionSingle()
        {
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Button Singleplayer clicked"));
            SceneManager.LoadScene(GameGlobal.Scene.ARENA);
        }

        void MainActionSettings()
        {
            this.mainSection.SetActive(false);
            this.settingsSection.SetActive(true);
        }

        void MainActionAbout()
        {
            this.mainSection.SetActive(false);
            this.aboutSection.SetActive(true);
        }

        void MainActionExit()
        {
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Button Exit clicked"));
            Application.Quit();
        }

        /*SETTINGS***************************************************************************************************/

        void SettingsActionAccept() {

        }

        void SettingsActionReset() {

        }

        void SettingsActionBack() {
            this.mainSection.SetActive(true);
            this.settingsSection.SetActive(false);  
        }

        /*ABOUT***************************************************************************************************/

        void AboutActionBack() {
            this.mainSection.SetActive(true);
            this.aboutSection.SetActive(false); 
        }

    }

}
