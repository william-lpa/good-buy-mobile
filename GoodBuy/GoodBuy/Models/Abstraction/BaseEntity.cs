using Newtonsoft.Json;
using GoodBuy.Service.Interfaces;
using GoodBuy.Service;

namespace GoodBuy.Model
{
    public abstract class BaseEntity : IEntity
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        //public BaseEntity() => LoadRelationalEntitiesCollections() singleton no azure Service?

        public virtual void LoadRelationalEntitiesCollections(AzureService entityService) { }




    }
}