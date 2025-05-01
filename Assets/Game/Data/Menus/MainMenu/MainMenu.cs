using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Game.Data.Menus.MainMenu
{
    public class MainMenu : MonoBehaviour
    {
        public void OnPlayButtonClicked()
        {
            SceneManager.LoadSceneAsync("WorldScene");
        }

        public void OnQuitButtonClicked()
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
            Application.Quit();
#endif
        }
    }
}
