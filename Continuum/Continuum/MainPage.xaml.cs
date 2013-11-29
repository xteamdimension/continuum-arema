using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using Continuum.Management;

namespace Continuum
{
    public partial class MainPage : PhoneApplicationPage
    {
        bool isButtonAvailable = false;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            Apertura.Begin();
            Apertura.Completed += new EventHandler(Apertura_Completed);
            if (Float1.GetCurrentTime().Milliseconds > 0)
                Float1.Stop();
            if (Float2.GetCurrentTime().Milliseconds > 0)
                Float2.Stop();
            base.OnNavigatedTo(e);
        }

        void Apertura_Completed(object sender, EventArgs e)
        {
            Float1.Begin();
            Float2.Begin();
            Float1.RepeatBehavior = RepeatBehavior.Forever;
            Float2.RepeatBehavior = RepeatBehavior.Forever;
            isButtonAvailable = true;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        void Chiusura_Completed(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
        }

        private void gamePage_Click(object sender, RoutedEventArgs e)
        {
            if (isButtonAvailable)
            {
                isButtonAvailable = false;
                Float1.Pause();
                Float2.Pause();
                Chiusura.Begin();
                Chiusura.Completed += new EventHandler(Chiusura_Completed);
            }
            else
            {
                Apertura.Stop();
                Chiusura.Stop();
                Float1.Stop();
                Float2.Stop();
                NavigationService.Navigate(new Uri("/GamePage.xaml", UriKind.Relative));
            }
        }

        private void tutorial_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/TutorialPage.xaml", UriKind.Relative));
        }

        private void scores_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/ScorePage.xaml", UriKind.Relative));
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Uri("/Credits.xaml", UriKind.Relative));
        }

        
    }
}