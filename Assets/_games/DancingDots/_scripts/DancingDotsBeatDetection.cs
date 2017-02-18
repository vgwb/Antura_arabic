using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace EA4S.Minigames.DancingDots
{
    public class DancingDotsBeatDetection : MonoBehaviour
    {

        public float pitchThreshold = 200f, RmsThreshold = 0.19f;
        DancingDotsQuadManager disco;
        DancingDotsGame game;


        public float RmsValue;
        public float DbValue;
        public float PitchValue;

        private const int QSamples = 2048;
        private const float RefValue = 0.1f;
        private const float Threshold = 0.02f;

        float[] _samples;
        private float[] _spectrum;
        private float _fSample;

        void Start()
        {
            //AudioProcessor processor = FindObjectOfType<AudioProcessor>();
            //processor.addAudioCallback(this);

            disco = GameObject.Find("Quads").GetComponent<DancingDotsQuadManager>();
            game = GameObject.Find("Dancing Dots Game Manager").GetComponent<DancingDotsGame>();
            _samples = new float[QSamples];
            _spectrum = new float[QSamples];
            _fSample = AudioSettings.outputSampleRate;
        }

        void Update()
        {
            AnalyzeSound();

            if (PitchValue > pitchThreshold || RmsValue > RmsThreshold)
            {
                disco.swap();
                disco.swap(game.DDBackgrounds);
            }

            string text = "RMS: " + RmsValue.ToString("F2") +
            " (" + DbValue.ToString("F1") + " dB)" +
            "Pitch: " + PitchValue.ToString("F0") + " Hz";

            //Debug.LogWarning(text);
        }

        void AnalyzeSound()
        {
            GetComponent<AudioSource>().GetOutputData(_samples, 0); // fill array with samples
            int i;
            float sum = 0;
            for (i = 0; i < QSamples; i++)
            {
                sum += _samples[i] * _samples[i]; // sum squared samples
            }
            RmsValue = Mathf.Sqrt(sum / QSamples); // rms = square root of average
            DbValue = 20 * Mathf.Log10(RmsValue / RefValue); // calculate dB
            if (DbValue < -160) DbValue = -160; // clamp it to -160dB min
                                                // get sound spectrum
            GetComponent<AudioSource>().GetSpectrumData(_spectrum, 0, FFTWindow.BlackmanHarris);
            float maxV = 0;
            var maxN = 0;
            for (i = 0; i < QSamples; i++)
            { // find max 
                if (!(_spectrum[i] > maxV) || !(_spectrum[i] > Threshold))
                    continue;

                maxV = _spectrum[i];
                maxN = i; // maxN is the index of max
            }
            float freqN = maxN; // pass the index to a float variable
            if (maxN > 0 && maxN < QSamples - 1)
            { // interpolate index using neighbours
                var dL = _spectrum[maxN - 1] / _spectrum[maxN];
                var dR = _spectrum[maxN + 1] / _spectrum[maxN];
                freqN += 0.5f * (dR * dR - dL * dL);
            }
            PitchValue = freqN * (_fSample / 2) / QSamples; // convert index to frequency
        }

        /*
        public float sensitivity = 9;
        public float loudness = 0;
        private AudioSource _audio;

        void Start()
        {
            floor = GameObject.Find("Quads").GetComponent<DancingDotsQuadManager>();
            _audio = GetComponent<AudioSource>();
            //_audio.clip = Microphone.Start(null, true, 10, 44100);
            //_audio.loop = true;
            //_audio.mute = false;
            //while (!(Microphone.GetPosition(null) > 0)) { }
            //_audio.Play();
        }
        void Update()
        {

            loudness = GetAveragedVolume() * sensitivity;
            if (loudness > 1)
            {
                Debug.LogWarning("Beat!" + Time.deltaTime);
                if (canSwap)
                {
                    floor.swap();
                    //canSwap = false;
                    //StartCoroutine(reset());
                }
            }
            else
                Debug.LogError("sdasd");
        }
        float GetAveragedVolume()
        {
            float[] data = new float[256];
            float a = 0;
            _audio.GetOutputData(data, 0);
            foreach (float s in data)
            {
                a += Mathf.Abs(s);
            }
            return a / 256;
        }*/

        /*    
        int qSamples = 1024;  // array size
     float refValue = 0.1f; // RMS value for 0 dB
     float rmsValue;   // sound level - RMS
     float dbValue;    // sound level - dB
     float volume = 2; // set how much the scale will vary

     private float[] samples; // audio samples

     void Start()
        {
            samples = new float[qSamples];
            GetComponent<AudioSource>().Play();
        }

        void GetVolume()
        {
            GetComponent<AudioSource>().GetOutputData(samples, 0); // fill array with samples
            int i;
            float sum = 0;
            for (i = 0; i < qSamples; i++)
            {
                sum += samples[i] * samples[i]; // sum squared samples
            }
            rmsValue = Mathf.Sqrt(sum / qSamples); // rms = square root of average
            dbValue = 20 * Mathf.Log10(rmsValue / refValue); // calculate dB
            if (dbValue < -160) dbValue = -160; // clamp it to -160dB min
        }
        public Transform t;
        public float threshold = 0.15f, modefier = 1;
        void Update()
        {
            GetVolume();
            if (volume * rmsValue > threshold)
                Debug.LogError("!!!");
            else
                Debug.LogWarning("__");
            //t = GameObject.Find("Cube (1)").transform;
            //t.localScale = new Vector3(t.localScale.x, volume * rmsValue * modefier, t.localScale.z);
        }
        */
    }
}