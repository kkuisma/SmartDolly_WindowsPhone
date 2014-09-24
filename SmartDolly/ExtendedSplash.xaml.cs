using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace SmartDolly
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplash : Page
    {
        public ExtendedSplash()
        {
            this.InitializeComponent();
        }

        public ExtendedSplash(SplashScreen splash)
        {
            InitializeComponent();

            this.InitializeComponent();
            this.extendedSplashImage.SetValue(Canvas.LeftProperty, splash.ImageLocation.X);
            this.extendedSplashImage.SetValue(Canvas.TopProperty, splash.ImageLocation.Y);
            this.extendedSplashImage.Height = splash.ImageLocation.Height;
            this.extendedSplashImage.Width = splash.ImageLocation.Width;
            // Position the extended splash screen’s progress ring.
            this.ProgressRing.SetValue(Canvas.TopProperty, splash.ImageLocation.Y + splash.ImageLocation.Height + 32);
            this.ProgressRing.SetValue(Canvas.LeftProperty, splash.ImageLocation.X + (splash.ImageLocation.Width / 2) - 15);
        }

        internal void onSplashScreenDismissed(Windows.ApplicationModel.Activation.SplashScreen sender, object e)
        {
            // The splash screen has been dismissed and the extended splash screen is now in view.
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }
    }
}
