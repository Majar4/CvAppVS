namespace CvAppen.Data
{
    public class Education
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string School { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int CVId { get; set; }
        public CV CV { get; set; }
        

    }
}
