# Sisitema de Inmobiliaria

Una aplicación web para gestionar propiedades, contratos, inquilinos y pagos.

## Autor

Ariel Ismael Miranda Salmin
Juan Cruz Zegarra

## Descripción

Esta aplicación permite a administradores y agentes gestionar una cartera de inmuebles, registrar y mantener inquilinos, crear y controlar contratos de arrendamiento, registrar pagos y generar vistas de calendario y listados. Está pensada para pequeñas inmobiliarias y administradores de propiedades que necesitan centralizar la información y agilizar procesos administrativos.

Características principales:

- Creación, modificación y listado de inmuebles.
- Gestión completa de inquilinos y propietarios.
- Administración de contratos y pagos con historial.
- Subida y manejo de imágenes para inmuebles.
- Interfaz web responsive con paneles de administración.

## Stack tecnológico

- Backend: ASP.NET Core 9 (C#)
- ADO.NET / repositorios
- Frontend: Razor Pages / MVC Views, Bootstrap
- Base de datos: SQL Server (archivo incluido: `inmobiliaria.sql`)
- Otros: JavaScript para manejo de imágenes y lógica client-side

## Requisitos

- .NET 9 SDK o versión compatible
- SQL Server (o SQL Server Express)
- Git

## Instalación y uso (entorno local)

1. Clonar el repositorio:

```powershell
git clone https://github.com/JuanCZ55/Inmobiliaria-Miranda-Zegarra.git
```

2. Restaurar paquetes e instalar dependencias:

```powershell
dotnet restore
```

3. Configurar la base de datos:

- Importa `inmobiliaria.sql` en tu servidor SQL (puedes usar SQL Server Management Studio o sqlcmd).
- Copia `appsettings.Development.json` a un nuevo archivo `.env.local` o ajusta `appsettings.Development.json` directamente con tu cadena de conexión. Ejemplo de cadena de conexión:

```json
"ConnectionStrings": {
	"DefaultConnection": "Server=localhost;Database=InmobiliariaDb;User Id=sa;Password=TuPassword;TrustServerCertificate=True;"
}
```

4. Ejecutar la aplicación:

```powershell
dotnet run
```

## Estructura del proyecto

Breve descripción de carpetas relevantes:

- `Controllers/` - Controladores MVC que gestionan las rutas y operaciones.
- `Models/` - Modelos de dominio, interfaces de repositorio y clases de acceso a datos.
- `Views/` - Vistas Razor para la interfaz de usuario.
- `wwwroot/` - Archivos estáticos: CSS, JS e imágenes.
- `Services/` - Servicios auxiliares (por ejemplo: manejo de archivos/imagenes).

## Cómo contribuir

Las contribuciones son bienvenidas. Pasos recomendados:

1. Haz un fork del repositorio.
2. Crea una rama con la descripción del cambio: `git checkout -b feat/nombre-caracteristica`.
3. Realiza commits claros y atómicos.
4. Envía un Pull Request describiendo los cambios y el motivo.

Antes de enviar cambios mayores, abre un issue para discutir la propuesta.

## Despliegue (opcional)

Recomendaciones rápidas para producción:

- Configura una instancia de SQL Server administrada o Azure SQL.
- Usa variables de entorno para cadenas de conexión y secretos.
- Publica la aplicación con `dotnet publish -c Release` y despliega en IIS, Azure App Service o un contenedor Docker.
