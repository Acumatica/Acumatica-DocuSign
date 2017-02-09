using DocuSign.eSign.Model;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Contain properties which are needed for creating new updating request 
    /// of the <see cref="DocuSignEnvelopeInfo"/> envelope.
    /// </summary>
    public class UpdateEnvelopeResponseModel
    {
        public EnvelopeUpdateSummary Envelope { get; set; }

        public ViewUrl ViewUrl { get; set; }
    }
}
