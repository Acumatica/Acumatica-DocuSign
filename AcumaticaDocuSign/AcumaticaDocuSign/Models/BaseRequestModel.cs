using DocuSign.eSign.Model;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Contain base properties for crud operations of the<see cref="EnvelopeDocument"/> document.
    /// </summary>
    public class BaseRequestModel
    {
        public DocuSignAccount DocuSignAccount { get; set; }

        public string EnvelopeId { get; set; }
    }
}
