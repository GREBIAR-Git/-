using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataBase.Models;

public class Reserved
{
    [Key] public long ClientId { get; set; }

    [Key] public long WindowId { get; set; }

    [Column(TypeName = "date")] public DateTime Date { get; set; }

    public Client? Client { get; set; }
    public Window? Window { get; set; }
}