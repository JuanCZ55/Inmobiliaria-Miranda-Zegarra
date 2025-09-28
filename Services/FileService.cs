using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Inmobiliaria.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        // Inyectamos IWebHostEnvironment para poder obtener la ruta a la carpeta wwwroot
        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> GuardarArchivoAsync(IFormFile archivo, string rutaCarpeta)
        {
            // Combinamos la ruta de wwwroot con la carpeta de destino que nos pasan
            var carpetaDestino = Path.Combine(_env.WebRootPath, rutaCarpeta);

            // Si la carpeta no existe, la creamos
            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            // Generamos un nombre de archivo único para evitar que se pisen archivos con el mismo nombre
            var nombreUnico = $"{Guid.NewGuid()}{Path.GetExtension(archivo.FileName)}";
            var rutaCompleta = Path.Combine(carpetaDestino, nombreUnico);

            // Usamos un "stream" para copiar el contenido del archivo subido al nuevo archivo en el disco
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Devolvemos solo el nombre único del archivo, que es lo que se guardará en la base de datos
            return nombreUnico;
        }

        public void BorrarArchivo(string nombreArchivo, string rutaCarpeta)
        {
            if (string.IsNullOrEmpty(nombreArchivo))
            {
                return;
            }

            var rutaCompleta = Path.Combine(_env.WebRootPath, rutaCarpeta, nombreArchivo);

            // Verificamos que el archivo exista antes de intentar borrarlo para evitar errores
            if (File.Exists(rutaCompleta))
            {
                File.Delete(rutaCompleta);
            }
        }
    }
}
