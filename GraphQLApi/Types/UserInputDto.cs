namespace myMicroservice.GraphQLApi.Types
{
    public class UserInputDto
    {

        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }

        public UserInputDto()
        {
        }
    }
}
