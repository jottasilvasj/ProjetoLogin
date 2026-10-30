using ProjetoLogin.Models;

namespace ProjetoLogin.Repositories
{
    public interface IUsuarioRepositorio
    {
        Task<bool> Cadastrar(UsuarioModel usuario, string Senha);

        Task<UsuarioModel> BuscarEmail(string Email);
    }
}
