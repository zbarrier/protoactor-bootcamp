using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MovieStreaming.Messages;
using Proto;

namespace MovieStreaming.Actors;

public class UserCoordinatorActor : IActor
{
    private readonly Dictionary<int, PID> _users = new Dictionary<int, PID>();

    public Task ReceiveAsync(IContext context)
    {
        switch (context.Message)
        {
            case PlayMovieMessage msg:
                ProcessPlayMovieMessage(context, msg);
                break;

            case StopMovieMessage msg:
                ProcessStopMovieMessage(context, msg);
                break;
        }
        return Task.CompletedTask;
    }

    private void ProcessPlayMovieMessage(IContext context, PlayMovieMessage msg)
    {
        var childActorRef = CreateChildUserIfNotExists(context, msg.UserId);
        context.Send(childActorRef, msg);
    }

    private void ProcessStopMovieMessage(IContext context, StopMovieMessage msg)
    {
        var childActorRef = CreateChildUserIfNotExists(context, msg.UserId);
        context.Send(childActorRef, msg);
    }

    private PID CreateChildUserIfNotExists(IContext context, int userId)
    {
        if (!_users.TryGetValue(userId, out var userPid))
        {
            var props = Props.FromProducer(() => new UserActor(userId));
            userPid = context.SpawnNamed(props, $"User{userId}");
            _users.Add(userId, userPid);

            ColorConsole.WriteLineCyan($"UserCoordinatorActor created new child UserActor for {userId} (Total Users: {_users.Count})");
        }

        return userPid;
    }
}
