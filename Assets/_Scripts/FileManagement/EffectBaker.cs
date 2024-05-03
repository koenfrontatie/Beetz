using UnityEngine;
using NWaves.Audio;
using NWaves.Effects;
using System.IO;
using NWaves.Utils;
using NWaves.Signals;

namespace FileManagement
{
    public class EffectBaker : MonoBehaviour
    {
        FileManager _fileviewer;
        private void Awake()
        {
            _fileviewer = GameObject.FindObjectOfType<FileManager>();
        }
        public void AddDistortion()
        {
            var path = _fileviewer.SelectedSamplePath;

            WaveFile waveFile;
            
            FileStream readStream;

            using (readStream = new FileStream(path, FileMode.Open))
            {
                waveFile = new WaveFile(readStream);
            }

            readStream.Close();

            var dist = new DistortionEffect(DistortionMode.SoftClipping, 40, -20);
            var outputsignal = dist.ApplyTo(waveFile.Signals[0], NWaves.Filters.Base.FilteringMethod.DifferenceEquation);
            waveFile.Signals[0] = outputsignal;

            FileStream writeStream;

            waveFile.SaveTo(writeStream = new FileStream($"{Path.Combine(Application.persistentDataPath, path)}", FileMode.Create));

            writeStream.Close();
        }

        public void AddReverb()
        {
            var path = _fileviewer.SelectedSamplePath;

            WaveFile waveFile;

            FileStream readStream;

            using (readStream = new FileStream(path, FileMode.Open))
            {
                waveFile = new WaveFile(readStream);
            }

            readStream.Close();

            var rev = new Reverb(.99f, .2f);
            var outputsignal = rev.ApplyTo(waveFile.Signals[0]);
            waveFile.Signals[0] = outputsignal;

            FileStream writeStream;

            waveFile.SaveTo(writeStream = new FileStream($"{Path.Combine(Application.persistentDataPath, path)}", FileMode.Create));

            writeStream.Close();
        }


        //public void AddReverb()
        //{
        //    var path = _fileviewer.SelectedSamplePath;

        //    WaveFile waveFile;

        //    using (var stream = new FileStream(path, FileMode.Open))
        //    {
        //        waveFile = new WaveFile(stream);
        //    }

        //    // Define delay parameters to simulate reverb
        //    var samplingRate = waveFile.Signals[0].SamplingRate; // Use the sample rate of the wave file
        //    var delayTimes = new[] { 0.1f, 0.11f, 0.12f }; // Array of delay times in seconds
        //    var feedback = 0.7f; // Feedback factor (0 to 1)
        //    var interpolationMode = InterpolationMode.Linear; // Choose an interpolation mode
        //    var reserveDelay = 1.0f; // Reserve delay time in seconds

        //    // Initialize a signal to hold the output
        //    DiscreteSignal outputSignal = waveFile.Signals[0].Copy();

        //    // Apply multiple delay effects to simulate reverb
        //    foreach (var delayTime in delayTimes)
        //    {
        //        var delay = new DelayEffect(samplingRate, delayTime, feedback, interpolationMode, reserveDelay);
        //        var delayedSignal = delay.ApplyTo(waveFile.Signals[0], NWaves.Filters.Base.FilteringMethod.DifferenceEquation);
        //        outputSignal = outputSignal + delayedSignal; // Use the + operator for superimposition
        //    }

        //    // Normalize the output signal to prevent clipping
        //    outputSignal.NormalizeMax(1);

        //    // Replace the original signal with the processed signal
        //    waveFile.Signals[0] = outputSignal;

        //    // Save the processed wave file to a new file
        //    using (var outputStream = new FileStream(path, FileMode.Create))
        //    {
        //        waveFile.SaveTo(outputStream);
        //    }
        //}


        // Update is called once per frame
        void Update()
        {
            //WaveFile waveFile;

            //using (var stream = new FileStream($"{Path.Combine(Application.persistentDataPath, "snare.wav")}", FileMode.Open))
            //{
            //    waveFile = new WaveFile(stream);
            //}
            //var dist = new DistortionEffect(DistortionMode.SoftClipping, 40, -20);
            //var outputsignal = dist.ApplyTo(waveFile.Signals[0], NWaves.Filters.Base.FilteringMethod.DifferenceEquation);
            //waveFile.Signals[0] = outputsignal;
            //waveFile.SaveTo(new FileStream($"{Path.Combine(Application.persistentDataPath, "babebabebab.wav")}", FileMode.Create));

            //WaveFile waveFile;

            //// Load the original wave file
            //using (var stream = new FileStream($"{Path.Combine(Application.persistentDataPath, "input.wav")}", FileMode.Open))
            //{
            //    waveFile = new WaveFile(stream);
            //}

            //// Create silence segment
            //var silenceDuration = 0.2; // in seconds
            //var silenceLength = (int)(silenceDuration * waveFile.Signals[0].SamplingRate);
            //var silenceSignal = DiscreteSignal.Constant(0, silenceLength, waveFile.Signals[0].SamplingRate);

            //// Concatenate input with silence
            //var concatenatedSignal = new DiscreteSignal(waveFile.Signals[0].SamplingRate * 10 * (waveFile.Signals[0].Length + silenceLength * 10));
            //for (int i = 0; i < 10; i++)
            //{
            //    concatenatedSignal.AddRange(waveFile.Signals[0]);
            //    concatenatedSignal.AddRange(silenceSignal);
            //}

            //// Convolution (block convolution or any other method you prefer)
            //// This step is optional depending on your specific requirements

            //// Save the convolved signal
            //WaveFile convolvedWaveFile = new WaveFile(waveFile.Signals[0].SamplingRate, waveFile.BitsPerSample, waveFile.Channels);
            //convolvedWaveFile.SetData(0, concatenatedSignal);
            //convolvedWaveFile.SaveTo(new FileStream($"{Path.Combine(Application.persistentDataPath, "output.wav")}", FileMode.Create));



            //WaveFile waveFile;

            //// Load the original wave file
            //using (var stream = new FileStream($"{Path.Combine(Application.persistentDataPath, "snare.wav")}", FileMode.Open))
            //{
            //    waveFile = new WaveFile(stream);
            //}

            //// Apply distortion effect
            //var dist = new DistortionEffect(DistortionMode.SoftClipping, 40, -20);
            //var outputsignal = dist.ApplyTo(waveFile.Signals[0], NWaves.Filters.Base.FilteringMethod.DifferenceEquation);
            //waveFile.Signals[0] = outputsignal;

            //// Repeat the signal 10 times with 0.2 seconds of silence in between
            //var repeatedSignal = new DiscreteSignal(outputsignal.SamplingRate * 10 * (outputsignal.Signals[0].Length + (int)(0.2 * outputsignal.SamplingRate)));
            //for (int i = 0; i < 10; i++)
            //{
            //    repeatedSignal.Concatenate(outputsignal);
            //    repeatedSignal.Concatenate(DiscreteSignal.Constant((int)(0.2 * outputsignal.SamplingRate)));
            //}

            //// Update the wave file with the repeated signal
            //waveFile.Signals[0] = repeatedSignal;

            //// Save the modified wave file
            //waveFile.SaveTo(new FileStream($"{Path.Combine(Application.persistentDataPath, "babebabebab.wav")}", FileMode.Create));
        }
    }
}
