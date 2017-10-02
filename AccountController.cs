using MvcSiteMapProvider.Web.Mvc.Filters;
using Newtonsoft.Json;
using POSTCP.Models;
using POSTCP.Services.Interface.Common;
using POSTCP.Web.Helper;
using POSTCP.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Security.AntiXss;
　
namespace POSTCP.Web.Controllers
{
    public class AccountController : BaseController
    {
        string logoutUrl = ConfigurationManager.AppSettings["SSOLogOutUrl"];
        ICommonService commonService;
        IT_UserInfoService userInfoService;
        IT_LOGlogService loglogService;
　
        public AccountController(IT_UserInfoService userInfoService, IT_LOGlogService loglogService, ICommonService commonService)
        {
            this.userInfoService = userInfoService;
            this.loglogService = loglogService;
            this.commonService = commonService;
        }
　
        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            //如果已登入過，則導回首頁。
            if (UserHelper.IsAuthenticated()) return RedirectToLocal(string.Empty);
　
            //如果是來源為ajax，表示該使用者已被登出，則需重新登入
            if (Request.IsAjaxRequest()) return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
　
            return View();
        }
　
        [AllowAnonymous]
        [SiteMapCacheRelease]
        public ActionResult SSOLogin(string strUsrID, string strCompID, string strAPID, string strSID)
        {
            if (!string.IsNullOrEmpty(strUsrID))
            {
                //test url http://localhost:1039/Account/SSOLogin?strUsrID=273872&strCompID=1&strAPID=1&strSID=1
                strUsrID = AntiXssEncoder.HtmlEncode(strUsrID, true);
　
                var sso = new SSO.ScUser();
                bool loginResult = sso.ChkSessionID("GSSSC30", strUsrID, strCompID, strAPID, strSID, string.Empty);
                bool haveMenu = (loginResult) ? commonService.CheckUserHaveMenu(strUsrID) : false;
　
                if (loginResult && haveMenu)
                {
                    LoginViewModel viewModel = new LoginViewModel()
                    {
                        UserID = strUsrID
                    };
　
                    Dictionary<string, string> loginInfo = new Dictionary<string, string>()
                    {
                        { "strUsrID", strUsrID },
                        { "strCompID", strCompID },
                        { "strAPID", strAPID },
                        { "strSID", strSID }
                    };
　
                    Session["LoginInfo"] = loginInfo;
　
                    SetLoginCookie(strUsrID);
　
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    T_UserInfo user = new T_UserInfo()
                    {
                        User_ID = strUsrID,
                        Unit_ID = string.Empty
                    };
　
                    InsertLoglog(user, "F");
                }
            }           
　
            ContentResult result = new ContentResult()
            {
                Content = string.Format("<body></body><script type=\"text/javascript\"> alert('無此系統權限'); document.body.insertAdjacentHTML( 'beforeend', \"{0}\"); document.getElementById(\"SSOLogOutForm\").submit(); </script>", GetSSOLogoutForm(strUsrID, strCompID, strAPID, strSID)),
                ContentType = "text/html"
            };
　
            return result;
        }
　
        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [SiteMapCacheRelease]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.UserID))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
　
                T_UserInfo user = userInfoService.Get(model.UserID);
                bool haveMenu = (user != null) ? commonService.CheckUserHaveMenu(user.User_ID) : false;
　
                if (user != null && haveMenu)
                {
                    SetLoginCookie(user);
　
                    return RedirectToLocal(returnUrl);
                }
                else
                {
                    ModelState.AddModelError("", "無使用系統權限");
                }
            }
　
            // 如果執行到這裡，發生某項失敗，則重新顯示表單
            return View(model);
        }
　
        //
        // POST: /Account/LogOff
        [HttpPost]
        [SiteMapCacheRelease]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            ActionResult result = RedirectToAction("Main", "Home");
　
            InsertLoglog(null, "O");
　
            //登出表單驗證
            FormsAuthentication.SignOut();
　
            if (SiteHelper.IsLoginBackDoor().Equals(false))
            {
                result = new ContentResult()
                {
                    Content = string.Format("<body></body><script type=\"text/javascript\"> document.body.insertAdjacentHTML( 'beforeend', \"{0}\"); document.getElementById(\"SSOLogOutForm\").submit(); </script>", GetSSOLogoutForm()),
                    ContentType = "text/html"
                };
　
            }
　
            //清除Session中的資料
            Session.Abandon();
　
            //清除 session id
            Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
　
            return result;
        }
　
        //
        // POST: /Account/UnloadPage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public void UnloadPage()
        {
            if (UserHelper.IsAuthenticated())
            {
                //登出表單驗證
                FormsAuthentication.SignOut();
　
                InsertLoglog(null, "X");
　
                if (SiteHelper.IsLoginBackDoor().Equals(false))
                {
                    using (WebClient wc = new WebClient())
                    {
                        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(logoutUrl);
　
                        request.Method = "POST";
                        request.ContentType = "application/x-www-form-urlencoded";
　
                        string param = GetSSOLogoutUrlParameter();
                        byte[] bs = Encoding.ASCII.GetBytes(param);
　
                        using (Stream reqStream = request.GetRequestStream())
                        {
                            reqStream.Write(bs, 0, bs.Length);
                        }
　
                        using (WebResponse response = request.GetResponse())
                        {
                            StreamReader sr = new StreamReader(response.GetResponseStream());
                            //Debug.WriteLine(sr.ReadToEnd());
                            sr.Close();
                        }
                    }
                }
　
                //清除Session中的資料
                Session.Abandon();
　
                //清除 session id
                Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            }
        }
　
        #region Private Method
　
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Main", "Home");
            }
        }
　
        /// <summary>
        /// 新增登入紀錄
        /// </summary>
        /// <param name="user">使用者資訊物件</param>
        /// <param name="code">代碼</param>
        private void InsertLoglog(T_UserInfo user, string code)
        {
            if (user == null) user = UserHelper.GetLoginUser();
　
            T_LOGlog log = new T_LOGlog()
            {
                IP_Address = UserHelper.GetUserIP(),
                Log_Code = code,
                Log_DateTime = DateTime.Now,
                Unit_ID = user.Unit_ID,
                User_ID = user.User_ID,
                Session_ID = Session.SessionID
            };
　
            loglogService.Insert(log);
        }
　
        /// <summary>
        /// 取得 SSO 登出用的 Url參數
        /// </summary>
        /// <returns></returns>
        private string GetSSOLogoutUrlParameter()
        {
            Dictionary<string, string> loginInfo = (Dictionary<string, string>)Session["LoginInfo"];
　
            return string.Format("strUSRID={0}&strCOMPID={1}&strAPID={2}&strSID={3}", loginInfo["strUsrID"], loginInfo["strCompID"], loginInfo["strAPID"], loginInfo["strSID"]);
        }
　
        private string GetSSOLogoutForm()
        {
            Dictionary<string, string> loginInfo = (Dictionary<string, string>)Session["LoginInfo"];
　
            return GetSSOLogoutForm(loginInfo["strUsrID"], loginInfo["strCompID"], loginInfo["strAPID"], loginInfo["strSID"]);
        }
　
        private string GetSSOLogoutForm(string strUsrID, string strCompID, string strAPID, string strSID)
        {
            return string.Format("<form action='{0}' target = '_parent' style = 'display: none;' method = 'post' id= 'SSOLogOutForm'><input type = 'hidden' name = 'strUSRID' value = '{1}' /> <input type = 'hidden' name = 'strCOMPID' value= '{2}'/><input type = 'hidden' name = 'strAPID' value= '{3}'/><input type = 'hidden' name = 'strSID' value= '{4}'/></form>", logoutUrl, strUsrID, strCompID, strAPID, strSID);
        }
　
        private void SetLoginCookie(string userID)
        {
            T_UserInfo user = userInfoService.Get(userID);
　
            SetLoginCookie(user);
        }
　
        private void SetLoginCookie(T_UserInfo user)
        {
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(
                1,
                user.User_ID,
                DateTime.Now,
                DateTime.Now.AddMinutes(FormsAuthentication.Timeout.TotalMinutes),
                false, // 將管理者登入的 Cookie 設定成 Session Cookie
                JsonConvert.SerializeObject(user),
                FormsAuthentication.FormsCookiePath);
　
            string encTicket = FormsAuthentication.Encrypt(ticket);
　
            Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
            InsertLoglog(user, "I");
        }
　
        #endregion
    }
}