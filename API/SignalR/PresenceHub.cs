using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;
        public PresenceHub(PresenceTracker presenceTracker)
        {
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var isOnline = await _presenceTracker.UserConnected(Context.User.GetUserName(), Context.ConnectionId);
            
            if (isOnline)
                await Clients.Others.SendAsync("UserIsOnline",Context.User.GetUserName());

            var currentUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers",currentUsers);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var offLine = await _presenceTracker.UserDisconnected(Context.User.GetUserName(), Context.ConnectionId);
            
            if (offLine)
                await Clients.Others.SendAsync("UserIsOffline",Context.User.GetUserName());

            // var currentUsers = await _presenceTracker.GetOnlineUsers();
            // await Clients.All.SendAsync("GetOnlineUsers",currentUsers);

            await base.OnDisconnectedAsync(exception);
        }

    }
}