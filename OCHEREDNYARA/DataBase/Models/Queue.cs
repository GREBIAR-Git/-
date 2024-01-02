using System.ComponentModel.DataAnnotations;

namespace DataBase.Models;

public class Queue
{
    [Key]
    public long Id { get; set; }

    public long RoomId { get; set; }

    public bool Freezing { get; set; }

    public Room? Room { get; set; }

    public List<Window>? Windows { get; set; }
}
