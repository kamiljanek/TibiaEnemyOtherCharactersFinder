using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnemyCharsFinder.Models
{
    public class ScrapSesion
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime DatePublished { get; set; }
        
        public string? OnlineCharacterNames { get; set; }
        public string? ServerName { get; set; }
    }
}
