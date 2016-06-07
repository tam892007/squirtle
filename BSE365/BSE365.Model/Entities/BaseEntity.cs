using System.ComponentModel.DataAnnotations.Schema;
using BSE365.Base.Infrastructures;

namespace BSE365.Model.Entities
{
    public abstract class BaseEntity : IObjectState
    {
        public int Id { get; set; }

        [NotMapped]
        public ObjectState ObjectState { get; set; }
    }
}