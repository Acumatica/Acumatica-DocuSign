using System;
using PX.Data;
using PX.TM;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Represents account and account preferencе of the third part document signature system "Docusign"
    /// used for defining <see cref="DocuSignEnvelopeInfo">Envelopes</see>.
    /// Records of this type are created and edited through the DocuSign Account (DS 30.10.00) screen
    /// (corresponds to the <see cref="DocuSignAccountEntry"/> graph).
    /// </summary>
    [Serializable]
    [PXCacheName(Messages.DocuSignAccount)]
    public class DocuSignAccount : IBqlTable
    {
        #region AccountID
        public abstract class accountID : IBqlField
        {
        }
        protected int? _AccountID;

        /// <summary>
        /// Unique identifier of the account. Database identity.
        /// </summary>
        [PXDBIdentity]
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
        #region AccountCD
        public abstract class accountCD : IBqlField
        {
        }
        protected string _AccountCD;

        /// <summary>
        /// Key field.
        /// The user-friendly unique identifier of the account.
        /// </summary>
        [PXDefault]
        [PXDBString(15, IsUnicode = true, IsKey = true, InputMask = ">CCCCCCCCCCCCCCC")]
        [PXUIField(DisplayName = Messages.DocuSignAccountCd, Visibility = PXUIVisibility.SelectorVisible)]
        [PXSelector(typeof(Search<DocuSignAccount.accountCD>))]
        public virtual string AccountCD
        {
            get
            {
                return this._AccountCD;
            }
            set
            {
                this._AccountCD = value;
            }
        }
        #endregion
        #region Active
        public abstract class active : IBqlField
        {
        }
        protected bool? _Active;

        /// <summary>
        /// Indicates whether the Account is active.
        /// </summary>
        [PXDBBool]
        [PXDefault(true, PersistingCheck = PXPersistingCheck.Nothing)]
        [PXUIField(DisplayName = Messages.DocuSignAccountActive)]
        public virtual bool? Active
        {
            get
            {
                return this._Active;
            }
            set
            {
                this._Active = value;
            }
        }
        #endregion
        #region Type
        public abstract class type : IBqlField
        {
        }
        protected string _Type;

        /// <summary>
        /// The type of the account.
        /// </summary>
        /// <value>
        /// Allowed values are:
        /// <c>"I"</c> - Individual,
        /// <c>"S"</c> - Shared.
        /// </value>
        [PXDBString(1, IsFixed = true)]
        [PXDefault(AccountTypes.Individual)]
        [PXUIField(DisplayName = Messages.DocuSignAccountType, Visibility = PXUIVisibility.SelectorVisible)]
        [PXStringList(new string[]
            {
                AccountTypes.Individual,
                AccountTypes.Shared
            },
            new string[]
            {
                Messages.DocuSignAccountAccountTypeIndividual,
                Messages.DocuSignAccountAccountTypeShared
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
        #region Email
        public abstract class email : IBqlField
        {
        }
        protected string _Email;

        /// <summary>
        /// Email address of the account.
        /// </summary>
        [PXDBEmail]
        [PXDefault]
        [PXUIField(DisplayName = Messages.DocuSignAccountEmail, Visibility = PXUIVisibility.SelectorVisible)]
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
        #region Password
        public abstract class password : IBqlField
        {
        }
        protected string _Password;

        /// <summary>
        /// Account password.
        /// </summary>
        [PXRSACryptString(100)]
        [PXDefault]
        [PXUIField(DisplayName = Messages.DocuSignAccountPassword)]
        public virtual string Password
        {
            get
            {
                return this._Password;
            }
            set
            {
                this._Password = value;
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
        [PXDBGuid]
        [PXDefault]
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
        #region IsTestApi
        public abstract class isTestApi : IBqlField
        {
        }
        protected bool? _IsTestApi;

        /// <summary>
        /// Defined if it is needed to use test api.
        /// </summary>
        [PXDBBool]
        [PXDefault(false)]
        [PXUIField(DisplayName = Messages.DocuSignAccountIsTestApi)]
        public virtual bool? IsTestApi
        {
            get
            {
                return this._IsTestApi;
            }
            set
            {
                this._IsTestApi = value;
            }
        }
        #endregion
        #region ApiUrl
        public abstract class apiUrl : IBqlField
        {
        }
        protected string _ApiUrl;

        /// <summary>
        /// Defined DocuSign test api url for <see cref="DocuSignService"> service</see>
        /// </summary>
        [PXDBString(255, IsUnicode = true)]
        [PXUIField(DisplayName = Messages.DocuSignAccountApiUrl)]
        public virtual string ApiUrl
        {
            get
            {
                return this._ApiUrl;
            }
            set
            {
                this._ApiUrl = value;
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
        /// </summary>
        [PXDBBool]
        [PXDefault(false)]
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
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(120)]
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
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(0)]
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
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(0)]
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
        /// </summary>
        [PXDBInt(MaxValue = 999)]
        [PXDefault(0)]
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

        public static class AccountTypes
        {
            public const string Shared = "S";
            public const string Individual = "I";
        }
    }
}