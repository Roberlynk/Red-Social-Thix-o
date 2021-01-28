using AutoMapper;
using Database.Model;
using Email;
using Microsoft.EntityFrameworkCore;
using Repository.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Repository.Repository
{
    public class UsuarioRepository : RepositoryBase<Usuario, ThixioContext>
    {
        private readonly AmigoRepository _amigoRepository;
        public UsuarioRepository(ThixioContext context) : base(context)
        {
            _amigoRepository = new AmigoRepository(context);
        }

        public async Task<LoginViewModel> GetLogin(string nombreUsuario, string contraseña)
        {
            var passwordEncrypted = PasswordEncryption(contraseña);

            var usuario = await base._context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario && u.Contraseña == passwordEncrypted);

            var vm = Mapper.Map<LoginViewModel>(usuario);

            return vm;
        }
        public Usuario GetRegister(RegisterViewModel vm)
        {
            var usuarioEntity = Mapper.Map<Usuario>(vm);

            usuarioEntity.Contraseña = PasswordEncryption(vm.Contraseña);

            return usuarioEntity;
        }

        public async Task<RegisterViewModel> GetForDelete(int? id)
        {
            var usuario = await base._context.Usuario.FirstOrDefaultAsync(u => u.IdUsuario == id);

            var usuarioEntity = Mapper.Map<RegisterViewModel>(usuario);

            return usuarioEntity;
        }
        public async Task<Usuario> UserExist(string nombreUsuario)
        {
            var usuario = await base._context.Usuario.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

            return usuario;
        }

        public async Task<Usuario> EmailExist(string correo)
        {
            var usuario = await base._context.Usuario.FirstOrDefaultAsync(u => u.Correo == correo);

            return usuario;
        }

        public Usuario ResetPassword(Usuario user, string token)
        {
            var userEntity = Mapper.Map<Usuario>(user);

            userEntity.Contraseña = PasswordEncryption(token);

            return userEntity;
        }

        public async Task<List<UsuarioViewModel>> GetUsuarioByAmigo(Usuario user)
        {

            var listAmigos = await _amigoRepository.GetAmigosByUsuario(user.IdUsuario);

            List<UsuarioViewModel> listAmigosUsuario = new List<UsuarioViewModel>();

            foreach (var amigo in listAmigos)
            {
                if (amigo.IdUsuario == user.IdUsuario)
                {
                    var listEntity = await _context.Usuario.Where(p => p.IdUsuario == amigo.IdAmigo).ToListAsync();
                    foreach (var a in listEntity)
                    {
                        var vm = Mapper.Map<UsuarioViewModel>(a);
                        listAmigosUsuario.Add(vm);
                    }
                }
                else if (amigo.IdAmigo == user.IdUsuario)
                {
                    var listEntity = await _context.Usuario.Where(p => p.IdUsuario == amigo.IdUsuario).ToListAsync();
                    foreach (var a in listEntity)
                    {
                        var vm = Mapper.Map<UsuarioViewModel>(a);
                        listAmigosUsuario.Add(vm);
                    }
                }
            }

            return listAmigosUsuario;
        }

        public async Task<string> NombreById(int? id)
        {
            var usuario = await base._context.Usuario.FirstOrDefaultAsync(u => u.IdUsuario == id);

            string stringNombre = usuario.Nombre + " " + usuario.Apellido;

            return stringNombre;
        }
        private string PasswordEncryption(string password)
        {
            using SHA256 sha256Hash = SHA256.Create();
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
