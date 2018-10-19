using System.Web.Mvc;

namespace WebApplication1.Areas.project
{
    public class projectAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "project";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "project_default",
                "project/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}