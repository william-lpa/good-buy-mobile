using GoodBuy.Models;
using System;

namespace GoodBuy.Service
{
    public interface IContactListService
    {
        void PickContactList(Action<User> callback);
        void ResolveContact(object contactResource);
        string TryGetPhoneNumber { get; }
        User PickProfileUser();

    }
}
