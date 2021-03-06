﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.SignalR.Client;

namespace PokeDaniel.Services
{
    public sealed class PokeClient : IPokeClient
    {
        private Uri baseUrl;
        private HttpClient httpClient;
        private HubConnection connection;

        public PokeClient(HttpClient httpClient, HubConnection hubConnection)
        {
            this.httpClient = httpClient;
            this.connection = hubConnection;

            connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await connection.StartAsync();
            };

            connection.On<ulong>("Poked", OnPoke);
        }

        private void OnPoke(ulong pokes)
        {
            Poked?.Invoke(this, new PokeEventArgs(pokes));
        }

        public async Task StartAsync()
        {
            await connection.StartAsync();
        }

        public async Task StopAsync()
        {
            await connection.StopAsync();
        }

        public async Task PokeAsync()
        {
            await httpClient.PostAsync("/api/Poke", new StringContent(string.Empty));
        }

        public async Task<ulong> GetPokesAsync()
        {
            var response = await httpClient.GetAsync("/api/Poke");
            return ulong.Parse(
                await response.Content.ReadAsStringAsync());
        }

        public event EventHandler<PokeEventArgs> Poked;
    }
}
