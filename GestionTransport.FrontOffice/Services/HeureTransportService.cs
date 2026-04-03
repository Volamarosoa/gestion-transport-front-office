using GestionTransport.FrontOffice.Models.Transport;
using GestionTransport.FrontOffice.Repositories.Interfaces;

namespace GestionTransport.FrontOffice.Services
{
    public class HeureTransportService
    {
        private readonly IHeureTransportRepository _heureTransportRepository;

        public HeureTransportService(IHeureTransportRepository heureTransportRepository)
        {
            _heureTransportRepository = heureTransportRepository;
        }

        public List<HeureTransportModel> GetAllActifs()
        {
            return _heureTransportRepository.GetActifs();
        }

        public HeureTransportModel GetById(int id)
        {
            var heure = _heureTransportRepository.GetById(id);
            if (heure == null)
                throw new KeyNotFoundException($"Heure de transport avec l'id {id} introuvable.");
            return heure;
        }

        public int Create(TimeSpan heure, string libelle)
        {
            var model = new HeureTransportModel
            {
                Heure = heure,
                Libelle = libelle,
                Actif = true,
                DateInsertion = DateTime.Now
            };

            return _heureTransportRepository.Create(model);
        }
    }
}
