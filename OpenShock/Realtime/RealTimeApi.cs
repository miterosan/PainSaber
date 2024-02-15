using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PainSaber.OpenShock.Realtime;
using PainSaber.OpenShock.RestModels;

namespace PainSaber.OpenShock
{
    public class RealTimeApi
    {
        public RealTimeApiState State { get; private set; }
        private readonly ClientWebSocket websocket = new ClientWebSocket();
        private Thread receiveThread;
        private Thread sendThread;

        public LiveDeviceState DeviceState { get; set; }

        private readonly Collection<IShockTransmission> transmissions = new Collection<IShockTransmission>();
        
        public IReadOnlyCollection<IShockTransmission> Transmissions => transmissions;
        private readonly ConcurrentQueue<IShockTransmission> newTransmissions = new ConcurrentQueue<IShockTransmission>();


        public void AddShockTransmission(IShockTransmission shockTransmission) 
        {
            PainSaberPlugin.Log.Debug("Added transmission");
            newTransmissions.Enqueue(shockTransmission);
        }

        public RealTimeApi(string apiKey)
        {
            websocket.Options.SetRequestHeader("OpenShockToken", apiKey);
            receiveBuffer = WebSocket.CreateClientBuffer(4096, 4096);
        }

        public async Task Connect(string deviceId, string gateway)
        {
            var builder = new UriBuilder
            {
                Scheme = "wss",
                Host = gateway,
                Path = $"1/ws/live/{deviceId}"
            };

            await websocket.ConnectAsync(builder.Uri, CancellationToken.None);

            receiveThread = new Thread(HandleResponses);
            receiveThread.Start();
            sendThread = new Thread(handleTransmissions);
            sendThread.Start();
        }

        private async void HandleResponses() 
        {
            while(websocket.State != WebSocketState.Closed) 
            {
                var response = await ReceiveNext();

                switch(response.ResponseType)
                {
                    case LiveResponseType.Ping:
                        await SendPong();
                        break;
                    case LiveResponseType.DeviceConnected:
                        DeviceState = LiveDeviceState.Connected;
                        break;
                    case LiveResponseType.DeviceNotConnected:
                        DeviceState = LiveDeviceState.NotConnected;
                        break;
                }
            }
        }

        private async void handleTransmissions() 
        {
            while(websocket.State != WebSocketState.Closed)
            {
                await Task.Delay(100);

                var currentTime = DateTime.UtcNow;

                // clean transmissions that are done
                foreach (var transmission in transmissions.Where(t => t.Endtime < currentTime).ToArray())
                    transmissions.Remove(transmission);

                // add all new transmissions
                while(!newTransmissions.IsEmpty)
                {
                    if (newTransmissions.TryDequeue(out IShockTransmission transmission))
                        transmissions.Add(transmission);
                }
               
                // determine level of each shocker by taking the highest value
                foreach (var group in transmissions.GroupBy(t => t.ShockerId))
                {
                    byte intensity = group.Max(t => t.GetIntensity());
                    
                    await SendFrame(group.Key, intensity, LiveControlType.Shock);
                }
            }
        }

        public async Task SendFrame(string shockerId, byte intensity, LiveControlType type) 
        {
            PainSaberPlugin.Log.Debug($"Sending Frame: {shockerId}, {intensity}, {type}");

            await Send(new BaseLiveRequest<LiveRequestType>{
                RequestType = LiveRequestType.Frame,
                Data = new ClientLiveFrame{
                    Intensity = intensity,
                    Shocker = Guid.Parse(shockerId),
                    Type = type
                }
            });
        }

        private readonly ArraySegment<byte> receiveBuffer;

        private async Task<BaseLiveResponse<LiveResponseType>> ReceiveNext() 
        {
            BaseLiveResponse<LiveResponseType> response = null;

            while(response == null) 
            {
                var receiveResult = await websocket.ReceiveAsync(receiveBuffer, CancellationToken.None);
                
                string message = Encoding.UTF8.GetString(receiveBuffer.Array, 0, receiveResult.Count);

                if (message.Trim() == string.Empty) continue;

                response = JsonConvert.DeserializeObject<BaseLiveResponse<LiveResponseType>>(message);
            }

            return response;
        }

        private async Task SendPong() 
        {
            await Send(new BaseLiveRequest<LiveRequestType>{
                RequestType = LiveRequestType.Pong,
                Data = new {
                    Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                }
            });
        }

        private async Task Send<T>(BaseLiveRequest<T> request) 
            where T : Enum
        {
            var message = JsonConvert.SerializeObject(request);
            var bytes = Encoding.UTF8.GetBytes(message);
            var sendbuffer = new ArraySegment<Byte>(bytes, 0, bytes.Length);
            await websocket.SendAsync(sendbuffer, WebSocketMessageType.Text, true, CancellationToken.None);

        }
    }
}