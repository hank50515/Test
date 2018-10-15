using System.Web;
using System.Web.Mvc;

public static class Html5Extensions
{
    public static IHtmlString GetMyName(this Html5Helper html)
    {
        var tagBuilder = new TagBuilder("span");
        tagBuilder.SetInnerText("Hello World");
        return new HtmlString(tagBuilder.ToString());
    }
}