using BlackCanvasApp.Authorization;
using BlankCanvasApp.Application.DTOs;
using BlankCanvasApp.Application.Interfaces;
using BlankCanvasApp.Domain.Models;
using BlankCanvasApp.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static BlankCanvasApp.Domain.Emuns.Constants;

namespace BlackCanvasApp.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IServicesRepository _servicesRepository;
        private readonly IRepresentative _representativeService; 
        public CustomerController(ICustomerRepository customerRepository, IServicesRepository servicesRepository, IRepresentative representativeService)
        {
            _customerRepository = customerRepository;
            _servicesRepository = servicesRepository;
            _representativeService = representativeService;
        }
        // GET: CustomerController
        public async Task<ActionResult> Search(string filter)
        {
            var customers = await _customerRepository.FindAsync(c => !c.IsDeleted);

            if (!string.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                customers = customers.Where(c =>
                    c.Name.ToLower().Contains(filter) ||
                    //c.LastName.ToLower().Contains(filter) ||
                    (c.Contact != null && c.Contact.ToLower().Contains(filter)) ||
                    (c.Email != null && c.Email.ToLower().Contains(filter))
                ).ToList();
            }

            return PartialView("CustomerList", customers);
        }

        // GET: CustomerController/Details/5
        [HasPermission(AppPermissions.VerClientes)]
        public async Task<IActionResult> CustomerList()
        {
            var customersDto = await _customerRepository.GetCustomerListDtoAsync();
            var services = await _servicesRepository.FindAsync(s => !s.IsDeleted);
            var representatives = await _representativeService.GetActiveRepresentativesAsync();

            //var dtos = customers.Select(c => c.ToListDto()).ToList();

            ViewBag.Services = services;
            ViewBag.Representatives = representatives;
            ViewBag.Statuses = CustomerStatusMeta.Data;
            return View(customersDto);
        }

        // GET: CustomerController/Create
        [HasPermission(AppPermissions.CrearClientes)]
        public ActionResult CreateCustomer()
        {
            return View();
        }
        // GET: CustomerController/Edit/5
        [HasPermission(AppPermissions.EditarClientes)]
        public async Task<ActionResult> EditCustomer(int id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            return Json(new { success = true, customer });
        }
        // GET: CustomerController/Delete/5
        [HttpGet]
        [HasPermission(AppPermissions.EliminarClientes)]
        public async Task<ActionResult> DeleteCustomer(int Id)
        {
            var customer = await _customerRepository.GetByIdAsync(Id);
            return View(customer);
        }

        // POST: CustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(AppPermissions.CrearClientes)]
        public async Task<ActionResult> Create(CustomerFormDto customerDto)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos." });

            try
            {
                var entity = customerDto.ToEntity();
                var result = await _customerRepository.AddAsync(entity);

                return Json(result
                    ? new { success = true, message = "Cliente creado correctamente." }
                    : new { success = false, message = "No se pudo crear el cliente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CustomerFormDto customer)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Datos inválidos." });

            try
            {
                var entity = await _customerRepository.Query()
                                    .Include(c => c.Services)
                                    .FirstOrDefaultAsync(c => c.Id == customer.Id);
                if (entity == null)
                    return Json(new { success = false, message = "Cliente no encontrado." });

                customer.ApplyTo(entity);
                var result = await _customerRepository.UpdateAsync(entity);

                return Json(result
                    ? new { success = true, message = "Cliente actualizado correctamente." }
                    : new { success = false, message = "No se pudo actualizar el cliente." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: CustomerController/UpdateServices
        [HttpPost]
        [ValidateAntiForgeryToken]
        [HasPermission(AppPermissions.EditarClientes)]
        public async Task<ActionResult> UpdateServices(CustomerServicesFormDto form)
        {
            try
            {
                var customer = await _customerRepository.Query()
                    .Include(c => c.Services)
                    .FirstOrDefaultAsync(c => c.Id == form.CustomerId);

                if (customer == null)
                    return Json(new { success = false, message = "Cliente no encontrado." });

                customer.Services ??= new List<CustomerServices>();

                var selectedIds = form.SelectedServiceIds.Distinct().ToList();
                var servicesToRemove = customer.Services
                    .Where(cs => !selectedIds.Contains(cs.ServiceId))
                    .ToList();

                foreach (var service in servicesToRemove)
                {
                    customer.Services.Remove(service);
                }

                var existingServiceIds = customer.Services
                    .Select(cs => cs.ServiceId)
                    .ToHashSet();

                var servicesToAdd = selectedIds
                    .Where(serviceId => !existingServiceIds.Contains(serviceId))
                    .Select(serviceId => new CustomerServices
                    {
                        CustomerId = customer.Id,
                        ServiceId = serviceId
                    });

                foreach (var service in servicesToAdd)
                {
                    customer.Services.Add(service);
                }

                var result = await _customerRepository.UpdateAsync(customer);

                return Json(result
                    ? new { success = true, message = "Servicios actualizados correctamente." }
                    : new { success = false, message = "No se pudieron actualizar los servicios." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int Id)
        {
            try
            {
                var result = await _customerRepository.DeleteAsync(Id);
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
