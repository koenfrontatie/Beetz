using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class ConsoleLogger : MonoBehaviour
{
    private string _log;
    private string _output;
    private string _stack;
    //[SerializeField] CanvasGroup _cg;
    [SerializeField] TMP_Text text;

    void OnEnable()
    {
        Application.logMessageReceived += Log;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= Log;
    }

    void Log(string logString, string stackTrace, LogType type)
    {
        _output = logString;
        _stack = stackTrace;
        _log = $"{_output}\n{_log}";
        if (_log.Length > 5000)
        {
            _log = _log.Substring(0, 4000);
        }
        text.text = _log;
    }

    //public void ToggleCanvasGroup()
    //{
    //    if (_cg.alpha != 0)
    //    {
    //        _cg.alpha = 0f;
    //        _cg.blocksRaycasts = false;

    //    }
    //    else
    //    {
    //        _cg.alpha = 1f;
    //        _cg.blocksRaycasts = true;
    //    }
    //}
}


// using UnityEngine;

//    namespace DebugStuff
//{
//    public class ConsoleToGUI : MonoBehaviour
//    {
//        //#if !UNITY_EDITOR
//        static string myLog = "";
//        private string output;
//        private string stack;

//        void OnEnable()
//{
//    Application.logMessageReceived += Log;
//}

//void OnDisable()
//{
//    Application.logMessageReceived -= Log;
//}

//        public void Log(string logString, string stackTrace, LogType type)
//        {
//output = logString;
//stack = stackTrace;
//myLog = output + "
//" + myLog;
//if (myLog.Length > 5000)
//{
//    myLog = myLog.Substring(0, 4000);
//}
//        }

//        void OnGUI()
//        {
//            //if (!Application.isEditor) //Do not display in editor ( or you can use the UNITY_EDITOR macro to also disable the rest)
//            {
//                myLog = GUI.TextArea(new Rect(10, 10, Screen.width - 10, Screen.height - 10), myLog);
//            }
//        }
//        //#endif
//    }
//}