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

            // Initialize serial port with correct settings
            _serialPort = new SerialPort("COM6", 115200); // Replace with the correct COM port
            _serialPort.Open();

            // Attach the event handler to the button
            btnSendColor.Click += BtnSendColor_Click;
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

                // Send the RGB values to the Arduino (3 bytes)
                if (_serialPort.IsOpen)
                {
                    _serialPort.WriteLine(colorString);  // Send the formatted string
                }
            }
        }

        // Close the serial port when the application is closed
        protected override void OnClosed(EventArgs e)
        {
            if (_serialPort.IsOpen)
            {
                _serialPort.Close();
            }
            base.OnClosed(e);
        }
    }
}
