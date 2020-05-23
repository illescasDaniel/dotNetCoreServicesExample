using Microsoft.AspNet.OData;

namespace myMicroservice.Helpers
{
    public static class OdataControllerExtensions
    {
        public static bool IsAuthenticated(this ODataController oDataController)
        {
            return oDataController.HttpContext.User.Identity.IsAuthenticated;
        }
    }
}
