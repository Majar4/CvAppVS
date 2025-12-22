namespace CvAppenVS.Models
{
    public class Competence
    {
        public int Id { get; set; }

        public string Title { get; set; }
        public int CVId { get; set; }
        public CV CV {  get; set; }
    }
}
