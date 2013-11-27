using Continuum.State;
using Continuum.Utilities;
using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;

namespace Continuum.Scores
{
    public class Score : IComparable
    {
        /// <summary>
        /// Nome del giocatore che ha conseguito il punteggio.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Le ore passate a giocare.
        /// </summary>
        public int totalHours { get; set; }

        /// <summary>
        /// I minuti passati a giocare.
        /// </summary>
        public int totalMinutes { get; set; }

        /// <summary>
        /// I secondi passati a giocare.
        /// </summary>
        public int totalSeconds { get; set; }

        /// <summary>
        /// I millisecondi passati a giocare.
        /// </summary>
        public int totalMilliseconds { get; set; }

        /// <summary>
        /// Giorno in cui è stato conseguito il punteggio.
        /// </summary>
        public int Day { get; set; }

        /// <summary>
        /// Mese in cui è stato conseguito il punteggio.
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// Anno in cui è stato conseguito il punteggio.
        /// </summary>
        public int Year { get; set; }
        
        /// <summary>
        /// Costruttore vuoto per serializzazione.
        /// </summary>
        public Score() { }

        /// <summary>
        /// Crea un oggetto Score.
        /// </summary>
        /// <param name="ts">TimeSpan indicante il tempo resistito durante il gioco.</param>
        public Score(TimeSpan ts)
        {
            totalHours = (int)ts.TotalHours;
            totalMinutes = ts.Minutes;
            totalSeconds = ts.Seconds;
            totalMilliseconds = ts.Milliseconds;

            Day = DateTime.Now.Day;
            Month = DateTime.Now.Month;
            Year = DateTime.Now.Year;
            this.Name = "PROSTATA";
        }

        /// <summary>
        /// Crea un oggetto Score.
        /// </summary>
        /// <param name="ts">TimeSpan indicante il tempo resistito durante il gioco.</param>
        public Score(TimeSpan ts, string Name)
        {
            totalHours = (int)ts.TotalHours;
            totalMinutes = ts.Minutes;
            totalSeconds = ts.Seconds;
            totalMilliseconds = ts.Milliseconds;

            Day = DateTime.Now.Day;
            Month = DateTime.Now.Month;
            Year = DateTime.Now.Year;
            this.Name = Name;
        }

        /// <summary>
        /// E' un ToSring, cosa vuoi che ti spieghi.
        /// </summary>
        /// <returns>La rappresentazione dell'oggetto in Stringa.</returns>
        public override string ToString()
        {
            return Name + " " + totalHours.ToString() + ":" + totalMinutes.ToString() + ":" + totalSeconds.ToString() + " " + Day.ToString() + "/" + Month.ToString() + "/" + Year.ToString(); ;
        }
    

        /// <summary>
        /// Confronta oggetti Score per poter ordinare la lista dei punteggi.
        /// </summary>
        /// <param name="obj">Deve essere un altro oggetto Score o sarà tutto inutile.</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            try
            {
                Score myS = (Score)obj;
                return new TimeSpan(0, myS.totalHours, myS.totalMinutes, myS.totalSeconds, myS.totalMilliseconds).CompareTo(new TimeSpan(0, totalHours, totalMinutes, totalSeconds, totalMilliseconds));
            }
            catch(InvalidCastException e)
            {
                e.ToString();
                return -1;
            }
        }

        public static Score[] ReadScores()
        {
            Score[] scores = null;
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (storage.FileExists("scores.xml"))
                {
                    //Recupero gli score che già ci sono                   
                    IsolatedStorageFileStream stream = storage.OpenFile("scores.xml", FileMode.Open, FileAccess.Read);
                    XmlSerializer serializer = new XmlSerializer(typeof(Score[]));
                    scores = (Score[])serializer.Deserialize(stream);
                    stream.Close();
                }
            }

            return scores;
        }

        public static Score[] WriteScores(float time, string name)
        {
            Score[] scores;
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                //QUI IL FILE DEVE ESISTERE CAZZO NEGRO
                if (storage.FileExists("scores.xml"))
                {
                    //Recupero gli score che già ci sono                   
                    IsolatedStorageFileStream stream = storage.OpenFile("scores.xml", FileMode.Open, FileAccess.Read);
                    XmlSerializer serializer = new XmlSerializer(typeof(Score[]));
                    scores = (Score[])serializer.Deserialize(stream);
                    stream.Close();

                    //Elimino il file per evitare problemi di scrittura
                    storage.DeleteFile("scores.xml");

                    //Scrivo
                    stream = storage.OpenFile("scores.xml", FileMode.OpenOrCreate, FileAccess.Write);

                    Score[] tmp = new Score[scores.Length + 1];

                    System.Array.Copy(scores, tmp, scores.Length);

                    tmp[scores.Length] = new Score(TimeSpan.FromSeconds(time), name);

                    Array.Sort(tmp);

                    if (tmp.Length > Constants.MAX_SCORES)
                    {
                        Score[] tmp2 = new Score[tmp.Length - 1];
                        Array.Copy(tmp, tmp2, tmp2.Length);
                        tmp = tmp2;
                    }

                    serializer.Serialize(stream, tmp);
                    stream.Close();
                }
                else
                {
                    IsolatedStorageFileStream stream = storage.OpenFile("scores.xml", FileMode.Create);
                    XmlSerializer serializer = new XmlSerializer(typeof(Score[]));
                    scores = new Score[1];
                    scores[0] = new Score(TimeSpan.FromSeconds(time), name);
                    serializer.Serialize(stream, scores);
                    stream.Close();
                }
                return scores;
            }
        }
    }
}
