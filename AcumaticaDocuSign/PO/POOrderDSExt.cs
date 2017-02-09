using System;
using PX.Data;
using PX.Objects.PO;

namespace AcumaticaDocuSign
{
    public class POOrderDSExt : PXCacheExtension<POOrder>
    {
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXSelector(typeof(Search<PORemitContact.contactID>))]
        public virtual Int32? RemitContactID { get; set; }
    }
}
