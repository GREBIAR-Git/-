using System.ComponentModel.DataAnnotations;

namespace DataBase.Models
{
    public class WeekDay
    {
        [Key]
        public long Id { get; set; }

        public string? Name { get; set; }

        public List<Window>? Windows { get; set; }
    }
}
