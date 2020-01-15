using System; 

namespace Model
{
    public interface IEntity
    {
        DateTime CreatedDate { get; set; }
        DateTime ModificationDate { get; set; }
    }
    public class Entity : IEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
