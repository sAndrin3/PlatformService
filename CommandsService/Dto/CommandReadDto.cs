using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CommandsService.Dto
{
    public class CommandReadDto
    {
        public int Id { get; set; }
        public int HowTo { get; set; }
        public int CommandLine { get; set; }
        public int PlatformId { get; set; }
    }
}