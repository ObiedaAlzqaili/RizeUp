namespace RizeUp.DTOs
{
    public class CoverLetterResponseDto
    {
       
        public string ApplicantName { get; set; } = "";
        public string ApplicantAddress { get; set; } = "";
        public string ApplicantCityState { get; set; } = "";
        public string ApplicantEmail { get; set; } = "";
        public string ApplicantPhone { get; set; } = "";
        public string Date { get; set; } = "";

       
        public string HiringManagerName { get; set; } = "";
        public string CompanyName { get; set; } = "";
        public string CompanyAddress { get; set; } = "";
        public string CompanyCityState { get; set; } = "";

       
        public List<string> Paragraphs { get; set; } = new();

        
        public string ClosingLine { get; set; } = "";
        public string Signature { get; set; } = "";
    }
}
