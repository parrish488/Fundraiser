using Fundraiser.Data;
using Fundraiser.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace Fundraiser.Controllers
{
    public class HomeController : Controller
    {
        private readonly FundraiserContext _context;

        public HomeController(FundraiserContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            HomeData data = new HomeData();

            data.TotalMembers = _context.Participants.Count();

            _context.Contributions.ToList().ForEach(x => data.TotalRaised += x.Amount);

            return View(data);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
