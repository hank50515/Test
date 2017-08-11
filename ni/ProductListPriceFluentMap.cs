using System; 
using System.Collections.Generic; 
using System.Text;
using FluentNHibernate.Mapping;
using GSS.ITSM.Model.Entities;

namespace NHinernateUnit {
    
    
    public class ScfunitemmMap : ClassMap<ProductListPriceHistory> {
        
        public ScfunitemmMap() {
			Table("ProductListPriceHistory");
			LazyLoad();         


        }
    }
}
