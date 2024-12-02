using System;
using System.IO.Ports;
using System.Windows;
using Xceed.Wpf.Toolkit;

namespace Arduino_RGB_Strip_Controller
{
    public partial class MainWindow : Window
    {
        private SerialPort _serialPort;

        public MainWindow()
        {
            InitializeComponent();

            // Populate COM ports in the ComboBox
            PopulateAndOpenComPort();

            // Wire up event handlers
            btnSendColor.Click += BtnSendColor_Click;
            btnRainbowWave.Click += BtnRainbowWave_Click;
        }

        // Populate the COM port list into ComboBox
        private void PopulateAndOpenComPort()
        {
            string[] comPorts = SerialPort.GetPortNames();

            if (comPorts.Any())
            {
                // Automatically select the first available COM port
                string selectedPort = comPorts[0];
                comPortComboBox.ItemsSource = comPorts; // Populate ComboBox with available ports
                comPortComboBox.SelectedIndex = 0; // Select the first port by default

                // Open the selected COM port
                try
                {
                    _serialPort = new SerialPort(selectedPort, 115200); // Set the port and baud rate
                    _serialPort.Open(); // Open the COM port
                }
                catch (Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show($"Error opening COM port: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("No COM ports found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler for COM port selection change
        private void ComPortComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (comPortComboBox.SelectedItem != null)
            {
                string selectedPort = comPortComboBox.SelectedItem.ToString();

                // If port is already open, close it
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.Close();
                }

                try
                {
                    // Open the newly selected COM port
                    _serialPort = new SerialPort(selectedPort, 115200); // Set the selected port and baud rate
                    _serialPort.Open(); // Open the COM port
                    Xceed.Wpf.Toolkit.MessageBox.Show($"Successfully opened {selectedPort}.", "COM Port Opened", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show($"Error opening COM port: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnSendColor_Click(object sender, RoutedEventArgs e)
        {
            // Get the selected color from the ColorPicker
            var selectedColor = colorPicker.SelectedColor;
            if (selectedColor.HasValue)
            {
                byte red = selectedColor.Value.R;
                byte green = selectedColor.Value.G;
                byte blue = selectedColor.Value.B;

                // Format the string to send to the Arduino
                string colorString = $"red: {red}, green: {green}, blue: {blue},";

                // Send the string to the Arduino
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _serialPort.WriteLine(colorString);  // Send the formatted string
                }
                else
                {
                    Xceed.Wpf.Toolkit.MessageBox.Show("COM port is not open.");
                }
            }
        }

        // Button click to start Rainbow Wave effect
        private void BtnRainbowWave_Click(object sender, RoutedEventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.WriteLine("rainbow_wave"); // Send the command to Arduino to start rainbow wave
            }
            else
            {
                Xceed.Wpf.Toolkit.MessageBox.Show("COM port is not open.");
            }
        }

        // Close the serial port when the application is closed
        protected override void OnClosed(EventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            base.OnClosed(e);
        }
    }
}
