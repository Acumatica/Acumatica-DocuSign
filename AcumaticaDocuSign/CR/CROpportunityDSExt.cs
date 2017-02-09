using System;
using PX.Data;
using PX.Objects.CR;

namespace AcumaticaDocuSign
{
    public class CROpportunityDSExt : PXCacheExtension<CROpportunity>
    {
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXSelector(typeof(CRContact.contactID))]
        public virtual Int32? OpportunityContactID { get; set; }
    }
}