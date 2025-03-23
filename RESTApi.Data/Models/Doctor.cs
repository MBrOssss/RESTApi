using Microsoft.EntityFrameworkCore;

namespace RESTApi.Data.Models
{
    public class Doctor : BaseModel
    {
        [Comment("Identyfikator")]
        public int Id { get; set; }

        [Comment("Imię")]
        public string FirstName { get; set; }

        [Comment("Nazwisko")]
        public string LastName { get; set; }

        [Comment("Specjalizacja")]
        public string Specialization { get; set; }
    }
}
