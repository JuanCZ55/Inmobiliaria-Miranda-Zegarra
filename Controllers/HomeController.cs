using System.Diagnostics;
using Inmobiliaria.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Inmobiliaria.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }

    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Capturamos la información de la excepción
        var exceptionHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

        return View(
            new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                // Guardamos solo el mensaje del error en nuestro modelo
                ErrorMessage = exceptionHandlerFeature?.Error.Message,
            }
        );
    }

    public IActionResult Error404()
    {
        return View();
    }

    public IActionResult AccesoDenegado()
    {
        return View();
    }
}
