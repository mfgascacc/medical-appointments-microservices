# Medical Appointments - Prueba Tecnica

Sistema de microservicios en .NET Framework 4.8 para gestion de personas, citas y recetas medicas.

## Arquitectura

- `People.Api`: gestion de personas (doctores y pacientes).
- `Appointments.Api`: gestion de citas y validaciones de agenda.
- `Prescriptions.Api`: gestion de recetas y consumo de eventos.
- `Messaging.Contracts`: contratos compartidos de eventos.

Cada microservicio mantiene separacion por capas:

- `*.Domain`: entidades y reglas de negocio.
- `*.Application`: contratos (repositorios).
- `*.Infrastructure`: EF6, DbContext y repositorios concretos.
- `*.Api`: endpoints Web API 2, DI y configuracion.

## Flujo sincronico vs asincronico

- **Sincronico**: `Appointments.Api` consulta `People.Api` para validar que `DoctorId` y `PatientId` existan y correspondan al tipo correcto.
- **Asincronico (RabbitMQ)**: cuando una cita queda `Finished`, `Appointments.Api` publica evento y `Prescriptions.Api` lo consume para crear receta.

## Requisitos

- Windows 10/11
- Visual Studio 2022 (ASP.NET + .NET Framework)
- SQL Server local (`Server=.` en connection strings)
- .NET SDK 9.x (para ejecutar pruebas con CLI)
- Docker Desktop (recomendado para RabbitMQ)

## Estructura principal

- `ClinicAppointmentsSystem/MedicalAppointments.sln`
- `People.*`
- `Appointments.*`
- `Prescriptions.*`
- `People.Domain.Tests`
- `Appointments.Domain.Tests`
- `Prescriptions.Domain.Tests`

## Base de datos (EF6)

Cada API usa su propia base de datos:

- `PeopleDb`
- `AppointmentsDb`
- `PrescriptionsDb`

Si necesitas recrear migraciones o aplicar cambios, usa Package Manager Console en Visual Studio (startup project correcto por microservicio).

## RabbitMQ (recomendado con Docker)

### Levantar contenedor

```powershell
docker run -d --name rabbitmq-dev -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### Verificar

- UI: [http://localhost:15672](http://localhost:15672)
- Usuario: `guest`
- Password: `guest`

### Comandos utiles

```powershell
docker ps
docker logs rabbitmq-dev
docker stop rabbitmq-dev
docker start rabbitmq-dev
docker rm -f rabbitmq-dev
```

## RabbitMQ local (sin Docker, opcional)

1. Instalar Erlang OTP.
2. Instalar RabbitMQ Server para Windows.
3. Habilitar management plugin:

```powershell
rabbitmq-plugins enable rabbitmq_management
```

4. Iniciar servicio:

```powershell
net start RabbitMQ
```

5. Entrar a [http://localhost:15672](http://localhost:15672).

## Ejecucion de APIs

En Visual Studio:

1. Abrir `ClinicAppointmentsSystem/MedicalAppointments.sln`.
2. Configurar multiple startup projects:
   - `People.Api`
   - `Appointments.Api`
   - `Prescriptions.Api`
3. Ejecutar.

Swagger por API:

- `https://localhost:44335/swagger` (People)
- `https://localhost:44319/swagger` (Appointments)
- `https://localhost:44380/swagger` (Prescriptions)

## Autenticacion JWT

Cada API tiene autenticacion por Bearer token JWT.

### Endpoints publicos

- `GET /api/health`
- `POST /api/auth/token`

### Endpoints protegidos

- `People.Api`: `/api/people/*`
- `Appointments.Api`: `/api/appointments/*`
- `Prescriptions.Api`: `/api/prescriptions/*`

### Configuracion por API (`Web.config`)

```xml
<add key="JwtIssuer" value="medical-appointments" />
<add key="JwtAudience" value="medical-appointments-clients" />
<add key="JwtSecret" value="replace-with-strong-secret-key" />
<add key="JwtUsername" value="admin" />
<add key="JwtPassword" value="admin123" />
```

Recomendacion: cambiar `JwtSecret`, `JwtUsername` y `JwtPassword` para ambientes reales.

### Obtener token

Request:

```http
POST /api/auth/token
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

Response (ejemplo):

```json
{
  "accessToken": "<jwt>",
  "tokenType": "Bearer",
  "expiresIn": 7200
}
```

### Consumir endpoints protegidos

Agregar header:

```http
Authorization: Bearer <jwt>
```

Nota de integracion interna: `Appointments.Api` solicita token de `People.Api` automaticamente para realizar validaciones de doctor/paciente en llamadas internas.

## Pruebas unitarias

Se agregaron pruebas de dominio para cubrir reglas criticas:

- `People.Domain.Tests` (constructor y validaciones de persona)
- `Appointments.Domain.Tests` (transiciones `Pending -> InProgress -> Finished`)
- `Prescriptions.Domain.Tests` (transiciones `Active -> Delivered/Expired`)

Ejecucion por CLI:

```powershell
dotnet test C:\Projects\People.Domain.Tests\People.Domain.Tests.csproj --configuration Debug
dotnet test C:\Projects\Appointments.Domain.Tests\Appointments.Domain.Tests.csproj --configuration Debug
dotnet test C:\Projects\Prescriptions.Domain.Tests\Prescriptions.Domain.Tests.csproj --configuration Debug
```

Nota: ejecutar `dotnet test` contra toda la solucion puede fallar en CLI por proyectos ASP.NET clasicos (`Microsoft.WebApplication.targets`). En Visual Studio funciona correctamente.

## Demo funcional sugerida (5 minutos)

1. Crear cita en `Appointments` con `status = 1`.
2. Actualizar la cita a `status = 3` (Finished).
3. Verificar en RabbitMQ que hay actividad de publish/consume.
4. Consultar `GET /api/prescriptions/by-patient/{patientId}` y confirmar receta creada por evento.

## Codigos HTTP relevantes

- `200 OK`: consulta o actualizacion exitosa.
- `201 Created`: creacion exitosa.
- `204 No Content`: eliminacion.
- `400 Bad Request`: payload invalido o reglas de negocio.
- `404 Not Found`: recurso no existe.
- `409 Conflict`: conflicto de agenda o codigo unico.

## Decisiones tecnicas clave

- Microservicios con BD separada por contexto.
- DDD basico con reglas en entidades de dominio.
- Repositorio + DI simple para desacoplar API de persistencia.
- Integracion sincronica para validaciones inmediatas.
- Integracion asincronica con RabbitMQ para procesos desacoplados.
