using System.Security.Claims;

namespace GestionTransport.FrontOffice.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static int GetEmployeId(this ClaimsPrincipal user)
    {
        var employeIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(employeIdClaim, out var employeId))
        {
            throw new InvalidOperationException("Identifiant employe introuvable dans les claims.");
        }

        return employeId;
    }
}

