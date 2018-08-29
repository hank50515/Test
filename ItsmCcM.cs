using System;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
　
　
namespace GSS.ITSM.Model.Entities {
　
    public class ItsmCcM {
        public virtual string CcSerilNo { get; set; }
        public virtual string SchSerilNo { get; set; }
        public virtual string CcType { get; set; }
        public virtual string CcUsrType { get; set; }
        public virtual string CcOuId { get; set; }
        public virtual string CcRolId { get; set; }
        public virtual string CcUsrId { get; set; }
        public virtual string TenantId { get; set; }
        public virtual DateTime CreDte { get; set; }
        public virtual string CreUsr { get; set; }
        public virtual DateTime ModDte { get; set; }
        public virtual string ModUsr { get; set; }
    }
}