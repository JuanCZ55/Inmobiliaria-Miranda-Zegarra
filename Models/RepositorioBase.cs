using Microsoft.Extensions.Configuration;

namespace Inmobiliaria.Models
{
  public abstract class RepositorioBase
  {
    protected readonly IConfiguration configuration;
    protected readonly string connectionString;

    protected RepositorioBase(IConfiguration configuration)
    {
      this.configuration = configuration;
      connectionString = configuration["ConnectionStrings:DefaultConnection"]
                        ?? throw new InvalidOperationException("La cadena de conexión no está configurada.");
    }
  }
}