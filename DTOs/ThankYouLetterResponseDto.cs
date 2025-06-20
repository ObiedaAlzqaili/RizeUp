namespace RizeUp.DTOs
{
    public class ThankYouLetterResponseDto
    {
        public string Subject { get; set; }
        public string InterviewerName { get; set; }
        public List<string> BodyParagraphs { get; set; }
        public string ClosingLine { get; set; }
        public string SenderName { get; set; }
        public string LinkedInProfile { get; set; }
        public string PhoneNumber { get; set; }
    }
}
