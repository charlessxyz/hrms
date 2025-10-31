using Microsoft.AspNetCore.Mvc;
using HRMS_CSharp.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace HRMS_CSharp.Controllers
{
    public class LogisticsController : Controller
    {
        private readonly HrmsContext _context;

        public LogisticsController(HrmsContext context)
        {
            _context = context;
        }

        // GET: Logistics/logistic_support
        public async Task<IActionResult> logistic_support()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var data = new
            {
                projects = await _context.Projects.ToListAsync(),
                supportview = await GetLogisticsupportValue(),
                employee = await _context.Employees.ToListAsync(),
                tasks = await _context.ProjectTasks.ToListAsync(),
                assets = await _context.Assets.ToListAsync()
            };

            return View(data);
        }

        // POST: Logistics/Add_Logistic
        [HttpPost]
        public async Task<IActionResult> Add_Logistic(string logid, string logname, string logqty)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (string.IsNullOrEmpty(logname) || logname.Length < 2 || logname.Length > 220)
                return Content("Name must be between 2 and 220 characters");

            int qty = int.Parse(logqty);
            var data = new Asset
            {
                AssName = logname,
                AssQty = qty.ToString(),
                InStock = qty.ToString()
            };

            if (string.IsNullOrEmpty(logid))
            {
                _context.Assets.Add(data);
            }
            else
            {
                data.AssId = int.Parse(logid);
                _context.Assets.Update(data);
            }

            await _context.SaveChangesAsync();
            return Content(string.IsNullOrEmpty(logid) ? "Successfully Added" : "Successfully Updated");
        }

        // POST: Logistics/Add_Logistic_Support
        [HttpPost]
        public async Task<IActionResult> Add_Logistic_Support(string assid, string logid, string assignid, string proid, string taskid, string assignqty, DateTime? startdate, DateTime? enddate, DateTime? backdate, string backqty, string remarks)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            int qty = int.Parse(assignqty);
            if (qty < 1)
                return Content("Quantity is required");

            var data = new EmpAsset
            {
                EmpId = int.Parse(assignid),
                AssetsId = int.Parse(logid),
                GivenDate = startdate ?? DateTime.Now,
                ReturnDate = enddate ?? DateTime.Now
            };

            if (string.IsNullOrEmpty(assid))
            {
                _context.EmpAssets.Add(data);
                // Update stock
                var asset = await _context.Assets.FindAsync(int.Parse(logid));
                if (asset != null)
                {
                    int currentStock = int.Parse(asset.InStock);
                    asset.InStock = (currentStock - qty).ToString();
                    _context.Assets.Update(asset);
                }
                await _context.SaveChangesAsync();
                return Content("Successfully Added");
            }
            else
            {
                data.Id = int.Parse(assid);
                _context.EmpAssets.Update(data);
                // Update stock for return
                var asset = await _context.Assets.FindAsync(int.Parse(logid));
                if (asset != null && !string.IsNullOrEmpty(backqty))
                {
                    int bqty = int.Parse(backqty);
                    int currentStock = int.Parse(asset.InStock);
                    asset.InStock = (currentStock + bqty).ToString();
                    _context.Assets.Update(asset);
                }
                await _context.SaveChangesAsync();
                return Content("Successfully Updated");
            }
        }

        // GET: Logistics/Logisticebyib
        [HttpGet]
        public async Task<IActionResult> Logisticebyib(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var logistic = await _context.Assets.FindAsync(int.Parse(id));
            return Json(new { logisticvaluebyid = logistic });
        }

        // GET: Logistics/Logisticesupportbyib
        [HttpGet]
        public async Task<IActionResult> Logisticesupportbyib(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var support = await GetLogisticsupportValueById(id);
            return Json(new { logisticsupport = support });
        }

        // GET: Logistics/GetInstock
        [HttpGet]
        public async Task<IActionResult> GetInstock(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var asset = await _context.Assets.FindAsync(int.Parse(id));
            return Content(asset?.InStock.ToString() ?? "0");
        }

        // GET: Logistics/Logisticedelet
        [HttpGet]
        public async Task<IActionResult> Logisticedelet(string D)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var asset = await _context.Assets.FindAsync(int.Parse(D));
            if (asset != null)
            {
                _context.Assets.Remove(asset);
                await _context.SaveChangesAsync();
            }
            return Content("Deleted");
        }

        // GET: Logistics/GetTaskforlogistic
        [HttpGet]
        public async Task<IActionResult> GetTaskforlogistic(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var tasks = await _context.ProjectTasks.Where(t => t.ProjectId.ToString() == id).ToListAsync();
            var options = tasks.Select(t => $"<option value='{t.Id}'>{t.TaskTitle}</option>");
            return Content(string.Join("", options));
        }

        // GET: Logistics/AssetscatByID
        [HttpGet]
        public async Task<IActionResult> AssetscatByID(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var category = await _context.AssetsCategories.FindAsync(int.Parse(id));
            return Json(new { assetscatval = category });
        }

        // GET: Logistics/GetAssignforlogistic
        [HttpGet]
        public async Task<IActionResult> GetAssignforlogistic(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var assigns = await _context.AssignTasks.Where(a => a.TaskId.ToString() == id).ToListAsync();
            var employeeIds = assigns.Select(a => a.AssignUser).Distinct();
            var employees = await _context.Employees.Where(e => employeeIds.Contains(e.EmId)).ToListAsync();
            var options = assigns.Select(a =>
            {
                var emp = employees.FirstOrDefault(e => e.EmId == a.AssignUser);
                return $"<option value='{a.AssignUser}'>{emp?.FirstName} {emp?.LastName}</option>";
            });
            return Content(string.Join("", options));
        }

        // GET: Logistics/Assets_Category
        public async Task<IActionResult> Assets_Category()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var categories = await _context.AssetsCategories.ToListAsync();
            return View(categories);
        }

        // POST: Logistics/Add_Assets_Category
        [HttpPost]
        public async Task<IActionResult> Add_Assets_Category(string catid, string cattype, string catname)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (string.IsNullOrEmpty(catname) || catname.Length < 1 || catname.Length > 220)
                return Content("Category name is required");

            var data = new AssetsCategory
            {
                CatName = catname,
                CatStatus = cattype
            };

            if (string.IsNullOrEmpty(catid))
            {
                _context.AssetsCategories.Add(data);
            }
            else
            {
                data.CatId = int.Parse(catid);
                _context.AssetsCategories.Update(data);
            }

            await _context.SaveChangesAsync();
            return Content(string.IsNullOrEmpty(catid) ? "Successfully Added" : "Successfully Updated");
        }

        // GET: Logistics/All_Assets
        public async Task<IActionResult> All_Assets()
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var data = new
            {
                assets = await _context.Assets.ToListAsync(),
                catvalue = await _context.AssetsCategories.ToListAsync()
            };

            return View(data);
        }

        // POST: Logistics/Add_Assets
        [HttpPost]
        public async Task<IActionResult> Add_Assets(string aid, string catid, string assname, string brand, string model, string code, string config, string purchase, string price, string pqty)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            if (string.IsNullOrEmpty(config) || config.Length < 2 || config.Length > 2024)
                return Content("Config is required");

            int qty = int.Parse(pqty);
            if (qty < 1)
                return Content("Quantity is required");

            var data = new Asset
            {
                Catid = catid,
                AssName = assname,
                AssBrand = brand,
                AssModel = model,
                AssCode = code,
                Configuration = config,
                PurchasingDate = string.IsNullOrEmpty(purchase) ? null : DateTime.Parse(purchase).ToString("yyyy-MM-dd"),
                AssPrice = string.IsNullOrEmpty(price) ? null : decimal.Parse(price).ToString(),
                AssQty = qty.ToString(),
                InStock = qty.ToString()
            };

            if (string.IsNullOrEmpty(aid))
            {
                _context.Assets.Add(data);
            }
            else
            {
                var existing = await _context.Assets.FindAsync(int.Parse(aid));
                if (existing != null)
                {
                    int existingQty = int.Parse(existing.AssQty);
                    int inqty = qty - existingQty;
                    int existingInStock = int.Parse(existing.InStock);
                    data.InStock = (existingInStock + inqty).ToString();
                }
                else
                {
                    data.InStock = qty.ToString();
                }
                data.AssId = int.Parse(aid);
                _context.Assets.Update(data);
            }

            await _context.SaveChangesAsync();
            return Content(string.IsNullOrEmpty(aid) ? "Successfully Added" : "Successfully Updated");
        }

        // GET: Logistics/AssetsByID
        [HttpGet]
        public async Task<IActionResult> AssetsByID(string id)
        {
            if (HttpContext.Session.GetString("user_login_access") != "1")
                return Redirect("/");

            var asset = await _context.Assets.FirstOrDefaultAsync(a => a.AssId == int.Parse(id));
            return Json(new { assetsByid = asset });
        }

        private async Task<List<dynamic>> GetLogisticsupportValue()
        {
            var result = await (from la in _context.EmpAssets
                                join e in _context.Employees on la.EmpId equals e.Id
                                join a in _context.Assets on la.AssetsId equals a.AssId
                                join pt in _context.ProjectTasks on la.AssetsId equals pt.Id into ptGroup
                                from pt in ptGroup.DefaultIfEmpty()
                                select new
                                {
                                    Id = la.Id,
                                    EmId = e.EmId,
                                    FirstName = e.FirstName,
                                    LastName = e.LastName,
                                    AssName = a.AssName,
                                    TaskId = pt.Id,
                                    TaskTitle = pt.TaskTitle
                                }).ToListAsync();
            return result.Cast<dynamic>().ToList();
        }

        private async Task<dynamic> GetLogisticsupportValueById(string id)
        {
            var result = await (from la in _context.EmpAssets.Where(la => la.Id == int.Parse(id))
                                join e in _context.Employees on la.EmpId equals e.Id
                                join a in _context.Assets on la.AssetsId equals a.AssId
                                join pt in _context.ProjectTasks on la.AssetsId equals pt.Id into ptGroup
                                from pt in ptGroup.DefaultIfEmpty()
                                select new
                                {
                                    la,
                                    EmId = e.EmId,
                                    FirstName = e.FirstName,
                                    LastName = e.LastName,
                                    AssName = a.AssName,
                                    TaskId = pt.Id,
                                    TaskTitle = pt.TaskTitle
                                }).FirstOrDefaultAsync();
            return result;
        }
    }
}
