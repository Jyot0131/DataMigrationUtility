using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationUtility.Domain
{
    public class BatchStats
    {
        public int Id { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public string Status { get; set; }
    }
}
