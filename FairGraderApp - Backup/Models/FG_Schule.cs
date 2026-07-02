using System.Security.Cryptography;

namespace FairGraderApp.Models
{
    public class FG_Schule
    {
        public int Version { get; set; } = 1;
        public string Name { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string Id { get; set; } = string.Empty;
        public List<FG_Klasse> Klassen { get; set; } = new List<FG_Klasse>();
        public FG_Stundenplan Stundenplan { get; set; } = new FG_Stundenplan();

        public FG_Schule(string name, string adresse, string id)
        {
            Random rnd = new Random();  
            Name = name;
            Adresse = adresse;
            Id = id;
            Klassen = new List<FG_Klasse>();
            Stundenplan = new FG_Stundenplan();
        }
    }
}