using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Database.Model;
using Email;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thix_o.ViewModels;

namespace Thix_o.Controllers
{
    public class InicioController : Controller
    {
        private readonly ThixioContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        public InicioController(ThixioContext context, IMapper mapper, IEmailSender emailSender)
        {
            _context = context;
            this._mapper = mapper;
            this._emailSender = emailSender;
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
                var passwordEncrypted = PasswordEncryption(vm.Contraseña);

                var user = await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == vm.NombreUsuario && u.Contraseña == passwordEncrypted);

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
                var usuarioEntity = Mapper.Map<Usuario>(vm);
                usuarioEntity.Contraseña = PasswordEncryption(vm.Contraseña);

                var user = await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == vm.NombreUsuario);

                var email = await _context.Usuario.FirstOrDefaultAsync(u => u.Correo == vm.Correo);

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
                    _context.Add(usuarioEntity);
                    await _context.SaveChangesAsync();

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

            var user = await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == vm.NombreUsuario);

            int longitud = 15;
            Guid miGuid = Guid.NewGuid();
            string token = miGuid.ToString().Replace("-", string.Empty).Substring(0, longitud);


            if (user == null)
            {
                ModelState.AddModelError("UserExist", "El usuario no existe");
            }
            else
            {
                var userEntity = Mapper.Map<Usuario>(user);
                userEntity.Contraseña = PasswordEncryption(token);

                _context.Update(userEntity);
                await _context.SaveChangesAsync();

                var message = new Message(new string[] { user.Correo }, "Seguridad", "Su nueva contraseña es: " + token);    

                await _emailSender.SendMailAsync(message);

                return RedirectToAction("Login");
            }

            return View(vm);
        }

        private string PasswordEncryption(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                StringBuilder builder = new StringBuilder();

                foreach (Byte t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}