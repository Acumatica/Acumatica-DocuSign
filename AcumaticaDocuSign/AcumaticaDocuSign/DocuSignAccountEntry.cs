using PX.Data;
using PX.Objects.Common;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Represents entry logic of the <see cref="DocuSignAccount"/>docusign account and account preferenc�.
    /// Records of this type are created and edited through the DocuSign Account (DS.30.10.00) screen
    /// </summary>
    public class DocuSignAccountEntry : PXGraph<DocuSignAccountEntry, DocuSignAccount>
    {
        #region Selects

        public PXSelect<DocuSignAccount> Accounts;

        public PXSelect<DocuSignAccount,
            Where<DocuSignAccount.accountID, Equal<Current<DocuSignAccount.accountID>>>> SelectedAccount;

        public PXSelect<DocuSignAccountUserRule,
            Where<DocuSignAccountUserRule.accountID, Equal<Current<DocuSignAccount.accountID>>>> Users;

        #endregion

        #region Actions

        public PXAction<DocuSignAccount> CheckConnection;

        /// <summary>
        /// Used for testing connection to docusign with credentials 
        /// related to current <see cref="DocuSignAccount"/> docusign account
        /// </summary>
        [PXButton(CommitChanges = true)]
        [PXUIField(DisplayName = Messages.TestConnectionFlow)]
        public virtual void checkConnection()
        {
            Actions.PressSave();
            var account = Accounts.Current;

            PXLongOperation.StartOperation(this, () =>
            {
                VerifyDocusingAccountIsActive(account);
                DocuSignService.Authenticate(account);
            });
        }

        #endregion

        #region Events

        protected virtual void DocuSignAccount_Type_FieldVerifying(PXCache sender, PXFieldVerifyingEventArgs e)
        {
            DocuSignAccount account = e.Row as DocuSignAccount;
            if (account == null)
            {
                return;
            }

            if ((string)e.NewValue == DocuSignAccount.AccountTypes.Shared)
            {
                var result = PXSelect<DocuSignAccount,
                    Where<DocuSignAccount.type, Equal<Required<DocuSignAccount.type>>>>.Select(this, e.NewValue);
                if (result.Count > 0)
                {
                    throw new PXSetPropertyException(Messages.SharedAccountExists);
                }
            }
        }

        protected virtual void DocuSignAccount_RowPersisting(
            PXCache sender,
            PXRowPersistingEventArgs e)
        {
            DocuSignAccount account = e.Row as DocuSignAccount;
            if (account == null)
            {
                return;
            }

            if ((e.Operation & PXDBOperation.Command) != PXDBOperation.Delete 
                && (account.Type == DocuSignAccount.AccountTypes.Shared && !Users.Any()))
            {
                throw new PXRowPersistingException(
                    typeof(DocuSignAccountUserRule).Name,
                    null,
                    Messages.DocuSignAccountSharedNotNullUsers);
            }

        }

        protected virtual void DocuSignAccount_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            var account = e.Row as DocuSignAccount;
            if (account == null)
            {
                return;
            }

            Users.Cache.AllowSelect = account.Type.Equals(DocuSignAccount.AccountTypes.Shared);
            PXUIFieldAttribute.SetVisible<DocuSignAccount.ownerID>(Accounts.Cache, null, !Users.Cache.AllowSelect);

            if (account.Type != DocuSignAccount.AccountTypes.Individual)
            {
                PXDefaultAttribute.SetPersistingCheck<DocuSignAccount.ownerID>(Accounts.Cache, null, PXPersistingCheck.Nothing);
            }

            if (account.IsTestApi != null)
            {
                PXUIFieldAttribute.SetEnabled<DocuSignAccount.apiUrl>(Accounts.Cache, null, account.IsTestApi.Value);
            }

            if (account.SendReminders != null)
            {
                PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.firstReminderDay>(Accounts.Cache, null, account.SendReminders.Value);
                PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.reminderFrequency>(Accounts.Cache, null, account.SendReminders.Value);
            }

        }

        protected virtual void DocuSignAccount_Type_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var account = e.Row as DocuSignAccount;
            if (account == null)
            {
                return;
            }

            if (account.Type.Equals(DocuSignAccount.AccountTypes.Shared))
            {
                account.OwnerID = null;
            }

            var users = Users.Select(account.AccountID);
            foreach (var user in users)
            {
                Users.Delete(user);
            }
        }

        protected virtual void DocuSignAccount_IsTestApi_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var account = e.Row as DocuSignAccount;

            if (account != null)
            {
                account.ApiUrl = null;
            }
        }

        protected virtual void DocuSignAccount_OwnerID_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            var account = e.Row as DocuSignAccount;
            if (account == null)
            {
                return;
            }
            var users = Users.Select(account.AccountID);
            foreach (var user in users)
            {
                Users.Delete(user);
            }
        }

        #endregion

        #region Private Functions

        private void VerifyDocusingAccountIsActive(DocuSignAccount account)
        {
            if (account.Active == false)
            {
                throw new PXException(Messages.DocuSignAccountInActive);
            }
        }

        #endregion
    }
}
