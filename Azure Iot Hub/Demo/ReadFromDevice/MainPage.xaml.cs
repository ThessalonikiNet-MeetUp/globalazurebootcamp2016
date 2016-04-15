using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices;
using System.Threading.Tasks;
using Emmellsoft.IoT.Rpi.SenseHat;
using System.ComponentModel;
using Windows.UI;
using RichardsTech.Sensors;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;


// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ReadFromDevice
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Measurement measurement = null;
        DispatcherTimer timer = null;
        ISenseHat senseHat = null;
        public MainPage()
        {
            this.InitializeComponent();
            
            InitData();
            SetTimer();
        }
        

        private  void SetTimer()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(4);
            timer.Tick +=   Timer_Tick;
            timer.Start();
        }
        Random r = new Random();
        private async void  Timer_Tick(object sender, object e)
        {
            System.Diagnostics.Debug.WriteLine("tick");
            measurement.Temperature = r.Next(30);
            //if (senseHat != null)
            //{
            //    senseHat.Sensors.HumiditySensor.Update();
            //    var temperature = senseHat.Sensors.Temperature ?? 0;
            //    senseHat.Sensors.ImuSensor.Update();
            //        var accelerationX = senseHat.Sensors.Acceleration?.X ?? 0;
            //        measurement.AccelerationX = accelerationX;
            //     measurement.Temperature = temperature;
            //    SendSenseHatDataToCloudAsync(measurement).Wait();
            //    System.Diagnostics.Debug.WriteLine(temperature);

            // send them to the cloud
            await AzureIoTHub.SendSenseHatDataToCloudAsync(measurement);

            //}
        }
        

        private void InitData()
        {
            measurement = new ReadFromDevice.Measurement();
            this.DataContext = measurement;
            measurement.Temperature = 0;
            measurement.Location = "location1";
        }

        private void Read_Click(object sender, RoutedEventArgs e)
        {
            //var deviceClient = DeviceClient.CreateFromConnectionString(deviceConnectionString, TransportType.Http1);
            // var messageInJson = JsonConvert.SerializeObject(data);
            // var message = new Message(Encoding.UTF8.GetBytes(messageInJson));
            // measurement.Temperature = -1;
           // var ans = Task.Run<double>(async () =>
           //{

           //    ISenseHat senseHat = await SenseHatFactory.Singleton.GetSenseHat().ConfigureAwait(false);
           //    // Joystick joystick = new Joystick();
           //    // joystick.Run(senseHat);
           //  //  GravityBlob gravityBlob = new GravityBlob(senseHat);
           //  //  gravityBlob.Run();
           //    senseHat.Sensors.HumiditySensor.Update();
           //    var temperature = senseHat.Sensors.Temperature ?? 0;
           //    System.Diagnostics.Debug.WriteLine(temperature);
           //   // measurement.Temperature = temparature;
           //    return temperature;
           //     // Measurement.Text = temparature.ToString();


           // });
           // measurement.Temperature = ans.Result;
           
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
           // System.Diagnostics.Debug.Write("Loaded Started ");
           // SenseHatFactory.Singleton.GetSenseHat().ContinueWith(t =>
           //   {
           //       if (t.IsCompleted)
           //       {
           //           senseHat = t.Result;
                      
           //       }
           //       else
           //       {
           //           System.Diagnostics.Debug.WriteLine("get sense hat failed");
           //       }
           //   }
           //);
           // System.Diagnostics.Debug.Write("Loaded Ended ");

        }

     
    }
    public class Measurement : INotifyPropertyChanged
    {
        private double temperature;

        public string Location { get;  set; }

        //  private double accelerationX;

        //public double AccelerationX
        //{
        //    get { return accelerationX; }
        //    set { accelerationX = value; NotifyPropertyChanged(nameof(AccelerationX)); }
        //}


        public double Temperature
        {
            get { return temperature; }
            set {
                temperature = value;
                NotifyPropertyChanged(nameof(Temperature));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged( string propertyName )
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }


    public sealed class GravityBlob 
    {
        ISenseHat SenseHat;

        public GravityBlob(ISenseHat senseHat)
        {
            SenseHat = senseHat;
        }

        public  void Run()
        {
            while (true)
            {
               // Sleep(TimeSpan.FromMilliseconds(50));

                if (!SenseHat.Sensors.ImuSensor.Update())
                {
                    continue;
                }

                if (!SenseHat.Sensors.Acceleration.HasValue)
                {
                    continue;
                }

                Color[,] colors = CreateGravityBlobScreen(SenseHat.Sensors.Acceleration.Value);

                SenseHat.Display.CopyColorsToScreen(colors);

                SenseHat.Display.Update();
            }
        }

        private static Color[,] CreateGravityBlobScreen(Vector3 vector)
        {
            double x0 = (vector.X + 1) * 5.5 - 2;
            double y0 = (vector.Y + 1) * 5.5 - 2;

            double distScale = 4;

            var colors = new Color[8, 8];

            bool isUpsideDown = vector.Z < 0;

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    double dx = x0 - x;
                    double dy = y0 - y;

                    double dist = Math.Sqrt(dx * dx + dy * dy) / distScale;
                    if (dist > 1)
                    {
                        dist = 1;
                    }

                    int colorIntensity = (int)Math.Round(255 * (1 - dist));
                    if (colorIntensity > 255)
                    {
                        colorIntensity = 255;
                    }

                    colors[x, y] = isUpsideDown
                        ? Color.FromArgb(255, (byte)colorIntensity, 0, 0)
                        : Color.FromArgb(255, 0, (byte)colorIntensity, 0);
                }
            }

            return colors;
        }
    }



    public class Joystick
    {
        public void Run(ISenseHat SenseHat)
        {
            // The initial position of the pixel.
            int x = 3;
            int y = 3;

            SenseHat.Display.Clear();

            while (true)
            {
                if (SenseHat.Joystick.Update()) // Has any of the buttons on the joystick changed?
                {
                    UpdatePosition(SenseHat, ref x, ref y); // Move the pixel.

                    SenseHat.Display.Clear(); // Clear the screen.

                    SenseHat.Display.Screen[x, y] = Colors.Yellow; // Draw the pixel.

                    SenseHat.Display.Update(); // Update the physical display.
                }

                // Take a short nap.

                //Sleep(TimeSpan.FromMilliseconds(2));
            }
        }

        private void UpdatePosition(ISenseHat SenseHat, ref int x, ref int y)
        {
            if (SenseHat.Joystick.LeftKey == KeyState.Pressed)
            {
                if (x > 0)
                {
                    x--;
                }
            }
            else if (SenseHat.Joystick.RightKey == KeyState.Pressed)
            {
                if (x < 7)
                {
                    x++;
                }
            }

            if (SenseHat.Joystick.UpKey == KeyState.Pressed)
            {
                if (y > 0)
                {
                    y--;
                }
            }
            else if (SenseHat.Joystick.DownKey == KeyState.Pressed)
            {
                if (y < 7)
                {
                    y++;
                }
            }
        }
    }
}
