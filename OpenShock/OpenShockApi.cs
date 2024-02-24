using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using PainSaber.OpenShock.RestModels;

namespace PainSaber.OpenShock
{
    public class OpenShockApi
    {

        private readonly string baseAdress = "https://api.shocklink.net";
        private readonly HttpClient client;

        public OpenShockApi(string apiKey)
        {
            client = new HttpClient {
                BaseAddress = new Uri(baseAdress)
            };

            client.DefaultRequestHeaders.Add("OpenShockToken", apiKey);
        }

        public async Task ControlShockers(ControlRequest controlRequest)
        {
            var requestData = JsonConvert.SerializeObject(new
            {
                shocks = new[] {
                    new {
                        controlRequest.Shocker.id,
                        type = (int)controlRequest.Type,
                        intensity = controlRequest.Amount,
                        controlRequest.Duration,
                    },
                },
                customName = controlRequest.Name
            });

            using(var jsonContent = new StringContent(requestData, Encoding.UTF8, "application/json"))
            {
                await client.PostAsync("/2/shockers/control", jsonContent);
            }
        }

        public async Task<Device[]> GetOwnShockers()
        {
            var response = await client.GetAsync($"/1/shockers/own");

            response.EnsureSuccessStatusCode();

            var message = await response.Content.ReadAsStringAsync();
            var devices = JsonConvert.DeserializeObject<BaseResponse<Device[]>>(message);

            return devices?.data ?? Array.Empty<Device>();
        }

        public async Task<LcgResponse> GetLiveControlGatewayInfo(string shockerId) {
            var response = await client.GetAsync($"/1/devices/{shockerId}/lcg");

            response.EnsureSuccessStatusCode();

            var message = await response.Content.ReadAsStringAsync();
            var devices = JsonConvert.DeserializeObject<BaseResponse<LcgResponse>>(message);

            return devices.data;
        }

    }
}