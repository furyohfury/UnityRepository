using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
namespace Cars
{
    public class MenuManager : MonoBehaviour
    {
        public void OnStartGameButton_Editor()
        {
            SceneManager.LoadScene(1);
        }
        public void OnQuitGameButton_Editor()
        {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;        
#elif !UNITY_EDITOR && UNITY_STANDALONE_WIN 
        Application.Quit();
#endif               
        }
    }
}