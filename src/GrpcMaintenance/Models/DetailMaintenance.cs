using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GrpcMaintenance.Models
{
    public class DetailMaintenance
    {
        [Key]
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime Date { get; set; }

        public int MaintenanceOrderId { get; set; }

        [ForeignKey(nameof(MaintenanceOrderId))]
        public MaintenanceOrder MaintenanceOrder { get; set; }
    }
}
