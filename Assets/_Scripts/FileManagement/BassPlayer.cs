using UnityEngine;
using NWaves.Audio;
using NWaves.Effects;
using System.IO;
using Un4seen.Bass;
using System;
using Un4seen.Bass.AddOn.Enc;

namespace FileManagement
{
    public class BassPlayer : MonoBehaviour
    {
        FileManager _fileviewer;
        private void OnEnable()
        {
            Events.LoadPlayGuid += PlayFromGuid;
        }

        private void OnDisable()
        {
            Events.LoadPlayGuid += PlayFromGuid;

        }
        //private void Awake()
        //{
        //    _fileviewer = GetComponent<FileManager>();
        //}

        public void PlayFromPath(string path)
        {
            //Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            var stream = Bass.BASS_StreamCreateFile(path, 0, 0, BASSFlag.BASS_DEFAULT | BASSFlag.BASS_MUSIC_AUTOFREE);
            Bass.BASS_ChannelPlay(stream, false);
        }

        public void PlayFromGuid(string guid)
        {
            string fullpath; // ---------- turns guid into url

            if (guid.Length < 3) // if template
            {
                fullpath = Path.Combine(Utils.PersistentBaseSamples, BaseSampleFromGuid(guid) + ".wav");
            }
            else
            {
                fullpath = Path.Combine(Utils.SampleSavepath, DataStorage.Instance.ProjectData.ID, guid, guid + ".wav");
                //Debug.Log(fullpath);
            }

            PlayFromPath(fullpath);
        }

        public string BaseSampleFromGuid(string guid)
        {

            switch (int.Parse(guid))
            {
                case 0:
                    return "1kick";
                    
                case 1:
                    return "2hat";
                    
                case 2:
                    return "3clap";
                    
                case 3:
                    return "4cow";
                    
                case 4:
                    return "5snare";
                    
                case 5:
                    return "6kick808";
                    
                case 6:
                    return "7tab05";
                    
                case 7:
                    return "8khat3";
                    
                case 8:
                    return "9walk";
                    
                case 9:
                    return "10chant";
                    
            }

            return "basesample404";
      
        }
    }
}