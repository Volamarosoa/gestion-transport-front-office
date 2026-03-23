using Microsoft.AspNetCore.Mvc;
using GestionTransport.FrontOffice.Models.Auth;

namespace GestionTransport.FrontOffice.Controllers;

public class AuthController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View(new LoginViewModel());
    }

    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // TODO: implémenter l'authentification réelle (hash, stockage)
        TempData["Info"] = "Authentification simulée.";
        return RedirectToAction("Bienvenue", "Home");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View(new RegisterViewModel());
    }

    [HttpPost]
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

        // TODO: sauvegarder l'utilisateur et sécuriser le mot de passe
        TempData["Info"] = "Inscription simulée.";
        return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View(new ForgotPasswordViewModel());
    }

    [HttpPost]
    public IActionResult ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // TODO: déclencher un flux de réinitialisation réel (email/token)
        TempData["Info"] = "Lien de réinitialisation simulé envoyé.";
        return RedirectToAction("Login");
    }
}
