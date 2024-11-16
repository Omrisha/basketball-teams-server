namespace NbaApi.Models;

public class Player
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int JerseyNumber { get; set; }
    public bool Active { get; set; }
    public string Position { get; set; }
}