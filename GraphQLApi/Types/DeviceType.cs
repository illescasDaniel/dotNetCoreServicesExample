using myMicroservice.GraphQLApi.Types;
using myMicroservice.Database.Entities;
using GraphQL.Types;

namespace myMicroservice.GraphQLApi.Types
{
    public class DeviceType : ObjectGraphType<Device>
    {
        public DeviceType()
        {
            Name = "Device";
            Field(d => d.DeviceId);
            Field(d => d.Name);
            Field(d => d.Version);
            Field(d => d.OwnerUserId);
            Field<UserType>(nameof(Device.Owner));
        }
    }
}
