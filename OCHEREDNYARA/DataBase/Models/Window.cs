using System.ComponentModel.DataAnnotations;

namespace DataBase.Models;

public class Window
{
    [Key]
    public long Id { get; set; }

    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public long QueueId { get; set; }

    public long WeekDayId { get; set; }

    public WeekDay? WeekDay { get; set; }

    public Queue? Queue { get; set; }

    public List<Reserved> Reserveds { get; } = new();
}
