namespace FairGraderApp.Models
{
    public class FG_Schueler
    {
        public string Name { get; set; } = string.Empty;
        public string Vorname { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public FG_Geschlecht Geschlecht { get; set; } = FG_Geschlecht.not_def;
        public Uri? Foto { get; set; }
        public List<FG_Bewertung> Bewertungen { get; set; } = new List<FG_Bewertung>();

        public FG_Schueler() { }

        public FG_Schueler(string name, string vorname, FG_Geschlecht geschlecht, string id, Uri? foto = null)
        {
            Name = name;
            Vorname = vorname;
            Id = id;
            Geschlecht = geschlecht;
            Foto = foto;
            Bewertungen = new List<FG_Bewertung>();
        }

        public void ChangeFixValues(string name, string vorname, FG_Geschlecht geschlecht)
        {
            Name = name;
            Vorname = vorname;
            Geschlecht = geschlecht;
        }

        public bool AddBewertung(double note, bool muendlich = true, string kommentar = "", DateTime dateTime = default)
        {
            if (dateTime == default) dateTime = DateTime.Now;
            if (note < 1.0 || note > 6.0) return false;
            if (dateTime > DateTime.Now) return false;

            var currentYear = DateTime.Now.Year;
            var lastAugust1st = new DateTime(DateTime.Now < new DateTime(currentYear, 8, 1) ? currentYear - 1 : currentYear, 8, 1);
            if (dateTime < lastAugust1st) return false;

            Bewertungen.Add(new FG_Bewertung(note, muendlich, kommentar, dateTime, FG_FairGrader.NewId()));
            return true;
        }

        public bool UpdateBewertung(FG_Bewertung alteBewertung, double note, bool muendlich, DateTime dateTime = default)
        {
            var index = Bewertungen.IndexOf(alteBewertung);
            if (index == -1) return false;

            if (dateTime == default) dateTime = DateTime.Now;
            if (note < 1.0 || note > 6.0) return false;
            if (dateTime > DateTime.Now) return false;

            var currentYear = DateTime.Now.Year;
            var lastAugust1st = new DateTime(DateTime.Now < new DateTime(currentYear, 8, 1) ? currentYear - 1 : currentYear, 8, 1);
            if (dateTime < lastAugust1st) return false;

            Bewertungen[index] = new FG_Bewertung(note, muendlich, alteBewertung.Kommentar, dateTime, alteBewertung.Id);
            return true;
        }

        public bool DeleteBewertung(FG_Bewertung bewertung) => Bewertungen.Remove(bewertung);

        public (List<FG_Bewertung> MuendlicheBewertungen, double Durchschnitt) GetMuendlicheBewertungen()
        {
            var list = Bewertungen.Where(b => b.Muendlich).ToList();
            var durchschnitt = list.Count > 0 ? list.Average(b => b.Note) : 0.0;
            return (list, durchschnitt);
        }

        public (List<FG_Bewertung> SchriftlicheBewertungen, double Durchschnitt) GetSchriftlicheBewertungen()
        {
            var list = Bewertungen.Where(b => !b.Muendlich).ToList();
            var durchschnitt = list.Count > 0 ? list.Average(b => b.Note) : 0.0;
            return (list, durchschnitt);
        }
    }
}