using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Inmobiliaria.Models;
using Inmobiliaria.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Threading.Tasks;
using Inmobiliaria.Services;
namespace Inmobiliaria.Controllers;


public class UsuarioController : Controller
{
  private readonly IRepositorioUsuario repositorio;
  private readonly PasswordHasher<Usuario> passwordHasher;
  private readonly IWebHostEnvironment environment;
  private readonly IFileService _fileService;
  private readonly IConfiguration config;

  private const string RUTA_CARPETA_AVATAR = "Uploads/User/Avatar";

  public UsuarioController(IRepositorioUsuario repositorio, IFileService fileService, IConfiguration config, IWebHostEnvironment environment)
  {
    this.repositorio = repositorio;
    this.config = config;
    this.environment = environment;
    this._fileService = fileService;
    this.passwordHasher = new PasswordHasher<Usuario>();
  }


  [HttpGet]
  [Authorize(Roles = "Administrador")]
  public IActionResult Registrar()
  {
    Usuario usuario = new Usuario();
    string? usuarioJson = TempData["Usuario"] as string;
    if (!string.IsNullOrEmpty(usuarioJson))
    {
      usuario = JsonSerializer.Deserialize<Usuario>(usuarioJson) ?? usuario;
      usuario.Password = "";
    }
    return View("Gestion", usuario);
  }

  [HttpPost]
  [Authorize(Roles = "Administrador")]
  public async Task<IActionResult> Registrar(Usuario user, string ConfirmPassword)
  {
    if (!ModelState.IsValid)
      return View("Gestion", user);
    if (user.Password != ConfirmPassword)
    {
      ModelState.AddModelError("ConfirmPassword", "Las contraseñas no coinciden.");
      return View("Gestion", user);
    }
    try
    {
      if (repositorio.ObtenerPorEmail(user.Email).IdUsuario > 0)
      {
        TempData["Error"] = "El email ya está registrado";
        return View("Gestion", user);
      }

      user.Password = passwordHasher.HashPassword(user, user.Password);

      if (user.AvatarFile != null && user.AvatarFile.Length > 0)
      {
        string nombreArchivo = await _fileService.GuardarArchivoAsync(
          user.AvatarFile,
          RUTA_CARPETA_AVATAR
        );
        user.AvatarURL = $"/{RUTA_CARPETA_AVATAR}/{nombreArchivo}";
      }

      var idUser = repositorio.Registrar(user);
      if (idUser <= 0)
      {
        TempData["Error"] = "Error al intentar registrar el Usuario";
        return View("Gestion", user);
      }
      TempData["Success"] = "Usuario creado correctamente.";
      return RedirectToAction("Ver", new { IdUsuario = idUser });
    }
    catch (Exception)
    {
      return RedirectToAction("Registrar");
    }
  }

  [HttpPost]
  [Authorize(Roles = "Administrador")]
  public async Task<IActionResult> ModificarUsuario(Usuario user, string? NewPassword, string? ConfirmPasswordModify)
  {
    ModelState.Remove(nameof(user.Password));
    if (!ModelState.IsValid)
    {
      TempData["Error"] = "Los datos ingresados no son válidos.";
      return RedirectToAction("Ver", new { IdUsuario = user.IdUsuario });
    }
    try
    {
      Usuario usuarioOriginal = repositorio.ObtenerPorId(user.IdUsuario);
      if (usuarioOriginal.IdUsuario <= 0)
      {
        TempData["Error"] = "Usuario no encontrado";
        return RedirectToAction("Listar");
      }

      Usuario usuarioPorEmail = repositorio.ObtenerPorEmail(user.Email);
      if (usuarioPorEmail.IdUsuario > 0 && usuarioPorEmail.IdUsuario != user.IdUsuario)
      {
        TempData["Error"] = "El email ya está registrado por otro usuario.";
        return RedirectToAction("Ver", new { IdUsuario = user.IdUsuario });
      }

      usuarioOriginal.Nombre = user.Nombre;
      usuarioOriginal.Apellido = user.Apellido;
      usuarioOriginal.Email = user.Email;
      usuarioOriginal.Rol = user.Rol;
      usuarioOriginal.Genero = user.Genero;

      var idUsuarioLogueadoClaim = User.FindFirstValue("IdUsuario");
      if (int.TryParse(idUsuarioLogueadoClaim, out int idUsuarioLogueado) && usuarioOriginal.IdUsuario == idUsuarioLogueado)
      {
        // Si es usuario loguado le obliga a estar activo por si intnta cambiar el estado por el formulario
        usuarioOriginal.Estado = 1;
      }
      else
      {
        usuarioOriginal.Estado = user.Estado;
      }

      if (!string.IsNullOrEmpty(NewPassword))
      {
        if (NewPassword.Length < 8)
        {
          TempData["Error"] = "La nueva contraseña debe tener un mínimo de 8 caracteres.";
          return RedirectToAction("Ver", new { IdUsuario = user.IdUsuario });
        }
        if (NewPassword != ConfirmPasswordModify)
        {
          TempData["Error"] = "Las nuevas contraseñas no coinciden.";
          return RedirectToAction("Ver", new { IdUsuario = user.IdUsuario });
        }
        usuarioOriginal.Password = passwordHasher.HashPassword(usuarioOriginal, NewPassword);
      }

      if (user.AvatarFile != null && user.AvatarFile.Length > 0)
      {
        if (!string.IsNullOrEmpty(usuarioOriginal.AvatarURL))
        {
          if (!usuarioOriginal.AvatarURL.Contains("AvatarMasculino.png") && !usuarioOriginal.AvatarURL.Contains("AvatarFemenino.png"))
          {
            var nombreArchivoAnterior = usuarioOriginal.AvatarURL.Split('/').Last();
            _fileService.BorrarArchivo(nombreArchivoAnterior, RUTA_CARPETA_AVATAR);
          }
        }
        string nombreArchivo = await _fileService.GuardarArchivoAsync(user.AvatarFile, RUTA_CARPETA_AVATAR);
        usuarioOriginal.AvatarURL = $"/{RUTA_CARPETA_AVATAR}/{nombreArchivo}";
      }
      else if (user.AvatarURL == "-1") // Si se marcó para eliminar
      {
        if (!string.IsNullOrEmpty(usuarioOriginal.AvatarURL))
        {
          if (!usuarioOriginal.AvatarURL.Contains("AvatarMasculino.png") && !usuarioOriginal.AvatarURL.Contains("AvatarFemenino.png"))
          {
            var nombreArchivoAnterior = usuarioOriginal.AvatarURL.Split('/').Last();
            _fileService.BorrarArchivo(nombreArchivoAnterior, RUTA_CARPETA_AVATAR);
          }
        }
        usuarioOriginal.AvatarURL = "";
      }
      if (repositorio.Modificar(usuarioOriginal) <= 0)
      {
        TempData["Error"] = "Error al intentar modificar el Usuario";
      return RedirectToAction("Ver", new { IdUsuario = user.IdUsuario });
      }

      if (usuarioOriginal.IdUsuario == idUsuarioLogueado)
      {
          var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
          var authProperties = authResult.Properties;
          var claims = new List<Claim>
          {
              new Claim(ClaimTypes.Name, usuarioOriginal.Email),
              new Claim(ClaimTypes.Role, usuarioOriginal.Rol == 1 ? "Administrador" : "Empleado"),
              new Claim("IdUsuario", usuarioOriginal.IdUsuario.ToString()),
              new Claim("AvatarURL", !string.IsNullOrEmpty(usuarioOriginal.AvatarURL) ? usuarioOriginal.AvatarURL : GetDefaultAvatar(usuarioOriginal)),
              new Claim("Nombre", usuarioOriginal.Nombre +" "+ usuarioOriginal.Apellido)
          };
          var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

          await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
      }

      TempData["Success"] = "Usuario modificado correctamente.";
      return RedirectToAction("Ver", new { IdUsuario = user.IdUsuario });
    }
    catch (Exception)
    {
      TempData["Error"] = "Ocurrió un error inesperado al modificar el usuario.";
      return RedirectToAction("Ver", new { IdUsuario = user.IdUsuario });
    }
  }


  [HttpGet]
  [Authorize(Roles = "Administrador")]
  public IActionResult Ver(int IdUsuario)
  {
    try
    {
      var idUsuarioLogueado = User.FindFirstValue("IdUsuario");
      if (idUsuarioLogueado != null)
      {
        ViewBag.IdUsuarioLogueado = int.Parse(idUsuarioLogueado);
      }
      Usuario usuario = repositorio.ObtenerPorId(IdUsuario);
      usuario.AvatarURL = !string.IsNullOrEmpty(usuario.AvatarURL) ? usuario.AvatarURL : GetDefaultAvatar(usuario);

      if (usuario.IdUsuario <= 0)
      {
        TempData["Error"] = "No se encontró el Usuario";
        return RedirectToAction("Index", "Home");
      }
      return View("Gestion", usuario);
    }
    catch (Exception)
    {
      TempData["Error"] = "Error al intentar obtener el Usuario";
      return RedirectToAction("Index", "Home");
    }
  }

  [HttpPost]
  public IActionResult Login(LoginViewModel user)
  {
    if (!ModelState.IsValid)
      return RedirectToAction("Index", "Home");

    Usuario usuario = repositorio.ObtenerPorEmail(user.Email);
    if (usuario.IdUsuario <= 0 || usuario.Estado != 1)
    {
      TempData["Error"] = "Credenciales inválidas";
      return RedirectToAction("Index", "Home");
    }

    var result = passwordHasher.VerifyHashedPassword(usuario, usuario.Password, user.Password);
    if (result == PasswordVerificationResult.Failed)
    {
      TempData["Error"] = "Credenciales inválidas";
      return RedirectToAction("Index", "Home");
    }

    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim(ClaimTypes.Role, usuario.Rol == 1 ? "Administrador" : usuario.Rol == 2 ? "Empleado" : "Desconcocido"),
                new Claim("IdUsuario", usuario.IdUsuario.ToString()),
                new Claim("AvatarURL", !string.IsNullOrEmpty(usuario.AvatarURL) ? usuario.AvatarURL : GetDefaultAvatar(usuario)),
                new Claim("Nombre", usuario.Nombre +" "+ usuario.Apellido)
            };

    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var authProperties = new AuthenticationProperties
    {
      IsPersistent = true,
      ExpiresUtc = DateTime.UtcNow.AddHours(1)
    };
    HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(claimsIdentity),
        authProperties
    );

    return RedirectToAction("Index", "Home");
  }

  [HttpGet]
  [Authorize(Roles = "Administrador,Empleado")]
  public IActionResult Perfil()
  {
    var email = User.FindFirstValue(ClaimTypes.Name);
    if (string.IsNullOrEmpty(email))
    {
      return Unauthorized("No se pudo identificar el email del usuario.");
    }

    var usuario = repositorio.ObtenerPorEmail(email);
    if (usuario.IdUsuario <= 0)
    {
      return NotFound();
    }

    var data = new
    {
      IdUsuario = usuario.IdUsuario,
      Nombre = usuario.Nombre,
      Apellido = usuario.Apellido,
      Email = usuario.Email,
      Genero = usuario.Genero,
      AvatarURL = !string.IsNullOrEmpty(usuario.AvatarURL) ? usuario.AvatarURL : GetDefaultAvatar(usuario)
    };
    var options = new JsonSerializerOptions
    {
      PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    return Json(data, options);
  }

  [HttpPost]
  [Authorize(Roles = "Administrador,Empleado")]
  [ValidateAntiForgeryToken]
  public async Task<IActionResult> EditarPerfil([FromForm] Usuario usuario)
  {
    var idUsuarioClaim = User.FindFirst("IdUsuario");
    if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuarioLogueado))
    {
      return Unauthorized(new { message = "Sesión inválida. No se pudo identificar al usuario." });
    }

    try
    {
      var usuarioOriginal = repositorio.ObtenerPorId(idUsuarioLogueado);
      if (usuarioOriginal.IdUsuario <= 0)
      {
        return NotFound(new { message = "Usuario no encontrado en la base de datos." });
      }

      var usuarioEmail = repositorio.ObtenerPorEmail(usuario.Email);
      if (usuarioEmail.IdUsuario > 0 && usuarioEmail.IdUsuario != usuarioOriginal.IdUsuario)
      {
        return BadRequest(new { message = "Email ya registrado." });
      }

      if (usuario.AvatarFile != null && usuario.AvatarFile.Length > 0)
      {

        string nombreArchivo = await _fileService.GuardarArchivoAsync(
          usuario.AvatarFile,
          RUTA_CARPETA_AVATAR
        );

        if (!string.IsNullOrEmpty(usuarioOriginal.AvatarURL))
        {
          var nombreArchivoAnterior = usuarioOriginal.AvatarURL.Split('/').Last();
          _fileService.BorrarArchivo(nombreArchivoAnterior, RUTA_CARPETA_AVATAR);
        }

        usuarioOriginal.AvatarURL = $"/{RUTA_CARPETA_AVATAR}/{nombreArchivo}";
      }
      else if (usuario.AvatarURL == "-1")
      {
        if (!string.IsNullOrEmpty(usuarioOriginal.AvatarURL))
        {
          if (!usuarioOriginal.AvatarURL.Contains("AvatarMasculino.png") && !usuarioOriginal.AvatarURL.Contains("AvatarFemenino.png"))
          {
            var nombreArchivoAnterior = usuarioOriginal.AvatarURL.Split('/').Last();
            _fileService.BorrarArchivo(nombreArchivoAnterior, RUTA_CARPETA_AVATAR);
          }
        }
        usuarioOriginal.AvatarURL = "";
      }
      usuarioOriginal.Nombre = usuario.Nombre;
      usuarioOriginal.Apellido = usuario.Apellido;
      usuarioOriginal.Email = usuario.Email;
      usuarioOriginal.Genero = usuario.Genero;

      repositorio.Modificar(usuarioOriginal);

      if (usuarioOriginal.IdUsuario == idUsuarioLogueado)
      {
          var authResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
          var authProperties = authResult.Properties;

          var claims = new List<Claim>
          {
              new Claim(ClaimTypes.Name, usuarioOriginal.Email),
              new Claim(ClaimTypes.Role, usuarioOriginal.Rol == 1 ? "Administrador" : "Empleado"),
              new Claim("IdUsuario", usuarioOriginal.IdUsuario.ToString()), 
              new Claim("Nombre", usuarioOriginal.Nombre +" "+ usuarioOriginal.Apellido),
              new Claim("AvatarURL", !string.IsNullOrEmpty(usuarioOriginal.AvatarURL) ? usuarioOriginal.AvatarURL : GetDefaultAvatar(usuarioOriginal)) // Actualizar el claim del avatar
          };
          var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
          await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
      }
      // Devolvemos la URL del avatar por defecto si se eliminó el personalizado.
      return Ok(new { message = "Perfil actualizado correctamente.", newAvatarUrl = !string.IsNullOrEmpty(usuarioOriginal.AvatarURL) ? usuarioOriginal.AvatarURL : GetDefaultAvatar(usuarioOriginal) });
    }
    catch (Exception)
    {
      return StatusCode(500, new { message = "Ocurrió un error inesperado al actualizar el perfil." });
    }
  }

  [HttpPost]
  [Authorize(Roles = "Administrador,Empleado")]
  [ValidateAntiForgeryToken]
  public IActionResult CambiarPassword(string OldPassword, string NewPassword, string ConfirmPassword)
  {
    if (!ModelState.IsValid)
    {
      return BadRequest(new { message = "La información proporcionada es incompleta." });
    }
    if (NewPassword != ConfirmPassword)
    {
      return BadRequest(new { message = "Las nuevas contraseñas no coinciden." });
    }

    var idUsuarioClaim = User.FindFirst("IdUsuario");
    if (idUsuarioClaim == null || !int.TryParse(idUsuarioClaim.Value, out int idUsuarioLogueado))
    {
      return Unauthorized(new { message = "Sesión inválida. No se pudo identificar al usuario." });
    }

    try
    {
      var usuario = repositorio.ObtenerPorId(idUsuarioLogueado);
      var verificationResult = passwordHasher.VerifyHashedPassword(usuario, usuario.Password, OldPassword);

      if (verificationResult == PasswordVerificationResult.Failed)
      {
        return BadRequest(new { message = "La contraseña actual es incorrecta." });
      }

      usuario.Password = passwordHasher.HashPassword(usuario, NewPassword);
      if (repositorio.Modificar(usuario)<=0)
      { 
        return StatusCode(500, new { message = "No se logro cambiar la contraseña." });
      }

      return Ok(new { message = "Contraseña actualizada correctamente." });
    }
    catch (Exception)
    {
      return StatusCode(500, new { message = "Ocurrió un error inesperado al cambiar la contraseña." });
    }
  }

  [HttpGet]
  [Authorize(Roles = "Administrador")]
  public IActionResult Listar(string? idUsuario, string? nombre, string? apellido, string? email, string? rol, string? estado, int PaginaActual = 1)
  {
    int registrosPorPagina = 9;
    int total = 0;
    int offset = (PaginaActual - 1) * registrosPorPagina;
    int limite = registrosPorPagina;
    List<Usuario> lista;
    try
    {
      total = repositorio.CantidadFiltro(idUsuario, nombre, apellido, email, rol, estado);
      limite = Math.Min(registrosPorPagina, total - offset);
      lista = repositorio.Filtrar(idUsuario, nombre, apellido, email, rol, estado, limite, offset);

      int totalPaginas = (int)Math.Ceiling((double)total / registrosPorPagina);

      ViewBag.PaginaActual = PaginaActual;
      ViewBag.TotalPaginas = totalPaginas;
      ViewBag.IdUsuario = idUsuario;
      ViewBag.Nombre = nombre;
      ViewBag.Apellido = apellido;
      ViewBag.Email = email;
      ViewBag.Rol = rol;
      ViewBag.Estado = estado;

      return View(lista);
    }
    catch (Exception)
    {
      ViewBag.PaginaActual = 1;
      ViewBag.TotalPaginas = 1;
      return View(new List<Usuario>());
    }
  }


  [HttpPost]
  [Authorize(Roles = "Administrador,Empleado")]
  [ValidateAntiForgeryToken]

  public async Task<IActionResult> Logout()
  {
    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return RedirectToAction("Index", "Home");
  }

  private string GetDefaultAvatar(Usuario usuario)
  {
    if (usuario.Genero == 2)
    {
      return "/images/user/AvatarFemenino.png";
    }

    return "/images/user/AvatarMasculino.png";
  }
}
