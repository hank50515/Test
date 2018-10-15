using System.Web.Mvc;

namespace WebApplication1
{
    public abstract class CustomWebViewPage : WebViewPage
    {
        public string Title
        {
            get;
            set;
        } 

        public Html5Helper Html5 { get; set; }

        public override void InitHelpers()
        {
            base.InitHelpers();
            Html5 = new Html5Helper(base.ViewContext, this);
        }
    }

    public abstract class CustomWebViewPage<TModel> : WebViewPage<TModel>
    {
        public Html5Helper Html5 { get; set; }

        public string Title
        {
            get;
            set;
        }

        public override void InitHelpers()
        {
            base.InitHelpers();
            Html5 = new Html5Helper(base.ViewContext, this);
        }
    }
}
