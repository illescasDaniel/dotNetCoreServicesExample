using System;
using GraphQL.Types;
using myMicroservice.Database.Entities;

namespace myMicroservice.GraphQL.Types
{
    public class DeviceType : ObjectGraphType<Device>
    {
        public DeviceType()
        {
            Field(d => d.DeviceId);
            Field(d => d.Name);
            Field(d => d.Version);
            Field(d => d.OwnerUserId);
            Field<UserType>(nameof(Device.Owner));
        }
    }
}
