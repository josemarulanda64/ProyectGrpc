using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GrpcMaintenance.Models
{
    public class MaintenanceOrder
    {
        [Key]
        public int Id { get; set; }

        public DateTime DateCreated { get; set; }

        public string State { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int EquipmentId { get; set; }
        public Equipment Equipment { get; set; }

        public ICollection<DetailMaintenance> Details { get; set; }
    }
}
