namespace DAL.Models
{
    public enum GuildRole
    {
        Member = 0,
        Officer = 1,
        Leader = 2
    }

    public enum GuildJoinPolicy
    {
        Open = 0,       // Anyone can join directly
        Approval = 1,   // Must apply and wait for approval
        InviteOnly = 2  // Must be invited by Leader/Officer
    }

    public enum GuildMessageType
    {
        Text = 0,
        System = 1,
        Join = 2,
        Leave = 3,
        Promotion = 4
    }

    public enum GuildLogAction
    {
        Join,
        Leave,
        Kick,
        Promote,
        Demote,
        TransferLeader,
        NoticeUpdated,
        IconUpdated,
        LevelUp,
        Invite,
        ApplicationApproved,
        ApplicationRejected,
        GuildDissolved
    }
}
