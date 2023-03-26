﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieStreaming.Messages;
using Proto;

namespace MovieStreaming.Actors;

public class PlaybackActor : IActor
{
    private Dictionary<string, PID> _links = new Dictionary<string, PID>();

    private string _currentlyWatching;

    public PlaybackActor() => Console.WriteLine("Creating a PlaybackActor");
    public Task ReceiveAsync(IContext context)
    {
        switch (context.Message)
        {
            case Started msg:
                ProcessStartedMessage(context, msg);
                break;

            case PlayMovieMessage msg:
                ProcessPlayMovieMessage(msg);
                break;

            case Recoverable msg:
                ProcessRecoverableMessage(context, msg);
                break;

            case Stopping msg:
                ProcessStoppingMessage(msg);
                break;

            case RequestActorPidMessage msg:
                ProcessRequestActorPidMessage(context, msg);
                break;
        }
        return Task.CompletedTask;
    }

    private void ProcessStartedMessage(IContext context, Started msg)
    {
        ColorConsole.WriteLineGreen("PlaybackActor Started");

        var props = Props.FromProducer(() => new UserCoordinatorActor());
        _links.Add("UserCoordinatorActor", context.Spawn(props));
    }

    private void ProcessPlayMovieMessage(PlayMovieMessage msg)
    {
        ColorConsole.WriteLineYellow($"PlayMovieMessage {msg.MovieTitle} for user {msg.UserId}");
    }

    private void ProcessRecoverableMessage(IContext context, Recoverable msg)
    {
        PID child;

        if (context.Children == null || context.Children.Count == 0)
        {
            var props = Props.FromProducer(() => new ChildActor());
            child = context.Spawn(props);
        }
        else
        {
            child = context.Children.First();
        }

        context.Forward(child);
    }

    private void ProcessStoppingMessage(Stopping msg)
    {
        ColorConsole.WriteLineGreen("PlaybackActor Stopping");
    }

    private void ProcessRequestActorPidMessage(IContext context, RequestActorPidMessage msg)
    {
        context.Respond(new ResponseActorPidMessage(msg.ActorName, _links[msg.ActorName]));
    }
}
