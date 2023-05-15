using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu {   

    public class PauseMenu : MonoBehaviour
    {
        [Header("Canvas")]
        public GameObject pauseMenuUI;
        public List<GameObject> canvasToHide;

        [Header("Buttons")]
        public Button backButton;
        public Button resumeButton;

        private bool isPaused;

        private void Start() {
            this.backButton.onClick.AddListener(ReturnToMenu);
            this.resumeButton.onClick.AddListener(ResumeGame);
        }

        public void PauseGame()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            isPaused = true;
            Time.timeScale = 0f;
            foreach(GameObject c in canvasToHide) {
                c.SetActive(false);
            }
            pauseMenuUI.SetActive(true);
        }

        public void ResumeGame()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            isPaused = false;
            Time.timeScale = 1f;
            pauseMenuUI.SetActive(false);
            foreach(GameObject c in canvasToHide) {
                c.SetActive(true);
            }
        }

        public void ReturnToMenu()
        {
            ResumeGame();
            SceneManager.LoadScene(GameGlobal.Scene.MAIN_MENU);
        }
    }

}