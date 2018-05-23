using System.ComponentModel;

namespace Fundraiser.Models
{
    public class Contribution
    {
        public int ID { get; set; }
        [DisplayName("Paddle")]
        public int ParticipantID { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }

        public Participant Participant { get; set; }
    }
}
