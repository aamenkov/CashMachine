using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CashMachineWebApp.Models
{
    public class Cassette
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CassetteId { get; set; }
        public int Value { get; set; }
        public int Amount { get; set; }
        public bool IsWorking { get; set; }
        [ForeignKey("ATMInfoKey")]
        public Guid AtmId { get; set; }
        

    }
}
