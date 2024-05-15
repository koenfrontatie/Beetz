using System.Collections;
using System.Collections.Generic;
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
        if(Input.GetKeyDown(KeyCode.N))
        {
            StartNwavesDelay();
        }   
    }

    void StartNwavesDelay()
    {
        WaveFile waveFile;

        // Load the input audio file
        using (var stream = new FileStream(FileManager.Instance.SelectedSamplePath, FileMode.Open))
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
            string outputPath = Path.Combine(Application.persistentDataPath, "modified_audio.wav");

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
