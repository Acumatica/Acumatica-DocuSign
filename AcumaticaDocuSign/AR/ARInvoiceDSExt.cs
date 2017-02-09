using System;
using PX.Objects.AR;
using PX.Data;

namespace AcumaticaDocuSign
{
    public class ARInvoiceDSExt : PXCacheExtension<ARInvoice>
    {
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXSelector(typeof(Search<ARContact.contactID>))]
        public virtual Int32? BillContactID { get; set; }
    }
}