using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Fundraiser.Models
{
    public class Participant
    {
        [DisplayName("Paddle")]
        public int ID { get; set; }
        public string Name { get; set; }
        [DisplayName("Total Contributed")]
        public decimal TotalContributed { get; set; }
        public bool Paid { get; set; }

        public ICollection<Contribution> Contributions { get; set; }
    }
}
