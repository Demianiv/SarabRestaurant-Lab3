
using Microsoft.AspNetCore.Mvc;
using SarabRestaurant.Models;
using SarabRestaurant.Services;

namespace SarabRestaurant.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailSender _emailSender;

        public EmailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new EmailViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(EmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                await _emailSender.SendEmailAsync(
                    model.Email,
                    "Message from Sarab Restaurant",
                    model.Message
                );

                TempData["Success"] = "Email sent successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(
                    "",
                    "Failed to send email. Please check SMTP settings."
                );

                return View(model);
            }
        }
    }
}
