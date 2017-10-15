using Microsoft.WindowsAzure.MobileServices;
using System;

namespace GoodBuy.Model
{
    public interface IEntity
    {
        [Version]
        string Version { get; set; }
        string Id { get; set; }
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
        bool Deleted { get; set; }

    }
}