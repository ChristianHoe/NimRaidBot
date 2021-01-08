using EventBot.DataAccess.Commands.Base;
using EventBot.DataAccess.Commands.Raid;

namespace EventBot.Business.Commands.Raid
{
    public class UserAddRequest
    {
        public long UserId;
        public string UserName;
        public long ChatId;
    }

    public interface IUserAdd
    {
        void Execute(UserAddRequest request);
    }

    public class UserAdd : IUserAdd
    {
        private readonly IAddUserCommand addUserCommand;
        private readonly IUserChannelRelationAddCommand userChannelRelationAddCommand;

        public UserAdd(
            IAddUserCommand addUserCommand,
            IUserChannelRelationAddCommand userChannelRelationAddCommand
            )
        {
            this.addUserCommand = addUserCommand;
            this.userChannelRelationAddCommand = userChannelRelationAddCommand;
        }

        public void Execute(UserAddRequest request)
        {
            this.addUserCommand.Execute(new AddUserRequest(UserId: request.UserId, FirstName: request.UserName));
            this.userChannelRelationAddCommand.Execute(new UserChannelRelationAddRequest(UserId: request.UserId, ChatId: request.ChatId));
        }
    }
}
