using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models;

public class Organization
{
    [Key] public long Id { get; set; }

    public string? Name { get; set; }

    [Column("THEME")] public int Theme { get; set; }

    [Column("THEMEDATA")] public string? ThemeData { get; set; }

    public int Password { get; set; }

    public List<Room> Rooms { get; set; } = [];
}