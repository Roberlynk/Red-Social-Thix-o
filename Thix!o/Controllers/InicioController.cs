using System;
using System.Threading.Tasks;
using Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repository.Repository;
using ViewModels;

namespace Thix_o.Controllers
{
    public class InicioController : Controller
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly IEmailSender _emailSender;
        public InicioController(UsuarioRepository usuarioRepository, IEmailSender emailSender)
        {
            _usuarioRepository = usuarioRepository;
            _emailSender = emailSender;
        }

        public IActionResult Index()
        {
            var session = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Publication");
            }

            return View();
        }
        public IActionResult Login()
        {
            var session = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Publication");
            }
    
            return View();
        }
        public IActionResult Register()
        {
            var session = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Publication");
            }

            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }
        public IActionResult Restablecer()
        {
            var session = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Publication");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            var session = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Publication");
            }


            if (ModelState.IsValid)
            {
                var user = await _usuarioRepository.GetLogin(vm.NombreUsuario, vm.Contraseña);

                if (user != null)
                {
                    HttpContext.Session.SetString("UserName", vm.NombreUsuario);
                    return RedirectToAction("Index", "Publication");
                }
                else
                {
                    ModelState.AddModelError("UserOrPasswordInvalid", "El usuario o la contraseña es incorrecta");
                }
            }

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {

            var session = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Publication");
            }

            if (ModelState.IsValid)
            {
                var usuarioEntity = _usuarioRepository.GetRegister(vm);

                var user = await _usuarioRepository.UserExist(vm.NombreUsuario);

                var email = await _usuarioRepository.EmailExist(vm.Correo);

                if (user != null)
                {
                    ModelState.AddModelError("UserExist", "Este usuario ya esta en uso");
                }
                else if (email != null)
                {
                    ModelState.AddModelError("EmailExist", "Este correo ya esta en uso");
                }
                else
                {
                    await _usuarioRepository.Add(usuarioEntity);

                    return RedirectToAction("Login");
                }
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Restablecer(RegisterViewModel vm)
        {
            var session = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Publication");
            }

            var user = await _usuarioRepository.UserExist(vm.NombreUsuario);

            int longitud = 15;
            Guid miGuid = Guid.NewGuid();
            string token = miGuid.ToString().Replace("-", string.Empty).Substring(0, longitud);


            if (user == null)
            {
                ModelState.AddModelError("UserExist", "El usuario no existe");
            }
            else
            {
                var userEntity = _usuarioRepository.ResetPassword(user, token);

                await _usuarioRepository.Update(userEntity);

                var message = new Message(new string[] { user.Correo }, "Seguridad", "Su nueva contraseña es: " + token);

                await _emailSender.SendMailAsync(message);

                return RedirectToAction("Login");
            }

            return View(vm);
        }
    }
}