using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace RizeUp.Models
{
    public class Service
    {
        [Key] public int Id { get; set; }
        public string? ServiceName { get; set; }
        public string? ServiceDescription { get; set; }
        public string? ServiceImage { get; set; }

        public int PortfolioId { get; set; }

        public Portfolio Portfolio { get; set; }
    }
}
