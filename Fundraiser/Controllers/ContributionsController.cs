using Fundraiser.Data;
using Fundraiser.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Fundraiser.Controllers
{
    public class ContributionsController : Controller
    {
        private readonly FundraiserContext _context;

        public ContributionsController(FundraiserContext context)
        {
            _context = context;
        }

        // GET: Contributions
        public async Task<IActionResult> Index()
        {
            var fundraiserContext = _context.Contributions.Include(c => c.Participant);
            return View(await fundraiserContext.ToListAsync());
        }

        // GET: Contributions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contribution = await _context.Contributions
                .Include(c => c.Participant)
                .SingleOrDefaultAsync(m => m.ID == id);

            if (contribution == null)
            {
                return NotFound();
            }

            return View(contribution);
        }

        // GET: Contributions/Create
        public IActionResult Create()
        {
            ViewData["ParticipantID"] = new SelectList(_context.Participants, "ID", "ID");
            return View();
        }

        // POST: Contributions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ParticipantID,Description,Amount")] Contribution contribution)
        {
            if (ModelState.IsValid)
            {
                _context.Add(contribution);
                await _context.SaveChangesAsync();

                SendNotification(contribution.ParticipantID);

                return RedirectToAction(nameof(Create));
            }
            ViewData["ParticipantID"] = new SelectList(_context.Participants, "ID", "ID", contribution.ParticipantID);
            return View(contribution);
        }

        // GET: Contributions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contribution = await _context.Contributions.SingleOrDefaultAsync(m => m.ID == id);
            if (contribution == null)
            {
                return NotFound();
            }
            ViewData["ParticipantID"] = new SelectList(_context.Participants, "ID", "ID", contribution.ParticipantID);
            return View(contribution);
        }

        // POST: Contributions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ParticipantID,Description,Amount")] Contribution contribution)
        {
            if (id != contribution.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contribution);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContributionExists(contribution.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                SendNotification(contribution.ParticipantID);

                return RedirectToAction(nameof(Index));
            }
            ViewData["ParticipantID"] = new SelectList(_context.Participants, "ID", "ID", contribution.ParticipantID);
            return View(contribution);
        }

        // GET: Contributions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contribution = await _context.Contributions
                .Include(c => c.Participant)
                .SingleOrDefaultAsync(m => m.ID == id);
            if (contribution == null)
            {
                return NotFound();
            }

            return View(contribution);
        }

        // POST: Contributions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contribution = await _context.Contributions.SingleOrDefaultAsync(m => m.ID == id);
            _context.Contributions.Remove(contribution);
            await _context.SaveChangesAsync();

            SendNotification(contribution.ParticipantID);

            return RedirectToAction(nameof(Index));
        }

        private bool ContributionExists(int id)
        {
            return _context.Contributions.Any(e => e.ID == id);
        }

        private async void SendNotification(int participantID)
        {
            var participant = await _context.Participants
            .Include(p => p.Contributions)
            .SingleOrDefaultAsync(m => m.ID == participantID);

            _context.Contributions.Where(c => c.ParticipantID == participantID).ToList().ForEach(x => participant.TotalContributed += x.Amount);

            if (participant != null && !string.IsNullOrEmpty(participant.EmailAddress))
            {
                var smtpClient = new SmtpClient
                {
                    Host = "smtp.gmail.com", // set your SMTP server name here
                    Port = 587, // Port 
                    EnableSsl = true,
                    Credentials = new NetworkCredential("jasonparrish8@gmail.com", "Jap080312")
                };

                string body = "You've contributed " + participant.TotalContributed.ToString("C2") + " so far!  Here's your contributions: \n\n";

                foreach (Contribution contribution in participant.Contributions)
                {
                    body += contribution.Description + ", " + contribution.Amount.ToString("C2") + "\n";
                }

                using (var message = new MailMessage("jasonparrish8@gmail.com", participant.EmailAddress)
                {
                    Subject = "Youth Auction Contribution Update",
                    Body = body
                })
                {
                    await smtpClient.SendMailAsync(message);
                }
            }
        }
    }
}
