using System;
using System.Collections.Generic;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Crypto.Encodings;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.Net.Http;

namespace Hotains.Controllers
{
    public class SSOController : BaseController
    {
        /// <summary>
        /// SSO 登入
        /// </summary>
        /// <param name="method">頁面名稱</param>
        /// <param name="account">登入人員身份證字號</param>
        /// <param name="agentName">登入人員姓名</param>
        /// <returns></returns>
        public string Login(string method, string account, string agentName, string channelCode)
        {
            string Msg = string.Empty;

            try
            {
                if (decrypt(account).IndexOf("Error") > 0)
                    Msg += "帳號解密失敗,";
                if (decrypt(agentName).IndexOf("Error") > 0)
                    Msg += "姓名解密失敗";

                string strURL = "https://" + Request.Url.Authority + "/SSO/Redirct?" +
                    "page=" + method +
                    "&account=" + Url.Encode(account) +
                    "&agentName=" + Url.Encode(agentName) +
                    "&channelCode=" + Url.Encode(channelCode);

                var Result = new
                {
                    jsonStatus = "success",
                    jsonResultData = strURL,
                    jsonMsg = "",
                };

                logger.Info($"Method:{method} Account:{account} AgentName:{agentName} ChannelCode:{channelCode} Status:Success URL:{strURL} ");
                return JsonConvert.SerializeObject(Result);
            }
            catch (Exception)
            {
                var Result = new
                {
                    jsonStatus = "error",
                    jsonResultData = "",
                    jsonMsg = Msg,
                };

                logger.Info($"Method:{method} Account:{account} AgentName:{agentName} Status:Error");
                return JsonConvert.SerializeObject(Result);
            }
        }

        /// <summary>
        /// Login回覆給SDS後，SDS執行導頁到此函式
        /// </summary>
        /// <returns></returns>
        public ActionResult Redirct()
        {
            string Msg = string.Empty;
            string strAccount = Request.QueryString["account"];
            string strAgentName = Request.QueryString["agentName"];
            string strPage = Request.QueryString["page"];
            string strChannelCode = Request.QueryString["channelCode"];
            string beforeaccount = strAccount;  //20180820 ADD BY WS-MICHAEL [解密前帳號]

            if (strAccount != null && strAgentName != null)
            {
                strAccount = decrypt(strAccount);
                strAgentName = decrypt(strAgentName);
                strChannelCode = decrypt(strChannelCode);

                if (strAccount.IndexOf("Error") > 0)
                    Msg += "帳號解密失敗,";
                if (strAgentName.IndexOf("Error") > 0)
                    Msg += "姓名解密失敗";
            }

            if (Msg == string.Empty)
            {
                Session["account"] = strAccount;
                Session["channelCode"] = strChannelCode;
                Session["agentName"] = strAgentName;
            }

            string strController = string.Empty;
            string strActionName = string.Empty;

            switch (strPage)
            {
                case "PAYMENT":
                    strController = "Payment";
                    strActionName = "PaymentIndex";
                    break;
                case "QUOTE":
                    strController = "Quote";
                    strActionName = "QuoteIndex";
                    getAgentList(beforeaccount);    //20180820 ADD BY WS-MICHAEL 取得[經手人代碼]及[經手人單位別]
                    break;
                case "PAADVPAY":
                    strController = "PAADVPAY";
                    strActionName = "PaAdvPayIndex";
                    getAgentList(beforeaccount);    
                    break;
                default:
                    strController = "Payment";
                    strActionName = "PaymentIndex";
                    break;
            }

            logger.Info($"Page:{strPage} Account:{strAccount} AgentName:{strAgentName}  ChannelCode:{strChannelCode}");
            return RedirectToAction(strActionName, strController);
        }

        /// <summary>
        /// 網址參數解密
        /// </summary>
        /// <param name="DecryptText"></param>
        /// <returns></returns>
        public string decrypt(string DecryptText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(DecryptText);

            PemReader pr = new PemReader(
                (StreamReader)System.IO.File.OpenText(Server.MapPath(@"~/private.pem"))
            );

            AsymmetricCipherKeyPair keys = (AsymmetricCipherKeyPair)pr.ReadObject();

            OaepEncoding eng = new OaepEncoding(new RsaEngine());
            eng.Init(false, keys.Private);

            int length = cipherTextBytes.Length;
            int blockSize = eng.GetInputBlockSize();
            List<byte> plainTextBytes = new List<byte>();
            for (int chunkPosition = 0;
                chunkPosition < length;
                chunkPosition += blockSize)
            {
                int chunkSize = Math.Min(blockSize, length - chunkPosition);
                plainTextBytes.AddRange(eng.ProcessBlock(
                    cipherTextBytes, chunkPosition, chunkSize
                ));
            }
            return Encoding.UTF8.GetString(plainTextBytes.ToArray());
        }

        /// <summary>
        /// 取得[經手人代碼]及[經手人單位別]
        /// </summary>
        /// <param name="beforeAccount">帳號</param>
        /// <returns>Josn格式</returns>
        public string getAgentList(string beforeAccount)
        {
            try
            {
                Session["beforeAccount"] = beforeAccount;
                string url = System.Configuration.ConfigurationManager.AppSettings["SDS_AGENTUA_URL"];
                var sResult = "";
                using (HttpClient client = new HttpClient())
                {
                    HttpContent contentPost = new StringContent(beforeAccount, System.Text.Encoding.UTF8, "text/plain");
                    // 發出 post 並取得結果
                    HttpResponseMessage response = client.PostAsync(url, contentPost).Result;
                    // 將回應結果內容取出並轉為 string
                    sResult = response.Content.ReadAsStringAsync().Result;
                }

                string[] sResultArr = sResult.Split(',');
                //經手人
                string agentCodeList = sResultArr[0].Substring(sResultArr[0].IndexOf(":") + 3, sResultArr[0].Length - sResultArr[0].IndexOf(":") - 4);
                //經手人單位別
                string branchCodeList = sResultArr[1].Substring(sResultArr[1].IndexOf(":") + 3, sResultArr[1].Length - sResultArr[1].IndexOf(":") - 4);
                //經手人ID
                string agentIdList = sResultArr[2].Substring(sResultArr[2].IndexOf(":") + 3, sResultArr[2].Length - sResultArr[2].IndexOf(":") - 7);
                //sResult = sResult.Replace("\\\\","");
                //DAO_Quotation.SDSParam Introduction = JsonConvert.DeserializeObject<DAO_Quotation.SDSParam>(sResult.ToString());
                //List<DAO_Quotation.SDSParam> Introduction2 = JsonConvert.DeserializeObject<List<DAO_Quotation.SDSParam>>(sResult);                
                agentCodeList = decrypt(agentCodeList.Replace("\\", ""));
                branchCodeList = decrypt(branchCodeList.Replace("\\", ""));
                agentIdList = decrypt(agentIdList.Replace("\\", ""));
                Session["agentCodeList"] = agentCodeList;
                Session["branchCodeList"] = branchCodeList;
                Session["agentIdList"] = agentIdList;
                //==[續保資料]處理====================================================================
                url = System.Configuration.ConfigurationManager.AppSettings["SDS_RENEWAGENTUA_URL"];
                sResult = "";
                using (HttpClient client = new HttpClient())
                {
                    HttpContent contentPost = new StringContent(beforeAccount, System.Text.Encoding.UTF8, "text/plain");
                    // 發出 post 並取得結果
                    HttpResponseMessage response = client.PostAsync(url, contentPost).Result;
                    // 將回應結果內容取出並轉為 string
                    sResult = response.Content.ReadAsStringAsync().Result;
                }
                sResult = "[" + sResult.Replace("\\\"", "'").Replace("\"", "").Replace("\\", "") + "]";
                System.Data.DataTable dt = JsonConvert.DeserializeObject<System.Data.DataTable>(sResult);
                if (dt.Rows.Count > 0)
                {
                    Session["salesReCodeList"] = decrypt(dt.Rows[0]["salesCodeList"].ToString());
                    Session["agentReCodeList"] = decrypt(dt.Rows[0]["agentCodeList"].ToString());
                }
                return "";
            }

            catch (Exception e)
            {
                return e.ToString();
            }
        }
    }
}