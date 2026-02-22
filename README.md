# LinkUp

LinkUp es una aplicación web de ejemplo que implementa una pequeña red social con funcionalidades de publicaciones, comentarios, relaciones de amistad y un mini-juego tipo Battleship. Está organizada con una arquitectura por capas para separar dominio, lógica de aplicación e infraestructura.

Principales características
- Autenticación y gestión de usuarios (ASP.NET Core Identity).
- Publicaciones con texto, imagen o enlace de YouTube.
- Comentarios anidados (replies) y reacciones (me gusta / no me gusta).
- Gestión de amigos y solicitudes de amistad.
- Mini-juego Battleship con posicionamiento de barcos, ataques por turnos y manejo de tiempos de inactividad.
- Subida de archivos (foto de perfil, imágenes de publicaciones).

Arquitectura y organización
- `LinkUp.Core.Domain`: entidades del dominio y contratos (interfaces de repositorio, enums, configuraciones).
- `LinkUp.Core.Application`: servicios de aplicación (casos de uso), DTOs, ViewModels y perfiles de AutoMapper.
- `LinkUp.Infrastructure.Persistence`: implementación de persistencia con EF Core (DbContext, configuraciones y repositorios).
- `LinkUp.Infrastructure.Identity`: configuración e implementación de ASP.NET Core Identity (usuarios y servicios relacionados).
- `LinkUp.Infrastructure.Shared`: servicios compartidos (por ejemplo, envío de correo).
- `LinkUp` (proyecto Web): controladores, vistas Razor y composition root (`Program.cs`).

Tecnologías
- .NET 9 / C# 13
- ASP.NET Core MVC (Razor views)
- Entity Framework Core (Code-First)
- ASP.NET Core Identity
- AutoMapper
- Bootstrap (estilos en vistas)

Seguridad de configuración y `appsettings.json`
Por seguridad el archivo `appsettings.json` que contiene la cadena de conexión (`ConnectionStrings:DefaultConnection`) y las credenciales de correo (`MailSettings`) está ignorado por git en este repositorio. Esto evita exponer información sensible (como credenciales SMTP o cadenas de conexión a bases de datos) en el control de código fuente.

Si clonas el repositorio y quieres ejecutar la aplicación localmente, no es necesario que el repositorio contenga el archivo `appsettings.json`. Solo debes crear un archivo `appsettings.json` en la raíz del proyecto web (LinkUp) con la misma estructura que se muestra a continuación.

Ejemplo mínimo de `appsettings.json` (sustituir los valores):

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "<TU_CADENA_DE_CONEXION_A_LA_BASE_DE_DATOS_AQUI>"
  },
  "MailSettings": {
    "EmailFrom": "<EMAIL_FROM_O_REMETENTE>",
    "SmtpHost": "<SMTP_HOST>",
    "SmtpPort": 587,
    "SmtpUser": "<SMTP_USER>",
    "SmtpPass": "<SMTP_PASSWORD>",
    "DisplayName": "LinkUp Social Network"
  }
}
```

Notas sobre los campos sensibles
- `ConnectionStrings:DefaultConnection`: cadena de conexión a SQL Server (o la base de datos que uses). Ejemplo local: `Server=localhost;Database=LinkUpDb;Trusted_Connection=True;MultipleActiveResultSets=true`.
- `MailSettings.EmailFrom`, `MailSettings.SmtpUser`, `MailSettings.SmtpPass`: credenciales de envío de correo. No las incluyas en el repositorio. Puedes usar servicios SMTP de desarrollo o herramientas locales para pruebas.

Cómo ejecutar (resumen)
1. Clona el repositorio:
   `git clone <repo-url>`
2. Crea el archivo `appsettings.json` en la raíz del proyecto web (LinkUp) con la estructura mostrada arriba y añade tus valores.
3. Restaura paquetes y compila:
   `dotnet restore`
   `dotnet build`
4. Aplica migraciones y crea la base de datos (opcional, si usas EF Migrations):
   - Desde la carpeta del proyecto web o del proyecto de `Infrastructure.Persistence`:
     `dotnet ef database update --project LinkUp.Infrastructure.Persistence --startup-project LinkUp`
5. Ejecuta la aplicación:
   `dotnet run --project LinkUp`

Contribuciones y pruebas
- Para desarrollo local puedes usar cuentas SMTP de prueba o herramientas como MailTrap / Papercut para capturar correos sin enviar a producción.
- Las migraciones están incluidas en el proyecto de persistencia.