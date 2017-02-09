using PX.Data;
using PX.SM;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Represent inqury logic fo the <see cref="DocuSignRecipient"/> recipients
    /// Records of this type can be viewed through the (DS.30.30.00) screen.
    /// </summary>
    public class DocuSignEnvelopeEnq : PXGraph<DocuSignEnvelopeEnq>
    {
        [PXFilterable]
        public PXSelectJoin<DocuSignEnvelopeInfo, 
            InnerJoin<UploadFileWithIDSelector, 
                On<UploadFileWithIDSelector.fileID, Equal<DocuSignEnvelopeInfo.fileID>>>> Envelopes;
        public PXSelectReadonly<DocuSignRecipient, 
            Where<DocuSignRecipient.envelopeInfoID, 
                Equal<Current<DocuSignEnvelopeInfo.envelopeInfoID>>,
                And<DocuSignRecipient.status, IsNotNull>>> Recipients;
    }
}
