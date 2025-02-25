using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets._Game.Menus.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public void OnPlayButtonClicked()
        {
            SceneManager.LoadSceneAsync("WorldScene");
        }

        public void OnQuitButtonClicked()
        {
            Debug.LogWarning("Game: QUIT action ignored in Unity Editor");

            Application.Quit();
        }
    }
}
