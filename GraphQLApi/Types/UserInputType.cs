using GraphQL.Types;

namespace myMicroservice.GraphQLApi.Types
{
    public class UserInputType : InputObjectGraphType<UserInputDto>
    {
        public UserInputType()
        {
            Name = "UserInput";
            Field(u => u.Name, nullable: true);
            Field(u => u.Surname, nullable: true);
            Field(u => u.Email, nullable: true);
        }
    }
}
