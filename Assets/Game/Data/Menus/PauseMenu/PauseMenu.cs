using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Game.Data.Menus.PauseMenu
{
    public class PauseMenu : MonoBehaviour
    {
        GameObject m_PauseMenuCanvas;

        float m_TimeScale;

        bool m_IsCursorVisible;
        CursorLockMode m_CursorLockState;

        public void OnPauseButtonClicked()
        {
            m_TimeScale = Time.timeScale;
            Time.timeScale = 0.0f;

            m_IsCursorVisible = Cursor.visible;
            Cursor.visible = true;

            m_CursorLockState = Cursor.lockState;
            Cursor.lockState = CursorLockMode.None;

            m_PauseMenuCanvas.SetActive(true);
        }

        public void OnResumeButtonClicked()
        {
            m_PauseMenuCanvas.SetActive(false);
        
            Cursor.lockState = m_CursorLockState;

            Cursor.visible = m_IsCursorVisible;

            Time.timeScale = m_TimeScale;
        }

        public void OnMainMenuButtonClicked()
        {
            OnResumeButtonClicked();

            SceneManager.LoadSceneAsync("MainMenuScene");
        }

        void Start()
        {
            m_PauseMenuCanvas = transform.GetChild(0).gameObject;
        }
    
        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (m_PauseMenuCanvas.activeInHierarchy)
                    OnResumeButtonClicked();
                else OnPauseButtonClicked();
            }
        }
    }
}
