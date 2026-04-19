using BlackCanvasApp.Models;
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
        public async Task<ActionResult> Search(string filter)
        {
            var customers = await _customerService.GetAllActiveAsync(); // O un método específico de búsqueda en tu servicio

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                customers = customers.Where(c =>
                    c.Name.ToLower().Contains(filter) ||
                    c.LastName.ToLower().Contains(filter) ||
                    (c.Contact != null && c.Contact.ToLower().Contains(filter)) ||
                    (c.Email != null && c.Email.ToLower().Contains(filter))
                ).ToList();
            }
            return PartialView("CustomerList", customers);
        }

        // GET: CustomerController/Details/5
        public async Task<IActionResult> CustomerList()
        {
            var customers = await _customerService.GetAllActiveAsync();
            return View(customers);
        }

        // GET: CustomerController/Create
        public ActionResult CreateCustomer()
        {
            return View();
        }
        // GET: CustomerController/Edit/5
        public async Task<ActionResult> EditCustomer(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            return View(customer);
        }
        // GET: CustomerController/Delete/5
        [HttpGet]
        public async Task<ActionResult> DeleteCustomer(int Id)
        {
            var customer = await _customerService.GetByIdAsync(Id);
            return View(customer);
        }

        // POST: CustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Customer customer)
        {
            try {
                //if (!ModelState.IsValid) return View("CustomerList",customer);
                var result = await _customerService.AddAsync(customer);

                if (result)
                {
                    TempData["SuccessMessage"] = "Cliente agregado correctamente ✅";
                    //return RedirectToAction("CustomerList");
                }
                    
            }
            catch(Exception ex)
             {
                TempData["ErrorMessage"] = $"Error de base de datos: {ex.Message}";
            }
            

            return RedirectToAction("CustomerList");
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Customer customer)
        {
            try
            {
                //if (!ModelState.IsValid) return View("CustomerList",customer);
                var result = await _customerService.UpdateAsync(customer);

                if (result)
                {
                    TempData["SuccessMessage"] = "Cliente modificado correctamente ✅";
                    //return RedirectToAction("CustomerList");
                }
            }
            catch(Exception ex)
            {
                TempData["ErrorMessage"] = $"Error de base de datos: {ex.Message}";
            }
            return RedirectToAction("CustomerList");
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int Id)
        {
            try
            {
                var result = await _customerService.DeleteAsync(Id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Cliente eliminado correctamente ✅";
                    //return RedirectToAction("CustomerList");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al intentar eliminar el cliente: {ex.Message}";
            }

            return RedirectToAction("CustomerList");
        }
    }
}
