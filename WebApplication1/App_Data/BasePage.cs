public class BasePage : System.Web.UI.Page
{
    public BasePage()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
    }

    //建立 Show 方法，提供給繼承的頁面使用
    protected void Show() { Response.Write("BasePage" + "<br />"); }
 }