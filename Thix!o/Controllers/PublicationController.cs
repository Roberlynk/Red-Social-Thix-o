using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Database.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repository.Repository;
using ViewModels;

namespace Thix_o.Controllers
{
    public class PublicationController : Controller
    {
        private readonly PublicacionRepository _publicacionRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly AmigoRepository _amigoRepository;
        private readonly ComentarioRepository _comentarioRepository;
        public PublicationController(PublicacionRepository publicacionRepository, UsuarioRepository usuarioRepository, AmigoRepository amigoRepository, ComentarioRepository comentarioRepository)
        {
            this._publicacionRepository = publicacionRepository;
            this._usuarioRepository = usuarioRepository;
            this._amigoRepository = amigoRepository;
            this._comentarioRepository = comentarioRepository;
        }
        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            var user = await _usuarioRepository.UserExist(session);

            var vmOrdenado = await _publicacionRepository.GetPublicacionVmOr2(user);

            return View(vmOrdenado);
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

            var publicacion = await _publicacionRepository.GetPublicacionById(id);

            if (publicacion == null)
            {
                return NotFound();
            }

            return View(publicacion);
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

            var publicacion = await _publicacionRepository.GetPublicacionById(id);

            if (publicacion == null)
            {
                return NotFound();
            }

            return View(publicacion);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction("Index", "Inicio");
        }

        public async Task<IActionResult> Publicacion(string contenido)
        {
            var session = HttpContext.Session.GetString("UserName");

            var id = await _usuarioRepository.UserExist(session);

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            if (ModelState.IsValid && !string.IsNullOrEmpty(contenido))
            {
                var publicacionEntity = _publicacionRepository.CreatePublicacion(contenido, id);

                await _publicacionRepository.Add(publicacionEntity);
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Comentario(int IdPublicacion, string contenido)
        {
            var session = HttpContext.Session.GetString("UserName");

            var id = await _usuarioRepository.UserExist(session);

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            if (ModelState.IsValid && !string.IsNullOrEmpty(contenido))
            {

                var comentarioEntity = _comentarioRepository.CreateComentario(id.IdUsuario, IdPublicacion, contenido);

                await _comentarioRepository.Add(comentarioEntity);
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
                    var publicacionEntity = _publicacionRepository.UpdatePublicacion(vm);

                    await _publicacionRepository.Update(publicacionEntity);
                }
                catch (DbUpdateConcurrencyException)    
                {
                    var publicacionExists = _publicacionRepository.GetPublicacionById(vm.IdPublicacion);

                    if (publicacionExists == null)
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

            var publicacion = await _publicacionRepository.GetById(id);

            var ids = await _comentarioRepository.GetComentarioByPublicacion(id);

            foreach (var item in ids)
            {
                await _comentarioRepository.Delete(item.IdComentario);

            }

            await _publicacionRepository.Delete(publicacion.IdPublicacion);

            return RedirectToAction(nameof(Index));
        }


    }
}