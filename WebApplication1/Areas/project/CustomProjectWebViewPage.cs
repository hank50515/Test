using System.Web.Mvc;

namespace WebApplication1
{
    public abstract class CustomProjectWebViewPage : WebViewPage
    {
        public Html5Helper2 Html5 { get; set; }

        public override void InitHelpers()
        {
            base.InitHelpers();
            Html5 = new Html5Helper2(base.ViewContext, this);
        }
    }

    public abstract class CustomProjectWebViewPage<TModel> : WebViewPage<TModel>
    {
        public Html5Helper2 Html5 { get; set; }

        public override void InitHelpers()
        {
            base.InitHelpers();
            Html5 = new Html5Helper2(base.ViewContext, this);
        }
    }
}
