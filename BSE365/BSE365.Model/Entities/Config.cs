using System.ComponentModel.DataAnnotations;

namespace BSE365.Model.Entities
{
    public class Config : BaseEntity
    {
        [StringLength(50)]
        public string Key { get; set; }

        public string Value { get; set; }
    }
}