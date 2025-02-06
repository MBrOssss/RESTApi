using Microsoft.EntityFrameworkCore;

namespace Data.Models
{
    public class BaseModel
    {
        [Comment("Data utworzenia wpisu")]
        public DateTime? CreatedDate { get; set; }

        [Comment("Data ostatniej aktualizacji wpisu")]
        public DateTime? UpdatedDate { get; set; }

        [Comment("Identyfikator użytkownika tworzącego wpis")]
        public string? CreatedUserId { get; set; }

        [Comment("Identyfikator użytkownika aktualizującego wpis")]
        public string? UpdatedUserId { get; set; }
    }
}
