namespace PainSaber.OpenShock.RestModels
{
    public class BaseResponse<T>
    {
        
        public string message { get; set; }

        public T data { get; set; }
    }
}