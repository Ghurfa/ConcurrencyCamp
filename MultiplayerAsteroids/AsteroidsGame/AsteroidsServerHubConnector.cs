using ConcurrencyAsteroidsLibrary.Hubs;
using ConcurrencyAsteroidsLibrary.Models;
using CoroutinesLib;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ShipModel = ConcurrencyAsteroidsLibrary.Models.Ship;
using BulletModel = ConcurrencyAsteroidsLibrary.Models.Bullet;

namespace AsteroidsGame
{
    class AsteroidsServerHubConnector : IAsteroidsHubServer
    {
        private HubConnection connection;

        public event Func<List<Rock>, Task> OnReceiveAsteroids;
        public event Func<List<BulletModel>, Task> OnReceiveBullets;
        public event Func<List<ShipModel>, Task> OnReceiveShips;
        public event Func<string, Task> OnDisconnect;

        public Action GetRemoteRocks;
        public Action GetRemoteShips;
        public Action GetRemoteBullets;
        public AsteroidsServerHubConnector(HubConnection connection)
        {
            this.connection = connection;

            //Receive asteroids
            GetRemoteRocks = async () =>
            {
                while (true)
                {
                    try
                    {
                        var channel = await connection.StreamAsChannelAsync<Rock>("GetRemoteRocks").ConfigureAwait(false);

                        List<Rock> asteroids = new List<Rock>();
                        while (await channel.WaitToReadAsync())
                        {
                            while (channel.TryRead(out var asteroid))
                            {
                                if (asteroid != null)
                                {
                                    asteroids.Add(asteroid);
                                }
                            }
                        }
                        await OnReceiveAsteroids?.Invoke(asteroids);
                    }
                    catch (InvalidOperationException ex)
                    {

                    }
                    catch (System.Net.WebSockets.WebSocketException ex)
                    {

                    }
                }
            };

            //Receive ships
            GetRemoteShips = async () =>
            {
                while (true)
                {
                    try
                    {
                        var channel = await connection.StreamAsChannelAsync<ShipModel>("GetRemoteShips").ConfigureAwait(false);

                        List<ShipModel> otherShips = new List<ShipModel>();
                        while (await channel.WaitToReadAsync())
                        {
                            while (channel.TryRead(out var ship))
                            {
                                if (ship != null)
                                {
                                    otherShips.Add(ship);
                                }
                            }
                        }
                        await OnReceiveShips?.Invoke(otherShips);
                    }
                    catch (InvalidOperationException ex)
                    {

                    }
                    catch (System.Net.WebSockets.WebSocketException ex)
                    {

                    }
                }
            };

            //Receive bullets
            GetRemoteBullets = async () =>
            {
                while (true)
                {
                    try
                    {
                        var channel = await connection.StreamAsChannelAsync<BulletModel>("GetRemoteBullets").ConfigureAwait(false);

                        List<BulletModel> bullets = new List<BulletModel>();
                        while (await channel.WaitToReadAsync())
                        {
                            while (channel.TryRead(out var bullet))
                            {
                                if(bullet != null)
                                {
                                    bullets.Add(bullet);
                                }
                            }
                        }
                        await OnReceiveBullets?.Invoke(bullets);
                    }
                    catch (InvalidOperationException ex)
                    {

                    }
                    catch (System.Net.WebSockets.WebSocketException ex)
                    {

                    }
                }
            };

            //Disconnect
            /*Task.Run(async () =>
            {
                while (true)
                {
                    var channel = await connection.("Disconnect").ConfigureAwait(false);

                    await OnDisconnect.Invoke();
                }
            });*/
        }

        public Task NewPlayer(string name)
        {
            return connection.InvokeAsync(nameof(NewPlayer), name);
        }
        public Task UpdateShipPosition(float x, float y, float rotation)
        {
            return connection.InvokeAsync(nameof(UpdateShipPosition), x, y, rotation);
        }
        public Task NewBullet()
        {
            return connection.InvokeAsync(nameof(NewBullet));
        }

        public Task<List<Rock>> ReceiveAsteroids(List<Rock> rocks)
        {
            throw new NotImplementedException();
        }

        public Task<List<Bullet>> ReceiveBullets(List<BulletModel> bullets)
        {
            throw new NotImplementedException();
        }

        public Task<List<ShipModel>> ReceiveShips(List<ShipModel> otherShips)
        {
            throw new NotImplementedException();
        }

    }
}
