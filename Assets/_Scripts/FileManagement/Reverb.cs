//using NWaves.Effects.Base;
//using NWaves.Filters.Base;
//using NWaves.Filters.Fda;
//using NWaves.Filters;
//using NWaves.Signals;

////public class Reverb : AudioEffect
////{
////    private readonly CombFeedbackFilter[] _combFiltersL;
////    private readonly CombFeedbackFilter[] _combFiltersR;
////    private readonly FirFilter[] _allpassFiltersL;
////    private readonly FirFilter[] _allpassFiltersR;
////    private readonly float _gain;
////    private readonly float _wet1;
////    private readonly float _wet2;
////    private readonly float _dry;

////    int[] combDelayLengths = new int[] { 1116, 1188, 1277, 1356 };
////    int[] allpassDelayLengths = new int[] { 556, 441, 341, 225 };
////    public Reverb(float gain, float wet1, float wet2, float dry)
////    {
////        _gain = gain;
////        _wet1 = wet1;
////        _wet2 = wet2;
////        _dry = dry;

////        // Initialize comb filters
////        _combFiltersL = new CombFeedbackFilter[combDelayLengths.Length];
////        _combFiltersR = new CombFeedbackFilter[combDelayLengths.Length];
////        for (int i = 0; i < combDelayLengths.Length; i++)
////        {
////            _combFiltersL[i] = new CombFeedbackFilter(combDelayLengths[i], 0.84f, 0.84f);
////            _combFiltersR[i] = new CombFeedbackFilter(combDelayLengths[i], 0.84f, 0.84f);
////        }

////        // Initialize all-pass filters
////        _allpassFiltersL = new FirFilter[allpassDelayLengths.Length];
////        _allpassFiltersR = new FirFilter[allpassDelayLengths.Length];
////        for (int i = 0; i < allpassDelayLengths.Length; i++)
////        {
////            _allpassFiltersL[i] = new FirFilter(DesignFilter.FirWinFdAp(1, allpassDelayLengths[i]));
////            _allpassFiltersR[i] = new FirFilter(DesignFilter.FirWinFdAp(1, allpassDelayLengths[i]));
////        }
////    }
////    public override DiscreteSignal ApplyTo(DiscreteSignal signal, FilteringMethod method = FilteringMethod.Auto)
////    {
////        var output = new float[signal.Length];

////        for (int n = 0; n < signal.Length; n++)
////        {
////            float outL = 0, outR = 0;
////            float input = (signal[n] + signal[n]) * _gain;

////            // Accumulate comb filters in parallel
////            for (int i = 0; i < _combFiltersL.Length; i++)
////            {
////                outL += _combFiltersL[i].Process(input);
////                outR += _combFiltersR[i].Process(input);
////            }

////            // Feed through all-pass filters in series
////            for (int i = 0; i < _allpassFiltersL.Length; i++)
////            {
////                // Apply the all-pass filter to the left channel
////                outL = _allpassFiltersL[i].ApplyTo(new DiscreteSignal(signal.SamplingRate, new[] { outL }), method).Samples[0];

////                // Apply the all-pass filter to the right channel
////                outR = _allpassFiltersR[i].ApplyTo(new DiscreteSignal(signal.SamplingRate, new[] { outR }), method).Samples[0];
////            }

////            // Calculate output REPLACING anything already there
////            output[n] = (outL * _wet1 + outR * _wet2 + signal[n] * _dry) / 2.0f; // Average the left and right channels
////        }

////        return new DiscreteSignal(signal.SamplingRate, output, true);
////    }

////    //Override the Process method for online processing(not used for offline processing)
////        public override float Process(float sample)
////    {
////        // This method is not used for offline processing
////        // You can leave it as it is or remove it
////        return sample;
////    }

////    // Override the Reset method to reset the state of the filters
////    public override void Reset()
////    {
////        // Reset state of comb filters
////        //foreach (var combFilter in _combFilters)
////        //{
////        //    combFilter.Reset();
////        //}

////        //// Reset state of all-pass filters
////        //foreach (var allpassFilter in _allpassFilters)
////        //{
////        //    allpassFilter.Reset();
////        //}
////    }

////}



//using NWaves.Effects.Base;
//using NWaves.Filters.Base;
//using NWaves.Filters.Fda;
//using NWaves.Filters;
//using NWaves.Signals;
//using TMPro;

//public class Reverb : AudioEffect
//{
//    private readonly CombFeedbackFilter[] _combFilters;
//    private readonly float[] _combGains;
//    private readonly FirFilter[] _allpassFilters;
//    FilterChain AllpassFilterChain;
//    FilterChain CombFilterChain;

//    public Reverb(float feedback, float feedforward)
//    {
//        // Initialize comb filters (adjust delay lengths and feedback/ff as needed)
//        _combFilters = new CombFeedbackFilter[]
//        {
//            new CombFeedbackFilter(441, feedback, feedforward),
//            new CombFeedbackFilter(651, feedback, feedforward),
//            // Add more comb filters as needed
//        };

//        CombFilterChain = new FilterChain(_allpassFilters);


//        //// Initialize gains for each comb filter
//        //_combGains = new float[_combFilters.Length];
//        //for (int i = 0; i < _combGains.Length; i++)
//        //{
//        //    _combGains[i] = 1.0f / _combFilters.Length;  // Equal gain for each comb filter
//        //}

//        // Initialize all-pass filters with appropriate delay lengths
//        _allpassFilters = new FirFilter[]
//        {
//            new FirFilter(DesignFilter.FirWinFdAp(1, 210)),
//            new FirFilter(DesignFilter.FirWinFdAp(2, 158)),
//            new FirFilter(DesignFilter.FirWinFdAp(3, 561)),
//            new FirFilter(DesignFilter.FirWinFdAp(4, 410))
//        };

//        AllpassFilterChain = new FilterChain(_allpassFilters);
//    }

//    // Override the ApplyTo method for offline processing
//    public override DiscreteSignal ApplyTo(DiscreteSignal signal, FilteringMethod method = FilteringMethod.Auto)
//    {
//        // Apply comb filters
//        //DiscreteSignal combOutput = signal;
//        //foreach (var combFilter in _combFilters)
//        //{
//        //    combOutput = combFilter.ApplyTo(combOutput);
//        //}

//        var combgain = CombFilterChain.EstimateGain();
//        var combOutput = CombFilterChain.ApplyTo(signal, combgain);

//        // Apply all-pass filters
//        //DiscreteSignal output = combOutput;
//        //foreach (var allpassFilter in _allpassFilters)
//        //{
//        //    output = allpassFilter.ApplyTo(output);
//        //}
//        var apgain = AllpassFilterChain.EstimateGain();

//        var output = AllpassFilterChain.ApplyTo(combOutput, apgain);

//        return output;
//    }

//    // Override the Process method for online processing (not used for offline processing)
//    public override float Process(float sample)
//    {
//        // This method is not used for offline processing
//        // You can leave it as it is or remove it
//        return sample;
//    }

//    // Override the Reset method to reset the state of the filters
//    public override void Reset()
//    {
//        // Reset state of comb filters
//        foreach (var combFilter in _combFilters)
//        {
//            combFilter.Reset();
//        }

//        // Reset state of all-pass filters
//        foreach (var allpassFilter in _allpassFilters)
//        {
//            allpassFilter.Reset();
//        }
//    }
//}
