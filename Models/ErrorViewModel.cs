using System.Reflection.Metadata.Ecma335;

namespace RizeUp.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        
    }
}
