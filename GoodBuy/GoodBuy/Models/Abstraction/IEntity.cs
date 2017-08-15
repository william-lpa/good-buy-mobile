using GoodBuy.Service;

namespace GoodBuy.Model
{
    public interface IEntity
    {
        string Id { get; set; }
        void LoadRelationalEntitiesCollections(AzureService entityService);
    }
}