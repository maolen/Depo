using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depo
{
    [Table("Buses")]
    public class Bus
    {
        public int Id { get; set; }
        public string VinNumber { get; set; }
        public int EngineerId { get; set; }
        public int StatusId { get; set; }
    }
}
