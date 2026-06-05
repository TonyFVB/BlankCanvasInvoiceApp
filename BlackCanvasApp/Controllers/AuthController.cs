using BlackCanvasApp.ViewModels;
using BlankCanvasApp.Domain.Models;
using BlankCanvasApp.Infrastructure;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BlackCanvasApp.Controllers
{
    /// <summary>
    /// Controlador de autenticación: Login, Register, ForgotPassword, Logout.
    /// No requiere autorización propia — maneja usuarios no autenticados.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IAntiforgery _antiforgery;

        // Inyecta IEmailSender si configuras envío real de correos
        // private readonly IEmailSender _emailSender;

        public AuthController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthController> logger,
            IAntiforgery antiforgery)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _antiforgery = antiforgery;
        }

        // ============================================================
        // GET /Auth/Login  (también es la ruta raíz "/")
        // ============================================================
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            // Si ya está autenticado, redirige directo al dashboard
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");

            ViewData["ReturnUrl"] = returnUrl;
            return View(new AuthModel());
        }

        // ============================================================
        // POST /Auth/Login
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            // Busca por email y usa el UserName para el SignIn
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !user.IsActive)
            {
                ViewBag.ErrorMessage = "Correo o contraseña incorrectos.";
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                userName: user.UserName!,
                password: model.Password,
                isPersistent: model.RememberMe,
                lockoutOnFailure: true
            );

            if (result.Succeeded)
            {
                // Actualiza fecha de último acceso
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Login exitoso: {Email}", model.Email);
                return RedirectToLocal(returnUrl);
            }

            if (result.IsLockedOut)
            {
                _logger.LogWarning("Cuenta bloqueada: {Email}", model.Email);
                ViewBag.ErrorMessage = "Cuenta bloqueada temporalmente. Intenta en 10 minutos.";
                return View(model);
            }

            ViewBag.ErrorMessage = "Correo o contraseña incorrectos.";
            return View(model);
        }
        // ============================================================
        // GET /Auth/Register
        // ============================================================
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToAction("Index", "Home");
 
            return View(new RegisterViewModel());
        }

        // ============================================================
        // POST /Auth/Register
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Verificar email duplicado
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {
                ModelState.AddModelError("Email", "Este correo ya está registrado.");
                return View(model);
            }

            // Verificar username duplicado
            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {
                ModelState.AddModelError("UserName", "Este nombre de usuario ya está en uso.");
                return View(model);
            }

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                IsActive = true,
                EmailConfirmed = true,  // ajusta a false si implementas confirmación por email
                CreatedAt = DateTime.UtcNow,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Asignar rol — solo Admin puede asignar rol Admin
                var roleToAssign = User.IsInRole(AppRoles.Admin) && model.Role == AppRoles.Admin
                    ? AppRoles.Admin
                    : AppRoles.Cliente;

                if (await _roleManager.RoleExistsAsync(roleToAssign))
                    await _userManager.AddToRoleAsync(user, roleToAssign);

                _logger.LogInformation("Nuevo usuario: {Email} | Rol: {Role}", user.Email, roleToAssign);

                TempData["SuccessMessage"] = "¡Cuenta creada exitosamente! Ya puedes iniciar sesión.";
                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        // ============================================================
        // POST /Auth/ForgotPassword
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Correo inválido." });

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action("ResetPassword", "Auth",
                    new { token, email = user.Email }, Request.Scheme);

                // TODO: enviar email con resetLink usando tu servicio de correo
                _logger.LogInformation("Reset link generado para {Email}: {Link}", model.Email, resetLink);
            }

            // Siempre éxito (no revelar si el email existe)
            return Json(new { success = true });
        }

        // ============================================================
        // POST /Auth/Logout
        // ============================================================
        [HttpGet]
        public IActionResult AccessDenied(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }


        // ── Helper ───────────────────────────────────────────────────
        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }
    }
}
