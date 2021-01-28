using AutoMapper;
using Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ViewModels;

namespace Thix_o.Infraestructure.AutoMapper
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            ConfigureUsuarioToLogin();
            ConfigureUsuarioToRegister();
            ConfigurePublicacion();
            ConfigureComentario();
            ConfigureUsuario();
        }
        private void ConfigureUsuarioToLogin()
        {
            CreateMap<LoginViewModel, Usuario>().ReverseMap();
        }
        private void ConfigureUsuarioToRegister()
        {
            CreateMap<RegisterViewModel, Usuario>().ReverseMap().ForMember(dest => dest.ConfimarContraseña, opt => opt.Ignore());
        }
        private void ConfigurePublicacion()
        {
            CreateMap<PublicacionViewModel, Publicacion>().ReverseMap().ForMember(dest => dest.NombreUsuario, opt => opt.Ignore()).ForMember(dest => dest.Comentarios, opt => opt.Ignore());
        }

        private void ConfigureComentario()
        {
            CreateMap<ComentarioViewModel, Comentario>().ReverseMap().ForMember(dest => dest.NombreUsuario, opt => opt.Ignore());
        }

        private void ConfigureUsuario()
        {
            CreateMap<UsuarioViewModel, Usuario>().ReverseMap();
        }
    }
}
