namespace CvAppenVS.Models
{
    public class CV
    {
        public int Id { get; set; }
        public string Competence { get; set; }
        public string Education { get; set; }
        public string Presentation { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId {  get; set; }
        public User User { get; set; }

        //Ingen adress här, får hämtas från User, annars duplicerad data
    }
}
