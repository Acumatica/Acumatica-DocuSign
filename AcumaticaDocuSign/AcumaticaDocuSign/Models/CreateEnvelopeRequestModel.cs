using System.Collections.Generic;
using PX.SM;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Contain properties which are needed for creating new request of 
    /// the <see cref="DocuSignEnvelopeInfo"/> envelope.
    /// </summary>
    public class CreateEnvelopeRequestModel: BaseRequestModel
    {
        public FileInfo FileInfo { get; set; }

        public DocuSignEnvelopeInfo EnvelopeInfo { get; set; }

        public List<DocuSignRecipient> Recipients { get; set; }

        public List<DocuSignRecipient> CarbonRecipients { get; set; }
    }
}
