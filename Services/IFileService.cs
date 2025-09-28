using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Inmobiliaria.Services
{
    public interface IFileService
    {
        Task<string> GuardarArchivoAsync(IFormFile archivo, string rutaCarpeta);

        void BorrarArchivo(string nombreArchivo, string rutaCarpeta);
    }
}
