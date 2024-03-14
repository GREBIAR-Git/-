using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models;

public class Client
{
    [Key] [Column("ID")] public long Id { get; set; }

    [Column("LEVEL")] public int Level { get; set; } = 0;

    [Column("THEME")] public int Theme { get; set; }

    public List<Reserved> Reserveds { get; set; } = [];

    public List<Favorite> Favorites { get; set; } = [];
}