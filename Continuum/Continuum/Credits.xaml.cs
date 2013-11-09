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
using Microsoft.Phone.Tasks;

namespace Continuum
{
    public partial class Page1 : PhoneApplicationPage
    {
        public Page1()
        {
            InitializeComponent();
        }

        private void HyperlinkButton_Click_1(object sender, RoutedEventArgs e)
        {
            WebBrowserTask t = new WebBrowserTask();
            t.Uri = new Uri("http://www.xteamdimension.com");
            t.Show();
        }
    }
}