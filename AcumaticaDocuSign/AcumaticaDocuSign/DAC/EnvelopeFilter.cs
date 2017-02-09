using System;
using PX.Data;
using PX.TM;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Represents envelope <see cref="DocuSignEnvelopeInfo"/> owner filter for 
    /// DocuSign Central (DS 40.10.00) screen.
    /// </summary>
    [Serializable]
    public class EnvelopeFilter : IBqlTable
    {
        #region MyOwner
        public abstract class myOwner : IBqlField
        {
        }
        protected bool? _MyOwner;

        /// <summary>
        /// Defined if rows should be filtered with current owner.
        /// </summary>
        [PXBool]
        [PXDefault(true)]
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeFilterMe)]
        public virtual bool? MyOwner
        {
            get
            {
                return this._MyOwner;
            }
            set
            {
                this._MyOwner = value;
            }
        }
        #endregion
        #region OwnerID
        public abstract class ownerID : IBqlField
        {
        }
        protected Guid? _OwnerID;

        /// <summary>
        /// Defined owner parameter of the filter.
        /// </summary>
        [PXDBGuid]
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeFilterOwner, Enabled = false)]
        [PXOwnerSelector]
        public virtual Guid? OwnerID
        {
            get
            {
                return _MyOwner.HasValue && _MyOwner.Value
                    ? CurrentOwnerID
                    : _OwnerID;
            }
            set
            {
                this._OwnerID = value;
            }
        }
        #endregion
        #region CurrentOwnerID
        public abstract class currentOwnerID : IBqlField
        {
        }
        protected Guid? _CurrentOwnerID;

        /// <summary>
        /// Defined current owner.
        /// </summary>
        [PXGuid]
        [PXDefault(typeof(AccessInfo.userID))]
        public virtual Guid? CurrentOwnerID
        {
            get
            {
                return this._CurrentOwnerID;
            }
            set
            {
                this._CurrentOwnerID = value;
            }
        }
        #endregion
    }
}