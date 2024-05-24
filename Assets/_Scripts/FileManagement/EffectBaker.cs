using UnityEngine;
using FileManagement;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Enc;
using Un4seen.Bass.AddOn.Fx;
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

public class EffectBaker : MonoBehaviour
{
    public BASS_DX8_REVERB _reverbSettings;
    public BASS_DX8_DISTORTION _distSettings;
    public BASS_BFX_ECHO4 _echoSettings;
    public BASS_BFX_PITCHSHIFT _pitchSettings;

    private int _channel;
    private int _reverbHandle;
    private int _distHandle;
    private int _echoHandle;
    private int _pitchHandle;

    private const int BufferSize = 144000;

    private int _revPriority = 50;
    private int _distPriority = 40;
    private int _echoPriority = 80;
    private int _pitchPriority = 60;

    [Range(0, 100f)]
    public float EdgeFactor;
    [Range(-60f, 0f)]
    public float GainFactor;

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
        //Bass.BASS_ChannelPause(_channel);
        Bass.BASS_ChannelSetPosition(_channel, 0);
        Bass.BASS_ChannelPlay(_channel, false);
    }

    private async void OnStateChanged(GameState state)
    {
        if (state == GameState.Biolab)
        {
            Metronome.NewBeat += OnNewBeat;

            Log($"BASS VERSION: {Bass.BASS_GetVersion()}");
            Log($"BASSFX VERSION: {BassFx.BASS_FX_GetVersion()}");

            SelectedTemplate = await FileManager.Instance.TemplateFromGuid(FileManager.Instance.SelectedSampleGuid);

            InitializeBassChannel();

            if (!InitializeBassEffects())
            {
                CleanupBass();
                return;
            }
        }
        else
        {
            Metronome.NewBeat -= OnNewBeat;
            CleanupBass();
        }
    }

    private void InitializeBassChannel()
    {
        Log("Opening live stream.");
        _channel = Bass.BASS_StreamCreateFile(FileManager.Instance.SamplePathFromGuid(SelectedTemplate), 0, 0, BASSFlag.BASS_MUSIC_PRESCAN);

        if (_channel == 0)
        {
            Log($"Failed to create stream: {Bass.BASS_ErrorGetCode()}");
        }
    }

    private bool InitializeBassEffects()
    {
        _distHandle = Bass.BASS_ChannelSetFX(_channel, BASSFXType.BASS_FX_DX8_DISTORTION, _distPriority);
        _pitchHandle = Bass.BASS_ChannelSetFX(_channel, BASSFXType.BASS_FX_BFX_PITCHSHIFT, _pitchPriority);
       
        if (_distHandle == 0 || _pitchHandle == 0)
        {
            Log($"Error during handle init: {Bass.BASS_ErrorGetCode()}");
            return false;
        }
        //Bass.BASS_FXSetParameters(_reverbHandle, _reverbSettings);
        Bass.BASS_FXSetParameters(_distHandle, _distSettings);
        // for some reason echo and pitch need to be set with getparam method first ------
        Bass.BASS_FXGetParameters(_pitchHandle, _pitchSettings);
        //Bass.BASS_FXGetParameters(_echoHandle, _echoSettings);
        // ---------------------------
        Bass.BASS_FXSetParameters(_pitchHandle, _pitchSettings);
        Bass.BASS_FXSetParameters(_echoHandle, _echoSettings);
        //if (!Bass.BASS_FXSetParameters(_distHandle, _distSettings) || !Bass.BASS_FXSetParameters(_pitchHandle, _pitchSettings))
        //{
        //    Log($"Error setting FX parameters: {Bass.BASS_ErrorGetCode()}");
        //    return false;
        //}

        return true;
    }

    private void CleanupBass()
    {
        if (_channel != 0)
        {
            Bass.BASS_StreamFree(_channel);
            _channel = 0;
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
        _distSettings.fEdge = Mathf.Clamp(value * EdgeFactor, 0f, 100f);
        _distSettings.fGain = Mathf.Clamp(value * GainFactor, -60f, 0f);
        _distSettings.fPreLowpassCutoff = Mathf.Clamp(value * 7000f, 100f, 8000f);
        Bass.BASS_FXSetParameters(_distHandle, _distSettings);
    }

    public void SetLiveEcho(float value)
    {
        _echoSettings.fWetMix = Mathf.Clamp(value * 2, 0f, 2f);
        _echoSettings.fDelay = .1f;
        _echoSettings.fFeedback = .8f;
        Bass.BASS_FXSetParameters(_echoHandle, _echoSettings);
    }

    public void SetLivePitch(float value)
    {
        float clampedValue = Mathf.Clamp(value, 0.5f, 2f);
        float roundedValue = Mathf.Round(clampedValue * 10f) * .1f;
        _pitchSettings.fPitchShift = roundedValue;
        Bass.BASS_FXSetParameters(_pitchHandle, _pitchSettings);
    }

    public async void BakeLiveEffects()
    {
        await Task.Run(() => BakeEffects(FileManager.Instance.SamplePathFromGuid(SelectedTemplate), FileManager.Instance.SelectedSamplePath));
    }

    public void BakeEffects(string inputSamplePath, string outputSamplePath)
    {
        int decoder = Bass.BASS_StreamCreateFile(inputSamplePath, 0, 0, BASSFlag.BASS_STREAM_DECODE);

        if (decoder == 0)
        {
            Log($"Failed to create decoder: {Bass.BASS_ErrorGetCode()}");
            return;
        }

        ApplyEffects(decoder);

        int encoder = BassEnc.BASS_Encode_Start(decoder, outputSamplePath, BASSEncode.BASS_ENCODE_PCM | GetEncodingFlags(decoder), null, IntPtr.Zero);

        if (encoder == 0)
        {
            Log($"Failed to start encoder: {Bass.BASS_ErrorGetCode()}");
            Bass.BASS_StreamFree(decoder);
            return;
        }

        byte[] encBuffer = new byte[BufferSize];

        while (Bass.BASS_ChannelIsActive(decoder) == BASSActive.BASS_ACTIVE_PLAYING && BassEnc.BASS_Encode_IsActive(encoder) == BASSActive.BASS_ACTIVE_PLAYING)
        {
            Bass.BASS_ChannelGetData(decoder, encBuffer, encBuffer.Length);
        }

        Bass.BASS_ChannelStop(decoder);
        Bass.BASS_ChannelStop(encoder);
        Bass.BASS_StreamFree(encoder);
        Bass.BASS_StreamFree(decoder);

        Log("Baking effects completed.");
    }

    private void ApplyEffects(int channel)
    {
        if (Bass.BASS_ChannelSetFX(channel, BASSFXType.BASS_FX_DX8_DISTORTION, _distPriority) != 0)
        {
            Bass.BASS_FXSetParameters(channel, _distSettings);
        }

        if (Bass.BASS_ChannelSetFX(channel, BASSFXType.BASS_FX_BFX_PITCHSHIFT, _pitchPriority) != 0)
        {
            Bass.BASS_FXSetParameters(channel, _pitchSettings);
        }
    }

    private BASSEncode GetEncodingFlags(int channel)
    {
        BASS_CHANNELINFO info = Bass.BASS_ChannelGetInfo(channel);

        if (info.origres > 24)
            return BASSEncode.BASS_ENCODE_FP_32BIT;
        else if (info.origres > 16)
            return BASSEncode.BASS_ENCODE_FP_24BIT;
        else if (info.origres > 8)
            return BASSEncode.BASS_ENCODE_FP_16BIT;
        else if (info.origres > 0)
            return BASSEncode.BASS_ENCODE_FP_8BIT;
        else
            return BASSEncode.BASS_ENCODE_DEFAULT;
    }

    private void Log(object message)
    {
        _logger?.Log(message, this);
    }

    private void OnDestroy()
    {
        CleanupBass();
        Bass.BASS_Free();
    }
}



//using UnityEngine;
//using FileManagement;
//using Un4seen.Bass;
//using System;
//using Un4seen.Bass.AddOn.Enc;
//using Un4seen.Bass.AddOn.Fx;
//using PlasticPipe.PlasticProtocol.Messages;
//using System.Text;
//using System.Runtime.InteropServices;

//public class EffectBaker : MonoBehaviour
//{
//    public BASS_DX8_REVERB _reverbSettings;
//    public BASS_DX8_DISTORTION _distSettings;
//    public BASS_BFX_ECHO4 _echoSettings;
//    public BASS_BFX_PITCHSHIFT _pitchSettings;
//    STREAMPROC _myStreamProc;
//    byte[] _procData;

//    [Range(0, 100f)]
//    public float EdgeFactor;
//    [Range(-60f, 0f)]
//    public float GainFactor;

//    int _channel;
//    int _reverbHandle;
//    int _distHandle;
//    int _echoHandle;
//    int _pitchHandle;

//    static int bufferSize = 144000;

//    int _revPriority, _distPriority, _echoPriority, _pitchPriority;

//    public string SelectedTemplate;

//    [SerializeField]
//    private KVDW.Logger _logger;

//    private void OnEnable()
//    {
//        GameManager.StateChanged += OnStateChanged;
//    }


//    private void OnDisable()
//    {
//        GameManager.StateChanged -= OnStateChanged;
//    }
//    private void OnNewBeat()
//    {

//        //Debug.Log(Bass.BASS_ChannelPause(_channel));
//        //Bass.BASS_ChannelPause(_channel);
//        Bass.BASS_ChannelSetPosition(_channel, 0);  
//        Bass.BASS_ChannelPlay(_channel, false);
//    }
//    private async void OnStateChanged(GameState state)
//    {
//        if (state == GameState.Biolab)
//        {
//            Metronome.NewBeat += OnNewBeat;

//            Log($"BASS VERSION: {Bass.BASS_GetVersion()}");
//            Log($"BASSFX VERSION: {BassFx.BASS_FX_GetVersion()}");

//            SelectedTemplate = await FileManager.Instance.TemplateFromGuid(FileManager.Instance.SelectedSampleGuid);

//            Log("Opening live stream.");
//            //_channel = Bass.BASS_StreamCreateFile(FileManager.Instance.SamplePathFromGuid(SelectedTemplate), 0, 0, 0);
//            _channel = Bass.BASS_StreamCreateFile(FileManager.Instance.SamplePathFromGuid(SelectedTemplate), 0, 0, BASSFlag.BASS_MUSIC_PRESCAN);

//            //Log("Input channel length: " + Bass.BASS_ChannelGetLength(_channel, BASSMode.BASS_POS_BYTES) / bufferSize);

//            //Bass.BASS_ChannelSetAttribute(_channel, BASSAttribute.BASS_ATTRIB_TAIL, 5f);
//            //int[] buffer = null;
//            //Bass.BASS_StreamPutData(_channel, buffer, bufferSize);

//            _revPriority = 50;
//            _distPriority = 40;
//            _echoPriority = 80;
//            _pitchPriority = 60;

//            //_reverbHandle = Bass.BASS_ChannelSetFX(_channel, BASSFXType.BASS_FX_DX8_REVERB, _revPriority);
//            _distHandle = Bass.BASS_ChannelSetFX(_channel, BASSFXType.BASS_FX_DX8_DISTORTION, _distPriority);
//            _pitchHandle = Bass.BASS_ChannelSetFX(_channel, BASSFXType.BASS_FX_BFX_PITCHSHIFT, _pitchPriority);
//            //_echoHandle = Bass.BASS_ChannelSetFX(_channel, BASSFXType.BASS_FX_BFX_ECHO4, _echoPriority);

//            var error1 = Bass.BASS_ErrorGetCode();

//            if (error1 != 0)
//            {
//                Log($"Error during handle init: {error1}");
//                return;
//            }

//            //Bass.BASS_FXSetParameters(_reverbHandle, _reverbSettings);
//            Bass.BASS_FXSetParameters(_distHandle, _distSettings);
//            // for some reason echo and pitch need to be set with getparam method first ------
//            Bass.BASS_FXGetParameters(_pitchHandle, _pitchSettings);
//            //Bass.BASS_FXGetParameters(_echoHandle, _echoSettings);
//            // ---------------------------
//            Bass.BASS_FXSetParameters(_pitchHandle, _pitchSettings);
//            Bass.BASS_FXSetParameters(_echoHandle, _echoSettings);

//            var error = Bass.BASS_ErrorGetCode();

//            if (error != 0)
//            {
//                Log($"Error during channel init: {error}");
//                return;  
//            }
//        }
//        else
//        {
//            Metronome.NewBeat -= OnNewBeat;

//            //Log("Closing live stream.");
//            Bass.BASS_StreamFree(_channel);
//            //Bass.BASS_StreamFree(_decoder);

//        }
//    }

//    public void SetLiveReverb(float value)
//    {
//        _reverbSettings.fReverbTime = Mathf.Clamp(value * 3000f, .001f, 3000f);
//        _reverbSettings.fReverbMix = Mathf.Clamp(value * 9f - 9f, -96f, 0f);
//        Bass.BASS_FXSetParameters(_reverbHandle, _reverbSettings);
//    }

//    public void SetLiveDistortion(float value)
//    {
//        //_distSettings.wet
//        _distSettings.fEdge = Mathf.Clamp(value * EdgeFactor, 0f, 100f);
//        Bass.BASS_FXSetParameters(_distHandle, _distSettings);
//        _distSettings.fGain = Mathf.Clamp(value * GainFactor,-60f, 0f);
//        _distSettings.fPreLowpassCutoff = Mathf.Clamp(value * 7000f, 100f, 8000f);
//    }

//    public void SetLiveEcho(float value)
//    {

//        _echoSettings.fWetMix = Mathf.Clamp(value * 2, 0f, 2f);
//        _echoSettings.fDelay = .1f;
//        _echoSettings.fFeedback = .8f;

//        Bass.BASS_FXSetParameters(_echoHandle, _echoSettings);
//    }

//    public void SetLivePitch(float value)
//    {
//        float clampedValue = Mathf.Clamp(value, 0.5f, 2f);
//        float roundedValue = Mathf.Round(clampedValue * 10f) * .1f;
//        _pitchSettings.fPitchShift = roundedValue;
//        Bass.BASS_FXSetParameters(_pitchHandle, _pitchSettings);      
//    }

//    #region DSP

//    private byte[] _encbuffer = new byte[1248];

//    public async void BakeLiveEffects()
//    {
//        //string temporaryPath = FileManager.Instance.SelectedSamplePath.Remove(FileManager.Instance.SelectedSamplePath.Length - 4, 4) + $"{System.DateTime.Now.GetHashCode()}.wav";
//        BakeEffects(FileManager.Instance.SamplePathFromGuid(SelectedTemplate), FileManager.Instance.SelectedSamplePath);
//        //await Task.Run(() => File.Delete(temporaryPath));
//    }

//    public void BakeEffects(string inputSamplePath, string outputSamplePath)
//    {

//        var decoder = Bass.BASS_StreamCreateFile(inputSamplePath, 0, 0, BASSFlag.BASS_STREAM_DECODE);

//        BASS_CHANNELINFO info = Bass.BASS_ChannelGetInfo(decoder);

//        Debug.Log(info.origres);

//        BASSEncode fpflag = BASSEncode.BASS_ENCODE_DEFAULT;

//        if (info.origres != 0)
//        {
//            if (info.origres > 24) fpflag = BASSEncode.BASS_ENCODE_FP_32BIT;

//            else if (info.origres > 16) fpflag = BASSEncode.BASS_ENCODE_FP_24BIT;

//            else if (info.origres > 8) fpflag = BASSEncode.BASS_ENCODE_FP_16BIT;

//            else if (info.origres != 0) fpflag = BASSEncode.BASS_ENCODE_FP_8BIT;
//        }

//        Debug.Log("length of decoder " + Bass.BASS_ChannelGetLength(decoder).ToString());

//        //Bass.BASS_ChannelSetLength(decoder, 0); 

//        //var rev = BASSFXType.BASS_FX_DX8_REVERB;

//        //int effectHandle = Bass.BASS_ChannelSetFX(decoder, rev, _revPriority);


//        //if (!Bass.BASS_FXSetParameters(effectHandle, _reverbSettings))
//        //{
//        //    var error = Bass.BASS_ErrorGetCode();

//        //    if (error != 0)
//        //    {
//        //        Log($"Error during rev apply: {error}");
//        //        return;
//        //    }
//        //    return;
//        //}

//        var dist = BASSFXType.BASS_FX_DX8_DISTORTION;
//        int dEffectHandle = Bass.BASS_ChannelSetFX(decoder, dist, _distPriority);

//        if (!Bass.BASS_FXSetParameters(dEffectHandle, _distSettings))
//        {
//            var error = Bass.BASS_ErrorGetCode();

//            if (error != 0)
//            {
//                Log($"Error during dist apply: {error}");
//                return;
//            }
//            return;
//        }

//        //var echo = BASSFXType.BASS_FX_BFX_ECHO4;

//        //int dEchoHandle = Bass.BASS_ChannelSetFX(decoder, echo, _echoPriority);

//        //if (!Bass.BASS_FXSetParameters(dEchoHandle, _echoSettings))
//        //{
//        //    var error = Bass.BASS_ErrorGetCode();

//        //    if (error != 0)
//        //    {
//        //        Log($"Error during echo apply: {error}");
//        //        return;
//        //    }
//        //    return;
//        //}


//        var pitch = BASSFXType.BASS_FX_BFX_PITCHSHIFT;
//        int pEffectHandle = Bass.BASS_ChannelSetFX(decoder, pitch, _pitchPriority);

//        if (!Bass.BASS_FXSetParameters(pEffectHandle, _pitchSettings))
//        {
//            var error = Bass.BASS_ErrorGetCode();

//            if (error != 0)
//            {
//                Log($"Error during pitch apply: {error}");
//                return;
//            }
//            return;
//        }
//        //Bass.BASS_ChannelSetAttribute(decoder, BASSAttribute.BASS_ATTRIB_TAIL, 5f);
//        //BassEnc.BASS_Encode_StartCA(decoder, outputSamplePath)
//        var encoder = BassEnc.BASS_Encode_Start(decoder, outputSamplePath, BASSEncode.BASS_ENCODE_PCM | fpflag, null, IntPtr.Zero);
//        //Bass.BASS_ChannelSetAttribute(encoder, BASSAttribute.BASS_ATTRIB_TAIL, 5f);

//        //int counter = 0;

//        while (Bass.BASS_ChannelIsActive(decoder) == BASSActive.BASS_ACTIVE_PLAYING && BassEnc.BASS_Encode_IsActive(encoder) == BASSActive.BASS_ACTIVE_PLAYING /*&& counter < 80000*/)
//        {
//            Bass.BASS_ChannelGetData(decoder, _encbuffer, _encbuffer.Length);
//            //BASSActive check = BassEnc.BASS_Encode_IsActive(decoder);
//            //Debug.Log("encode on decoder: " + check.ToString());

//            Debug.Log("Encoding");
//            //counter++;
//        }

//        //BassEnc.BASS_Encode_Write(encoder, _encbuffer, _encbuffer.Length);

//        Debug.Log("length of encoder " + Bass.BASS_ChannelGetLength(encoder).ToString());

//        Bass.BASS_ChannelStop(decoder);
//        Bass.BASS_ChannelStop(encoder);
//        Bass.BASS_StreamFree(encoder);
//        Bass.BASS_StreamFree(decoder);

//        //while (Bass.BASS_ChannelIsActive(decoder) == BASSActive.BASS_ACTIVE_PLAYING || Bass.BASS_ChannelIsActive(decoder) == BASSActive.BASS_ACTIVE_STALLED)
//        //{
//        //    int data = Bass.BASS_ChannelGetData(decoder, _encbuffer, _encbuffer.Length);
//        //    if (data == -1) break;  // Break the loop if no data is returned
//        //    Debug.Log("Encoding");
//        //}
//    }






//    void Log(object message)
//    {
//        if (_logger)
//            _logger.Log(message, this);
//    }

//    #endregion
//    void OnDestroy()
//    {
//        Bass.BASS_Free();
//    }
//}
