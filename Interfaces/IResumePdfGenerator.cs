using RizeUp.DTOs;

namespace RizeUp.Interfaces
{
    public interface IResumePdfGenerator
    {
        byte[] GenerateResumePdf(ResumeDto resume);
    }
}
