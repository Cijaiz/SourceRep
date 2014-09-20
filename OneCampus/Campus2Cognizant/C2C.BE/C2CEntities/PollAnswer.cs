using System.ComponentModel.DataAnnotations;
namespace C2C.BusinessEntities.C2CEntities
{
    public class PollAnswer
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public string Answer { get; set; }

        public int VoteCount { get; set; }
        public bool IsDeleted { get; set; }
    }
}
