using UnityEngine;
using System.IO;
using Un4seen.Bass;
using System;

namespace FileManagement
{
    public class BassPlayer : MonoBehaviour
    {
        [SerializeField]
        private KVDW.Logger _logger;
        private void OnEnable()
        {
            Events.LoadPlayGuid += PlayFromGuid;
        }

        private void OnDisable()
        {
            Events.LoadPlayGuid += PlayFromGuid;

        }

        private void Start()
        {
      
                bool isInitialized = Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                if (!isInitialized)
                {
                    Debug.LogError("Failed to initialize BASS library.");
                    return;
                }
         
        }

        public void PlayFromPath(string path)
        {
            //Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            var stream = Bass.BASS_StreamCreateFile(path, 0, 0, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_MUSIC_AUTOFREE);
            Bass.BASS_ChannelPlay(stream, false);
        }

        public void PlayFromGuid(string guid)
        {
            string fullpath = FileManager.Instance.SamplePathFromGuid(guid); // ---------- turns guid into url
            
            Log($"Playing path: {fullpath}");

            PlayFromPath(fullpath);
        }

        void Log(object message)
        {
            if(_logger)
                _logger.Log(message, this);
        }
        void OnDestroy()
        {
            Bass.BASS_Free();
        }
    }
}