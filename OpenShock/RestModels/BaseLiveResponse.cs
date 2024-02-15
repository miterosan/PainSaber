using System;

namespace PainSaber.OpenShock.RestModels
{
    public class BaseLiveResponse<T> where T : Enum
    {
        public T ResponseType { get; set; }
        public object Data { get; set; }
    }
}