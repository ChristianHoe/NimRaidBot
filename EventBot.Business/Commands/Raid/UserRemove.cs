using EventBot.DataAccess.Commands.Base;

namespace EventBot.Business.Commands.Raid
{
    public sealed record UserRemoveRequest(
        long UserId,
        long ChatId
    );

    public interface IUserRemove
    {
        void Execute(UserRemoveRequest request);
    }

    public sealed class UserRemove : IUserRemove
    {
        private readonly IUserChannelRelationRemoveCommand userChannelRelationRemoveCommand;

        public UserRemove(
            IUserChannelRelationRemoveCommand userChannelRelationRemoveCommand
            )
        {
            this.userChannelRelationRemoveCommand = userChannelRelationRemoveCommand;
        }

        public void Execute(UserRemoveRequest request)
        {
            this.userChannelRelationRemoveCommand.Execute(new UserChannelRelationRemoveRequest(UserId: request.UserId, ChatId: request.ChatId));
        }
    }
}
