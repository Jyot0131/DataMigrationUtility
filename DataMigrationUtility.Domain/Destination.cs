using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataMigrationUtility.Domain
{
    public class Destination
    {
        public int Id { get; set; }
        public int SourceId { get; set; }
        public Source Source { get; set; }
        public int Sum { get; set; }
    }
}
