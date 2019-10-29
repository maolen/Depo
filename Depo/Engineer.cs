using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Depo
{ 
    [Table("Engineers")]
    public class Engineer
    {
        public int Id { get; set; }
        public string FullName { get; set; }
    }
}
