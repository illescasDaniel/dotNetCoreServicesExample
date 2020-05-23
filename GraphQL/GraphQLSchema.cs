using GraphQL;
using GraphQL.Types;
namespace myMicroservice.GraphQL
{
    public class GraphQLSchema : Schema
    {
        public GraphQLSchema(IDependencyResolver resolver) : base(resolver)
        {
            Query = resolver.Resolve<GraphQLQuery>();
            // Mutation = ...
        }
    }
}
