using Database.Model;
using Microsoft.EntityFrameworkCore;
using Repository.RepositoryBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repository
{
    public class AmigoRepository : RepositoryBase<Amigo, ThixioContext>
    {
        public AmigoRepository(ThixioContext context) : base(context)
        {
        }

        public Amigo CreateAmigo(Usuario user, Usuario id)
        {
            var amigoEntity = new Amigo
            {
                IdUsuario = user.IdUsuario,
                IdAmigo = id.IdUsuario
            };

            return amigoEntity;
        }

        public async Task<Amigo> FindAmigos(Usuario user, Usuario id)
        {
            var amigo = await base._context.Amigo.FirstOrDefaultAsync(u => u.IdAmigo == user.IdUsuario && u.IdUsuario == id.IdUsuario);

            if (amigo != null)
            {
                return amigo;
            }
            else 
            {
                amigo = await base._context.Amigo.FirstOrDefaultAsync(u => u.IdUsuario == user.IdUsuario && u.IdAmigo == id.IdUsuario);
            }

            return amigo;
        }

        public async Task<List<Amigo>> GetAmigosByUsuario(int IdUsuario)
        {
            var listAmigo = await base._context.Amigo.Where(p => p.IdUsuario == IdUsuario || p.IdAmigo == IdUsuario).ToListAsync();

            return listAmigo;
        }

        public async Task<Amigo> RemoveAmigo(Usuario user, int id)
        {
            var amigo = await base._context.Amigo.FirstOrDefaultAsync(u => u.IdAmigo == user.IdUsuario && u.IdUsuario == id);

            if (amigo != null)
            {
                return amigo;
            }
            else
            {
                amigo = await base._context.Amigo.FirstOrDefaultAsync(u => u.IdUsuario == user.IdUsuario && u.IdAmigo == id);
            }

            return amigo;
        }

        public async Task<Boolean> Deletear(Amigo amigo)
        {
            base._context.Amigo.Remove(amigo);

            await base._context.SaveChangesAsync();

            return true;
        }
    }
}
