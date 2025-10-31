using Microsoft.AspNetCore.Mvc;
using HRMS_CSharp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace HRMS_CSharp.Controllers
{
    public class SettingsController : Controller
    {
        private readonly HrmsContext _context;

        public SettingsController(HrmsContext context)
        {
            _context = context;
        }

        // GET: Settings/Index
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("user_login_access") == "1")
                return RedirectToAction("Dashboard", "Dashboard");
            return View("Login");
        }

        // GET: Settings/Settings
        public async Task<IActionResult> Settings()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var settings = await _context.Settings.ToDictionaryAsync(s => s.Skey!, s => s.Svalue);
            return View(settings);
        }

        // POST: Settings/Add_Settings
        [HttpPost]
        public async Task<IActionResult> Add_Settings(string id, string title, string description, string copyright, string contact, string currency, string symbol, string email, string address, string address2, IFormFile img_url)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            // Basic validation
            if (string.IsNullOrEmpty(title) || title.Length < 5 || title.Length > 60)
                return Content("Title must be between 5 and 60 characters");
            if (string.IsNullOrEmpty(description) || description.Length < 20 || description.Length > 512)
                return Content("Description must be between 20 and 512 characters");
            if (!string.IsNullOrEmpty(address) && (address.Length < 5 || address.Length > 600))
                return Content("Address must be between 5 and 600 characters");
            if (!string.IsNullOrEmpty(address2) && (address2.Length < 5 || address2.Length > 600))
                return Content("Address2 must be between 5 and 600 characters");

            string imgUrl = null;
            if (img_url != null)
            {
                var allowedTypes = new[] { "image/gif", "image/jpg", "image/png", "image/jpeg", "image/svg+xml" };
                if (!allowedTypes.Contains(img_url.ContentType))
                    return Content("Invalid file type. Only gif, jpg, png, jpeg, svg allowed.");
                if (img_url.Length > 13038000) // ~13MB
                    return Content("File size too large. Max 13MB.");

                var fileName = Path.GetFileName(img_url.FileName);
                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", fileName);
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                // Check if old logo exists and delete
                var oldLogo = await _context.Settings.FirstOrDefaultAsync(s => s.Skey == "sitelogo");
                if (oldLogo != null && !string.IsNullOrEmpty(oldLogo.Svalue))
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", oldLogo.Svalue);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await img_url.CopyToAsync(stream);
                }
                imgUrl = fileName;
            }

            // Update or insert settings
            var settingsToUpdate = new Dictionary<string, string>
            {
                { "sitetitle", title },
                { "description", description },
                { "copyright", copyright },
                { "contact", contact },
                { "currency", currency },
                { "symbol", symbol },
                { "system_email", email },
                { "address", address },
                { "address2", address2 }
            };

            if (!string.IsNullOrEmpty(imgUrl))
                settingsToUpdate["sitelogo"] = imgUrl;

            foreach (var kvp in settingsToUpdate)
            {
                var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Skey == kvp.Key);
                if (setting != null)
                {
                    setting.Svalue = kvp.Value;
                    _context.Settings.Update(setting);
                }
                else
                {
                    _context.Settings.Add(new Settings { Skey = kvp.Key, Svalue = kvp.Value });
                }
            }

            await _context.SaveChangesAsync();
            return Content("Successfully Updated");
        }
    }
}
