using System.ComponentModel.DataAnnotations;

namespace DataBase.Models;

public class Favorite
{
    [Key] public long ClientId { get; set; }

    [Key] public long RoomId { get; set; }

    public Client? Client { get; set; }
    public Room? Room { get; set; }
}