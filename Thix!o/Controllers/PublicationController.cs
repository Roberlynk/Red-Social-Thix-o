using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Thix_o.ViewModels;

namespace Thix_o.Controllers
{
    public class PublicationController : Controller
    {
        private readonly ThixioContext _context;

        public PublicationController(ThixioContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            var id = await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == session);
      
            var listEntity = await _context.Publicacion.Where(p => p.IdUsuario == id.IdUsuario).OrderByDescending(p => p.FechaHora).ToListAsync();

            List<PublicacionViewModel> vms = new List<PublicacionViewModel>();

            listEntity.ForEach(item =>
            {
                vms.Add(new PublicacionViewModel
                {
                    IdPublicacion = item.IdPublicacion,
                    Contenido = item.Contenido,
                    FechaHora = item.FechaHora
                });

            });

            List<Comentario> listComentario = new List<Comentario>();

            List<Usuario> listUsuario = new List<Usuario>();

            listComentario = await _context.Comentario.ToListAsync();

            listComentario.GroupBy(c => c.FechaHora);

            listUsuario = await _context.Usuario.ToListAsync();

            ViewBag.Usuarios = listUsuario;

            ViewBag.NombreP = id.Nombre + " " + id.Apellido;

            ViewBag.Comentarios = listComentario;


            return View(vms);
        }

        public async Task<IActionResult> Edit(int? id)
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

            var publicacion = await _context.Publicacion.FindAsync(id);
            if (publicacion == null)
            {
                return NotFound();
            }
            var vm = new PublicacionViewModel
            {
                Contenido = publicacion.Contenido,
                IdPublicacion = publicacion.IdPublicacion
            };
            return View(vm);
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

            var publicacion = await _context.Publicacion.FirstOrDefaultAsync(m => m.IdPublicacion == id);

            if (publicacion == null)
            {
                return NotFound();
            }

            var vm = new PublicacionViewModel
            {
                Contenido = publicacion.Contenido,
                IdPublicacion = publicacion.IdPublicacion
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Publicacion(string contenido)
        {
            var session = HttpContext.Session.GetString("UserName");

            var id = await _context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == session);

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            if (ModelState.IsValid && !string.IsNullOrEmpty(contenido))
            {
                var publicacionEntity = new Publicacion();

                publicacionEntity.Contenido = contenido;
                publicacionEntity.IdUsuario = id.IdUsuario;

                _context.Add(publicacionEntity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
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
                var usuarioEntity = new Comentario();

                usuarioEntity.Contenido = contenido;
                usuarioEntity.IdUsuario = id.IdUsuario;
                usuarioEntity.IdPublicacion = IdPublicacion;

                _context.Add(usuarioEntity);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PublicacionViewModel vm)
        {
            var session = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            if (id != vm.IdPublicacion)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var TipoEntity = new Publicacion
                    {
                        IdPublicacion = vm.IdPublicacion,
                        Contenido = vm.Contenido
                    };
                    _context.Update(TipoEntity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)    
                {
                    if (!PublicacionExists(vm.IdPublicacion))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
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

            var publicacion = await _context.Publicacion.FindAsync(id);

            var ids = await _context.Comentario.Where(u => u.IdPublicacion == id).ToListAsync();

            foreach (var item in ids)
            {
                _context.Comentario.Remove(item);

                await _context.SaveChangesAsync();
            }

            _context.Publicacion.Remove(publicacion);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        private bool PublicacionExists(int id)
        {
            return _context.Publicacion.Any(e => e.IdPublicacion == id);
        }


    }
}