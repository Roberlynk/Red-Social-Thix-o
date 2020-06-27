using AutoMapper;
using Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thix_o.ViewModels;

namespace Thix_o.Infraestructure.AutoMapper
{
    public class AutoMapperConfiguration : Profile
    {
        public AutoMapperConfiguration()
        {
            ConfigureUsuario();
            ConfigurePublicacion();
        }
        private void ConfigureUsuario()
        {
            CreateMap<RegisterViewModel, Usuario>().ReverseMap().ForMember(dest => dest.ConfimarContraseña, opt => opt.Ignore());
        }
        private void ConfigurePublicacion()
        {
            CreateMap<PublicacionViewModel, Publicacion>().ReverseMap();
        }
    }
}
