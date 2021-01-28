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
    public class AmigoController : Controller
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly AmigoRepository _amigoRepository;
        private readonly ComentarioRepository _comentarioRepository;
        private readonly PublicacionRepository _publicacionRepository;
        public AmigoController(UsuarioRepository usuarioRepository, AmigoRepository amigoRepository, ComentarioRepository comentarioRepository, PublicacionRepository publicacionRepository)
        {
            this._usuarioRepository = usuarioRepository;
            this._amigoRepository = amigoRepository;
            this._comentarioRepository = comentarioRepository;
            this._publicacionRepository = publicacionRepository;
        }

        public async Task<IActionResult> Index()
        {
            var session = HttpContext.Session.GetString("UserName");

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            var user = await _usuarioRepository.UserExist(session);

            var vmOrdenado = await _publicacionRepository.GetPublicacionVmOr1(user);
            
            ViewBag.Amigos = await _usuarioRepository.GetUsuarioByAmigo(user);

            return View(vmOrdenado);
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

            var user = await _usuarioRepository.UserExist(session);

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            if (ModelState.IsValid && !string.IsNullOrEmpty(contenido))
            {
                var comentarioEntity = _comentarioRepository.CreateComentario(user.IdUsuario, IdPublicacion, contenido);

                await _comentarioRepository.Add(comentarioEntity);
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

            var vm = await _usuarioRepository.GetForDelete(id);

            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarAmigo(RegisterViewModel vm)
        {
            var session = HttpContext.Session.GetString("UserName");

            var id = await _usuarioRepository.UserExist(session);

            if (string.IsNullOrEmpty(session))
            {
                return RedirectToAction("Index", "Inicio");
            }

            var user = await _usuarioRepository.UserExist(vm.NombreUsuario);

            if (user == null)
            {
                ModelState.AddModelError("UserExist", "El usuario no existe");
            }
            else
            {
                var amigo = await _amigoRepository.FindAmigos(user, id);

                if (amigo != null)
                {
                    ModelState.AddModelError("UserA", "El usuario ya es su amigo");
                }
                else
                {

                    var amigoEntity = _amigoRepository.CreateAmigo(user, id);

                    await _amigoRepository.Add(amigoEntity);

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

            var user = await _usuarioRepository.UserExist(session);

            var amigo = await _amigoRepository.RemoveAmigo(user, id);

            if (amigo != null)
            {
                await _amigoRepository.Deletear(amigo);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}