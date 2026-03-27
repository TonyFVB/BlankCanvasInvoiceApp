using BlackCanvasApp.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BlackCanvasApp.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomer _customerService;
        public CustomerController(ICustomer customerService)
        {
            _customerService = customerService;
        }
        // GET: CustomerController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CustomerController/Details/5
        public async Task<IActionResult> CustomerList()
        {
            var customers = await _customerService.GetAllAsync();
            return View(customers);
        }

        // GET: CustomerController/Create
        public ActionResult CreateCustomer()
        {
            return View();
        }
        // GET: CustomerController/Edit/5
        public ActionResult EditCustomer(int id)
        {
            return View();
        }
        // GET: CustomerController/Delete/5
        public ActionResult DeleteCustomer(int id)
        {
            return View();
        }

        // POST: CustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
