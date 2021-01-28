using Database.Model;
using Repository.RepositoryBase;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;
using AutoMapper;
using Email;

namespace Repository.Repository
{
    public class ComentarioRepository : RepositoryBase<Comentario, ThixioContext>
    {
        private readonly UsuarioRepository _usuarioRepository;
        public ComentarioRepository(ThixioContext context) : base(context)
        {
            _usuarioRepository = new UsuarioRepository(context);
        }
        public Comentario CreateComentario(int idUser, int idPublicacion, string contenido)
        {
            var comentarioEntity = new Comentario
            {
                Contenido = contenido,
                IdUsuario = idUser,
                IdPublicacion = idPublicacion
            };

            return comentarioEntity;
        }

        public async Task<List<ComentarioViewModel>> ListComentarioByPublicacion(int id)
        {
            var listComentario = await base._context.Comentario.Where(u => u.IdPublicacion == id).ToListAsync();

            List<ComentarioViewModel> listComentarioPublicacion = new List<ComentarioViewModel>();

            foreach (var a in listComentario)
            {
                var vm = Mapper.Map<ComentarioViewModel>(a);
                vm.NombreUsuario = await _usuarioRepository.NombreById(a.IdUsuario);
                listComentarioPublicacion.Add(vm);
            }

            listComentarioPublicacion.OrderByDescending(p => p.FechaHora).ToList();

            return listComentarioPublicacion;
        }
        public async Task<List<Comentario>> GetComentarioByPublicacion(int id)
        {
            var comentarioList = await base._context.Comentario.Where(u => u.IdPublicacion == id).ToListAsync();

            return comentarioList;
        }


    }
}
