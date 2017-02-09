using System;
using PX.Data;
using PX.Objects.CR;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Represents recipient who are will be sign <see cref="DocuSignEnvelopeInfo"/> envelope in docusign.
    /// Records of this type are created and edited through the DocuSign Document (DS 30.20.00) screen
    /// (corresponds to the <see cref="DocuSignDocumentEntry"/> graph).
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.DocuSignRecipient)]
    public class DocuSignRecipient : IBqlTable
    {
        #region RecipientID
        public abstract class recipientID : IBqlField
        {
        }
        protected int? _RecipientID;

        /// <summary>
        /// Unique identifier of the recipient. Database identity.
        /// </summary>
        [PXDBIdentity(IsKey = true)]
        public virtual int? RecipientID
        {
            get
            {
                return this._RecipientID;
            }
            set
            {
                this._RecipientID = value;
            }
        }
        #endregion
        #region EnvelopeInfoID
        public abstract class envelopeInfoID : IBqlField
        {
        }
        protected int? _EnvelopeInfoID;

        /// <summary>
        /// Identifier of the <see cref="DocuSignEnvelopeInfo"/>, whom the this recipient belongs.
        /// </summary>
        [PXDBInt]
        [PXDBDefault(typeof(DocuSignEnvelopeInfo.envelopeInfoID))]
        [PXParent(
            typeof(Select<DocuSignEnvelopeInfo,
                        Where<DocuSignEnvelopeInfo.envelopeInfoID,
                            Equal<Current<DocuSignEnvelopeInfo.envelopeInfoID>>>>))]
        public virtual int? EnvelopeInfoID
        {
            get
            {
                return this._EnvelopeInfoID;
            }
            set
            {
                this._EnvelopeInfoID = value;
            }
        }
        #endregion
        #region Email
        public abstract class email : IBqlField
        {
        }
        protected string _Email;

        /// <summary>
        /// Email id of the recipient. Notification of the document to sign is sent to this email id.
        /// </summary>
        [PXDefault]
        [PXUIField(DisplayName = Messages.DocuSignAccountEmail)]
        [PXDBEmail]
        [PXSelector(
            typeof(Search<Contact.eMail>), ValidateValue = false)]
        public virtual string Email
        {
            get
            {
                return this._Email;
            }
            set
            {
                this._Email = value;
            }
        }
        #endregion
        #region Name
        public abstract class name : IBqlField
        {
        }
        protected string _Name;

        /// <summary>
        /// Legal name of the recipient.
        /// </summary>
        [PXDefault]
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.DocuSignAccountName)]
        public virtual string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this._Name = value;
            }
        }
        #endregion
        #region Position
        public abstract class position : IBqlField, IBqlOperand
        {
        }
        protected int? _Position;

        /// <summary>
        /// Used for ordering recipients.
        /// </summary>
        [PXDBInt]
        [PXUIField(DisplayName = Messages.DocuSignRecipientPosition)]
        public virtual int? Position
        {
            get
            {
                return this._Position;
            }
            set
            {
                this._Position = value;
            }
        }
        #endregion
        #region CustomMessage
        public abstract class customMessage : IBqlField
        {
        }
        protected string _CustomMessage;

        /// <summary>
        /// Specifies the email body of the message sent to the recipient.
        /// </summary>
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.DocuSignRecipientCustomMessage)]
        public virtual string CustomMessage
        {
            get
            {
                return this._CustomMessage;
            }
            set
            {
                this._CustomMessage = value;
            }
        }
        #endregion
        #region Type
        public abstract class type : IBqlField
        {
        }
        protected string _Type;

        /// <summary>
        /// The type of the recipient.
        /// </summary>
        /// <value>
        /// Allowed values are:
        /// <c>"1"</c> - CopyRecipient,
        /// <c>"2"</c> - Signer.
        /// </value>
        [PXDBString(50, IsUnicode = true)]
        [PXDefault(RecipientTypes.Signer)]
        [PXStringList(
            new string[]
            {
                RecipientTypes.CopyRecipient,
                RecipientTypes.Signer
            },
            new string[]
            {
                Messages.DocuSignRecipientTypeCopyRecipient,
                Messages.DocuSignRecipientTypeSigner
            })]
        public virtual string Type
        {
            get
            {
                return this._Type;
            }
            set
            {
                this._Type = value;
            }
        }
        #endregion
        #region Status
        public abstract class status : PX.Data.IBqlField
        {
        }
        protected string _Status;

        /// <summary>
        /// Indicates the envelope status. 
        /// </summary>
        [PXDBString(50, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.DocuSignRecipientStatus)]
        [PXStringList(
             new string[]
             {
                 RecipientStatus.New,
                 RecipientStatus.Created,
                 RecipientStatus.Completed,
                 RecipientStatus.Delivered,
                 RecipientStatus.Sent,
                 RecipientStatus.Declined,
                 RecipientStatus.Signed,
                 RecipientStatus.FaxPending,
                 RecipientStatus.AutoResponded
             },
             new string[]
             {
                 Messages.DocuSignRecipientStatusNew,
                 Messages.DocuSignRecipientStatusCreated,
                 Messages.DocuSignRecipientStatusCompleted,
                 Messages.DocuSignRecipientStatusDelivered,
                 Messages.DocuSignRecipientStatusSent,
                 Messages.DocuSignRecipientStatusDeclined,
                 Messages.DocuSignRecipientStatusSigned,
                 Messages.DocuSignRecipientStatusFaxPending,
                 Messages.DocuSignRecipientStatusAutoResponded,
             })]
        public virtual string Status
        {
            get
            {
                return this._Status;
            }
            set
            {
                this._Status = value;
            }
        }
        #endregion
        #region SignedDateTime
        public abstract class signedDateTime : PX.Data.IBqlField
        {
        }
        protected DateTime? _SignedDateTime;

        /// <summary>
        /// Date when envelope was signed by recipient.
        /// </summary>
        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = Messages.DocuSignRecipientSignedDateTime)]
        public virtual DateTime? SignedDateTime
        {
            get
            {
                return this._SignedDateTime;
            }
            set
            {
                this._SignedDateTime = value;
            }
        }
        #endregion
        #region DeliveredDateTime
        public abstract class deliveredDateTime : PX.Data.IBqlField
        {
        }
        protected DateTime? _DeliveredDateTime;

        /// <summary>
        /// Date when envelope was delovered to recipient.
        /// </summary>
        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = Messages.DocuSignRecipientDeliveredDateTime)]
        public virtual DateTime? DeliveredDateTime
        {
            get
            {
                return this._DeliveredDateTime;
            }
            set
            {
                this._DeliveredDateTime = value;
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

        public static class RecipientTypes
        {
            public const string Signer = "Signer";
            public const string CopyRecipient = "CopyRecipient";
        }

        public static class RecipientStatus
        {
            public const string New = "";
            public const string Created = "created";
            public const string Sent = "sent";
            public const string Declined = "declined";
            public const string Delivered = "delivered";
            public const string Signed = "signed";
            public const string Completed = "completed";
            public const string FaxPending = "faxpending";
            public const string AutoResponded = "autoresponded";
        }
    }
}