# LinkUp

Proyecto de red social educativa desarrollado en ASP.NET Core (.NET 9).

## Descripci�n breve

LinkUp es una aplicaci�n demo que implementa funcionalidades t�picas de una red social: registro y autenticaci�n de usuarios (ASP.NET Core Identity), publicaciones (texto, imagen o enlace de YouTube), comentarios anidados y reacciones, gesti�n de amigos y solicitudes, y un mini-juego Battleship con gesti�n de partidas y turnos. El proyecto sigue una arquitectura por capas (Onion) para facilitar separaci�n de responsabilidades y mantenibilidad.

## Arquitectura y organizaci�n

- **LinkUp.Core.Domain**: modelo de dominio (entidades, enums, contratos de repositorios).
- **LinkUp.Core.Application**: l�gica de aplicaci�n, DTOs, ViewModels, servicios de negocio y mapeos (AutoMapper).
- **LinkUp.Infrastructure.Persistence**: implementaci�n de persistencia con EF Core (DbContext, configuraciones y repositorios).
- **LinkUp.Infrastructure.Identity**: configuraci�n de ASP.NET Core Identity y servicios relacionados con usuarios.
- **LinkUp.Infrastructure.Shared**: servicios compartidos como env�o de correo.
- **LinkUp (proyecto web)**: interfaz y controladores (MVC), vistas Razor, utilidades y composici�n (Program.cs).

## Tecnolog�as principales

- **.NET 9**
- **ASP.NET Core MVC** (Razor views)
- **Entity Framework Core** (Code-First)
- **ASP.NET Core Identity**
- **AutoMapper**
- **Bootstrap** (estilos)

## Instalaci�n y puesta en marcha (resumen)

1. Clonar el repositorio:

   ```bash
   git clone https://github.com/saidabel26/LinkUp.git
   ```

2. Configurar `appsettings.json` en el proyecto `LinkUp`:

   - Importante: Antes de ejecutar la aplicaci�n debes a�adir tu propia configuraci�n.

   Ejemplo de cadena de conexi�n:

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=TU_SERVIDOR;Database=LinkUpDb;Integrated Security=true;TrustServerCertificate=true;MultipleActiveResultSets=true;"
   }
   ```

   Ejemplo de secci�n de correo (MailSettings):

   ```json
   "MailSettings": {
     "EmailFrom": "tu-correo@dominio.com",
     "SmtpHost": "smtp.dominio.com",
     "SmtpPort": 587,
     "SmtpUser": "TU_SMTP_USER",
     "SmtpPass": "TU_SMTP_PASS"
   }
   ```

   - Nota: las claves `EmailFrom`, `SmtpUser` y `SmtpPass` deben contener tus propias credenciales SMTP.

3. Aplicar migraciones a la base de datos

   El proyecto contiene migraciones para la base de datos de dominio y para Identity. Para actualizarlas ejecuta (desde la ra�z del repo):

   ```bash
   dotnet ef database update --project LinkUp.Infrastructure.Persistence --startup-project LinkUp
   dotnet ef database update --project LinkUp.Infrastructure.Identity --startup-project LinkUp
   ```

   - Si trabajas desde Visual Studio puedes usar el Package Manager Console o el UI de migraciones.

4. Ejecutar la aplicaci�n

   ```bash
   dotnet run --project LinkUp
   ```

   Luego abre el navegador en `https://localhost:5001` (o el puerto que indique la salida).

## Notas y recomendaciones

- Para pruebas locales con correo puedes usar servicios como Mailtrap o configurar un SMTP de pruebas.
- Si deseas reusar la base de datos de producci�n o un servidor SQL externo, actualiza `DefaultConnection` en `appsettings.json` y ejecuta las migraciones.