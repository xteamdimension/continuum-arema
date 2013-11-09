using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Continuum.Scores;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace Continuum
{
    public partial class ScorePage : PhoneApplicationPage
    {
        public ScorePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {

                if (storage.FileExists("scores.xml"))
                {
                    IsolatedStorageFileStream stream = storage.OpenFile("scores.xml", FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(typeof(Score[]));
                    //stackPanel1.DataContext = 
                    Score[] scores = (Score[])serializer.Deserialize(stream);
                    foreach(Score x in scores) {
                        TextBlock temp = new TextBlock();
                        temp.Text = x.ToString();
                        stackPanel1.Children.Add(temp);
                    }
                    stackPanel1.UpdateLayout();
                    stream.Close();
                }
                else
                {
                    MessageBox.Show("SUCA ROFL NO PUNTEGGI STERRO FROCETTO");
                }
            }
            base.OnNavigatedTo(e);
        }
    }
}