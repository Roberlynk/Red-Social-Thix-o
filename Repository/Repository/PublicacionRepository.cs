using AutoMapper;
using Database.Model;
using Email;
using Microsoft.EntityFrameworkCore;
using Repository.RepositoryBase;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Repository.Repository
{
    public class PublicacionRepository : RepositoryBase<Publicacion, ThixioContext>
    {
        private readonly UsuarioRepository _usuarioRepository;
        private readonly ComentarioRepository _comentarioRepository;
        private readonly AmigoRepository _amigoRepository;
        public PublicacionRepository(ThixioContext context) : base(context)
        {
            _usuarioRepository = new UsuarioRepository(context);
            _comentarioRepository = new ComentarioRepository(context);
            _amigoRepository = new AmigoRepository(context);
        }

        public async Task<List<PublicacionViewModel>> GetPublicacionVmOr1(Usuario user)
        {
            List<PublicacionViewModel> vms = new List<PublicacionViewModel>();

            var listAmigos = await _amigoRepository.GetAmigosByUsuario(user.IdUsuario);

            var Amigos = await _usuarioRepository.GetUsuarioByAmigo(user);

            foreach (var amigo in listAmigos)
            {
                if (amigo.IdUsuario == user.IdUsuario)
                {
                    var listEntity = await _context.Publicacion.Where(p => p.IdUsuario == amigo.IdAmigo).ToListAsync();
                    foreach (var p in listEntity)
                    {
                        var vm = Mapper.Map<PublicacionViewModel>(p);
                        vm.Comentarios = await _comentarioRepository.ListComentarioByPublicacion(p.IdPublicacion);
                        vm.NombreUsuario = await _usuarioRepository.NombreById(p.IdUsuario);
                        vms.Add(vm);
                    }
                }
                else if (amigo.IdAmigo == user.IdUsuario)
                {
                    var listEntity = await _context.Publicacion.Where(p => p.IdUsuario == amigo.IdUsuario).ToListAsync();
                    foreach (var p in listEntity)
                    {
                        var vm = Mapper.Map<PublicacionViewModel>(p);
                        vm.Comentarios = await _comentarioRepository.ListComentarioByPublicacion(p.IdPublicacion);
                        vm.NombreUsuario = await _usuarioRepository.NombreById(p.IdUsuario);
                        vms.Add(vm);
                    }
                }
            }

            vms.OrderByDescending(p => p.FechaHora).ToList();

            return vms;

        }

        public async Task<List<PublicacionViewModel>> GetPublicacionVmOr2(Usuario user)
        {
            List<PublicacionViewModel> vms = new List<PublicacionViewModel>();

            var listEntity = await base._context.Publicacion.Where(p => p.IdUsuario == user.IdUsuario).OrderByDescending(p => p.FechaHora).ToListAsync();

            foreach (var publicacion in listEntity)
            {
                var vm = Mapper.Map<PublicacionViewModel>(publicacion);
                vm.Comentarios = await _comentarioRepository.ListComentarioByPublicacion(publicacion.IdPublicacion);
                vm.NombreUsuario = await _usuarioRepository.NombreById(publicacion.IdUsuario);
                vms.Add(vm);
            }

            return vms;

        }
        public async Task<PublicacionViewModel> GetPublicacionById(int? id)
        {
            var publicacion = await base._context.Publicacion.FirstOrDefaultAsync(u => u.IdPublicacion == id);

            var vm = Mapper.Map<PublicacionViewModel>(publicacion);

            return vm;

        }

        public Publicacion CreatePublicacion(string contenido, Usuario user)
        {
            var publicacionEntity = new Publicacion
            {
                Contenido = contenido,
                IdUsuario = user.IdUsuario
            };

            return publicacionEntity;
        }

        public Publicacion UpdatePublicacion(PublicacionViewModel vm)
        {
            var publicacionEntity = Mapper.Map<Publicacion>(vm);

            return publicacionEntity;
        }


    }
}
