using System;
using Core.Application.Attribute;
using Core.Geo.ViewModel;
using Core.Users.Domain;

namespace Core.Geo.Impl
{
    [DefaultImplementation]
    public class PhoneFactory : IPhoneFactory
    {
        public Phone CreateDomain(PhoneEditModel model)
        {
            return new Phone
            {
                Id = model.Id,
                Number = model.PhoneNumber,
                TypeId = model.PhoneType,
                IsTransferable= model.IsTransferable
            };
        }

        public PhoneEditModel CreateEditModel(Phone domain,string phone=null)
        {
            return new PhoneEditModel
            {
                Id = domain.Id,
                //PhoneNumber = domain.Number,
                PhoneNumber = phone!=null? phone:domain.Number,
                PhoneType = domain.TypeId,
                IsTransferable= domain.IsTransferable,
                TempId= domain.Id
            };
        }


    }
}
