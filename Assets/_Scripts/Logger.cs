using UnityEngine;

namespace KVDW
{
    [AddComponentMenu("KVDW/Logger")]
    public class Logger : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField]
        private bool _showLogs;
        
        public void Log(object message, Object sender)
        {
            if(_showLogs)
                Debug.Log(message, sender);
        }
    }
}
