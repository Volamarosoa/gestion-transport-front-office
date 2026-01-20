namespace GestionTransport.FrontOffice.Services;

public class AffectationArchivageService
{
    // À appeler quotidiennement (via un job planifié)
    public async Task ArchiverAffectationsPassees()
    {
        // Récupère les affectations dont la date est passée
        var affectationsAArchiver = await GetAffectationsNonArchiveesAvecDatePassee();
        
        foreach (var affectation in affectationsAArchiver)
        {
            affectation.Archiver();
        }
    }
    
    // Méthode privée à implémenter avec votre repository
    private async Task<List<AffectationModel>> GetAffectationsNonArchiveesAvecDatePassee()
    {
        // Implementation dépend de votre couche d'accès aux données
        throw new NotImplementedException();
    }
}