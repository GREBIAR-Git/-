using System.ComponentModel.DataAnnotations;

namespace DataBase.Models;

public class Room
{
    [Key]
    public long Id { get; set; }

    public string? Name { get; set; }

    public long OwnerId { get; set; }

    public Organization? Organization { get; set; }

    public List<Queue> Queues { get; set; } = new();

    public List<Favorite> Favorites { get; set; } = new();
}
