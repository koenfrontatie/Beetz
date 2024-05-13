using UnityEngine;
using FileManagement;
using System.IO;
using Un4seen.Bass;
using System;
using Un4seen.Bass.AddOn.Enc;

    public class EffectBaker : MonoBehaviour
    {
        FileManager _fileviewer;

        private void Awake()
        {
            _fileviewer = GameObject.FindObjectOfType<FileManager>();
        }

#region DSP
        
        private byte[] _encbuffer = new byte[12048];
        public void AddReverbToSelectedSample()
        {
            var renameOriginal = _fileviewer.SelectedSamplePath.Remove(_fileviewer.SelectedSamplePath.Length - 4, 4) + $"{System.DateTime.Now.GetHashCode()}.wav";
            File.Move(_fileviewer.SelectedSamplePath, renameOriginal);
            AddReverb(renameOriginal, _fileviewer.SelectedSamplePath);
            File.Delete(renameOriginal);
        }

        public void AddReverbToSelectedSampleObject(float val, string guid, string template)
        {
        //    var inputTemplatePath = _fileviewer.SamplePathFromGuid(template);
        //    Debug.Log(inputTemplatePath);
        //    Debug.Log(_fileviewer.SelectedSamplePath);
        ////var betteroutputPath = _fileviewer.UniqueSamplesDirectory + guid + "/" + guid + ".wav";
        //var betteroutputPath = Path.Combine(_fileviewer.UniqueSampleDirectory, guid, guid + ".wav");

        //Debug.Log(betteroutputPath);
        //    //var renameOriginal = _fileviewer.SelectedSamplePath.Remove(_fileviewer.SelectedSamplePath.Length - 4, 4) + $"{System.DateTime.Now.GetHashCode()}.wav";
        //    //File.Move(_fileviewer.SelectedSamplePath, renameOriginal);
            
        //    AddReverb(val, inputTemplatePath, betteroutputPath);
            //File.Delete(renameOriginal);
        }

        public void AddReverb(float value, string inputSamplePath, string outputSamplePath)
        {
        float inputvalue = value;

            var decoder = Bass.BASS_StreamCreateFile(inputSamplePath, 0, 0, BASSFlag.BASS_STREAM_DECODE);

            BASS_CHANNELINFO info = Bass.BASS_ChannelGetInfo(decoder);

            Debug.Log(info.origres);

            BASSEncode fpflag = BASSEncode.BASS_ENCODE_DEFAULT;


            if (info.origres != 0)
            {
                if (info.origres > 24) fpflag = BASSEncode.BASS_ENCODE_FP_32BIT;

                else if (info.origres > 16) fpflag = BASSEncode.BASS_ENCODE_FP_24BIT;

                else if (info.origres > 8) fpflag = BASSEncode.BASS_ENCODE_FP_16BIT;

                else if (info.origres != 0) fpflag = BASSEncode.BASS_ENCODE_FP_8BIT;
            }

            var rev = BASSFXType.BASS_FX_DX8_REVERB;
            int effectHandle = Bass.BASS_ChannelSetFX(decoder, rev, 1);

        BASS_DX8_REVERB reverbParameters = new BASS_DX8_REVERB
        {
            fInGain = -10f,
            fReverbMix = -40f,
            fReverbTime = 1200f,
            fHighFreqRTRatio = 0.1f,
        };

        if (!Bass.BASS_FXSetParameters(effectHandle, reverbParameters))
            {
                Debug.LogError("Failed to set reverb parameters.");
                return;
            }

            var encoder = BassEnc.BASS_Encode_Start(decoder, outputSamplePath, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE | fpflag, null, IntPtr.Zero);

            int counter = 0;

            while (Bass.BASS_ChannelIsActive(decoder) == BASSActive.BASS_ACTIVE_PLAYING && BassEnc.BASS_Encode_IsActive(encoder) == BASSActive.BASS_ACTIVE_PLAYING && counter < 80000)
            {
                Bass.BASS_ChannelGetData(decoder, _encbuffer, 6500);
                Debug.Log("Encoding");
                counter++;
            }
        }

        public void AddReverb(string inputSamplePath, string outputSamplePath)
        {

            var decoder = Bass.BASS_StreamCreateFile(inputSamplePath, 0, 0, BASSFlag.BASS_STREAM_DECODE);
            
            BASS_CHANNELINFO info = Bass.BASS_ChannelGetInfo(decoder);
            
            Debug.Log(info.origres);

            BASSEncode fpflag = BASSEncode.BASS_ENCODE_DEFAULT;
            
            
            if (info.origres != 0)
            {
                if (info.origres > 24) fpflag = BASSEncode.BASS_ENCODE_FP_32BIT;

                else if (info.origres > 16) fpflag = BASSEncode.BASS_ENCODE_FP_24BIT;

                else if (info.origres > 8) fpflag = BASSEncode.BASS_ENCODE_FP_16BIT;

                else if (info.origres != 0) fpflag = BASSEncode.BASS_ENCODE_FP_8BIT;
            }

            var rev = BASSFXType.BASS_FX_DX8_REVERB;
            int effectHandle = Bass.BASS_ChannelSetFX(decoder, rev, 1);

            BASS_DX8_REVERB reverbParameters = new BASS_DX8_REVERB
            {
                fInGain = -3f,
                fReverbMix = -5f,
                fReverbTime = 900f,
                fHighFreqRTRatio = 0.5f,
            };

            if (!Bass.BASS_FXSetParameters(effectHandle, reverbParameters))
            {
                Debug.LogError("Failed to set reverb parameters.");
                return;
            }

            var encoder = BassEnc.BASS_Encode_Start(decoder, outputSamplePath, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE | fpflag, null, IntPtr.Zero);

            int counter = 0;
            
            while (Bass.BASS_ChannelIsActive(decoder) == BASSActive.BASS_ACTIVE_PLAYING && BassEnc.BASS_Encode_IsActive(encoder) == BASSActive.BASS_ACTIVE_PLAYING && counter < 80000)
            {
                Bass.BASS_ChannelGetData(decoder, _encbuffer, 6500);
                Debug.Log("Encoding");
                counter++;
            }
        }
        //public void AddDistortion()
        //{
        //    var path = _fileviewer.SelectedSamplePath;
        //    WaveFile waveFile;
            
        //    FileStream readStream;

        //    using (readStream = new FileStream(path, FileMode.Open))
        //    {
        //        waveFile = new WaveFile(readStream);
        //    }

        //    readStream.Close();

        //    var dist = new DistortionEffect(DistortionMode.SoftClipping, 40, -20);
        //    var outputsignal = dist.ApplyTo(waveFile.Signals[0], NWaves.Filters.Base.FilteringMethod.DifferenceEquation);
        //    waveFile.Signals[0] = outputsignal;

        //    FileStream writeStream;

        //    //waveFile.SaveTo(writeStream = new FileStream($"{Path.Combine(Application.persistentDataPath, path)}", FileMode.Create));
        //    waveFile.SaveTo(writeStream = new FileStream(path, FileMode.Create));
        //    writeStream.Close();
        //}

#endregion
        void OnDestroy()
        {
            Bass.BASS_Free();
        }
    }
