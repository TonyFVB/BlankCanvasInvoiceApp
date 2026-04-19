using BlackCanvasApp.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using static System.Net.WebRequestMethods;

namespace BlackCanvasApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exception = HttpContext.Items["Exception"] as Exception;
            var exceptionFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();

            //var finalStatusCode = statusCode ?? HttpContext.Response.StatusCode;

            var model = new ErrorViewModel
            {
                RequestId = HttpContext.TraceIdentifier,
                Message = "Ha ocurrido un error inesperado",
                Details = exception?.Message,
                StatusCode = HttpContext.Response.StatusCode,
                Path = HttpContext.Request.Path,
                ShowDetails = exception != null
                
            };
            model.Message = model.StatusCode switch
            {
                401 => "¡Ups! Parece que tu sesión ha expirado. Por favor, vuelve a iniciar sesión para continuar.",
                403 => "No tienes permiso para acceder a esta página o recurso.",
                404 => "Página no encontrada",
                500 => "Error interno del servidor",
                _ => "Algo salió mal"
            };
            return View(model);
        }
    }
}
