using Microsoft.JSInterop;
using System.Text.Json;
using System.IO;
using System.Text;

namespace FairGraderApp.Models
{
    public class FG_FairGrader
    {
        private const string STORAGE_KEY = "FairGrader_Schulen";
        private bool _loadedFromStorage;
        private IJSRuntime? _jsRuntime; // <--- Feld hinzugefügt

        public List<FG_Schule> Schulen { get; set; } = new List<FG_Schule>();

        public FG_FairGrader()
        {
            Schulen = new List<FG_Schule>();
        }

        /// <summary>
        /// Einmalig Schulen aus dem Browser-Storage laden (nach dem ersten Render).
        /// </summary>
        public async Task EnsureLoadedFromBrowserStorageAsync(IJSRuntime jsRuntime)
        {
            if (_loadedFromStorage) return;

            _jsRuntime = jsRuntime; // <--- jsRuntime speichern

            try
            {
                var jsonString = await jsRuntime.InvokeAsync<string>("localStorage.getItem", STORAGE_KEY);
                if (!string.IsNullOrEmpty(jsonString))
                {
                    var schulenData = JsonSerializer.Deserialize<List<FG_Schule>>(jsonString);
                    if (schulenData != null)
                    {
                        Schulen.Clear();
                        Schulen.AddRange(schulenData);
                        Schulen.Sort((s1, s2) => string.Compare(s1.Name, s2.Name, StringComparison.OrdinalIgnoreCase));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Laden der Schuldaten: {ex.Message}");
            }
            finally
            {
                _loadedFromStorage = true;
            }
        }

        /// <summary>
        /// Speichert die aktuellen Schuldaten im Browser-Speicher.
        /// </summary>
        public async Task SaveSchulenToStorageAsync()
        {
            if (_jsRuntime == null) return;

            try
            {
                var jsonString = JsonSerializer.Serialize(Schulen);
                await _jsRuntime.InvokeVoidAsync("localStorage.setItem", STORAGE_KEY, jsonString);
            }
            catch (Exception ex)
            {
                // Fehlerbehandlung
                Console.WriteLine($"Fehler beim Speichern der Schuldaten: {ex.Message}");
            }
        }

        /// <summary>
        /// Fügt eine neue Schule zur Schulliste hinzu, wenn sie nicht bereits vorhanden ist.
        /// </summary>
        /// <param name="schule">Die hinzuzufügende Schule</param>
        /// <returns>True, wenn die Schule erfolgreich hinzugefügt wurde; andernfalls false</returns>
        public async Task<bool> AddSchuleAsync(FG_Schule schule)
        {
            // Prüfe, ob die Schule bereits in der Liste vorhanden ist
            if (Schulen.Contains(schule))
            {
                return false;
            }

            // Schule zur Liste hinzufügen
            Schulen.Add(schule);

            // Liste nach Name sortieren
            Schulen.Sort((s1, s2) => string.Compare(s1.Name, s2.Name, StringComparison.OrdinalIgnoreCase));

            // Daten speichern
            await SaveSchulenToStorageAsync();

            return true;
        }
        /// <summary>
        /// Fügt eine neue Schule zur Schulliste hinzu, wenn sie nicht bereits vorhanden ist.
        /// </summary>
        /// <param name="schule">Die hinzuzufügende Schule</param>
        /// <returns>True, wenn die Schule erfolgreich hinzugefügt wurde; andernfalls false</returns>
        public async Task<bool> AddSchuelerAsync(string schuleId, string klasseId, FG_Schueler schueler) {
            // Schule anhand der ID suchen
            var schule = Schulen.FirstOrDefault(s => string.Equals(s.Id, schuleId, StringComparison.OrdinalIgnoreCase));
            if (schule == null) {
                return false;
            }
            // Klasse in der Schule anhand der ID suchen
            var klasse = schule.Klassen.FirstOrDefault(k => string.Equals(k.Id, klasseId, StringComparison.OrdinalIgnoreCase));
            if (klasse == null) {
                return false;
            }
            // Prüfe, ob der Schüler bereits in der Klasse vorhanden ist
            if (klasse.Schueler.Contains(schueler)) {
                return false;
            }
            // Schüler zur Klasse hinzufügen
            klasse.Schueler.Add(schueler);
            // Schülerliste der Klasse nach Name und Vorname sortieren
            klasse.Schueler.Sort((s1, s2) => {
                int nameComparison = string.Compare(s1.Name, s2.Name, StringComparison.OrdinalIgnoreCase);
                if (nameComparison != 0)
                    return nameComparison;
                return string.Compare(s1.Vorname, s2.Vorname, StringComparison.OrdinalIgnoreCase);
            });
            // SortList aktualisieren
            klasse.SortList.Clear();
            foreach (var sch in klasse.Schueler) {
                klasse.SortList.Add(sch.Id);
            }
            // Daten im LocalStorage speichern
            await SaveSchulenToStorageAsync();
            return true;
        }

        /// <summary>
        /// Aktualisiert die Daten einer bestehenden Schule in der Schulliste.
        /// </summary>
        public async Task<bool> UpdateSchuleAsync(FG_Schule alteSchule, string neuerName, string neueAdresse)
        {
            if (!Schulen.Contains(alteSchule))
            {
                return false;
            }

            alteSchule.Name = neuerName;
            alteSchule.Adresse = neueAdresse;

            Schulen.Sort((s1, s2) => string.Compare(s1.Name, s2.Name, StringComparison.OrdinalIgnoreCase));

            await SaveSchulenToStorageAsync();

            return true;
        }

        /// <summary>
        /// Entfernt eine Schule aus der Schulliste.
        /// </summary>
        public async Task<bool> DeleteSchuleAsync(FG_Schule schule)
        {
            bool removed = Schulen.Remove(schule);
            
            if (removed)
            {
                await SaveSchulenToStorageAsync();
            }
            
            return removed;
        }

        /// <summary>
        /// Sucht eine Schule anhand des Namens.
        /// </summary>
        public FG_Schule? GetSchule(string name)
        {
            return Schulen.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gibt alle Schulen sortiert nach Namen zurück.
        /// </summary>
        public List<FG_Schule> GetSchulenSortiert()
        {
            return Schulen.OrderBy(s => s.Name, StringComparer.OrdinalIgnoreCase).ToList();
        }

        public static string NewId() {
            return DateTime.Now.ToString("yyyyMMdd-HHmmss.fff") + "_" + new Random().Next(1000).ToString("D3");
        }

        // Synchrone Versionen für Rückwärtskompatibilität (rufen asynchrone Versionen auf)
        public bool AddSchule(FG_Schule schule) => AddSchuleAsync(schule).GetAwaiter().GetResult();
        public bool UpdateSchule(FG_Schule alteSchule, string neuerName, string neueAdresse) => UpdateSchuleAsync(alteSchule, neuerName, neueAdresse).GetAwaiter().GetResult();
        public bool DeleteSchule(FG_Schule schule) => DeleteSchuleAsync(schule).GetAwaiter().GetResult();
        public bool DeleteKlasse(string schulId, string klasseId) => DeleteKlasseAsync(schulId, klasseId).GetAwaiter().GetResult();
        public bool UpdateKlasse(string schulId, string klasseId, string neuerName, string neuerZug, int neueStufe, string neuesFach) => UpdateKlasseAsync(schulId, klasseId, neuerName, neuerZug, neueStufe, neuesFach).GetAwaiter().GetResult();
        public bool DeleteSchueler(string schulId, string klasseId, string schuelerId) => DeleteSchuelerAsync(schulId, klasseId, schuelerId).GetAwaiter().GetResult();
        public bool UpdateSchueler(string schulId, string klasseId, string schuelerId, string neuerName, string neuerVorname, FG_Geschlecht neuesGeschlecht) => UpdateSchuelerAsync(schulId, klasseId, schuelerId, neuerName, neuerVorname, neuesGeschlecht).GetAwaiter().GetResult();


        /// <summary>
        /// Entfernt eine Klasse aus einer Schule anhand der IDs.
        /// </summary>
        /// <param name="schulId">Die ID der Schule</param>
        /// <param name="klasseId">Die ID der Klasse</param>
        /// <returns>True, wenn die Klasse erfolgreich entfernt wurde; andernfalls false</returns>
        public async Task<bool> DeleteKlasseAsync(string schulId, string klasseId)
        {
            // Schule anhand der ID suchen
            var schule = Schulen.FirstOrDefault(s => string.Equals(s.Id, schulId, StringComparison.OrdinalIgnoreCase));
            if (schule == null)
            {
                return false;
            }

            // Klasse in der Schule anhand der ID suchen
            var klasse = schule.Klassen.FirstOrDefault(k => string.Equals(k.Id, klasseId, StringComparison.OrdinalIgnoreCase));
            if (klasse == null)
            {
                return false;
            }

            // Klasse aus der Schule entfernen
            bool removed = schule.Klassen.Remove(klasse);
            
            if (removed)
            {
                // Daten im LocalStorage speichern
                await SaveSchulenToStorageAsync();
            }
            
            return removed;
        }

        /// <summary>
        /// Aktualisiert die Daten einer bestehenden Klasse in einer Schule anhand der IDs.
        /// </summary>
        /// <param name="schulId">Die ID der Schule</param>
        /// <param name="klasseId">Die ID der Klasse</param>
        /// <param name="neuerName">Der neue Name der Klasse</param>
        /// <param name="neuerZug">Der neue Zug der Klasse</param>
        /// <param name="neueStufe">Die neue Stufe der Klasse</param>
        /// <param name="neuesFach">Das neue Fach der Klasse</param>
        /// <returns>True, wenn die Klasse erfolgreich aktualisiert wurde; andernfalls false</returns>
        public async Task<bool> UpdateKlasseAsync(string schulId, string klasseId, string neuerName, string neuerZug, int neueStufe, string neuesFach)
        {
            // Schule anhand der ID suchen
            var schule = Schulen.FirstOrDefault(s => string.Equals(s.Id, schulId, StringComparison.OrdinalIgnoreCase));
            if (schule == null)
            {
                return false;
            }

            // Klasse in der Schule anhand der ID suchen
            var klasse = schule.Klassen.FirstOrDefault(k => string.Equals(k.Id, klasseId, StringComparison.OrdinalIgnoreCase));
            if (klasse == null)
            {
                return false;
            }

            // Klasse aktualisieren mit der UpdateKlasse-Methode
            bool updated = klasse.UpdateKlasse(neuerName, neuerZug, neueStufe, neuesFach);
            
            if (updated)
            {
                // Klassenliste der Schule nach Name sortieren
                schule.Klassen.Sort((k1, k2) => string.Compare(k1.Name, k2.Name, StringComparison.OrdinalIgnoreCase));
                
                // Daten im LocalStorage speichern
                await SaveSchulenToStorageAsync();
            }
            
            return updated;
        }

        /// <summary>
        /// Entfernt einen Schüler aus einer Klasse anhand der IDs.
        /// </summary>
        /// <param name="schulId">Die ID der Schule</param>
        /// <param name="klasseId">Die ID der Klasse</param>
        /// <param name="schuelerId">Die ID des Schülers</param>
        /// <returns>True, wenn der Schüler erfolgreich entfernt wurde; andernfalls false</returns>
        public async Task<bool> DeleteSchuelerAsync(string schulId, string klasseId, string schuelerId)
        {
            // Schule anhand der ID suchen
            var schule = Schulen.FirstOrDefault(s => string.Equals(s.Id, schulId, StringComparison.OrdinalIgnoreCase));
            if (schule == null)
            {
                return false;
            }

            // Klasse in der Schule anhand der ID suchen
            var klasse = schule.Klassen.FirstOrDefault(k => string.Equals(k.Id, klasseId, StringComparison.OrdinalIgnoreCase));
            if (klasse == null)
            {
                return false;
            }

            // Schüler in der Klasse anhand der ID suchen
            var schueler = klasse.Schueler.FirstOrDefault(s => string.Equals(s.Id, schuelerId, StringComparison.OrdinalIgnoreCase));
            if (schueler == null)
            {
                return false;
            }

            // Schüler aus der Klasse entfernen
            bool removed = klasse.DeleteSchueler(schueler);
            
            if (removed)
            {
                // Schüler-ID aus der SortList entfernen
                klasse.SortList.Remove(schuelerId);
                
                // Daten im LocalStorage speichern
                await SaveSchulenToStorageAsync();
            }
            
            return removed;
        }

        /// <summary>
        /// Aktualisiert die Daten eines bestehenden Schülers in einer Klasse anhand der IDs.
        /// </summary>
        /// <param name="schulId">Die ID der Schule</param>
        /// <param name="klasseId">Die ID der Klasse</param>
        /// <param name="schuelerId">Die ID des Schülers</param>
        /// <param name="neuerName">Der neue Nachname des Schülers</param>
        /// <param name="neuerVorname">Der neue Vorname des Schülers</param>
        /// <param name="neuesGeschlecht">Das neue Geschlecht des Schülers</param>
        /// <returns>True, wenn der Schüler erfolgreich aktualisiert wurde; andernfalls false</returns>
        public async Task<bool> UpdateSchuelerAsync(string schulId, string klasseId, string schuelerId, string neuerName, string neuerVorname, FG_Geschlecht neuesGeschlecht)
        {
            // Schule anhand der ID suchen
            var schule = Schulen.FirstOrDefault(s => string.Equals(s.Id, schulId, StringComparison.OrdinalIgnoreCase));
            if (schule == null)
            {
                return false;
            }

            // Klasse in der Schule anhand der ID suchen
            var klasse = schule.Klassen.FirstOrDefault(k => string.Equals(k.Id, klasseId, StringComparison.OrdinalIgnoreCase));
            if (klasse == null)
            {
                return false;
            }

            // Schüler in der Klasse anhand der ID suchen
            var schueler = klasse.Schueler.FirstOrDefault(s => string.Equals(s.Id, schuelerId, StringComparison.OrdinalIgnoreCase));
            if (schueler == null)
            {
                return false;
            }

            // Schüler aktualisieren mit der UpdateSchueler-Methode
            bool updated = klasse.UpdateSchueler(schueler, neuerName, neuerVorname, neuesGeschlecht);
            
            if (updated)
            {
                // Schülerliste der Klasse nach Name und Vorname sortieren
                klasse.Schueler.Sort((s1, s2) => {
                    int nameComparison = string.Compare(s1.Name, s2.Name, StringComparison.OrdinalIgnoreCase);
                    if (nameComparison != 0)
                        return nameComparison;

                    return string.Compare(s1.Vorname, s2.Vorname, StringComparison.OrdinalIgnoreCase);
                });

                // SortList aktualisieren
                klasse.SortList.Clear();
                foreach (var sch in klasse.Schueler)
                {
                    klasse.SortList.Add(sch.Id);
                }
                
                // Daten im LocalStorage speichern
                await SaveSchulenToStorageAsync();
            }
            
            return updated;
        }
    }
}