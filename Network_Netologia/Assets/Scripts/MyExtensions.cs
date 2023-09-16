using UnityEngine;
using System.Linq;
using TMPro;

namespace Network
{
    public static class Debugger
    {
        private static TextMeshProUGUI _console;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Onstart()
        {
            _console = GameObject.FindObjectsOfType<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "Console");
#if UNITY_EDITOR
            if (_console == null) Debug.Log("console not found!");
#endif
        }
        public static void Log(object message)
        {
#if UNITY_EDITOR
            Debug.Log("\n" + message);
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR
            _console.text = "\n" + message;
#endif
        }
    }
}
