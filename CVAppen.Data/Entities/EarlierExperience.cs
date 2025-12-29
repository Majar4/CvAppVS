namespace CvAppen.Data
{
    public class EarlierExperience
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }

        public string Company { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int CVId { get; set; }
        public CV CV { get; set; }
    }
}
