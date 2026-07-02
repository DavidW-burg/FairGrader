using System.Security.Cryptography;

namespace FairGraderApp.Models
{
    public enum FG_Geschlecht : int {
        not_def = 0,
        male = 1,
        female = 2
    }

    public class FG_Bewertung {
        public DateTime DateTime { get; }
        public double Note { get; }
        public bool Muendlich { get; }
        public string Kommentar { get; }
        public string Id { get; }


        /// <summary>
        /// Erstellt eine neue Bewertung mit den angegebenen Parametern.
        /// </summary>
        /// <param name="note">Die Bewertungsnote</param>
        /// <param name="muendlich">Gibt an, ob es sich um eine mündliche Bewertung handelt</param>
        /// <param name="kommentar">Kommentar zur Bewertung</param>
        /// <param name="dateTime">Das Datum und die Uhrzeit der Bewertung</param>
        public FG_Bewertung(double note, bool muendlich, string kommentar, DateTime dateTime, string id) {
            Note = note;
            Muendlich = muendlich;
            Kommentar = kommentar;
            DateTime = dateTime == default ? DateTime.Now : dateTime;
            Id = id;
        }
    }
}