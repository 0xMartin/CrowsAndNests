using UnityEngine;
using UnityEngine.UI;
using Game.MiniGameUtils;
using UnityEngine.SceneManagement;
using TMPro;
using System;

namespace Menu
{

    /// <summary>
    /// Manager pro end game menu
    /// </summary>
    public class EndMenuManager : MonoBehaviour
    {
        [Header("UI-Button")]
        public Button back;

        [Header("UI-Text")]
        public GameObject scoreTextObj;
        public GameObject timeTextObj;
        public GameObject roundTextObj;


        private TextMeshProUGUI scoreText;

        private TextMeshProUGUI timeText;

        private TextMeshProUGUI roundText;

        void Start()
        {
            if (back != null)
                back.onClick.AddListener(OnClick_Back);
            else
                Debug.LogError(GameGlobal.Util.BuildMessage(typeof(EndMenuManager), "Faild to init listener for Button back"));
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(EndMenuManager), "Setup Done"));
        }

        void Awake()
        {
            this.scoreText = this.scoreTextObj.GetComponent<TextMeshProUGUI>();
            this.timeText = this.timeTextObj.GetComponent<TextMeshProUGUI>();
            this.roundText = this.roundTextObj.GetComponent<TextMeshProUGUI>();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            GameStats stats = GameGlobal.DataTransmissions.Instance.LoadData<GameStats>("final_stats");
            if(stats != null) {
                this.scoreText.SetText("0");
                for(int i = 0; i < stats.NameList.Count; ++i) {
                    if(stats.NameList[i].Equals("You")) {
                        this.scoreText.SetText(stats.ScoreList[i].ToString());
                        break;
                    }
                }

                TimeSpan difference = stats.GameEnd - stats.GameStart;
                double seconds = difference.TotalSeconds;
                this.timeText.SetText(GameGlobal.Util.FormatTime((float)seconds));

                this.roundText.SetText(stats.GameCount.ToString());
            }
        }

        void OnClick_Back()
        {
            Debug.Log(GameGlobal.Util.BuildMessage(typeof(EndMenuManager), "Button Back clicked"));
            SceneManager.LoadScene(GameGlobal.Scene.MAIN_MENU);
        }

    }

}