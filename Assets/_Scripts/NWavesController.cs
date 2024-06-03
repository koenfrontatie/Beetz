using UnityEngine;
using NWaves;
using NWaves.Audio;
using NWaves.Effects;
using System.IO;
using FileManagement;
using System;
using NWaves.Filters.Base;
using NWaves.Signals;
public class NWavesController : MonoBehaviour
{

    [SerializeField]
    private float _delayTime, _feedbackTime;

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.N))
        //{
        //    StartNwavesDelay();
        //}
        if (Input.GetKeyDown(KeyCode.C))
        {
            //StartNwavesPitch();
            StartNwavesFree();
        }


    }

    public async void StartNwavesFree()
    {
        WaveFile waveFile;
        float[] paddedSamples;
        float[] paddedSamplesL;
        float[] paddedSamplesR;
        DiscreteSignal padding;

        var template = await FileManager.Instance.TemplateFromGuid(FileManager.Instance.SelectedSampleGuid);
        var templatePath = FileManager.Instance.SamplePathFromGuid(template);
        // Load the input audio file
        using (var stream = new FileStream(templatePath, FileMode.Open))
        {
            waveFile = new WaveFile(stream);
        }

        int numSamples = waveFile[Channels.Left].Length;

        if (waveFile.WaveFmt.ChannelCount == 1)
        {
            paddedSamples = new float[numSamples * 4];
            padding = new DiscreteSignal(waveFile.Signals[0].SamplingRate, paddedSamples, true);
            waveFile.Signals[0] = waveFile.Signals[0].Concatenate(padding);
        }
        else
        {
            paddedSamplesL = new float[numSamples * 4];
            paddedSamplesR = new float[numSamples * 4];

            var paddingL = new DiscreteSignal(waveFile.Signals[0].SamplingRate, paddedSamplesL, true);
            var paddingR = new DiscreteSignal(waveFile.Signals[0].SamplingRate, paddedSamplesR, true);

            waveFile.Signals[0] = waveFile.Signals[0].Concatenate(paddingL);
            waveFile.Signals[1] = waveFile.Signals[1].Concatenate(paddingR);
        }



        // Create a delay effect with specified parameters
        //var delay = new EchoEffect(waveFile.Signals[0].SamplingRate, _delayTime, _feedbackTime, NWaves.Utils.InterpolationMode.Linear, 2f);
        //var reverb = new FreeverbReverbEffect(waveFile.Signals[0].SamplingRate, 0.015f, 0.5f, 0.5f);
        // Apply the delay effect to the input signal

        var reverb = new FreeverbReverbEffect(waveFile.Signals[0].SamplingRate, .1f, 1f, 0f);



        var outputSignal = reverb.ApplyTo(waveFile.Signals[0], waveFile.Signals[1]);

        // Replace the original signal with the new signal
        waveFile.Signals[0] = outputSignal.Item1;
        waveFile.Signals[1] = outputSignal.Item2;

        // Save the modified audio to a new file
        try
        {
            // Construct the output file path
            string outputPath = Path.Combine(Application.persistentDataPath, "modified_audio.wav");
            //string outputPath = FileManager.Instance.SelectedSamplePath;

            // Save the modified audio to the output file
            using (var outputStream = new FileStream(outputPath, FileMode.Create))
            {
                waveFile.SaveTo(outputStream);
            }
            Debug.Log("Modified audio saved successfully to: " + outputPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save modified audio: " + ex.Message);
        }
    }

    public async void StartNwavesPitch()
    {
        WaveFile waveFile;

        var template = await FileManager.Instance.TemplateFromGuid(FileManager.Instance.SelectedSampleGuid);
        var templatePath = FileManager.Instance.SamplePathFromGuid(template);
        // Load the input audio file
        using (var stream = new FileStream(templatePath, FileMode.Open))
        {
            waveFile = new WaveFile(stream);
        }

        float[] paddedSamples = new float[waveFile.Signals[0].Length * 4];

        var padding = new DiscreteSignal(waveFile.Signals[0].SamplingRate, paddedSamples, true);

        var total = waveFile.Signals[0].Concatenate(padding);

        // Create a delay effect with specified parameters
        var delay = new EchoEffect(waveFile.Signals[0].SamplingRate, _delayTime, _feedbackTime, NWaves.Utils.InterpolationMode.Linear, 2f);
        
        var pitch = new PitchShiftEffect(2f);


        // Apply the delay effect to the input signal
        var outputSignal = pitch.ApplyTo(total, FilteringMethod.DifferenceEquation);

        // Replace the original signal with the new signal
        waveFile.Signals[0] = outputSignal;


        // Save the modified audio to a new file
        try
        {
            // Construct the output file path
            //string outputPath = Path.Combine(Application.persistentDataPath, "modified_audio_pitch.wav");
            string outputPath = FileManager.Instance.SelectedSamplePath;
            // Save the modified audio to the output file
            using (var outputStream = new FileStream(outputPath, FileMode.Create))
            {
                waveFile.SaveTo(outputStream);
            }
            Debug.Log("Modified audio saved successfully to: " + outputPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save modified audio: " + ex.Message);
        }
    }
    public async void StartNwavesDelay()
    {
        WaveFile waveFile;

        var template = await FileManager.Instance.TemplateFromGuid(FileManager.Instance.SelectedSampleGuid);
        var templatePath = FileManager.Instance.SamplePathFromGuid(template);
        // Load the input audio file
        using (var stream = new FileStream(templatePath, FileMode.Open))
        {
            waveFile = new WaveFile(stream);
        }

        float[] paddedSamples = new float[waveFile.Signals[0].Length * 4];

        var padding = new DiscreteSignal(waveFile.Signals[0].SamplingRate, paddedSamples, true);

        var total = waveFile.Signals[0].Concatenate(padding);

        // Create a delay effect with specified parameters
        var delay = new EchoEffect(waveFile.Signals[0].SamplingRate, _delayTime, _feedbackTime, NWaves.Utils.InterpolationMode.Linear, 2f);

        // Apply the delay effect to the input signal
        var outputSignal = delay.ApplyTo(total, FilteringMethod.DifferenceEquation);

        // Replace the original signal with the new signal
        waveFile.Signals[0] = outputSignal;

        // Save the modified audio to a new file
        try
        {
            // Construct the output file path
            //string outputPath = Path.Combine(Application.persistentDataPath, "modified_audio.wav");
            string outputPath = FileManager.Instance.SelectedSamplePath;

            // Save the modified audio to the output file
            using (var outputStream = new FileStream(outputPath, FileMode.Create))
            {
                waveFile.SaveTo(outputStream);
            }
            Debug.Log("Modified audio saved successfully to: " + outputPath);
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to save modified audio: " + ex.Message);
        }
    }
}
