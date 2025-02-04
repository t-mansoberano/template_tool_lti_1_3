using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Federation;
using gec.Application.Contracts.Infrastructure.Federation.Models;

namespace gec.Infrastructure.Federation;

public class FederationService : IFederationService
{
    public async Task<Result<FederationContext>> HandleAuthAsync()
    {
        await Task.Delay(100); // Simula una llamada asíncrona.

        // Crear un usuario simulado. En un contexto real, esto puede ser extraído
        // de un sistema de identidad, token, etc.
        var user = new User
        {
            Name = "John Doe",
            Email = "johndoe@example.com",
            UserId = "12345",
            IsInstructor = false,
            IsStudent = false,
            IsExternalCollaborator = true,
            IsWithoutRole = false,
            Picture = "https://example.com/johndoe.jpg"
        };

        // Crear el contexto de federación con el usuario autenticado.
        var context = new FederationContext
        {
            User = user
        };

        return Result.Success(context);
    }
}