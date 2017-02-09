using System;
using System.Collections.Generic;
using System.Linq;
using PX.Data;
using PX.SM;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Represents envelope which will be send to docusign
    /// Records of this type are created and edited through the DocuSign Document (DS 30.20.00) screen
    /// (corresponds to the <see cref="DocuSignDocumentEntry"/> graph).
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.DocuSignEnvelopeInfo)]
    public class DocuSignEnvelopeInfo : IBqlTable
    {
        #region EnvelopeInfoID
        public abstract class envelopeInfoID : IBqlField
        {
        }
        protected int? _EnvelopeInfoID;

        /// <summary>
        /// Unique identifier of the envelope info. Database identity.
        /// </summary>
        [PXDBIdentity(IsKey = true)]
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
        #region FileID
        public abstract class fileID : IBqlField
        {
        }
        protected Guid? _FileID;

        /// <summary>
        /// The indetifier of the file which will be send to docusign.
        /// </summary>
        [PXDBGuid]
        [PXDBDefault(typeof(UploadFileRevision.fileID))]
        [PXParent(typeof(Select<UploadFileRevision, Where<UploadFileRevision.fileID, Equal<Current<DocuSignEnvelopeInfo.fileID>>>>))]
        public virtual Guid? FileID
        {
            get
            {
                return this._FileID;
            }
            set
            {
                this._FileID = value;
            }
        }
        #endregion
        #region FileRevisionID
        public abstract class fileRevisionID : IBqlField
        {
        }
        protected int? _FileRevisionID;

        /// <summary>
        /// The indetifier of the file revision.
        /// </summary>
        [PXDBInt]
        [PXDBDefault(typeof(UploadFileRevision.fileRevisionID))]
        [PXParent(typeof(Select<UploadFileRevision, Where<UploadFileRevision.fileRevisionID, Equal<Current<DocuSignEnvelopeInfo.fileRevisionID>>>>))]
        public virtual int? FileRevisionID
        {
            get
            {
                return this._FileRevisionID;
            }
            set
            {
                this._FileRevisionID = value;
            }
        }
        #endregion
        #region EnvelopeID
        public abstract class envelopeID : IBqlField
        {
        }
        protected string _EnvelopeID;

        /// <summary>
        /// The unique indetifier of the docusign envelope.
        /// Store value which came from <see cref="DocuSignService"/> docusign service.
        /// </summary>
        [PXDBString(50, IsUnicode = true)]
        public virtual string EnvelopeID
        {
            get
            {
                return this._EnvelopeID;
            }
            set
            {
                this._EnvelopeID = value;
            }
        }
        #endregion
        #region LastStatus
        public abstract class lastStatus : IBqlField
        {
        }
        protected string _LastStatus;

        /// <summary>
        /// The latest status of the envelope.
        /// </summary>
        /// <value>
        /// Allowed values are:
        /// <c>"new"</c> - New,
        /// <c>"created"</c> - Created,
        /// <c>"deleted"</c> - Deleted.
        /// <c>"sent"</c> - Sent.
        /// <c>"delivered"</c> - Delivered.
        /// <c>"signed"</c> - Signed.
        /// <c>"completed"</c> - Completed.
        /// <c>"voided"</c> - Voided.
        /// </value>
        [PXDBString(15, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeLastStatus)]
        [PXStringList(
             new string[]
             {
                 EnvelopeStatus.New,
                 EnvelopeStatus.Created,
                 EnvelopeStatus.Completed,
                 EnvelopeStatus.Delivered,
                 EnvelopeStatus.Sent,
                 EnvelopeStatus.Declined,
                 EnvelopeStatus.Signed,
                 EnvelopeStatus.Voided,
                 EnvelopeStatus.Deleted
             },
             new string[]
             {
                 Messages.DocuSignEnvelopeTypeNew,
                 Messages.DocuSignEnvelopeTypeCreated,
                 Messages.DocuSignEnvelopeTypeCompleted,
                 Messages.DocuSignEnvelopeTypeDelivered,
                 Messages.DocuSignEnvelopeTypeSent,
                 Messages.DocuSignEnvelopeTypeDeclined,
                 Messages.DocuSignEnvelopeTypeSigned,
                 Messages.DocuSignEnvelopeTypeVoided,
                 Messages.DocuSignEnvelopeTypeDeleted
             })]
        public virtual string LastStatus
        {
            get
            {
                return this._LastStatus;
            }
            set
            {
                this._LastStatus = value;
            }
        }
        #endregion
        #region ActivityDate
        public abstract class activityDate : IBqlField
        {
        }
        protected DateTime? _ActivityDate;

        /// <summary>
        /// The date of the item was last modified.
        /// </summary>
        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeActivityDate)]
        public virtual DateTime? ActivityDate
        {
            get
            {
                return this._ActivityDate;
            }
            set
            {
                this._ActivityDate = value;
            }
        }
        #endregion
        #region DocuSignAccountID
        public abstract class docuSignAccountID : IBqlField
        {
        }
        protected int? _DocuSignAccountID;

        /// <summary>
        /// Identifier of the <see cref="DocuSignAccount"/>, whom the this envelope belongs.
        /// </summary>
        [PXDBInt]
        [PXDefault]
        [PXUIField(DisplayName = Messages.DocuSignAccount)]
        [PXSelector(typeof(Search2<DocuSignAccount.accountID,
            LeftJoin<DocuSignAccountUserRule, On<DocuSignAccountUserRule.accountID, Equal<DocuSignAccount.accountID>>>,
            Where<DocuSignAccount.active, Equal<True>,
                And2<Where<DocuSignAccount.ownerID, IsNotNull, And<DocuSignAccount.ownerID, Equal<Current<AccessInfo.userID>>>>,
                    Or<Where<DocuSignAccountUserRule.ownerID, IsNotNull, And<DocuSignAccountUserRule.ownerID, Equal<Current<AccessInfo.userID>>>>>>>,
            OrderBy<Asc<DocuSignAccount.type, Asc<AccessInfo.userName>>>>),
            SubstituteKey = typeof(DocuSignAccount.accountCD))]
        public virtual int? DocuSignAccountID
        {
            get
            {
                return this._DocuSignAccountID;
            }
            set
            {
                this._DocuSignAccountID = value;
            }
        }
        #endregion
        #region Theme
        public abstract class theme : IBqlField
        {
        }
        protected string _Theme;

        /// <summary>
        /// Specifies the subject of the email that is sent to all recipients.
        /// </summary>
        [PXDBString(100, IsUnicode = true)]
        [PXDefault]
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeTheme)]
        public virtual string Theme
        {
            get
            {
                return this._Theme;
            }
            set
            {
                this._Theme = value;
            }
        }
        #endregion
        #region MessageBody
        public abstract class messageBody : IBqlField
        {
        }
        protected string _MessageBody;

        /// <summary>
        /// Specifies the body of the email that is sent to all recipients.
        /// </summary>
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeMessageBody)]
        public virtual string MessageBody
        {
            get
            {
                return this._MessageBody;
            }
            set
            {
                this._MessageBody = value;
            }
        }
        #endregion
        #region IsOrder
        public abstract class isOrder : IBqlField
        {
        }
        protected bool? _IsOrder;

        /// <summary>
        /// Specifies if it is needed to use ordering of the <see cref="DocuSignRecipient"/>
        /// recipients in current envelope.
        /// </summary>
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeIsOrdering)]
        public virtual bool? IsOrder
        {
            get
            {
                return this._IsOrder;
            }
            set
            {
                this._IsOrder = value;
            }
        }
        #endregion
        #region SendReminders
        public abstract class sendReminders : IBqlField
        {
        }
        protected bool? _SendReminders;

        /// <summary>
        /// Defindes if it is needed to send reminders to <see cref="DocuSignRecipient">Recipients</see>.
        /// Default value is retrieved from <see cref="DocuSignAccount">Account</see>.
        /// </summary>
        [PXDBBool]
        [PXDefault(typeof(Search<DocuSignAccount.sendReminders, Where<DocuSignAccount.accountID, Equal<Current<docuSignAccountID>>>>))]
        [PXUIField(DisplayName = Messages.DocuSignAccountSendReminders)]
        public virtual bool? SendReminders
        {
            get
            {
                return this._SendReminders;
            }
            set
            {
                this._SendReminders = value;
            }
        }
        #endregion
        #region ExpiredDays
        public abstract class expiredDays : IBqlField
        {
        }
        protected int? _ExpiredDays;

        /// <summary>
        /// An integer that sets the number of days the envelope is active.
        /// Default value is retrieved from <see cref="DocuSignAccount">Account</see>.
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(typeof(Search<DocuSignAccount.expiredDays, Where<DocuSignAccount.accountID, Equal<Current<docuSignAccountID>>>>))]
        [PXUIField(DisplayName = Messages.DocuSignAccountExpiredDays)]
        public virtual int? ExpiredDays
        {
            get
            {
                return this._ExpiredDays;
            }
            set
            {
                this._ExpiredDays = value;
            }
        }
        #endregion
        #region WarnDays
        public abstract class warnDays : IBqlField
        {
        }
        protected int? _WarnDays;

        /// <summary>
        /// An integer that sets the number of days before envelope expiration that an expiration 
        /// warning email is sent to the recipient. If set to 0 (zero), no warning email is sent.
        /// Default value is retrieved from <see cref="DocuSignAccount">Account</see>.
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(typeof(Search<DocuSignAccount.warnDays, Where<DocuSignAccount.accountID, Equal<Current<docuSignAccountID>>>>))]
        [PXUIField(DisplayName = Messages.DocuSignAccountWarnDays)]
        public virtual int? WarnDays
        {
            get
            {
                return this._WarnDays;
            }
            set
            {
                this._WarnDays = value;
            }
        }
        #endregion
        #region FirstReminderDay
        public abstract class firstReminderDay : IBqlField
        {
        }
        protected int? _FirstReminderDay;

        /// <summary>
        /// An interger that sets the number of days after the recipient receives the envelope 
        /// that reminder emails are sent to the recipient.
        /// Default value is retrieved from <see cref="DocuSignAccount">Account</see>.
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(typeof(Search<DocuSignAccount.firstReminderDay, Where<DocuSignAccount.accountID, Equal<Current<docuSignAccountID>>>>))]
        [PXUIField(DisplayName = Messages.DocuSignAccountFirstReminderDay)]
        public virtual int? FirstReminderDay
        {
            get
            {
                return this._FirstReminderDay;
            }
            set
            {
                this._FirstReminderDay = value;
            }
        }
        #endregion
        #region ReminderFrequency
        public abstract class reminderFrequency : IBqlField
        {
        }
        protected int? _ReminderFrequency;

        /// <summary>
        /// An interger that sets the interval, in days, between reminder emails.
        /// Default value is retrieved from <see cref="DocuSignAccount">Account</see>.
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(typeof(Search<DocuSignAccount.reminderFrequency, Where<DocuSignAccount.accountID, Equal<Current<docuSignAccountID>>>>))]
        [PXUIField(DisplayName = Messages.DocuSignAccountReminderFrequency)]
        public virtual int? ReminderFrequency
        {
            get
            {
                return this._ReminderFrequency;
            }
            set
            {
                this._ReminderFrequency = value;
            }
        }
        #endregion
        #region Selected
        public abstract class selected : PX.Data.IBqlField
        {
        }
        protected bool? _Selected;

        /// <summary>
        /// Defines if current envelope is selected.
        /// </summary>
        [PXBool]
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeSelected)]
        public virtual bool? Selected
        {
            get
            {
                return this._Selected;
            }
            set
            {
                this._Selected = value;
            }
        }
        #endregion
        #region IsFinalVersion
        public abstract class isFinalVersion : IBqlField
        {
        }
        protected bool? _IsFinalVersion;

        /// <summary>
        /// Defines if current envelope is final version of the signed document.
        /// </summary>
        [PXDBBool]
        [PXDefault(false)]
        public virtual bool? IsFinalVersion
        {
            get
            {
                return this._IsFinalVersion;
            }
            set
            {
                this._IsFinalVersion = value;
            }
        }
        #endregion
        #region CompletedFileID
        public abstract class completedFileID : IBqlField
        {
        }
        protected Guid? _CompletedFileID;

        /// <summary>
        /// The identifier of the uploaded final version of the signed document.
        /// </summary>
        [PXDBGuid]
        public virtual Guid? CompletedFileID
        {
            get
            {
                return this._CompletedFileID;
            }
            set
            {
                this._CompletedFileID = value;
            }
        }
        #endregion
        #region CompletedFileName
        public abstract class completedFileName : IBqlField
        {
        }
        protected string _CompletedFileName;

        /// <summary>
        /// The name of the uploaded final version of the signed document.
        /// </summary>
        [PXDBString(IsUnicode = true)]
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeCompletedFileName)]
        public virtual string CompletedFileName
        {
            get
            {
                return this._CompletedFileName;
            }
            set
            {
                this._CompletedFileName = value;
            }
        }
        #endregion
        #region IsActionsAvailable
        public abstract class isActionsAvailable : IBqlField
        {
        }

        /// <summary>
        /// Defines if actions (remind or void) are availiable for current envelope.
        /// </summary>
        [PXBool]
        [PXUIField(Enabled = false, Visible = false, Visibility = PXUIVisibility.Invisible)]
        public virtual bool? IsActionsAvailable
        {
            get
            {
                var listStatus = new List<string>
                {
                    EnvelopeStatus.New,
                    EnvelopeStatus.Created,
                    EnvelopeStatus.Completed,
                    EnvelopeStatus.Voided,
                    EnvelopeStatus.Declined,
                    EnvelopeStatus.Deleted
                };
                return listStatus.All(x => !listStatus.Contains(LastStatus));
            }
        }
        #endregion
        #region IsDeleteAvailable
        public abstract class isDeleteAvailable : IBqlField
        {
        }

        /// <summary>
        /// Defines if actions (remind or void) are availiable for current envelope.
        /// </summary>
        [PXBool]
        [PXUIField(Enabled = false, Visible = false, Visibility = PXUIVisibility.Invisible)]
        public virtual bool? IsDeleteAvailable
        {
            get
            {
                var listStatus = new List<string>
                {
                    EnvelopeStatus.New,
                    EnvelopeStatus.Created
                };
                return listStatus.Any(x => listStatus.Contains(LastStatus));
            }
        }
        #endregion
        #region SendDate
        public abstract class sendDate : IBqlField
        {
        }
        protected DateTime? _SendDate;

        /// <summary>
        /// The Date when current envelope was send to docusign.
        /// </summary>
        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeSendDate)]
        public virtual DateTime? SendDate
        {
            get
            {
                return this._SendDate;
            }
            set
            {
                this._SendDate = value;
            }
        }
        #endregion
        #region ExpirationDate
        public abstract class expirationDate : IBqlField
        {
        }
        protected DateTime? _ExpirationDate;

        /// <summary>
        /// The Date when current envelope expired on docusign.
        /// </summary>
        [PXDBDate(PreserveTime = true)]
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeExpirationDate)]
        public virtual DateTime? ExpirationDate
        {
            get
            {
                return this._ExpirationDate;
            }
            set
            {
                this._ExpirationDate = value;
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
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeCreatedDateTime)]
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

    public static class EnvelopeStatus
    {
        public const string New = "";
        public const string Created = "created";
        public const string Sent = "sent";
        public const string Declined = "declined";
        public const string Delivered = "delivered";
        public const string Signed = "signed";
        public const string Completed = "completed";
        public const string Voided = "voided";
        public const string Deleted = "deleted";
    }

    public class envelopeStatusCompleted : Constant<string>
    {
        public envelopeStatusCompleted() : base(EnvelopeStatus.Completed) { }
    }

    public class envelopeStatusVoided : Constant<string>
    {
        public envelopeStatusVoided() : base(EnvelopeStatus.Voided) { }
    }
}