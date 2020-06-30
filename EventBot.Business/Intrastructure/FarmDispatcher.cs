using EventBot.Business.Commands;
using EventBot.Business.Commands.Farm;

namespace EventBot.Business.Intrastructure
{
    public class FarmDispatcher : BaseDispatcher
    {
        public FarmDispatcher(
            ISetupPollCommand createPoll2Command,
            IPollAnswer raidPollAnswer,
            IEventSetupAnswer eventSetupAnswer,
            MemberAdded memberAdded,
            MemberRemoved memberRemoved
            ) : base()
        {
            base.commands.Add(createPoll2Command);
            base.answers.Add(raidPollAnswer);
            base.answers.Add(eventSetupAnswer);
            base.memberAdded = memberAdded;
            base.memberRemoved = memberRemoved;
        }
    }
}
