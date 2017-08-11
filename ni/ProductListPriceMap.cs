using GSS.Radar.Domain.Models.Base.Map;
using NHibernate.Mapping.ByCode;

namespace NHinernateUnit
{
    public class ProductListPriceMap : ClassMappingBase<ProductListPrice>
    {
        public override void Mapping()
        {
            Id(x => x.ProductID, map => map.Generator(Generators.Identity));

            Property(x => x.StartDate);

            base.Mapping();
        }
    }
}