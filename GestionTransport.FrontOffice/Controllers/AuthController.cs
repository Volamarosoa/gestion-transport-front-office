using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models.Auth;
using GestionTransport.FrontOffice.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Security.Cryptography;

namespace GestionTransport.FrontOffice.Controllers;

[AllowAnonymous]
public class AuthController : Controller
{
    private readonly IAuthentificationRepository _authentificationRepository;
    private readonly IEmployeRepository _employeRepository;

    public AuthController(
        IAuthentificationRepository authentificationRepository,
        IEmployeRepository employeRepository)
    {
        _authentificationRepository = authentificationRepository;
        _employeRepository = employeRepository;
    }

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var auth = _authentificationRepository.GetByMatricule(model.Matricule.Trim());
        if (auth == null || !auth.Actif || !BCrypt.Net.BCrypt.Verify(model.MotDePasse, auth.MotDePasse))
        {
            ModelState.AddModelError(string.Empty, "Matricule ou mot de passe invalide.");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, auth.IdEmploye.ToString()),
            new(ClaimTypes.Name, auth.Matricule),
            new(ClaimTypes.Role, auth.Role)
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(7) : DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return RedirectToAction("Bienvenue", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.MotDePasse != model.ConfirmationMotDePasse)
        {
            ModelState.AddModelError("ConfirmationMotDePasse", "Les mots de passe ne correspondent pas.");
            return View(model);
        }

        var employe = _employeRepository.GetByMatricule(model.Matricule.Trim());
        if (employe == null)
        {
            ModelState.AddModelError("Matricule", "Ce matricule n'existe pas.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.Email) &&
            !string.IsNullOrWhiteSpace(employe.Email) &&
            !string.Equals(model.Email.Trim(), employe.Email, StringComparison.OrdinalIgnoreCase))
        {
            ModelState.AddModelError("Email", "L'email ne correspond pas a l'employe.");
            return View(model);
        }

        if (_authentificationRepository.ExistsForEmploye(employe.Id))
        {
            ModelState.AddModelError(string.Empty, "Un compte existe deja pour cet employe.");
            return View(model);
        }

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.MotDePasse);
        var roleId = _authentificationRepository.GetOrCreateRole("EMPLOYE");
        _authentificationRepository.Create(employe.Id, passwordHash, roleId);

        TempData["Info"] = "Inscription reussie. Vous pouvez maintenant vous connecter.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (string.IsNullOrWhiteSpace(model.Matricule) && string.IsNullOrWhiteSpace(model.Email))
        {
            ModelState.AddModelError(string.Empty, "Saisissez un matricule ou un email.");
            return View(model);
        }

        var employeId = _authentificationRepository.FindEmployeIdByMatriculeOrEmail(model.Matricule, model.Email);
        if (employeId.HasValue)
        {
            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            _authentificationRepository.SaveResetToken(employeId.Value, token, DateTime.UtcNow.AddHours(1));
            TempData["ResetToken"] = token;
        }

        TempData["Info"] = "Si un compte correspond, un token de reinitialisation a ete genere.";
        return RedirectToAction("Login");
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
