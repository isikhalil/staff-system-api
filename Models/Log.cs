
using System;

namespace Web.Models
{
    public class Log
    {
        public long Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Category { get; set; }
        
        public string Message { get; set; }
    }
}