namespace C2C.Core.Constants.C2CWeb
{
    public enum Module : int
    {
        User = 1,
        Role = 2,
        Like = 3,
        Share = 4,
        Comment = 5,
        Group = 6,
        MetaMaster = 7,
        WelcomNote = 8,
        Poll = 9,
        Blog = 10,
        KnowCognizant = 11,
        Quiz = 12,
        FAQ = 13,
        LocationGuide = 14,
        Forum = 15,
    }

    public enum Status : int
    {
        Deleted = 0,
        Pending = 1,
        Active = 2,
        InActive = 3
    }

    public enum ApplicationPermission
    {
        ManageUsers = 1,

        ManageProfile = 2,

        ManageRoles = 11,

        ManageBlogs = 21,

        CanLike = 31,

        ShareContent = 41,

        CanComment = 51,
        //Approve,Un-approve,Delete Comment
        ManageComments = 52,

        ManageGroups = 61,

        ManageMetaMaster = 71,

        ManageWelcomeNote = 81,

        CanVote = 91,

        ManagePoll = 92,

        ManageSiteSetting = 101,
    }

    public enum MetaMasterKey : int
    {
        BlogCategory = 1
    }
}
