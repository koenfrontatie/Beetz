using UnityEngine;
using FileManagement;

public class ManagedBassController : MonoBehaviour
{
    int _streamHandle;
    int _fxHandle;
    [SerializeField]
    ManagedBass.Fx.EchoParameters param;

    public ManagedBass.EffectType EFFECTTYPE;

    void Start()
    {
        ManagedBass.Bass.Init();
        param = new ManagedBass.Fx.EchoParameters();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            TryManagedBass();
        }
    }

    void TryManagedBass()
    {
        _streamHandle = ManagedBass.Bass.CreateStream(FileManager.Instance.SamplePathFromGuid("8"), 0, 0, ManagedBass.BassFlags.AutoFree);

        //var echoParams = new ManagedBass.Fx.EchoParameters();

        //int echoEffect = new ManagedBass.Fx.EchoEffect();

        param.fDryMix = .1f;

        param.fWetMix = 1.5f;
        param.fFeedback = 0.5f;
        param.fDelay = .5f;


        //var effect = new ManagedBass.Effect(ec);
        //ManagedBass.Bass.FXSetParameters(echoEffect., param);
        //Debug.Log(param.GetType());

        _fxHandle = ManagedBass.Bass.ChannelSetFX(_streamHandle, EFFECTTYPE, 1);
        //ManagedBass.Errors;
        var error = ManagedBass.Bass.LastError;
        Debug.Log(error);
        ManagedBass.Bass.FXSetParameters(_fxHandle, param);

        ManagedBass.Bass.ChannelPlay(_streamHandle);

    }
}
