using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrpcMaintenance.Models
{
    public class Equipment
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public string SerialNumber { get; set; }

        public ICollection<MaintenanceOrder> Orders { get; set; }
    }
}
