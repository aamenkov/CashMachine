using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Transactions;

namespace CashMachineWebApp.Models
{
    public class ATM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid AtmId { get; set; }

        public List<Cassette> CassetteList { get; set; }


    }
}
