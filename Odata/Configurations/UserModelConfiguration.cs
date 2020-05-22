using System;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using myMicroservice.Database.Entities;

namespace myMicroservice.Database.Odata.Configurations
{
    public class UserModelConfiguration : IModelConfiguration
    {
        /// <summary>
        /// Applies model configurations using the provided builder for the specified API version.
        /// </summary>
        /// <param name="builder">The <see cref="ODataModelBuilder">builder</see> used to apply configurations.</param>
        /// <param name="apiVersion">The <see cref="ApiVersion">API version</see> associated with the <paramref name="builder"/>.</param>
        public void Apply(ODataModelBuilder builder, ApiVersion apiVersion)
        {

            if (apiVersion > new ApiVersion(2, 0)) { return; }

            //userCompleConfig.ComplexProperty(user => c.P2);
            //userCompleConfig.ComplexProperty(user => c.P2);

            var userEntityConfig = builder.EntitySet<User>("Users").EntityType;
            userEntityConfig.HasKey(u => u.UserId)
                            .HasMany(user => user.Devices);

            userEntityConfig.Select().Expand().Count().Filter().OrderBy().Page();

            //person.HasKey(p => p.Id);
            //person.Select().OrderBy("firstName", "lastName");

            //if (apiVersion < ApiVersions.V3)
            //{
            //    person.Ignore(p => p.Phone);
            //}

            //if (apiVersion <= ApiVersions.V1)
            //{
            //    person.Ignore(p => p.Email);
            //}

            //if (apiVersion > ApiVersions.V1)
            //{
            //    person.ContainsOptional(p => p.HomeAddress);
            //    person.Ignore(p => p.WorkAddress);

            //    var function = person.Collection.Function("NewHires");

            //    function.Parameter<DateTime>("Since");
            //    function.ReturnsFromEntitySet<Person>("People");
            //}

            //if (apiVersion > ApiVersions.V2)
            //{
            //    person.ContainsOptional(p => p.WorkAddress);
            //    person.Action("Promote").Parameter<string>("title");
            //}
        }
    }
}
