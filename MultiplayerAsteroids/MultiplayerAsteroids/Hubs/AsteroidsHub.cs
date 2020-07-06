using ConcurrencyAsteroidsLibrary.Hubs;
using ConcurrencyAsteroidsLibrary.Models;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace MultiplayerAsteroids.Hubs
{
    public class AsteroidsHub : Hub, IAsteroidsHubServer
    {
        private Dictionary<string, Ship> players = new Dictionary<string, Ship>();
        public async Task SetPlayerName()
        {
            ;
        }
        public async Task SendMessage(string message)
        {
            Clients.AllExcept(this.Context.ConnectionId).SendAsync("ReceiveMessage", message);
        }

        public async Task NewPlayer(string name)
        {
            players.Add(name, null);
            await Clients.AllExcept(this.Context.ConnectionId).SendAsync("ReceiveShips", players.Values.ToList());

        }

        public Task UpdateShipPosition(float x, float y)
        {
            throw new NotImplementedException();
        }
    }
}
