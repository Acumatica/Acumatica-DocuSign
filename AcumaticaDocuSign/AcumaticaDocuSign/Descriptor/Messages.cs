using PX.Common;

namespace AcumaticaDocuSign
{
    [PXLocalizable(Messages.Prefix)]
    public static class Messages
    {
        #region DAC Names
        public const string DocuSignAccount = "DocuSign Account";
        public const string DocuSignAccountUserRule = "DocuSign Account User Rule";
        public const string DocuSignEnvelopeInfo = "DocuSign Envelope";
        public const string DocuSignRecipient = "DocuSign Recipient";
        #endregion

        #region DAC Field Names
        public const string DocuSignAccountCd = "DocuSign Account";
        public const string DocuSignAccountActive = "Active";
        public const string DocuSignAccountType = "Type";
        public const string DocuSignAccountEmail = "Email";
        public const string DocuSignAccountName = "Name";
        public const string DocuSignAccountOwner = "Acumatica User Name";
        public const string DocuSignAccountPassword = "Password";
        public const string DocuSignAccountIsTestApi = "Use Test API";
        public const string DocuSignAccountApiUrl = "API URL";
        public const string DocuSignAccountSendReminders = "Send automatic reminders";
        public const string DocuSignAccountExpiredDays = "# of days before request expires";
        public const string DocuSignAccountWarnDays = "# of days to warn before expiration";
        public const string DocuSignAccountFirstReminderDay = "# of days before sending 1st reminder";
        public const string DocuSignAccountReminderFrequency = "# of days between reminders";
        public const string DocuSignAccountAccountTypeShared = "Shared";
        public const string DocuSignAccountAccountTypeIndividual = "Individual";
        public const string DocuSignEnvelopeTypeNew = "";
        public const string DocuSignEnvelopeTypeCreated = "Draft";
        public const string DocuSignEnvelopeTypeSent = "Sent";
        public const string DocuSignEnvelopeTypeDelivered = "Delivered";
        public const string DocuSignEnvelopeTypeSigned = "Partially Signed";
        public const string DocuSignEnvelopeTypeCompleted = "Completed";
        public const string DocuSignEnvelopeTypeDeclined = "Declined";
        public const string DocuSignEnvelopeTypeVoided = "Voided";
        public const string DocuSignEnvelopeTypeDeleted = "Deleted";
        public const string DocuSignEnvelopeLastStatus = "DocuSign Status";
        public const string DocuSignEnvelopeDocusignAccount = "DocuSign Owner";
        public const string DocuSignEnvelopeActivityDate = "DocuSign Activity Date";
        public const string DocuSignEnvelopeCreatedDate = "Creation Date";
        public const string DocuSignEnvelopeSendDate = "DocuSign Send Date";
        public const string DocuSignEnvelopeExpirationDate = "Expiration Date";
        public const string DocuSignEnvelopeTheme = "Subject";
        public const string DocuSignEnvelopeMessageBody = "Message Body";
        public const string DocuSignEnvelopeVoidReason = "Void Reason";
        public const string DocuSignEnvelopeSelected = "Selected";
        public const string DocuSignEnvelopeIsOrdering = "Set signing order";
        public const string DocuSignEnvelopeCompletedFileName = "DocuSign Version";
        public const string DocuSignEnvelopeCreatedDateTime = "Created Date";
        public const string DocuSignRecipientPosition = "Position";
        public const string DocuSignRecipientCustomMessage = "Custom Message";
        public const string DocuSignRecipientTypeSigner = "Needs to Sign";
        public const string DocuSignRecipientTypeCopyRecipient = "Receives a Copy";
        public const string DocuSignRecipientStatus = "Status";
        public const string DocuSignRecipientSignedDateTime = "Signed Date";
        public const string DocuSignRecipientDeliveredDateTime = "Delivered Date";
        public const string DocuSignRecipientStatusNew = "";
        public const string DocuSignRecipientStatusCreated = "Created";
        public const string DocuSignRecipientStatusSent = "Sent";
        public const string DocuSignRecipientStatusDeclined = "Declined";
        public const string DocuSignRecipientStatusDelivered = "Delivered";
        public const string DocuSignRecipientStatusSigned = "Signed";
        public const string DocuSignRecipientStatusCompleted = "Completed";
        public const string DocuSignRecipientStatusFaxPending = "Fax Pending";
        public const string DocuSignRecipientStatusAutoResponded = "Auto Responded";
        public const string DocuSignEnvelopeFilterMe = "Me";
        public const string DocuSignEnvelopeFilterOwner = "Owner";
        public const string SiteMapTitleDisplayName = "Source Screen Name";
        public const string DocuSignEnvelopeDefaultVoidReason = "Document voided.";

        #endregion

        #region Custom Actions
        public const string TestConnectionFlow = "Test Connection";
        public const string DocusignSelected = "DocuSign";
        public const string Void = "Void Document";
        public const string Remind = "Remind Recipient";
        public const string ViewDocuSign = "View Document";
        public const string ViewHistory = "View History";
        public const string ViewFile = "View File";
        public const string CheckStatus = "Check Status";
        public const string DocuSignEnvelopeSend = "Send";
        public const string DocuSignEnvelopeSave = "Save";
        public const string DocuSignEnvelopeSaveAndClose = "Save & Close";
        public const string Ok = "Ok";
        #endregion

        #region Validation messages
        public const string EnvelopeStatusChanged = "This document was already sent previously. Please refresh your screen to see latest status information.";
        public const string EnvelopeVoidIsNotAvailable = "Status was changed. Void action is not available.";
        public const string EnvelopeRemindIsNotAvailable = "Status was changed. Remind action is not available.";
        public const string DocuSignAccountNotExists = "DocuSign Account is not setup for logged-in user. Please contact your System administrator.";
        public const string DocuSignRecipientsRequired = "At least one recipient is required.";
        public const string DocuSignRecipientsUnique = "Each recipient in the same signing order must be unique.";
        public const string DocuSignEnvelopeNotExists = "This Document is not available in DocuSign.";
        public const string DocuSignEnvelopeLock = "This Document is locked by DocuSign until {0}. Please try later.";
        public const string DocuSignAccountInActive = "Docusign Account is inactive.";
        public const string Prefix = "DS Error";
        public const string SharedAccountExists = "Shared account already exists";
        public const string DocuSignEnvelopeDeleteConfirm = "Are you sure you want to delete and void this document from DocuSign?";
        public const string DocuSignEnvelopeDeleteFileConfirm = "Current File record will be deleted";
        public const string DocuSignAccountSharedNotNullUsers = "At least one Acumatica user should be specified";
        public const string DocuSignEnvelopeLockNotExist = "EDIT_LOCK_ENVELOPE_NOT_LOCKED";

        #endregion
    }
}
