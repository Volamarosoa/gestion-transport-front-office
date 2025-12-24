using System;
using System.ComponentModel.DataAnnotations;

namespace GestionTransport.FrontOffice.Models
{
    public class TransportModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La date est obligatoire")]
        [Display(Name = "Date du transport")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Le type de transport est obligatoire")]
        [Display(Name = "Type de transport")]
        public string Type { get; set; } = string.Empty; // "Aller", "Retour", "AllerRetour"
        // public required string Type { get; set; }

        [Display(Name = "Adresse de départ")]
        public string? AdresseDepart { get; set; }

        [Display(Name = "Adresse d'arrivée")]
        public string? AdresseArrivee { get; set; }

        [Display(Name = "Heure souhaitée")]
        [DataType(DataType.Time)]
        public TimeSpan? HeureSouhaitee { get; set; }

        [Display(Name = "Commentaire")]
        [MaxLength(500)]
        public string? Commentaire { get; set; }

        [Display(Name = "Demande urgente")]
        public bool Urgent { get; set; }

        // Propriétés supplémentaires pour l'affichage
        public string? Statut { get; set; } // "En attente", "Approuvé", "Refusé"
        public int? EmployeeId { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}