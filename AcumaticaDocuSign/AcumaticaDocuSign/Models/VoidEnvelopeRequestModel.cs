namespace AcumaticaDocuSign
{
    /// <summary>
    /// Contain properties which are needed for voiding request of 
    /// the <see cref="DocuSignEnvelopeInfo"/> envelope.
    /// </summary>
    public class VoidEnvelopeRequestModel : BaseRequestModel
    {
        public string VoidReason { get; set; }
    }
}
