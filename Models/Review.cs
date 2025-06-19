namespace RizeUp.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public EndUser EndUser { get; set; }
        public string UserName { get; set; }
        public int Rating { get; set; }           
        public string Comment { get; set; }        
        public DateTime CreatedAt { get; set; }
    }

}
