using GraphQL.Types;
using myMicroservice.Database.Entities;

namespace myMicroservice.GraphQL.Types
{
    public class UserType : ObjectGraphType<User>
    {
        public UserType()
        {
            Field(u => u.UserId);
            Field(u => u.Username);
            Field(u => u.Name);
            Field(u => u.Surname);
            //Field(u => u.HashedPassword);
            Field(u => u.Email);
            Field<ListGraphType<DeviceType>>(nameof(User.Devices));
        }
    }
}
