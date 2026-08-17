namespace DAL.Models
{
    // Executes guild role operation.
    public enum GuildRole
    {
        Member = 0,
        Officer = 1,
        Leader = 2
    }

    // Executes guild join policy operation.
    public enum GuildJoinPolicy
    {
        Open = 0,
        Approval = 1,
        InviteOnly = 2
    }

    // Executes guild message type operation.
    public enum GuildMessageType
    {
        Text = 0,
        System = 1,
        Join = 2,
        Leave = 3,
        Promotion = 4
    }

    // Executes guild log action operation.
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
