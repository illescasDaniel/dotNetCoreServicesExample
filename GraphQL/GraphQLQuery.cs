using System.Linq;
using GraphQL.Types;
using Microsoft.EntityFrameworkCore;
using myMicroservice.Database;
using myMicroservice.GraphQL.Types;

namespace myMicroservice.GraphQL
{
    public class GraphQLQuery : ObjectGraphType
    {

        public GraphQLQuery(DatabaseContext dbContext)
        {
            // User by id
            FieldAsync<UserType>(
                "User",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id", Description = "User id" }
                ),
                resolve: async context =>
                {
                    var id = context.GetArgument<int>("id");
                    var user = await dbContext.Users.FindAsync(id);
                    return user;
                }
            );

            // Users
            FieldAsync<ListGraphType<UserType>>(
                "Users",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "limit", Description = "Number of users returned", DefaultValue = 10 }
                ),
                resolve: async context =>
                {
                    var limit = context.GetArgument<int>("limit", defaultValue: 10);
                    var user = await dbContext.Users
                                //.Include(u => u.Devices) // looks like we don't need to
                                .OrderBy(u => u.Username)
                                .Take(limit)
                                .ToListAsync();

                    return user;
                }
            );

            // Device by id
            FieldAsync<UserType>(
                "Device",
                arguments: new QueryArguments(
                    new QueryArgument<NonNullGraphType<IdGraphType>> { Name = "id", Description = "Device id" }
                ),
                resolve: async context =>
                {
                    var id = context.GetArgument<int>("id");
                    var user = await dbContext.Devices.FindAsync(id);
                    return user;
                }
            );

            // Devices
            FieldAsync<ListGraphType<UserType>>(
                "Devices",
                arguments: new QueryArguments(
                    new QueryArgument<IdGraphType> { Name = "limit", Description = "Number of devices returned", DefaultValue = 10 }
                ),
                resolve: async context =>
                {
                    var limit = context.GetArgument<int>("limit", defaultValue: 10);
                    var user = await dbContext.Devices
                                //.Include(u => u.Devices) // looks like we don't need to
                                .OrderBy(d => d.Name)
                                .Take(limit)
                                .ToListAsync();

                    return user;
                }
            );
        }
    }
}
