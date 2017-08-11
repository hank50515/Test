using System; 
using System.Collections.Generic; 
using System.Text;
using FluentNHibernate.Mapping;
using GSS.ITSM.Model.Entities;

namespace NHinernateUnit {
    
    
    public class ProductListPriceFluentMap : ClassMap<ProductListPriceHistory> {
        
        public ProductListPriceFluentMap() {
			Table("ProductListPriceHistory");
			LazyLoad();         


        }
    }
}
