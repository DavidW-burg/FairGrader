namespace FairGraderApp.Models
{
    public class FG_Stundenplan
    {
        public string Klasse { get; set; } = string.Empty;
        public string Fach { get; set; } = string.Empty;
        public int Wochentag { get; set; }
        public DateTime Beginn { get; set; }
        public TimeSpan Dauer { get; set; }
    }
}