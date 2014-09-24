using BluetoothConnectionManager;
using DollyProtocol;
using SmartDolly.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.Media.SpeechSynthesis;
using Windows.Networking.Proximity;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace SmartDolly
{
    public sealed partial class PivotPage : Page
    {
        private const string DollySettingsName = "DollySettings";
        private const string MotorSettingsName = "MotorSettings";
        private const string CameraSettingsName = "CameraSettings";

        private const string BtDeviceName = "myDolly";

        private ConnectionManager connectionManager;

        private readonly NavigationHelper navigationHelper;
        private ObservableDictionary appViewModel = new ObservableDictionary();
        private Dolly dollyViewModel;
        private Camera cameraViewModel;
        private Motor motorViewModel;

        private readonly ResourceLoader resourceLoader = ResourceLoader.GetForCurrentView("Resources");

        public PivotPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            connectionManager = new ConnectionManager();
            dollyViewModel = new Dolly();
            cameraViewModel = new Camera();
            motorViewModel = new Motor();

            connectionManager.MessageReceived += connectionManager_MessageReceived;
            dollyViewModel.SetPropertyEventHandler += viewModel_SetProperty;
            dollyViewModel.GetPropertyEventHandler += viewModel_GetProperty;
            dollyViewModel.PingReceivedEventHandler += dollyViewModel_PingReceivedEventHandler;
            cameraViewModel.SetPropertyEventHandler += viewModel_SetProperty;
            cameraViewModel.GetPropertyEventHandler += viewModel_GetProperty;
            motorViewModel.SetPropertyEventHandler += viewModel_SetProperty;
            motorViewModel.GetPropertyEventHandler += viewModel_GetProperty;
            ConnectingProgressRing.IsActive = true;
            Connect();
        }

        async void dollyViewModel_PingReceivedEventHandler(object sender, EventArgs e)
        {
            ConnectingProgressRing.IsActive = false;

            string pingResponseMsg =
                TargetObject.Dolly.ToString() +
                Dolly.Property.Ping.ToString() +
                Command.GetResponse.ToString() +
                Protocol.ValueSeparator.ToString();
            await connectionManager.SendCommand(pingResponseMsg);

            dollyViewModel.getInitValues();
            cameraViewModel.getInitValues();
            motorViewModel.getInitValues();
        }

        async void viewModel_GetProperty(object sender, GetPropertyEventArgs e)
        {
            string requestMsg = e.TargetObject.ToString() + e.Property.ToString() + e.Command.ToString() + Protocol.ValueSeparator;
            Debug.WriteLine("GET - " + requestMsg);
            await connectionManager.SendCommand(requestMsg);
        }

        async void viewModel_SetProperty(object sender, SetPropertyEventArgs a)
        {
            string responseMsg = a.TargetObject.ToString() + a.Property.ToString() + a.Command.ToString() + Protocol.ValueSeparator + a.Value;
            await connectionManager.SendCommand(responseMsg);
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary AppViewModel
        {
            get { return this.appViewModel; }
        }

        public Dolly DollyViewModel
        {
            get { return this.dollyViewModel; }
        }
        public Camera CameraViewModel
        {
            get { return this.cameraViewModel; }
        }
        public Motor MotorViewModel
        {
            get { return this.motorViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>.
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            this.AppViewModel[DollySettingsName] = DollyViewModel;
            this.AppViewModel[CameraSettingsName] = CameraViewModel;
            this.AppViewModel[MotorSettingsName] = MotorViewModel;
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache. Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/>.</param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            // TODO: Save the unique state of the page here.
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            Debug.WriteLine("PivotPage OnNavigatedTo " + e);
            connectionManager.Initialize();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            //connectionManager.Terminate();
            Debug.WriteLine("PivotPage OnNavigatedFrom " + e);
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private async void Connect()
        {
            try
            {
                bool connectionStatus = false;
                int retries = 0;

                while (connectionStatus == false && retries < 3)
                {
                    Debug.WriteLine("Try BT connection: " + retries++);
                    PeerFinder.AlternateIdentities["Bluetooth:Paired"] = "";
                    var pairedDevices = await PeerFinder.FindAllPeersAsync();

                    if (pairedDevices.Count == 0)
                    {
                        Debug.WriteLine("No paired devices were found.");
                    }
                    else
                    {
                        foreach (var pairedDevice in pairedDevices)
                        {
                            if (pairedDevice.DisplayName == BtDeviceName)
                            {
                                Debug.WriteLine("BT Device Name found: " + BtDeviceName);
                                connectionStatus = await connectionManager.Connect(pairedDevice.HostName);                    
                                if(connectionStatus == true)
                                {
                                    Debug.WriteLine("BT Connection OK");
                                    App.Speak("Dolly connected.");
                                }
                                else
                                {
                                    Debug.WriteLine("BT Connection FAILED!!!");
                                }   

//                            DeviceName.IsReadOnly = true;
//                            DeviceName.Visibility = Visibility.Collapsed;
//                            BTNameText.Visibility = Visibility.Collapsed;
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Cannot connect to BL device!");
                Debug.WriteLine(ex.Message);
            }
        }

        async void connectionManager_MessageReceived(string message)
        {
            Debug.WriteLine("Message received:" + message);

            switch (message[0]) // check the target object
            {
                case TargetObject.Dolly:
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        DollyViewModel.handleMessage(message.Substring(1));
                    });
                    break;
                case TargetObject.Camera:
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        CameraViewModel.handleMessage(message.Substring(1));
                    });
                    break;
                case TargetObject.Motor:
                    await this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        MotorViewModel.handleMessage(message.Substring(1));
                    });
                    break;
                default:
                    Debug.WriteLine("Unknown target object: " + message[0] + " in message: " + message);
                    break;
            }
        }

        private void DoMeasuring_Click(object sender, RoutedEventArgs e)
        {
            DollyViewModel.Measuring = true;
        }
        private void DoHoming_Click(object sender, RoutedEventArgs e)
        {
            DollyViewModel.Homing = true;
        }
    }

    public class MainSwitchConverter : IValueConverter
    {
        public object Start { get; set; }
        public object Stop { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? Start : Stop;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var s = (string)value;
            return (s.Equals(Stop)) ? false : true;
        }
    }

    public class LimitSwitchConverter : IValueConverter
    {
        public object On { get; set; }
        public object Off { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? On : Off;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var s = (string)value;
            return (s.Equals(Off)) ? false : true;
        }
    }

    public class MeasureStateConverter : IValueConverter
    {
        public object Start { get; set; }
        public object Stop { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? Start : Stop;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var s = (string)value;
            return (s.Equals(Stop)) ? false : true;
        }
    }


    public class ExposureModeValueConverter : IValueConverter
    {
        public object CameraMode { get; set; }
        public object BulbMode { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? CameraMode : BulbMode;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var s = (string)value;
            return (s.Equals(BulbMode)) ? false : true;
        }
    }

    public class DirectionValueConverter : IValueConverter
    {
        public object Right { get; set; }
        public object Left { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? Right : Left;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var s = (string)value;
            return (s.Equals(Left)) ? false : true;
        }
    }

    public sealed class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return null;

            if (parameter == null)
                return value;

            return string.Format((string)parameter, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            string language)
        {
            throw new NotImplementedException();
        }
    }
}
