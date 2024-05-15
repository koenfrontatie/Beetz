using UnityEngine;
using FileManagement;
using System.IO;
using Un4seen.Bass;
using System;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.Fx;
using System.Text;


public class EffectBaker : MonoBehaviour
{
    public BASS_DX8_REVERB _reverbSettings;
    public BASS_DX8_DISTORTION _distSettings;
    public BASS_DX8_ECHO _echoSettings;
    public BASS_BFX_PITCHSHIFT _pitchSettings;
    [Range(0, 100f)]
    public float EdgeFactor;
    [Range(-60f, 0f)]
    public float GainFactor;

    int _channel;
    int _reverbHandle;
    int _distHandle;
    int _echoHandle;
    int _pitchHandle;

    //fxchain order

    int _revPriority, _distPriority, _echoPriority, _pitchPriority;

    public string SelectedTemplate;

    [SerializeField]
    private KVDW.Logger _logger;

    private void OnEnable()
    {
        GameManager.StateChanged += OnStateChanged;
    }


    private void OnDisable()
    {
        GameManager.StateChanged -= OnStateChanged;
    }
    private void OnNewBeat()
    {

        //Debug.Log(Bass.BASS_ChannelPause(_channel));
        Bass.BASS_ChannelPause(_channel);
        Bass.BASS_ChannelSetPosition(_channel, 0);  
        Bass.BASS_ChannelPlay(_channel, false);
    }
    private async void OnStateChanged(GameState state)
    {
        if (state == GameState.Biolab)
        {
            Metronome.NewBeat += OnNewBeat;
            BassFx.LoadMe();
            //Debug.Log(Bass.BASS_GetInfo());
            //var info = Bass.BASS_GetVersion();
           
            //Debug.Log(BassFx.BASS_FX_GetVersion());

            //var echo = BASSFXType.BASS_FX_BFX_ECHO;

            //_pitchSettings = new BASS_BFX_PITCHSHIFT();
            var guid = FileManager.Instance.SelectedSampleGuid;

            var data = await AssetBuilder.Instance.GetSampleData(guid);

            SelectedTemplate = data.Template.ToString();

            Log("Opening live stream.");
            
            _channel = Bass.BASS_StreamCreateFile(FileManager.Instance.SamplePathFromGuid(SelectedTemplate), 0, 0, BASSFlag.BASS_MUSIC_FX);
            Bass.BASS_ChannelSetAttribute(_channel, BASSAttribute.BASS_ATTRIB_TAIL, 1.5f);

            _revPriority = 50;
            _distPriority = 40;
            _echoPriority = 30;
            _pitchPriority = 60;

            _reverbHandle = Bass.BASS_ChannelSetFX(_channel, BASSFXType.BASS_FX_DX8_REVERB, _revPriority);
            _distHandle = Bass.BASS_ChannelSetFX(_channel, BASSFXType.BASS_FX_DX8_DISTORTION, _distPriority);
            _pitchHandle = Bass.BASS_ChannelSetFX(_channel, BASSFXType.BASS_FX_BFX_PITCHSHIFT, _pitchPriority);
            //_echoHandle = Bass.BASS_ChannelSetFX(_channel, BASSFXType.BASS_FX_BFX_ECHO, _echoPriority);
            //_echoHandle = Bass.BASS_ChannelSetFX(_channel, BASSFXType.BASS_FX_BFX_ECHO4, _echoPriority);

            Bass.BASS_FXSetParameters(_reverbHandle, _reverbSettings);
            Bass.BASS_FXSetParameters(_distHandle, _distSettings);
            //Bass.BASS_FXSetParameters(_pitchHandle, new BASS_BFX_PITCHSHIFT());
            //Bass.BASS_FXSetParameters(_echoHandle, _echoSettings);

            //Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_BUFFER, 5000);

            var error = Bass.BASS_ErrorGetCode();
            
            if (error != 0)
            {
                Log($"Error during channel init: {error}");
                return;  
            }
        }
        else
        {
            Metronome.NewBeat -= OnNewBeat;

            Log("Closing live stream.");
            Bass.BASS_StreamFree(_channel);
        }
    }

    public void SetLiveReverb(float value)
    {
        _reverbSettings.fReverbTime = Mathf.Clamp(value * 3000f, .001f, 3000f);
        _reverbSettings.fReverbMix = Mathf.Clamp(value * 9f - 9f, -96f, 0f);
        Bass.BASS_FXSetParameters(_reverbHandle, _reverbSettings);
    }

    public void SetLiveDistortion(float value)
    {
        //_echoSettings.fWetDryMix = Mathf.Clamp(value * 100f, 0f, 100f);
        //_distSettings.fp
        _distSettings.fEdge = Mathf.Clamp(value * EdgeFactor, 0f, 100f);
        Bass.BASS_FXSetParameters(_distHandle, _distSettings);
        _distSettings.fGain = Mathf.Clamp(value * GainFactor,-60f, 0f);
    }

    public void SetLiveEcho(float value)
    {
        //_echoSettings.fWetDryMix = Mathf.Clamp(value * 100f, 0f, 100f);
        //_echoSettings.fFeedback = Mathf.Clamp(value * 100f, 0f, 100f);
        //_echoSettings.fEdge = Mathf.Clamp(value * 25f, 0f, 100f);

        //Bass.BASS_FXSetParameters(_echoHandle, );
        _echoSettings.fWetDryMix = Mathf.Clamp(value * 100f, 0f, 100f);
    }

    public void SetLivePitch(float value)
    {
        //var single = Math.Round(Mathf.Clamp(value, -.5f, 2f), 1).ToString();
        //Single.TryParse(single, out var result);
        //Debug.Log(result);
        //_pitchSettings.fPitchShift = result;
        //Bass.BASS_FXSetParameters(_pitchHandle, _pitchSettings);
        //bool success = Bass.BASS_FXSetParameters(_pitchHandle, _pitchSettings);
        //if (!success)
        //{
        //    var error = Bass.BASS_ErrorGetCode();
        //    Log($"Failed to set pitch parameters. Error: {error}");
        //}




        //BASS_BFX_PITCHSHIFT getsettings = new BASS_BFX_PITCHSHIFT() ;

        //Bass.BASS_FXGetParameters(_pitchHandle, getsettings);

        //Debug.Log(getsettings.fPitchShift);
        //Debug.Log(getsettings);

    }

    #region DSP

    private byte[] _encbuffer = new byte[12048];
    
    public async void BakeLiveEffects()
    {
        //string temporaryPath = FileManager.Instance.SelectedSamplePath.Remove(FileManager.Instance.SelectedSamplePath.Length - 4, 4) + $"{System.DateTime.Now.GetHashCode()}.wav";
        BakeEffects(FileManager.Instance.SamplePathFromGuid(SelectedTemplate), FileManager.Instance.SelectedSamplePath);
        //await Task.Run(() => File.Delete(temporaryPath));
    }
    //public void AddReverb(string inputSamplePath, string outputSamplePath)
    //{

    //    var decoder = Bass.BASS_StreamCreateFile(inputSamplePath, 0, 0, BASSFlag.BASS_STREAM_DECODE);
            
    //    BASS_CHANNELINFO info = Bass.BASS_ChannelGetInfo(decoder);
            
    //    Debug.Log(info.origres);

    //    BASSEncode fpflag = BASSEncode.BASS_ENCODE_DEFAULT;
            
            
    //    if (info.origres != 0)
    //    {
    //        if (info.origres > 24) fpflag = BASSEncode.BASS_ENCODE_FP_32BIT;

    //        else if (info.origres > 16) fpflag = BASSEncode.BASS_ENCODE_FP_24BIT;

    //        else if (info.origres > 8) fpflag = BASSEncode.BASS_ENCODE_FP_16BIT;

    //        else if (info.origres != 0) fpflag = BASSEncode.BASS_ENCODE_FP_8BIT;
    //    }

    //    var rev = BASSFXType.BASS_FX_DX8_REVERB;
    //    int effectHandle = Bass.BASS_ChannelSetFX(decoder, rev, 1);

    //    BASS_DX8_REVERB reverbParameters = new BASS_DX8_REVERB
    //    {
    //        fInGain = -3f,
    //        fReverbMix = -5f,
    //        fReverbTime = 900f,
    //        fHighFreqRTRatio = 0.5f,
    //    };

    //    if (!Bass.BASS_FXSetParameters(effectHandle, reverbParameters))
    //    {
    //        Debug.LogError("Failed to set reverb parameters.");
    //        return;
    //    }

    //    var encoder = BassEnc.BASS_Encode_Start(decoder, outputSamplePath, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE | fpflag, null, IntPtr.Zero);

    //    int counter = 0;
            
    //    while (Bass.BASS_ChannelIsActive(decoder) == BASSActive.BASS_ACTIVE_PLAYING && BassEnc.BASS_Encode_IsActive(encoder) == BASSActive.BASS_ACTIVE_PLAYING && counter < 80000)
    //    {
    //        Bass.BASS_ChannelGetData(decoder, _encbuffer, 6500);
    //        Debug.Log("Encoding");
    //        counter++;
    //    }
    //}

    public void BakeEffects(string inputSamplePath, string outputSamplePath)
    {

        var decoder = Bass.BASS_StreamCreateFile(inputSamplePath, 0, 0, BASSFlag.BASS_STREAM_DECODE);

        Bass.BASS_ChannelSetAttribute(decoder, BASSAttribute.BASS_ATTRIB_TAIL, 1.5f);
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
        int effectHandle = Bass.BASS_ChannelSetFX(decoder, rev, _revPriority);


        if (!Bass.BASS_FXSetParameters(effectHandle, _reverbSettings))
        {
            //Debug.LogError("Failed to set reverb parameters.");
            var error = Bass.BASS_ErrorGetCode();

            if (error != 0)
            {
                Log($"Error during rev init: {error}");
                return;
            }
            return;
        }

        var dist = BASSFXType.BASS_FX_DX8_DISTORTION;
        int dEffectHandle = Bass.BASS_ChannelSetFX(decoder, dist, _distPriority);

        if (!Bass.BASS_FXSetParameters(dEffectHandle, _distSettings))
        {
            //Debug.LogError("Failed to set distortion parameters.");
            var error = Bass.BASS_ErrorGetCode();

            if (error != 0)
            {
                Log($"Error during dist init: {error}");
                return;
            }
            return;
        }

        //var echo = BASSFXType.BASS_FX_BFX_ECHO;

        //int dEchoHandle = Bass.BASS_ChannelSetFX(decoder, echo, _echoPriority);

        //if (!Bass.BASS_FXSetParameters(dEchoHandle, _echoSettings))
        //{
        //    Debug.LogError("Failed to set echo parameters.");
        //    var error = Bass.BASS_ErrorGetCode();

        //    if (error != 0)
        //    {
        //        Log($"Error during echo init: {error}");
        //        return;
        //    }
        //    return;
        //}


        //var pitch = BASSFXType.BASS_FX_BFX_PITCHSHIFT;
        //int pEffectHandle = Bass.BASS_ChannelSetFX(decoder, pitch, _pitchPriority);

        //if (!Bass.BASS_FXSetParameters(pEffectHandle, pitch))
        //{
        //    //Debug.LogError("Failed to set pitch parameters.");
        //    var error = Bass.BASS_ErrorGetCode();

        //    if (error != 0)
        //    {
        //        Log($"Error during pitch init: {error}");
        //        return;
        //    }
        //    return;
        //}

        var encoder = BassEnc.BASS_Encode_Start(decoder, outputSamplePath, BASSEncode.BASS_ENCODE_PCM | BASSEncode.BASS_ENCODE_AUTOFREE | fpflag, null, IntPtr.Zero);

        int counter = 0;

        while (Bass.BASS_ChannelIsActive(decoder) == BASSActive.BASS_ACTIVE_PLAYING && BassEnc.BASS_Encode_IsActive(encoder) == BASSActive.BASS_ACTIVE_PLAYING && counter < 80000)
        {
            Bass.BASS_ChannelGetData(decoder, _encbuffer, 6500);
            Debug.Log("Encoding");
            counter++;
        }
    }

    void Log(object message)
    {
        if (_logger)
            _logger.Log(message, this);
    }

    #endregion
    void OnDestroy()
    {
        Bass.BASS_Free();
    }
}
