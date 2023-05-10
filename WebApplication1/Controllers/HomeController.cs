using Microsoft.AspNetCore.Mvc;
using sayfaASP.Models;
using System.Diagnostics;
using WebApplication1.Filter;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
	[UserFilter]
	public class HomeController : Controller
	{
		private readonly DataContext _context;
		public HomeController(DataContext context)
		{
			_context = context;
		}

		public IActionResult Index()
		{
			return View();
		}
		public IActionResult List()
		{
			var users = from u in _context.Users
						join ui in _context.UsersInfo on u.Name equals ui.NameId
						join uu in _context.UsersUnit on u.Unit equals uu.UnitId
						select new
						{
							u.Id,
							u.taxNo,
							u.tcNo,
							u.Email,
							uu.UnitAd,
							ui.NameUser,
							ui.Surname,
							ui.Phone
						};

			var viewModel = users.Select(u => new UserViewModel
			{
				Id = u.Id,
				taxNo = u.taxNo,
				tcNo = u.tcNo,
				Email = u.Email,
				UnitAd = u.UnitAd,
				NameUser = u.NameUser,
				Surname = u.Surname,
				Phone = u.Phone
			}).ToList();

			var Session = HttpContext.Session.GetString("VergiNo");
			var SessionTc = HttpContext.Session.GetString("TcNo");
			return View(viewModel.Where(x => x.taxNo.Equals(Session) || x.tcNo.Equals(SessionTc)));
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
		public IActionResult Secim()
		{
			return View();
		}
	}
}