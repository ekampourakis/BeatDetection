using System;
using System.Windows.Forms;

namespace BeatDetection {

    public partial class Main : Form {

        BeatDetector Detector = new BeatDetector();

        public Main() {
            InitializeComponent();
        }

        private void InitGUI() {
            // Select normal sensitivity by default
            ComboBox_MidsSensitivity.SelectedIndex = 2;
            ComboBox_BassSensitivity.SelectedIndex = 2;

            // Load all available deviced
            ComboBox_InputDevices.Items.AddRange(Detector.ReturnDevices().ToArray());
            if (ComboBox_InputDevices.Items.Count > 0) {
                ComboBox_InputDevices.SelectedIndex = 0;
            }         
        }

        private void TestGUI_Load(object sender, EventArgs e) {
            InitGUI();
        }

        private void BeatDetected(BeatType Beat) {
            Console.WriteLine("Beat Detected @ " + Environment.TickCount);
        }

        private void Button_StartStop_Click(object sender, EventArgs e) {
            Detector.StartAnalysis();
        }

        private SensivityLevel TextToSensitivity(String Value) {
            switch (Value) {
                case "Very Low":
                    return SensivityLevel.VERY_LOW;
                case "Low":
                    return SensivityLevel.LOW;
                case "High":
                    return SensivityLevel.HIGH;
                case "Very High":
                    return SensivityLevel.VERY_HIGH;
                default:
                    return SensivityLevel.NORMAL;
            }
        }

        private void ComboBox_InputDevices_SelectedIndexChanged(object sender, EventArgs e) {
            if (ComboBox_InputDevices.SelectedItem.ToString() != "") {
                Detector.StopAnalysis();
                Detector.Free();
                int DeviceCode = Convert.ToUInt16(ComboBox_InputDevices.SelectedItem.ToString().Substring(0, ComboBox_InputDevices.SelectedItem.ToString().IndexOf("-")));
                SensivityLevel BassSensitivity = TextToSensitivity(ComboBox_BassSensitivity.SelectedItem.ToString());
                SensivityLevel MidsSensitivity = TextToSensitivity(ComboBox_MidsSensitivity.SelectedItem.ToString());
                Detector = new BeatDetector(DeviceCode, 44100, BassSensitivity, MidsSensitivity);
                Detector.Subscribe(BeatDetected);
            }
        }
    }
}
