using System;
using PX.Data;
using PX.SM;
using PX.TM;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Represents the mapping rule to <see cref="DocuSignAccount">docusign account</see>
    /// and <see cref="Users">Acumatica user</see>.
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.DocuSignAccountUserRule)]
    public class DocuSignAccountUserRule : IBqlTable
    {
        #region AccountID
        public abstract class accountID : IBqlField
        {
        }
        protected int? _AccountID;

        /// <summary>
        /// Unique identifier of the docusign account.
        /// </summary>
        [PXDBInt(IsKey = true)]
        [PXDBDefault(typeof(DocuSignAccount.accountID))]
        [PXParent(
            typeof(Select<DocuSignAccount,
                    Where<DocuSignAccount.accountID,
                        Equal<Current<DocuSignAccountUserRule.accountID>>>>))]
        public virtual int? AccountID
        {
            get
            {
                return this._AccountID;
            }
            set
            {
                this._AccountID = value;
            }
        }
        #endregion
        #region OwnerID
        public abstract class ownerID : IBqlField
        {
        }
        protected Guid? _OwnerID;

        /// <summary>
        /// Default acumatica user related to docusign account.
        /// </summary>
        [PXDBGuid(IsKey = true)]
        [PXDefault(PersistingCheck = PXPersistingCheck.Nothing)]
        [PXOwnerSelector]
        [PXUIField(DisplayName = Messages.DocuSignAccountOwner)]
        public virtual Guid? OwnerID
        {
            get
            {
                return this._OwnerID;
            }
            set
            {
                this._OwnerID = value;
            }
        }
        #endregion
        #region tstamp
        public abstract class Tstamp : IBqlField
        {
        }
        protected byte[] _tstamp;
        [PXDBTimestamp]
        public virtual byte[] tstamp
        {
            get
            {
                return this._tstamp;
            }
            set
            {
                this._tstamp = value;
            }
        }
        #endregion
        #region CreatedByID
        public abstract class createdByID : IBqlField
        {
        }
        protected Guid? _CreatedByID;
        [PXDBCreatedByID]
        public virtual Guid? CreatedByID
        {
            get
            {
                return this._CreatedByID;
            }
            set
            {
                this._CreatedByID = value;
            }
        }
        #endregion
        #region CreatedByScreenID
        public abstract class createdByScreenID : IBqlField
        {
        }
        protected string _CreatedByScreenID;
        [PXDBCreatedByScreenID]
        public virtual string CreatedByScreenID
        {
            get
            {
                return this._CreatedByScreenID;
            }
            set
            {
                this._CreatedByScreenID = value;
            }
        }
        #endregion
        #region CreatedDateTime
        public abstract class createdDateTime : IBqlField
        {
        }
        protected DateTime? _CreatedDateTime;
        [PXDBCreatedDateTime]
        public virtual DateTime? CreatedDateTime
        {
            get
            {
                return this._CreatedDateTime;
            }
            set
            {
                this._CreatedDateTime = value;
            }
        }
        #endregion
        #region LastModifiedByID
        public abstract class lastModifiedByID : IBqlField
        {
        }
        protected Guid? _LastModifiedByID;
        [PXDBLastModifiedByID]
        public virtual Guid? LastModifiedByID
        {
            get
            {
                return this._LastModifiedByID;
            }
            set
            {
                this._LastModifiedByID = value;
            }
        }
        #endregion
        #region LastModifiedByScreenID
        public abstract class lastModifiedByScreenID : IBqlField
        {
        }
        protected string _LastModifiedByScreenID;
        [PXDBLastModifiedByScreenID]
        public virtual string LastModifiedByScreenID
        {
            get
            {
                return this._LastModifiedByScreenID;
            }
            set
            {
                this._LastModifiedByScreenID = value;
            }
        }
        #endregion
        #region LastModifiedDateTime
        public abstract class lastModifiedDateTime : IBqlField
        {
        }
        protected DateTime? _LastModifiedDateTime;
        [PXDBLastModifiedDateTime]
        public virtual DateTime? LastModifiedDateTime
        {
            get
            {
                return this._LastModifiedDateTime;
            }
            set
            {
                this._LastModifiedDateTime = value;
            }
        }
        #endregion
    }
}
