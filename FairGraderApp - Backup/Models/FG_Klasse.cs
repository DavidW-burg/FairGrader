using System.Security.Cryptography;

namespace FairGraderApp.Models
{
    public class FG_Klasse
    {
        private string _name = string.Empty;
        private int _stufe;
        private string _zug = string.Empty;
        private string _fach = string.Empty;
        private DateTime _einschulungsjahr;

        public string Name => _name;    
        public string Id { get; }
        public int Stufe => _stufe;
        public string Zug => _zug;
        public DateTime Einschulungsjahr => _einschulungsjahr;
        public string Fach => _fach;
        public List<FG_Schueler> Schueler { get; set; } = new();
        public List<string> SortList { get; set; } = new();

        public int Sitzplan { get; } = -1;

        /// <summary>
        /// Erstellt eine neue Klasse mit den angegebenen Grunddaten.
        /// </summary>
        /// <param name="name">Der Name der Klasse</param>
        /// <param name="zug">Der Zug der Klasse (z.B. "a", "b", "c")</param>
        /// <param name="fach">Das Hauptfach der Klasse</param>
        public FG_Klasse(string name, string zug, int Stufe, string fach, string id) {
            Random rnd = new Random();  
            _name = name;
            Id = id;
            _zug = zug;
            _fach = fach;
            if (!SetStufe(Stufe)) {
                _stufe = 0;
                _einschulungsjahr = default;
            }
            Sitzplan = 0;
            Schueler = new List<FG_Schueler>();
            SortList = new List<string>();
        }

        public bool UpdateKlasse(string name, string zug, int stufe, string fach)
        {
            if (SetStufe(stufe)) {
                _name = name;
                _zug = zug;
                _fach = fach;
                return true;
            }
            else {
                return false;
            }
        }

        /// <summary>
        /// Setzt die Stufe und berechnet das entsprechende Einschulungsjahr.
        /// </summary>
        /// <param name="stufe">Die Klassenstufe (muss zwischen 1 und 13 liegen)</param>
        /// <returns>True, wenn die Stufe erfolgreich gesetzt wurde; andernfalls false</returns>
        public bool SetStufe(int stufe)
        {
            // Validierung: Stufe muss zwischen 1 und 13 liegen
            if (stufe < 1 || stufe > 13)
            {
                return false;
            }

            // Aktuelles Datum und Jahr
            var heute = DateTime.Now;
            var aktuellesJahr = heute.Year;
            var august1st = new DateTime(aktuellesJahr, 8, 1);

            int einschulungsJahr;

            // Berechnung des Einschulungsjahres
            if (heute < august1st)
            {
                // Vor dem 1. August: aktuelles Jahr minus Stufenanzahl
                einschulungsJahr = aktuellesJahr - stufe;
            }
            else
            {
                // Ab dem 1. August: aktuelles Jahr minus (Stufenanzahl - 1)
                einschulungsJahr = aktuellesJahr - (stufe - 1);
            }

            // Werte setzen
            _stufe = stufe;
            _einschulungsjahr = new DateTime(einschulungsJahr, 8, 1);

            return true;
        }

        /// <summary>
        /// Fügt einen neuen Schüler zur Klassenliste hinzu, wenn er nicht bereits vorhanden ist.
        /// </summary>
        /// <param name="schueler">Der hinzuzufügende Schüler</param>
        /// <param name="autoSort">Gibt an, ob die Liste nach dem Hinzufügen automatisch sortiert werden soll</param>
        /// <returns>True, wenn der Schüler erfolgreich hinzugefügt wurde; andernfalls false</returns>
        public bool AddSchueler(FG_Schueler schueler, bool autoSort)
        {
            // Prüfe, ob der Schüler bereits in der Liste vorhanden ist
            if (Schueler.Contains(schueler))
            {
                return false;
            }
            // Schüler zur Liste hinzufügen
            Schueler.Add(schueler);

            // Liste nach Name und Vorname sortieren
            if (autoSort) {
                Schueler.Sort((s1, s2) => {
                    int nameComparison = string.Compare(s1.Name, s2.Name, StringComparison.OrdinalIgnoreCase);
                    if (nameComparison != 0)
                        return nameComparison;

                return string.Compare(s1.Vorname, s2.Vorname, StringComparison.OrdinalIgnoreCase);
            });
            }
            SortList.Clear();
            foreach (var sch in Schueler) { 
                SortList.Add(sch.Id);
            }

                return true;
        }

        /// <summary>
        /// Aktualisiert die Daten eines bestehenden Schülers in der Klassenliste.
        /// </summary>
        /// <param name="alterSchueler">Der zu aktualisierende Schüler</param>
        /// <param name="neuerName">Der neue Nachname</param>
        /// <param name="neuerVorname">Der neue Vorname</param>
        /// <param name="neuesGeschlecht">Das neue Geschlecht</param>
        /// <returns>True, wenn der Schüler erfolgreich aktualisiert wurde; andernfalls false</returns>
        public bool UpdateSchueler(FG_Schueler alterSchueler, string neuerName, string neuerVorname, FG_Geschlecht neuesGeschlecht)
        {
            if (!Schueler.Contains(alterSchueler))
            {
                return false;
            }

            alterSchueler.ChangeFixValues(neuerName, neuerVorname, neuesGeschlecht);
            return true;
        }

        /// <summary>
        /// Entfernt einen Schüler aus der Klassenliste.
        /// </summary>
        /// <param name="schueler">Der zu entfernende Schüler</param>
        /// <returns>True, wenn der Schüler erfolgreich entfernt wurde; andernfalls false</returns>
        public bool DeleteSchueler(FG_Schueler schueler)
        {
            return Schueler.Remove(schueler);
        }

        /// <summary>
        /// Sucht einen Schüler anhand von Name und Vorname.
        /// </summary>
        /// <param name="name">Der Nachname des gesuchten Schülers</param>
        /// <param name="vorname">Der Vorname des gesuchten Schülers</param>
        /// <returns>Der gefundene Schüler oder null, wenn kein Schüler gefunden wurde</returns>
        public FG_Schueler? GetSchueler(string name, string vorname)
        {
            return Schueler.FirstOrDefault(s => s.Name == name && s.Vorname == vorname);
        }

        /// <summary>
        /// Erstellt eine Kopie der Klasse mit einem neuen Fach und kopierten Schülern ohne deren Bewertungen.
        /// </summary>
        /// <param name="neuesFach">Das neue Hauptfach für die kopierte Klasse</param>
        /// <returns>Eine neue FG_Klasse-Instanz als Kopie mit dem neuen Fach</returns>
        public FG_Klasse CreateCopy(string neuesFach)
        {
            // Neue Klasse mit den gleichen Grunddaten aber neuem Fach erstellen
            var kopierteKlasse = new FG_Klasse(Name, Zug, Stufe, neuesFach, FG_FairGrader.NewId());
            
            // Schüler kopieren (ohne Bewertungen, da neue Schüler-Instanzen erstellt werden)
            foreach (var schueler in Schueler)
            {
                var kopierterSchueler = new FG_Schueler(
                    schueler.Name, 
                    schueler.Vorname, 
                    schueler.Geschlecht, 
                    schueler.Id,
                    schueler.Foto
                );                
                kopierteKlasse.AddSchueler(kopierterSchueler, false);
            }            
            return kopierteKlasse;
        }
    }
}