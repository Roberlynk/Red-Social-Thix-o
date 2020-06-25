using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Database.Model;
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

        public InicioController(ThixioContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var session = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Usuario");
            }

            return View();
        }
        public IActionResult Login()
        {
            var session = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Usuario");
            }

            return View();
        }
        public IActionResult Register()
        {
            var session = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Usuario");
            }

            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            var session = HttpContext.Session.GetString("UserName");

            if (!string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Usuario");
            }

            if (ModelState.IsValid)
            {
                var passwordEncrypted = PasswordEncryption(vm.Contraseña);

                var user = await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == vm.NombreUsuario && u.Contraseña == passwordEncrypted);

                if (user != null)
                {
                    HttpContext.Session.SetString("UserName", vm.NombreUsuario);
                    return RedirectToAction("Index", "Usuario");
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
                return RedirectToAction("Index", "Usuario");
            }

            if (ModelState.IsValid)
            {
                var usuarioEntity = new Usuario();

                usuarioEntity.Nombre = vm.Nombre;
                usuarioEntity.Apellido = vm.Apellido;
                usuarioEntity.Telefono = vm.Telefono;
                usuarioEntity.Correo = vm.Correo;
                usuarioEntity.NombreUsuario = vm.NombreUsuario;
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