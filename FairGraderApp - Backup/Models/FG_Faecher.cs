namespace FairGraderApp.Models
{
    public class FG_Faecher
    {
        public List<string> Fach { get; } = new List<string>();

        /// <summary>
        /// Fügt ein neues Fach zur Liste hinzu, wenn es noch nicht vorhanden ist.
        /// </summary>
        /// <param name="fachName">Der Name des hinzuzufügenden Fachs</param>
        /// <returns>True, wenn das Fach erfolgreich hinzugefügt wurde; false, wenn es bereits vorhanden ist</returns>
        public bool AddFach(string fachName)
        {
            // Validiere, ob das Fach bereits in der Liste vorhanden ist
            if (Fach.Contains(fachName))
            {
                return false;
            }

            // Fach zur Liste hinzufügen
            Fach.Add(fachName);
            return true;
        }
    }
}