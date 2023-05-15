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
        public Toggle toggleBloom;
        public Toggle toggleColorGrading;
        public Toggle toggleVignette;
        public Toggle toggleChromaticAberration;
        public Toggle toggleMotionBlur;

        [Header("About Section")]
        public Button buttonBack2;

        [Header("Sounds")]
        public AudioSource clickSound;

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

            LoadConfig();
        }
 
        /*MAIN***************************************************************************************************/

        void MainActionSingle()
        {
            clickSound.Play();
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Button Singleplayer clicked"));
            SceneManager.LoadScene(GameGlobal.Scene.ARENA);
        }

        void MainActionSettings()
        {
            clickSound.Play();
            this.mainSection.SetActive(false);
            this.settingsSection.SetActive(true);
            LoadConfig();
        }

        void MainActionAbout()
        {
            clickSound.Play();
            this.mainSection.SetActive(false);
            this.aboutSection.SetActive(true);
        }

        void MainActionExit()
        {
            clickSound.Play();
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(MainMenuManager), "Button Exit clicked"));
            Application.Quit();
        }

        /*SETTINGS***************************************************************************************************/

        void SettingsActionAccept() {
            clickSound.Play();
            SaveSettings();
        }

        void SettingsActionReset() {
            clickSound.Play();
            ResetSettings();
        }

        void SettingsActionBack() {
            clickSound.Play();
            this.mainSection.SetActive(true);
            this.settingsSection.SetActive(false);  
        }

        void LoadConfig() {
            this.toggleBloom.isOn = PlayerPrefs.GetInt("Bloom") == 1;
            this.toggleColorGrading.isOn = PlayerPrefs.GetInt("ColorGrading") == 1;
            this.toggleVignette.isOn = PlayerPrefs.GetInt("Vignette") == 1;
            this.toggleChromaticAberration.isOn = PlayerPrefs.GetInt("ChromaticAberration") == 1;
            this.toggleMotionBlur.isOn = PlayerPrefs.GetInt("MotionBlur") == 1;
        }

        void SaveSettings() {
            PlayerPrefs.SetInt("Bloom", this.toggleBloom.isOn ? 1 : 0);
            PlayerPrefs.SetInt("ColorGrading", this.toggleColorGrading.isOn ? 1 : 0);
            PlayerPrefs.SetInt("Vignette", this.toggleVignette.isOn ? 1 : 0);
            PlayerPrefs.SetInt("ChromaticAberration", this.toggleChromaticAberration.isOn ? 1 : 0);
            PlayerPrefs.SetInt("MotionBlur", this.toggleMotionBlur.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        void ResetSettings() {
            this.toggleBloom.isOn = true;
            this.toggleColorGrading.isOn = true;
            this.toggleVignette.isOn = true;
            this.toggleChromaticAberration.isOn = true;
            this.toggleMotionBlur.isOn = true;
            SaveSettings();
        }

        /*ABOUT***************************************************************************************************/

        void AboutActionBack() {
            clickSound.Play();
            this.mainSection.SetActive(true);
            this.aboutSection.SetActive(false); 
        }

    }

}
