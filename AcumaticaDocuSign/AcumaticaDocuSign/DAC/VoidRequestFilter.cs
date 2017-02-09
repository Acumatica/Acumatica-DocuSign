using System;
using PX.Data;

namespace AcumaticaDocuSign
{
    [Serializable]
    public class VoidRequestFilter : IBqlTable
    {
        #region VoidReason
        public abstract class voidReason : IBqlField
        {
        }
        protected string _VoidReason;

        /// <summary>
        /// Specifies the void reason for the <see cref="DocuSignEnvelopeInfo"/> envelope.
        /// </summary>
        [PXString(IsUnicode = true)]
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeVoidReason)]
        public virtual string VoidReason
        {
            get
            {
                return this._VoidReason;
            }
            set
            {
                this._VoidReason = value;
            }
        }
        #endregion
    }
}
