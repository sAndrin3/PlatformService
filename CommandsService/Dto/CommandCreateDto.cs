using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CommandsService.Dto
{
    public class CommandCreateDto
    {
        [Required]
        public int HowTo { get; set; }
        [Required]
        public int CommandLine { get; set; }
    }
}