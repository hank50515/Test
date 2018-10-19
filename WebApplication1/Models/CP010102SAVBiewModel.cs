using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
　
namespace POSTCP.Web.ViewModels.CP01.CP0101SAV
{
    public class CP010102SAVBiewModel
    {
        /// <summary>
        /// 報表日期
        /// </summary>
        [Display(Name = "報表日期")]
        public string FLD00001 { get; set; }
　
        /// <summary>
        /// 電腦局號
        /// </summary>
        [Required]
        [Display(Name = "電腦局號", Prompt = "6位數字")]
        [StringLength(6)]
        [RegularExpression("\\d{6}", ErrorMessageResourceName = "InValid6Digits")]
        public string FLD00007 { get; set; }
　
        /// <summary>
        /// 交易代號
        /// </summary>
        [Required]
        [Display(Name = "交易代號", Prompt = "4位英數字")]
        [StringLength(4)]
        [RegularExpression("[a-zA-Z0-9]{4}", ErrorMessageResourceName = "InValid4Characters")]
        public string FLD00011 { get; set; }
　
        /// <summary>
        /// 交易類別
        /// </summary>
        [Required]
        [Display(Name = "交易類別")]
        public string FLD00004 { get; set; }
　
        /// <summary>
        /// 當日/次日
        /// </summary>
        [Required]
        [Display(Name = "本日/次日")]
        public string FLD00006 { get; set; }
　
        /// <summary>
        /// 起始帳號
        /// </summary>
        [Display(Name = "起始帳號", Prompt = "6位數字以內")]
        [StringLength(6)]
        [RegularExpression("\\d{0,6}", ErrorMessageResourceName = "InValidLessThan6Digits")]
        public string FLD00008Start { get; set; }
　
        /// <summary>
        /// 截止帳號
        /// </summary>
        [Display(Name = "截止帳號", Prompt = "6位數字以內")]
        [StringLength(6)]
        [RegularExpression("\\d{0,6}", ErrorMessageResourceName = "InValidLessThan6Digits")]
        public string FLD00008End { get; set; }
　
    }
}