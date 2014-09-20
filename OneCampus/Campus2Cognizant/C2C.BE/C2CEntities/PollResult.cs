using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace C2C.BusinessEntities.C2CEntities
{
    public class PollResult
    {
        public int Id { get; set; }
        public int PollId { get; set; }
        public int PollAnswerId { get; set; }
        public int AnsweredBy { get; set; }
        public DateTime AnsweredOn { get; set; }
    }
}
