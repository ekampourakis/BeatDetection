﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace BeatDetection {

    [Serializable]
    internal class InitExceptions : Exception {
        public InitExceptions() { }
        public InitExceptions(string message) : base(message) { }
        public InitExceptions(string message, Exception innerException) : base(message, innerException) { }
        protected InitExceptions(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    [Serializable]
    internal class BassWasapiInitException : Exception {
        public BassWasapiInitException() { }
        public BassWasapiInitException(string message) : base(message) { }
        public BassWasapiInitException(string message, Exception innerException) : base(message, innerException) { }
        protected BassWasapiInitException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    public enum SensivityLevel {
        VERY_LOW = 120,
        LOW = 110,
        NORMAL = 100,
        HIGH = 80,
        VERY_HIGH = 70
    };

    public enum BeatType {
        NO_BEAT = 0,
        BASS_ONLY = 1,
        MIDS_ONLY = 2,
        BASS_AND_MIDS = 3,
        HIGHS_ONLY = 4,
        BASS_AND_HIGHS = 5,
        MIDS_AND_HIGHS = 6,
        BASS_MIDS_AND_HIGHS = 7
    };

    public sealed class BeatDetector {
        #region Fields

        // Constants
        private const int BANDS = 10;
        private const int HISTORY = 50;

        // Events
        public delegate void BeatDetectedHandler(BeatType Beat);
        private event BeatDetectedHandler OnDetected;

        // Threading
        private Thread _AnalysisThread;

        // BASS Process
        private WASAPIPROC _WasapiProcess = new WASAPIPROC(BeatDetector.Process);

        // Analysis settings
        private int _SamplingRate;
        private int _DeviceCode;
        private SensivityLevel _BASSSensivity;
        private SensivityLevel _MIDSSensivity;

        // Analysis data
        private float[] _FFTData = new float[4096];
        public double[,] _History = new double[BANDS, HISTORY];

        #endregion

        #region Setup methods

        public BeatDetector(int DeviceCode = -1, int SamplingRate = 44100, SensivityLevel BASSSensivity = SensivityLevel.NORMAL, SensivityLevel MIDSSensivity = SensivityLevel.NORMAL) {
            _SamplingRate = SamplingRate;
            _BASSSensivity = BASSSensivity;
            _MIDSSensivity = MIDSSensivity;
            _DeviceCode = DeviceCode;
            Init();
        }

        private void Init() {
            // Initialize BASS on default device
            if (!Bass.BASS_Init(0, _SamplingRate, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero)) {
                throw new InitExceptions(Bass.BASS_ErrorGetCode().ToString());
            }

            if (_DeviceCode != -1) {
                // Initialize WASAPI
                if (!BassWasapi.BASS_WASAPI_Init(_DeviceCode, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _WasapiProcess, IntPtr.Zero)) {
                    throw new BassWasapiInitException(Bass.BASS_ErrorGetCode().ToString());
                }
                BassWasapi.BASS_WASAPI_Start();    
            }
            Thread.Sleep(500);
        }

        ~BeatDetector() {
            // Kill working thread and clean after BASS
            if (_AnalysisThread != null && _AnalysisThread.IsAlive) { _AnalysisThread.Abort(); }
            Free();
        }

        public void SetBassSensivity(SensivityLevel Sensivity) { _BASSSensivity = Sensivity; }

        public void SetMidsSensivity(SensivityLevel Sensivity) { _MIDSSensivity = Sensivity; }

        #endregion

        #region BASS-dedicated Methods

        // WASAPI callback, required for continuous recording
        private static int Process(IntPtr buffer, int length, IntPtr user) { return length; }

        // Cleans after BASS
        public void Free() {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }

        #endregion

        #region Analysis public methods

        // Starts a new Analysis Thread
        public void StartAnalysis() {
            // Kills currently running analysis thread if alive
            if (_AnalysisThread != null && _AnalysisThread.IsAlive) { _AnalysisThread.Abort(); }

            // Starts a new high-priority thread
            _AnalysisThread = new Thread(delegate () {
                while (true) {
                    Thread.Sleep(5);
                    PerformAnalysis();
                }
            }) {
                Priority = ThreadPriority.Highest
            };
            _AnalysisThread.Start();
        }

        // Kills running thread
        public void StopAnalysis() {
            if (_AnalysisThread != null && _AnalysisThread.IsAlive) { _AnalysisThread.Abort(); }
        }

        #endregion

        #region Event handling

        public void Subscribe(BeatDetectedHandler Delegate) { OnDetected += Delegate; }

        public void UnSubscribe(BeatDetectedHandler Delegate) { OnDetected -= Delegate; }

        #endregion

        #region Analysis private methods

        // Shifts history 'Loops' places to the right
        private void ShiftHistory(int Loops) {
            for (int i = 0; i < BANDS; i++) {
                for (int j = HISTORY - 1 - Loops; j >= 0; j--) {
                    _History[i, j + Loops] = _History[i, j];
                }
            }
        }

        // Performs FFT analysis in order to detect beat
        private void PerformAnalysis() {
            // Specifes on which result end which band (dividing it into 10 bands)
            // 19 - bass, 187 - mids, rest is highs
            int[] BandRange = { 4, 8, 18, 38, 48, 94, 140, 186, 466, 1022, 22000 };
            double[] BandsTemp = new double[BANDS];
            int n = 0;
            int level = BassWasapi.BASS_WASAPI_GetLevel();

            // Get FFT
            int ret = BassWasapi.BASS_WASAPI_GetData(_FFTData, (int)BASSData.BASS_DATA_FFT1024 | (int)BASSData.BASS_DATA_FFT_COMPLEX); //get channel fft data
            if (ret < -1) return;

            // Calculate the energy of every result and divide it into subbands
            float sum = 0;

            for (int i = 2; i < 2048; i = i + 2) {
                float real = _FFTData[i];
                float complex = _FFTData[i + 1];
                sum += (float)Math.Sqrt((double)(real * real + complex * complex));

                if (i == BandRange[n]) {
                    BandsTemp[n++] = (BANDS * sum) / 1024;
                    sum = 0;
                }
            }

            // Detect beat basing on FFT results
            DetectBeat(BandsTemp, level);

            // Shift the history register and save new values
            ShiftHistory(1);

            for (int i = 0; i < BANDS; i++) {
                _History[i, 0] = BandsTemp[i];
            }
        }

        // Calculate the average value of every band
        private double[] CalculateAverages() {
            double[] avg = new double[BANDS];

            for (int i = 0; i < BANDS; i++) {
                double sum = 0;

                for (int j = 0; j < HISTORY; j++) {
                    sum += _History[i, j];
                }

                avg[i] = (sum / HISTORY);
            }

            return avg;
        }

        // Detects beat basing on analysis result
        // Beat detection is marked on the first three bits of the returned value
        private void DetectBeat(double[] Energies, int volume) {
            // Sound height ranges (1, 2 is bass, next 6 is mids)
            int Bass = 3;
            int Mids = 6;

            double[] avg = CalculateAverages();
            byte result = 0;
            double volumelevel = (double)volume / 32768 * 100;   // Volume level in %

            // Search all bands for a beat
            for (int i = 0; i < BANDS && result == 0; i++) {
                // Set the C parameter
                double C = 0;

                // Set sensitivity 
                if (i < Bass) {
                    C = 2.3 * ((double)_BASSSensivity / 100);
                } else if (i < Mids) {
                    C = 2.89 * ((double)_MIDSSensivity / 100);
                } else {
                    C = 3 * ((double)_MIDSSensivity / 100);
                }

                // Compare energies in all bands with C*average
                // Second rule is for noise reduction
                if (Energies[i] > (C * avg[i]) && volumelevel > 1) {
                    byte res = 0;
                    if (i < Bass) {
                        res = 1;
                    } else if (i < Mids) {
                        res = 2;
                    } else {
                        res = 4;
                    }
                    result = (byte)(result | res);
                }
            }

            if (result > 0) { OnDetected(ByteToBeatType(result)); }
        }

        #endregion

        #region Various function
        public void PrintDevices() {
            for (int Index = 0; Index < BassWasapi.BASS_WASAPI_GetDeviceCount(); Index++) {
                var BassDevice = BassWasapi.BASS_WASAPI_GetDeviceInfo(Index);
                if (BassDevice.IsEnabled && BassDevice.IsLoopback) {
                    Console.WriteLine(string.Format("{0} - {1}", Index, BassDevice.name));
                }
            }
        }

        public List<String> ReturnDevices() {
            List<String> Devices = new List<String>();
            for (int Index = 0; Index < BassWasapi.BASS_WASAPI_GetDeviceCount(); Index++) {
                var BassDevice = BassWasapi.BASS_WASAPI_GetDeviceInfo(Index);
                if (BassDevice.IsEnabled && BassDevice.IsLoopback) {
                    Devices.Add(string.Format("{0} - {1}", Index, BassDevice.name));
                }
            }
            return Devices;
        }

        public double[] HistoryBands() {
            double[] Tmp = new double[BANDS];
            for (int Index = 0; Index < BANDS; Index++) {
                Tmp[Index] = _History[Index, 0];
            }
            return Tmp;
        }

        private BeatType ByteToBeatType(byte Result) {
            switch (Result) {
                case 1:
                    return BeatType.BASS_ONLY;
                case 2:
                    return BeatType.MIDS_ONLY;
                case 3:
                    return BeatType.BASS_AND_MIDS;
                case 4:
                    return BeatType.HIGHS_ONLY;
                case 5:
                    return BeatType.BASS_AND_HIGHS;
                case 6:
                    return BeatType.MIDS_AND_HIGHS;
                case 7:
                    return BeatType.BASS_MIDS_AND_HIGHS;
                default:
                    return BeatType.NO_BEAT;
            }
        }
        #endregion
    }
}