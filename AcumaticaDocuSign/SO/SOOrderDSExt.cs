using System;
using PX.Data;
using PX.Objects.SO;

namespace AcumaticaDocuSign
{
    public class SOOrderPXExt : PXCacheExtension<SOOrder>
    {
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXSelector(typeof(Search<SOBillingContact.contactID>))]
        public int? BillContactID { get; set; }
    }
}