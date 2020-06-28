using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Database.Model;
using Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thix_o.ViewModels;

namespace Thix_o.Controllers
{
    public class AmigoController : Controller
    {
        private readonly ThixioContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;
        public AmigoController(ThixioContext context, IMapper mapper, IEmailSender emailSender)
        {
            _context = context;
            this._mapper = mapper;
            this._emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            var id = await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == session);

            var amigos = await _context.Amigo.Where(p => p.IdUsuario == id.IdUsuario || p.IdAmigo == id.IdUsuario).ToListAsync();

            List<PublicacionViewModel> publicacion = new List<PublicacionViewModel>();

            List<Usuario> listAmigo = new List<Usuario>();

            foreach (var amigo in amigos)
            {
                if (amigo.IdUsuario == id.IdUsuario)
                {
                    var listEntity = await _context.Publicacion.Where(p => p.IdUsuario == amigo.IdAmigo).ToListAsync();
                    foreach (var p in listEntity)
                    {
                        var vm = Mapper.Map<PublicacionViewModel>(p);
                        publicacion.Add(vm);
                    }
                    var listEntity2 = await _context.Usuario.Where(p => p.IdUsuario == amigo.IdAmigo).ToListAsync();
                    foreach (var a in listEntity2)
                    {
                        listAmigo.Add(a);
                    }
                }
                else if (amigo.IdAmigo == id.IdUsuario)
                {
                    var listEntity = await _context.Publicacion.Where(p => p.IdUsuario == amigo.IdUsuario).ToListAsync();
                    foreach (var p in listEntity)
                    {
                        var vm = Mapper.Map<PublicacionViewModel>(p);
                        publicacion.Add(vm);
                    }
                    var listEntity2 = await _context.Usuario.Where(p => p.IdUsuario == amigo.IdUsuario).ToListAsync();
                    foreach (var a in listEntity2)
                    {
                        listAmigo.Add(a);
                    }
                }
            }

            var ordenado = publicacion.OrderByDescending(p => p.FechaHora).ToList();

            var listComentario = await _context.Comentario.ToListAsync();

            var listUsuario = await _context.Usuario.ToListAsync();

            ViewBag.Publicaciones = ordenado;

            ViewBag.Usuarios = listUsuario;

            ViewBag.Comentarios = listComentario;

            ViewBag.Amigos = listAmigo;


            return View(ordenado);
        }

        public IActionResult AgregarAmigo()
        {
            var session = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }
            return View();

        }
        public async Task<IActionResult> Comentario(int IdPublicacion, string contenido)
        {
            var session = HttpContext.Session.GetString("UserName");

            var id = await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == session);

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            if (ModelState.IsValid && !string.IsNullOrEmpty(contenido))
            {
                var comentarioEntity = new Comentario();

                comentarioEntity.Contenido = contenido;
                comentarioEntity.IdUsuario = id.IdUsuario;
                comentarioEntity.IdPublicacion = IdPublicacion;

                _context.Add(comentarioEntity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(int? id)
        {
            var session = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FirstOrDefaultAsync(m => m.IdUsuario == id);

            if (usuario == null)
            {
                return NotFound();
            }
            var vm = Mapper.Map<RegisterViewModel>(usuario);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAmigo(RegisterViewModel vm)
        {
            var session = HttpContext.Session.GetString("UserName");

            var id = await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == session);

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            var user = await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == vm.NombreUsuario);

            if (user == null)
            {
                ModelState.AddModelError("UserExist", "El usuario no existe");
            }
            else
            {
                var amigo = await _context.Amigo.FirstOrDefaultAsync(u => u.IdAmigo == user.IdUsuario && u.IdUsuario == id.IdUsuario);

                var amigo2 = await _context.Amigo.FirstOrDefaultAsync(u => u.IdUsuario == user.IdUsuario && u.IdAmigo == id.IdUsuario);

                if (amigo != null || amigo2 != null)
                {
                    ModelState.AddModelError("UserA", "El usuario ya es su amigo");
                }
                else
                {
                    var amigoEntity = new Amigo
                    {
                        IdUsuario = user.IdUsuario,
                        IdAmigo = id.IdUsuario
                    };

                    _context.Add(amigoEntity);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }
            return View(vm);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            var usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == session);

            var amigo = await _context.Amigo.FirstOrDefaultAsync(u => u.IdAmigo == usuario.IdUsuario && u.IdUsuario == id);

            var amigo2 = await _context.Amigo.FirstOrDefaultAsync(u => u.IdUsuario == usuario.IdUsuario && u.IdAmigo == id);

            if (amigo != null)
            {
                _context.Amigo.Remove(amigo);

                await _context.SaveChangesAsync();
            }
            else if (amigo2 != null)
            {
                _context.Amigo.Remove(amigo2);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}