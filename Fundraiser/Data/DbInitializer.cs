using Fundraiser.Models;
using System.Linq;

namespace Fundraiser.Data
{
    public static class DbInitializer
    {
        public static void Initialize(FundraiserContext context)
        {
            context.Database.EnsureCreated();

            // Look for any participants.
            if (context.Participants.Any())
            {
                return;   // DB has been seeded
            }

            var participants = new Participant[]
            {
               new Participant{Name="Member 1", EmailAddress="jparrish@solutionreach.com"},
               new Participant{Name="Member 2"},
               new Participant{Name="Member 3"}
            };

            foreach (Participant p in participants)
            {
               context.Participants.Add(p);
            }

            context.SaveChanges();

            var contributions = new Contribution[]
            {
            new Contribution{ParticipantID=1,Amount=(decimal)10.50,Description="Item 1"},
            new Contribution{ParticipantID=1,Amount=(decimal)11.50,Description="Item 2"},
            new Contribution{ParticipantID=1,Amount=(decimal)12.50,Description="Item 3"},
            new Contribution{ParticipantID=2,Amount=(decimal)13.50,Description="Item 4"},
            new Contribution{ParticipantID=2,Amount=(decimal)14.50,Description="Item 5"},
            new Contribution{ParticipantID=2,Amount=(decimal)15.50,Description="Item 6"},
            new Contribution{ParticipantID=3,Amount=(decimal)16.50,Description="Item 7"},
            new Contribution{ParticipantID=3,Amount=(decimal)17.50,Description="Item 8"},
            new Contribution{ParticipantID=3,Amount=(decimal)18.50,Description="Item 9"},
            };
            foreach (Contribution c in contributions)
            {
               context.Contributions.Add(c);
            }

            context.SaveChanges();
        }
    }
}
