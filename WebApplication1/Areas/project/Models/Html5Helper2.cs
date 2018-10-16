using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

public class Html5Helper2
{
    public Html5Helper2(ViewContext viewContext,
    IViewDataContainer viewDataContainer)
    : this(viewContext, viewDataContainer, RouteTable.Routes)
    {
    }
    public Html5Helper2(ViewContext viewContext,
    IViewDataContainer viewDataContainer, RouteCollection routeCollection)
    {
        ViewContext = viewContext;
        ViewData = new ViewDataDictionary(viewDataContainer.ViewData);
    }
    public ViewDataDictionary ViewData
    {
        get;
        private set;
    }
    public ViewContext ViewContext
    {
        get;
        private set;
    }

    public IHtmlString GetMyHelloWord()
    {
        var tagBuilder = new TagBuilder("span");
        tagBuilder.SetInnerText("Hello World");
        return new HtmlString(tagBuilder.ToString());
    }
}