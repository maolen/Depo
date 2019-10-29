using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depo
{ 
    [Table("Statuses")]
    public class Status
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }
}
