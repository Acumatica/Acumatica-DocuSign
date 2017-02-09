using System.Collections;
using PX.Data;
using PX.Objects.CR;
using PX.SM;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Represent synchronization logic for <see cref="DocuSignEnvelopeInfo"/> envelopes.
    /// Records of this type can be viewed through the (DS 50.10.00) screen.
    /// </summary>
    public class DocuSignSyncProcess : PXGraph<DocuSignSyncProcess>
    {
        public PXCancel<DocuSignEnvelopeInfo> Cancel;

        [PXFilterable]
        public PXProcessingJoin<DocuSignEnvelopeInfo,
            InnerJoin<UploadFileRevision,
                On<UploadFileRevision.fileID, Equal<DocuSignEnvelopeInfo.fileID>,
                    And<UploadFileRevision.fileRevisionID, Equal<DocuSignEnvelopeInfo.fileRevisionID>>>>,
            Where<DocuSignEnvelopeInfo.lastStatus, NotEqual<Empty>,
                And<DocuSignEnvelopeInfo.lastStatus, NotEqual<envelopeStatusCompleted>,
                    And<DocuSignEnvelopeInfo.lastStatus, NotEqual<envelopeStatusVoided>,
                        And<DocuSignEnvelopeInfo.isFinalVersion, NotEqual<True>>>>>> Envelopes;

        public virtual IEnumerable envelopes()
        {
            var query = PXSelectJoin<DocuSignEnvelopeInfo,
                InnerJoin<UploadFileRevision,
                    On<UploadFileRevision.fileID, Equal<DocuSignEnvelopeInfo.fileID>,
                        And<UploadFileRevision.fileRevisionID, Equal<DocuSignEnvelopeInfo.fileRevisionID>>>>,
                Where<DocuSignEnvelopeInfo.lastStatus, NotEqual<Empty>,
                    And<DocuSignEnvelopeInfo.lastStatus, NotEqual<envelopeStatusCompleted>,
                        And<DocuSignEnvelopeInfo.lastStatus, NotEqual<envelopeStatusVoided>,
                            And<DocuSignEnvelopeInfo.isFinalVersion, NotEqual<True>>>>>>
                            .Select(this, EnvelopeStatus.Completed, EnvelopeStatus.Voided);

            return query;
        }

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute),
                                  "DisplayName",
            Messages.DocuSignEnvelopeDocusignAccount)]
        protected virtual void DocuSignEnvelopeInfo_DocuSignAccountID_CacheAttached(PXCache sender)
        {
        }

        public DocuSignSyncProcess()
        {
            Envelopes.SetSelected<DocuSignEnvelopeInfo.selected>();
            Envelopes.SetProcessDelegate<WikiFileMaintenance>((graph, envelope) =>
            {
                var ext = graph.GetExtension<WikiFileMaintenanceDSExt>();
                var uploadGraph = CreateInstance<UploadFileMaintenance>();
                var taskGraph = CreateInstance<CRTaskMaint>();
                ext.CheckStatus(graph, envelope, uploadGraph, taskGraph);
            });
        }
    }
}
