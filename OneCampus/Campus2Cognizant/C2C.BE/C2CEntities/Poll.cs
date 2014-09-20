using C2C.Core.Constants.C2CWeb;
using C2C.Core.DataAnnotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace C2C.BusinessEntities.C2CEntities
{
    public class Poll : Audit
    {
        public Poll()
        {
            PollAnswers = new List<PollAnswer>();
            PollResults = new List<PollResult>();
            GroupList = new List<Group>();
            SelectedGroupIds = new List<int>();
        }
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Maximum Length is 100")]
        public string Question { get; set; }

        [Display(Name = "Visible From")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime VisibleFrom { get; set; }

        [Display(Name = "Visible Till")]
        [DataType(DataType.Date)]
        [CompareDate("VisibleFrom",true, CompareDate.Type.GreaterOrEqual)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime VisibleTill { get; set; }

        public List<PollAnswer> PollAnswers { get; set; }
        public List<PollResult> PollResults { get; set; }

        public Status Status { get; set; }
        public bool Notify { get; set; }
        public bool HasPermission { get; set; }

        public bool IsVoted { get; set; }
        public bool IsVotingStarted { get; set; }

        public List<Group> GroupList { get; set; }
        public List<int> SelectedGroupIds { get; set; }
    }
}
