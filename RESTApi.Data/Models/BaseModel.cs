using Microsoft.EntityFrameworkCore;

namespace RESTApi.Data.Models
{
    public class BaseModel
    {
        [Comment("Data utworzenia wpisu")]
        public DateTime? CreatedDate { get; set; }

        [Comment("Data ostatniej aktualizacji wpisu")]
        public DateTime? UpdatedDate { get; set; }
    }
}
