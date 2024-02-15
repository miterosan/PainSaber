using System;

namespace PainSaber.OpenShock.RestModels
{
    public class BaseLiveRequest<T> where T: Enum
    {
        public T RequestType { get; set; }
        public object Data { get; set; }
    }
}