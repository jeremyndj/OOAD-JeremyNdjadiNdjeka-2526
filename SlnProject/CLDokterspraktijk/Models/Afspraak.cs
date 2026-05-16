namespace CLDokterspraktijk.Models;

public class Afspraak
{
    public int Id { get; set; }
    public DateTime Moment { get; set; }
    public string Klacht { get; set; }
    public int PatientId { get; set; }
    public int DokterId { get; set; }
}
