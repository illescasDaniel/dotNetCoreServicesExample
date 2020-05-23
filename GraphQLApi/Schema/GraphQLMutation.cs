using System;
using System.Security.Claims;
using GraphQL;
using GraphQL.Types;
using myMicroservice.Database;
using myMicroservice.Database.Entities;
using myMicroservice.GraphQLApi.Types;
using myMicroservice.Helpers;

namespace myMicroservice.GraphQLApi.Schema
{
    public class GraphQLMutation : ObjectGraphType
    {
        public GraphQLMutation(DatabaseContext dbContext, IUserAuthenticationService authenticationService)
        {
            FieldAsync<UserType>(
                "updateUser",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id" },
                    new QueryArgument<NonNullGraphType<UserInputType>> { Name = "user" }
                ),
                resolve: async context =>
                {
                    CheckAuthentication(context, authenticationService);

                    var userId = context.GetArgument<int>("id");

                    User? user = await dbContext.Users.FindAsync(userId);

                    if (user == null)
                    {
                        throw new ExecutionError("User not found");
                    }

                    var userInput = context.GetArgument<UserInputDto>("user");

                    if (user.Name != userInput.Name && userInput.Name != null)
                    {
                        user.Name = userInput.Name;
                    }
                    if (user.Surname != userInput.Surname && userInput.Surname != null)
                    {
                        user.Surname = userInput.Surname;
                    }
                    if (user.Email != userInput.Email && userInput.Email != null)
                    {
                        user.Email = userInput.Email;
                    }

                    await dbContext.SaveChangesAsync();

                    return user;
                }
            );
        }

        //

        private void CheckAuthentication(ResolveFieldContext<object> context, IUserAuthenticationService authenticationService)
        {
            if (!authenticationService.IsAuthenticated((ClaimsPrincipal)context.UserContext))
            {
                throw new ExecutionError("User not authenticated");
            }
        }
    }
}
