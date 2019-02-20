using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Fundraiser.Data;
using Fundraiser.Models;
using System.Net.Mail;
using System.Net;

namespace Fundraiser.Controllers
{
    public class ParticipantsController : Controller
    {
        private readonly FundraiserContext _context;

        public ParticipantsController(FundraiserContext context)
        {
            _context = context;
        }

        // GET: Participants
        public async Task<IActionResult> Index()
        {
            return View(await _context.Participants.ToListAsync());
        }

        // GET: Participants/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _context.Participants
                .Include(p => p.Contributions)
                .SingleOrDefaultAsync(m => m.ID == id);

            _context.Contributions.Where(c => c.ParticipantID == id).ToList().ForEach(x => participant.TotalContributed += x.Amount);

            if (participant == null)
            {
                return NotFound();
            }

            return View(participant);
        }

        // GET: Participants/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Participants/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,EmailAddress")] Participant participant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(participant);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = participant.ID });
            }
            return View(participant);
        }

        // GET: Participants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _context.Participants.SingleOrDefaultAsync(m => m.ID == id);
            if (participant == null)
            {
                return NotFound();
            }
            return View(participant);
        }

        // POST: Participants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,EmailAddress,Paid")] Participant participant)
        {
            if (id != participant.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(participant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ParticipantExists(participant.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", new { id = participant.ID });
            }
            return View(participant);
        }

        // GET: Participants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var participant = await _context.Participants
                .SingleOrDefaultAsync(m => m.ID == id);
            if (participant == null)
            {
                return NotFound();
            }

            return View(participant);
        }

        // POST: Participants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var participant = await _context.Participants.SingleOrDefaultAsync(m => m.ID == id);
            _context.Participants.Remove(participant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Complete()
        {
            foreach (Participant participant in _context.Participants)
            {
                _context.Contributions.Where(c => c.ParticipantID == participant.ID).ToList().ForEach(x => participant.TotalContributed += x.Amount);

                // If they exist, have an email, have made contributions, and not paid yet.
                if (participant != null && !string.IsNullOrEmpty(participant.EmailAddress) && participant.Contributions != null && participant.Contributions.Count > 0 && !participant.Paid)
                {
                    var smtpClient = new SmtpClient
                    {
                        Host = "smtp.gmail.com", // set your SMTP server name here
                        Port = 587, // Port 
                        EnableSsl = true,
                        Credentials = new NetworkCredential("jasonparrish8@gmail.com", "Jap080312")
                    };

                    string body = "Thanks for participating in the Ruston View 2nd Ward Youth Auction!  You contributed " + participant.TotalContributed.ToString("C2") + "!  Here's your contributions: \n\n";

                    foreach (Contribution contribution in participant.Contributions)
                    {
                        body += contribution.Description + ", " + contribution.Amount.ToString("C2") + "\n";
                    }

                    body += "\nPlease see the Ward Clerk for instructions on how to complete your donation.";

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

            return RedirectToAction(nameof(Index));
        }

        private bool ParticipantExists(int id)
        {
            return _context.Participants.Any(e => e.ID == id);
        }
    }
}
