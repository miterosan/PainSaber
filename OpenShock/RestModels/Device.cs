namespace PainSaber.OpenShock.RestModels
{
    public class Device
    {
        public string name { get; set; }
        public string id { get; set; }
        public string createdOn { get; set; }

        public Shocker[] shockers { get;set; }
    }
}