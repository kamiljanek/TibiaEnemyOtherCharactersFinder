using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyCharsFinder.Models
{
    public class Character
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? OtherCharacterTry { get; set; }
        public DateTime LastDatePublished { get; set; }
    }
}
