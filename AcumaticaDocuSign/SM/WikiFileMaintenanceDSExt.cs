using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Compilation;
using DocuSign.eSign.Model;
using PX.Common;
using PX.Data;
using PX.Objects.CR;
using PX.SM;
using PX.Web.UI;
using Note = PX.Data.Note;
using System.Reflection;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Extends graph <see cref="WikiFileMaintenance"/> for envelopes functionality.
    /// Records of this type are created and edited through the Wiki File Maintenance (SM.20.25.10) screen.
    /// </summary>
    public class WikiFileMaintenanceDSExt : PXGraphExtension<WikiFileMaintenance>
    {
        #region Selects

        public PXSelectJoin<UploadFileRevisionNoData,
            LeftJoin<DocuSignEnvelopeInfo, On<DocuSignEnvelopeInfo.fileID, Equal<UploadFileRevisionNoData.fileID>,
                And<DocuSignEnvelopeInfo.fileRevisionID, Equal<UploadFileRevisionNoData.fileRevisionID>>>>,
            Where<UploadFileRevisionNoData.fileID, Equal<Current<UploadFileWithIDSelector.fileID>>>> RevisionsWithAction;

        public PXSelect<Contact> Contacts;

        public PXSelect<DocuSignEnvelopeInfo,
            Where<DocuSignEnvelopeInfo.fileID, Equal<Current<UploadFileRevisionNoData.fileID>>,
                And<DocuSignEnvelopeInfo.fileRevisionID, Equal<Current<UploadFileRevisionNoData.fileRevisionID>>>>> Envelope;

        public PXSelect<DocuSignRecipient,
            Where<DocuSignRecipient.envelopeInfoID, Equal<Current<DocuSignEnvelopeInfo.envelopeInfoID>>>,
            OrderBy<Asc<DocuSignRecipient.position>>> Recipients;

        public PXSelectJoin<DocuSignAccount,
            LeftJoin<DocuSignAccountUserRule, On<DocuSignAccountUserRule.accountID, Equal<DocuSignAccount.accountID>>>,
            Where<DocuSignAccount.active, Equal<True>,
                And2<Where<DocuSignAccount.ownerID, IsNotNull, And<DocuSignAccount.ownerID, Equal<Current<AccessInfo.userID>>>>,
                    Or<Where<DocuSignAccountUserRule.ownerID, IsNotNull, And<DocuSignAccountUserRule.ownerID, Equal<Current<AccessInfo.userID>>>>>>>,
            OrderBy<Asc<DocuSignAccount.type, Asc<AccessInfo.userName>>>> DocuSignAccount;

        #endregion

        #region Events
        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute),
                                  "DisplayName",
                                  Messages.DocuSignEnvelopeDocusignAccount)]
        protected virtual void DocuSignEnvelopeInfo_DocuSignAccountID_CacheAttached(PXCache sender)
        {
        }

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute),
                                  "DisplayName",
                                  Messages.DocuSignEnvelopeCompletedFileName)]
        protected void UploadFileWithIDSelector_Name_CacheAttached(PXCache sender)
        {
        }

        protected virtual void DocuSignEnvelopeInfo_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            DocuSignEnvelopeInfo row = (DocuSignEnvelopeInfo)e.Row;
            if (row == null)
            {
                return;
            }

            PXUIFieldAttribute.SetVisible<DocuSignRecipient.position>(Recipients.Cache, null, row.IsOrder != null && row.IsOrder.Value);
            PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.firstReminderDay>(Envelope.Cache, null, row.SendReminders != null && row.SendReminders.Value);
            PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.reminderFrequency>(Envelope.Cache, null, row.SendReminders != null && row.SendReminders.Value);

            if (row.LastStatus != EnvelopeStatus.New)
            {
                PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.firstReminderDay>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.reminderFrequency>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.expiredDays>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.warnDays>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.theme>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.messageBody>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.sendReminders>(Envelope.Cache, null, false);
            }

            if (row.LastStatus != EnvelopeStatus.New && row.LastStatus != EnvelopeStatus.Created)
            {
                PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.docuSignAccountID>(Envelope.Cache, null, false);
                PXUIFieldAttribute.SetEnabled<DocuSignEnvelopeInfo.isOrder>(Envelope.Cache, null, false);

                Recipients.AllowUpdate = false;
                Recipients.AllowDelete = false;
                Recipients.AllowInsert = false;
            }
        }

        protected virtual void DocuSignRecipient_Email_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            DocuSignRecipient doc = (DocuSignRecipient)e.Row;
            if (doc != null)
            {
                Contact contact = Contacts.Search<Contact.eMail>(doc.Email);
                if (contact != null)
                {
                    doc.Name = contact.DisplayName;
                }
            }
        }

        protected virtual void DocuSignEnvelopeInfo_IsOrder_FieldUpdated(PXCache sender, PXFieldUpdatedEventArgs e)
        {
            DocuSignEnvelopeInfo env = (DocuSignEnvelopeInfo)e.Row;
            if (env?.IsOrder != null && !env.IsOrder.Value && Envelope.Current != null)
            {
                var recipients = Recipients.Select();
                foreach (DocuSignRecipient recipient in recipients)
                {
                    recipient.Position = null;
                }
            }
        }

        #endregion

        #region Actions

        /// <summary>
        /// Override default refresh action on grid.
        /// </summary>
        public PXAction<UploadFileWithIDSelector> Refresh;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.Refresh)]
        public virtual IEnumerable refresh(PXAdapter adapter)
        {
            CleanCache();
            var revisions = RevisionsWithAction.Select();
            PXLongOperation.StartOperation(Base, () =>
            {
                var pxResult = revisions.Select(joinResult => joinResult as PXResult<UploadFileRevisionNoData, DocuSignEnvelopeInfo>);
                var graph = PXGraph.CreateInstance<WikiFileMaintenance>();
                var uploadGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
                var taskGraph = PXGraph.CreateInstance<CRTaskMaint>();
                foreach (DocuSignEnvelopeInfo envelope in pxResult)
                {
                    if (!string.IsNullOrEmpty(envelope?.LastStatus)
                    && envelope.LastStatus != EnvelopeStatus.New)
                    {
                        CheckStatus(graph, envelope, uploadGraph, taskGraph);
                    }
                }
            });
            return adapter.Get();
        }

        /// <summary>
        /// Create new smart panel with <see cref="DocuSignEnvelopeInfo"/> envelope.
        /// </summary>
        public PXAction<UploadFileWithIDSelector> DocuSignSelected;
        [PXUIField(DisplayName = Messages.DocusignSelected, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select)]
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.DataEntry, CommitChanges = true)]
        public virtual IEnumerable docuSignSelected(PXAdapter adapter)
        {
            if (RevisionsWithAction.Current != null)
            {
                CleanCache();

                var revision = RevisionsWithAction.Current;

                DocuSignEnvelopeInfo currentEnvelope = Envelope
                    .Search<DocuSignEnvelopeInfo.fileID, DocuSignEnvelopeInfo.fileRevisionID>
                    (revision.FileID, revision.FileRevisionID);

                Envelope.Current = currentEnvelope ?? CreateNewEnvelope();

                var result = Envelope.AskExt((graph, view) =>
                {
                    if (currentEnvelope != null && currentEnvelope.LastStatus != EnvelopeStatus.New)
                    {
                        var uploadGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
                        var taskGraph = PXGraph.CreateInstance<CRTaskMaint>();
                        CheckStatus(Base, Envelope.Current, uploadGraph, taskGraph);
                    }
                }, true);
                if (result == WebDialogResult.OK)
                {
                    PerformSendToDocuSignRequest();
                }
            }
            return adapter.Get();
        }

        public PXAction<UploadFileWithIDSelector> SendToDocusign;
        [PXUIField(DisplayName = "Send", MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select, Visible = true, Enabled = true)]
        [PXButton(CommitChanges = true)]
        public virtual IEnumerable sendToDocusign(PXAdapter adapter)
        {
            Base.Actions.PressSave();
            return adapter.Get();
        }


        /// <summary>
        /// Redirect to final uploaded signed version of <see cref="DocuSignEnvelopeInfo"/> envelope.
        /// </summary>
        public PXAction<UploadFileWithIDSelector> ViewFile;
        [PXUIField(DisplayName = Messages.ViewFile, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
        [PXButton]
        public virtual IEnumerable viewFile(PXAdapter adapter)
        {
            if (RevisionsWithAction.Current != null)
            {
                DocuSignEnvelopeInfo existingEnvelope = Envelope.Search<DocuSignEnvelopeInfo.fileID, DocuSignEnvelopeInfo.fileRevisionID>
                    (RevisionsWithAction.Current.FileID, RevisionsWithAction.Current.FileRevisionID);
                if (existingEnvelope?.CompletedFileID != null)
                {
                    var graph = PXGraph.CreateInstance<WikiFileMaintenance>();
                    UploadFileWithIDSelector file = graph.Files.Search<UploadFileWithIDSelector.fileID>(existingEnvelope.CompletedFileID);

                    if (file != null)
                    {
                        throw new PXRedirectToFileException(file.FileID, 1, true);
                    }
                }
            }
            return adapter.Get();
        }

        /// <summary>
        /// Save & Close smart panel related to current <see cref="DocuSignEnvelopeInfo"/> envelope.
        /// </summary>
        public PXAction<UploadFileWithIDSelector> SaveAndCloseEnvelopeInfo;
        [PXUIField(DisplayName = Messages.DocuSignEnvelopeSaveAndClose, MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable saveAndCloseEnvelopeInfo(PXAdapter adapter)
        {

            Base.Actions.PressSave();
            return adapter.Get();
        }

        /// <summary>
        /// Save smart panel related to current <see cref="DocuSignEnvelopeInfo"/> envelope.
        /// </summary>
        public PXAction<UploadFileWithIDSelector> SaveEnvelopeInfo;

        [PXUIField(DisplayName = Messages.DocuSignEnvelopeSave, MapEnableRights = PXCacheRights.Select,
            MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXLookupButton]
        public virtual IEnumerable saveEnvelopeInfo(PXAdapter adapter)
        {
            Base.Actions.PressSave();
            return adapter.Get();
        }

        public PXDelete<UploadFileWithIDSelector> Delete;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        [PXDeleteButton(ConfirmationMessage = null)]
        public virtual IEnumerable delete(PXAdapter adapter)
        {
            var revisions = RevisionsWithAction.Select();
            var envelopes =
                PXSelect
                    <DocuSignEnvelopeInfo,
                        Where<DocuSignEnvelopeInfo.fileID, Equal<Current<UploadFileWithIDSelector.fileID>>>>.Select(Base);
            if (envelopes.Any())
            {
                var result = Base.Files.View.Ask(Messages.DocuSignEnvelopeDeleteConfirm, MessageButtons.YesNo, true);
                if (result == WebDialogResult.Yes)
                {
                    var uploadGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
                    var taskGraph = PXGraph.CreateInstance<CRTaskMaint>();
                    var pxResult =
                        revisions.Select(
                            joinResult => joinResult as PXResult<UploadFileRevisionNoData, DocuSignEnvelopeInfo>);
                    foreach (DocuSignEnvelopeInfo envelope in pxResult)
                    {
                        if (envelope.LastStatus != EnvelopeStatus.New)
                        {
                            CheckStatus(Base, envelope, uploadGraph, taskGraph);
                        }

                        CleanBaseEnvelope(envelope);

                        if (envelope.IsActionsAvailable != null
                            && envelope.IsActionsAvailable.Value
                            && envelope.EnvelopeInfoID.HasValue)
                        {
                            VoidDeletedDocument(envelope);
                        }
                        else if (envelope.LastStatus == EnvelopeStatus.Created)
                        {
                            VoidDraftDocument(envelope);
                        }

                    }
                    Base.Delete.Press();
                }
            }
            else
            {
                var result = Base.Files.View.Ask(Messages.DocuSignEnvelopeDeleteFileConfirm, MessageButtons.OKCancel, true);
                if (result == WebDialogResult.OK)
                {
                    Base.Delete.Press();
                }
            }

            return adapter.Get();
        }

        public PXAction<UploadFileWithIDSelector> DeleteRow;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = true)]
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.RecordDel)]
        public virtual IEnumerable deleteRow(PXAdapter adapter)
        {
            var revision = RevisionsWithAction.Current;

            if (revision != null)
            {
                DocuSignEnvelopeInfo envelope = PXSelect<DocuSignEnvelopeInfo,
                    Where<DocuSignEnvelopeInfo.fileID, Equal<Required<UploadFileRevisionNoData.fileID>>,
                        And
                            <DocuSignEnvelopeInfo.fileRevisionID,
                                Equal<Required<UploadFileRevisionNoData.fileRevisionID>>>>>.Select(Base, revision.FileID, revision.FileRevisionID);
                if (envelope != null)
                {
                    var result = Base.Files.View.Ask(Messages.DocuSignEnvelopeDeleteConfirm, MessageButtons.YesNo, true);
                    if (result == WebDialogResult.Yes)
                    {
                        if (envelope.LastStatus != EnvelopeStatus.New)
                        {
                            var uploadGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
                            var taskGraph = PXGraph.CreateInstance<CRTaskMaint>();
                            CheckStatus(Base, envelope, uploadGraph, taskGraph);
                        }

                        RevisionsWithAction.Delete(revision);
                        CleanBaseEnvelope(envelope);

                        if (envelope.IsActionsAvailable != null
                            && envelope.IsActionsAvailable.Value
                            && envelope.EnvelopeInfoID.HasValue)
                        {
                            VoidDeletedDocument(envelope);
                        }
                        else if (envelope.LastStatus == EnvelopeStatus.Created)
                        {
                            VoidDraftDocument(envelope);
                        }
                    }
                }
                else
                {
                    RevisionsWithAction.Delete(revision);
                }
            }
            return adapter.Get();
        }

        public DocuSignEnvelopeInfo CheckStatus(WikiFileMaintenance graph, DocuSignEnvelopeInfo envelope,
            UploadFileMaintenance uploadGraph, CRTaskMaint taskGraph)
        {
            graph.Clear();
            graph.Files.Current = graph.Files.Search<UploadFileWithIDSelector.fileID>(envelope.FileID);

            var graphExtension = graph.GetExtension<WikiFileMaintenanceDSExt>();
            graphExtension.Envelope.Current = envelope;

            var docusignService = new DocuSignService();
            DocuSignAccount dsAccount = graphExtension.DocuSignAccount
                .Search<DocuSignAccount.accountID>(envelope.DocuSignAccountID);

            VerifyAccount(dsAccount);

            var model = GetBaseRequestModel(envelope, dsAccount);
            var history = docusignService.GetEnvelopeHistory(model);

            using (var ts = new PXTransactionScope())
            {
                if (IsStatusChangeToComplete(envelope, history))
                {
                    var fileInfo = CreateCompletedFile(envelope, docusignService,
                        model, graph, graphExtension, uploadGraph);
                    CreateTaskForRelatedOwner(envelope, fileInfo, history, taskGraph);
                }

                UpdateDocuSignEnvelope(envelope, history, graphExtension);

                CleanExisitingRecipients(envelope, graphExtension);
                UpdateDocuSignSigners(history.Recipients.Signers, graphExtension);
                UpdateCopyRecipients(history.Recipients.CarbonCopies, graphExtension);

                graph.Actions.PressSave();
                ts.Complete();
            }
            return envelope;
        }

        #endregion

        #region Private Functions

        /// <summary>
        /// Create new <see cref="DocuSignEnvelopeInfo"/> envelope in acumatica DB.
        /// </summary>
        /// <param name="graphExtension"></param>
        private DocuSignEnvelopeInfo CreateNewEnvelope()
        {
            DocuSignAccount account = DocuSignAccount.Select();
            VerifyAccount(account);

            var envelopeInfo = new DocuSignEnvelopeInfo
            {
                FileID = RevisionsWithAction.Current.FileID,
                FileRevisionID = RevisionsWithAction.Current.FileRevisionID,
                DocuSignAccountID = account.AccountID,
                LastStatus = EnvelopeStatus.New,
                Theme = string.Empty,
                MessageBody = string.Empty
            };

            envelopeInfo = Envelope.Insert(envelopeInfo);

            CreateDefaultRecipients();

            return envelopeInfo;
        }

        /// <summary>
        /// Create default <see cref="DocuSignRecipient"/> recipients in acumatica DB.
        /// Search <see cref="Contact"/>contacts from privary view of the related note of current file.
        /// </summary>
        /// <param name="graphExtension"></param>
        private void CreateDefaultRecipients()
        {
            var noteSet = PXSelectJoin<NoteDoc, InnerJoin<Note, On<NoteDoc.noteID, Equal<Note.noteID>>>,
                Where<NoteDoc.fileID, Equal<Optional<UploadFileWithIDSelector.fileID>>>,
                OrderBy<Asc<NoteDoc.entityType>>>.Select(Base, RevisionsWithAction.Current.FileID);

            var listContactIds = new List<object>();
            var listDocuSignAssignees = new List<DocuSignRecipient>();

            foreach (var pxResult in noteSet.Select(joinResult => joinResult as PXResult<NoteDoc, Note>))
            {
                if (pxResult == null)
                {
                    return;
                }

                var note = (Note)pxResult;
                if (note == null)
                {
                    return;
                }

                var type = PXBuildManager.GetType(note.EntityType, false);
                if (type != null && note.NoteID.HasValue)
                {
                    var entityHelper = new EntityHelper(Base);
                    var item = entityHelper.GetEntityRow(type, note.NoteID.Value);

                    PXCache cache = Base.Caches[item.GetType()];

                    if (item.GetType() == typeof(Contact))
                    {
                        foreach (string field in cache.Fields)
                        {
                            var attributes = cache.GetAttributes(null, field).ToList();
                            var identityAttribute = attributes.OfType<PXDBIdentityAttribute>().FirstOrDefault();
                            if (identityAttribute != null)
                            {
                                var value = item.GetType().GetProperty(field).GetValue(item);
                                if (value is int)
                                {
                                    listContactIds.Add(value);
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (string field in cache.Fields)
                        {
                            var attributes = cache.GetAttributes(null, field).ToList();
                            var selectorAttribute = attributes.OfType<PXSelectorAttribute>().FirstOrDefault();
                            if (selectorAttribute != null)
                            {
                                if ((selectorAttribute as PXSelectorAttribute).Field.DeclaringType.GetTypeInfo()
                                                                              .ImplementedInterfaces.Contains(typeof(PX.Objects.CS.IContact)) ||
                                    (selectorAttribute as PXSelectorAttribute).Field.DeclaringType.GetTypeInfo()
                                                                              .ImplementedInterfaces.Contains(typeof(IContactBase)))
                                {
                                    var contactInfo = PXSelectorAttribute.Select(cache, item, selectorAttribute.FieldName);
                                    if (contactInfo == null) continue;
                                    string sEmail = String.Empty, sDisplayName = String.Empty;
                                    sEmail = (contactInfo is IContactBase) ?
                                                (contactInfo as IContactBase).EMail : (contactInfo is PX.Objects.CS.IContact) ?
                                                (contactInfo as PX.Objects.CS.IContact).Email : String.Empty;

                                    sDisplayName = (contactInfo.GetType().GetProperty("DisplayName") != null) ?
                                                        Convert.ToString(contactInfo.GetType().GetProperty("DisplayName").GetValue(contactInfo)) :
                                                        Convert.ToString(contactInfo.GetType().GetProperty("FullName").GetValue(contactInfo));
                                    if (String.IsNullOrEmpty(sDisplayName))
                                        sDisplayName = Convert.ToString(contactInfo.GetType().GetProperty("FullName").GetValue(contactInfo));
                                    if (!String.IsNullOrEmpty(sEmail) && !(String.IsNullOrEmpty(sDisplayName)))
                                    {
                                        if (!listDocuSignAssignees.Any(x => x.Email == sEmail && x.Name == sDisplayName))
                                        {
                                            var recipient = new DocuSignRecipient
                                            {
                                                Email = sEmail,
                                                Name = sDisplayName
                                            };
                                            listDocuSignAssignees.Add(recipient);
                                            Recipients.Insert(recipient);
                                        }
                                    }
                                }
                            }
                        }
                    }

                }
            }

            var list = new List<DocuSignRecipient>();
            listContactIds
                .Where(x => x != null)
                .Distinct()
                .ForEach(x => CreateDefaultRecipient(x, list));
        }

        /// <summary>
        /// Insert new <see cref="DocuSignRecipient"/> recipient in acumatica DB.
        /// </summary>
        private void CreateDefaultRecipient(object contactId, ICollection<DocuSignRecipient> rec)
        {
            Contact contact = Contacts.Search<Contact.contactID>(contactId).FirstOrDefault();
            if (!string.IsNullOrEmpty(contact?.EMail)
                && !string.IsNullOrEmpty(contact.FullName)
                && !rec.Any(x => x.Email == contact.EMail && x.Name == contact.FullName))
            {
                var recipient = new DocuSignRecipient
                {
                    Email = contact.EMail,
                    Name = contact.FullName,
                };

                rec.Add(recipient);
                Recipients.Insert(recipient);
            }
        }

        private CreateEnvelopeRequestModel CreateSendRequestModel(DocuSignAccount account, WikiFileMaintenanceDSExt graphDsExt)
        {
            if (graphDsExt.Envelope.Current.FileID != null && graphDsExt.Envelope.Current.FileRevisionID != null)
            {
                var fileGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
                fileGraph.Clear();
                fileGraph.Files.Current = fileGraph.Files.Search<UploadFileWithIDSelector.fileID>(graphDsExt.Envelope.Current.FileID);

                var file = fileGraph.GetFile(Envelope.Current.FileID.Value, Envelope.Current.FileRevisionID.Value);

                var requestModel = new CreateEnvelopeRequestModel
                {
                    EnvelopeInfo = graphDsExt.Envelope.Current,
                    Recipients = new List<DocuSignRecipient>(),
                    CarbonRecipients = new List<DocuSignRecipient>(),
                    FileInfo = file,
                    DocuSignAccount = account,
                    EnvelopeId = graphDsExt.Envelope.Current.EnvelopeID
                };

                InitRecipientsForSendRequest(requestModel, graphDsExt);

                return requestModel;
            }

            throw new PXException(Messages.DocuSignEnvelopeNotExists);
        }

        private void InitRecipientsForSendRequest(CreateEnvelopeRequestModel requestModel, WikiFileMaintenanceDSExt graphDsExt)
        {
            var recipients = graphDsExt.Recipients.Select();

            foreach (DocuSignRecipient recipient in recipients)
            {
                switch (recipient.Type)
                {
                    case DocuSignRecipient.RecipientTypes.Signer:
                        requestModel.Recipients.Add(recipient);
                        break;
                    case DocuSignRecipient.RecipientTypes.CopyRecipient:
                        requestModel.CarbonRecipients.Add(recipient);
                        break;
                }
            }
        }

        private void SendCreateEnvelopeRequest(DocuSignService dsService, DocuSignEnvelopeInfo envelope,
            DocuSignAccount account, WikiFileMaintenanceDSExt graphDsExt)
        {
            var sendRequest = CreateSendRequestModel(account, graphDsExt);
            var sendResponse = dsService.CreateEnvelope(sendRequest);

            envelope.EnvelopeID = sendResponse.EnvelopeSummary.EnvelopeId;
            envelope.ActivityDate = DateTime.Parse(sendResponse.EnvelopeSummary.StatusDateTime);
            envelope.SendDate = DateTime.Parse(sendResponse.EnvelopeSummary.StatusDateTime);
            envelope.LastStatus = sendResponse.EnvelopeSummary.Status;

            graphDsExt.Envelope.Update(envelope);
            graphDsExt.Base.Actions.PressSave();

            throw new PXRedirectToUrlException(sendResponse.ViewUrl.Url, string.Empty);
        }

        private void SendUpdateEnvelopeRequest(DocuSignService dsService, DocuSignEnvelopeInfo envelope,
            DocuSignAccount account, WikiFileMaintenanceDSExt graphDsExt)
        {
            var updateRequest = CreateSendRequestModel(account, graphDsExt);
            updateRequest.EnvelopeInfo.EnvelopeID = envelope.EnvelopeID;


            var updateResponse = dsService.UpdateEnvelope(updateRequest);

            graphDsExt.Envelope.Update(envelope);
            graphDsExt.Base.Actions.PressSave();

            throw new PXRedirectToUrlException(updateResponse.ViewUrl.Url, string.Empty);
        }

        private DocuSignEnvelopeInfo GetActualEnvelope(WikiFileMaintenance graph, DocuSignEnvelopeInfo existingEnvelope)
        {
            var uploadGraph = PXGraph.CreateInstance<UploadFileMaintenance>();
            var taskGraph = PXGraph.CreateInstance<CRTaskMaint>();
            return existingEnvelope.LastStatus != EnvelopeStatus.New
                ? CheckStatus(graph, existingEnvelope, uploadGraph, taskGraph)
                : existingEnvelope;
        }

        private void UpdateDocuSignEnvelope(DocuSignEnvelopeInfo envelope,
            GetEnvelopeHistoryResponseModel history, WikiFileMaintenanceDSExt graphDsExt)
        {
            envelope.ActivityDate = DateTime.Parse(history.Envelope.LastModifiedDateTime);
            envelope.LastStatus = history.Envelope.Status;
            envelope.MessageBody = history.Envelope.EmailBlurb;
            envelope.Theme = history.Envelope.EmailSubject;
            envelope.ExpiredDays = int.Parse(history.Notification.Expirations.ExpireAfter);
            envelope.WarnDays = int.Parse(history.Notification.Expirations.ExpireWarn);
            envelope.WarnDays = int.Parse(history.Notification.Expirations.ExpireWarn);
            envelope.FirstReminderDay = int.Parse(history.Notification.Reminders.ReminderDelay);
            envelope.ReminderFrequency = int.Parse(history.Notification.Reminders.ReminderFrequency);
            envelope.SendReminders = bool.Parse(history.Notification.Reminders.ReminderEnabled);

            DateTime expirationDate;
            envelope.ExpirationDate = DateTime.TryParse(history.Envelope.SentDateTime, out expirationDate)
                ? expirationDate.AddDays(envelope.ExpiredDays.Value)
                : envelope.SendDate?.AddDays(envelope.ExpiredDays.Value);

            graphDsExt.Envelope.Update(envelope);
        }

        private void UpdateCopyRecipients(IEnumerable<CarbonCopy> carbonCopies, WikiFileMaintenanceDSExt graphDsExt)
        {
            foreach (var carbonCopy in carbonCopies)
            {
                var recipient = new DocuSignRecipient
                {
                    Email = carbonCopy.Email,
                    Name = carbonCopy.Name,
                    Status = carbonCopy.Status,
                    DeliveredDateTime = carbonCopy.DeliveredDateTime != null
                        ? Convert.ToDateTime(carbonCopy.DeliveredDateTime)
                        : default(DateTime?),
                    SignedDateTime = carbonCopy.SignedDateTime != null
                        ? Convert.ToDateTime(carbonCopy.SignedDateTime)
                        : default(DateTime?),
                    Type = DocuSignRecipient.RecipientTypes.CopyRecipient,
                    CustomMessage = carbonCopy.Note,
                    Position = string.IsNullOrEmpty(carbonCopy.RoutingOrder)
                    ? (int?)null
                    : int.Parse(carbonCopy.RoutingOrder)
                };
                graphDsExt.Recipients.Insert(recipient);

            }
        }

        private void UpdateDocuSignSigners(IEnumerable<Signer> signers, WikiFileMaintenanceDSExt graphDsExt)
        {
            foreach (var signer in signers)
            {
                var recipient = new DocuSignRecipient
                {
                    Email = signer.Email,
                    Name = signer.Name,
                    Status = signer.Status,
                    DeliveredDateTime = signer.DeliveredDateTime != null
                        ? Convert.ToDateTime(signer.DeliveredDateTime)
                        : default(DateTime?),
                    SignedDateTime = signer.SignedDateTime != null
                        ? Convert.ToDateTime(signer.SignedDateTime)
                        : default(DateTime?),
                    Type = DocuSignRecipient.RecipientTypes.Signer,
                    CustomMessage = signer.Note,
                    Position = string.IsNullOrEmpty(signer.RoutingOrder)
                        ? (int?)null
                        : int.Parse(signer.RoutingOrder)
                };
                graphDsExt.Recipients.Insert(recipient);

            }
        }

        /// <summary>
        /// Used for creating new task <see cref="CRActivity"/> after current envelope signed.
        /// </summary>
        private void CreateTaskForRelatedOwner(DocuSignEnvelopeInfo envelope, FileInfo fileInfo,
            GetEnvelopeHistoryResponseModel history, CRTaskMaint graph)
        {
            graph.Clear();

            CRActivity activity = graph.Tasks.Insert();
            activity.PercentCompletion = 0;
            activity.OwnerID = envelope.CreatedByID;
            activity.Subject = $"Document {fileInfo.FullName} was signed.";
            activity.ClassID = CRActivityClass.Task;
            activity.StartDate = DateTime.Parse(history.Envelope.CompletedDateTime);
            graph.Actions.PressSave();
        }

        /// <summary>
        /// Used for creating new file <see cref="UploadFile"/> after current envelope completed.
        /// </summary>
        private FileInfo CreateCompletedFile(DocuSignEnvelopeInfo envelope, DocuSignService docusignService,
            BaseRequestModel model, WikiFileMaintenance graph,
            WikiFileMaintenanceDSExt graphDsExt, UploadFileMaintenance uploadGraph)
        {
            uploadGraph.Clear();
            var responseModel = docusignService.GetEnvelopeDocument(model);

            graph.Files.Current = PXSelect<UploadFileWithIDSelector,
                Where<UploadFileWithIDSelector.fileID, Equal<Required<UploadFileWithIDSelector.fileID>>>>
                .Select(Base, envelope.FileID, envelope.FileRevisionID);

            var currentFile = uploadGraph.GetFile(envelope.FileID.Value);

            var fileExtension = GetFileExtension(currentFile.OriginalName);
            var docuSignedFileName = currentFile.FullName.Replace(fileExtension, " DocuSigned.pdf");
            var uniqueDocuSignedFileName = CreateFileWithUniqueName(uploadGraph, docuSignedFileName, envelope.FileRevisionID);
            var docuSignFile = new FileInfo(Guid.NewGuid(), uniqueDocuSignedFileName, null, responseModel.Document);
            uploadGraph.SaveFile(docuSignFile);

            NoteDoc currentNoteDoc =
                PXSelect<NoteDoc, Where<NoteDoc.fileID, Equal<Required<NoteDoc.fileID>>>>.Select(Base,
                    envelope.FileID);

            if (currentNoteDoc != null)
            {
                NoteDoc noteDoc = new NoteDoc
                {
                    NoteID = currentNoteDoc.NoteID,
                    FileID = docuSignFile.UID
                };

                graph.EntitiesRecords.Insert(noteDoc);
            }
            var envelopeInfo = new DocuSignEnvelopeInfo
            {
                FileID = docuSignFile.UID,
                FileRevisionID = 1,
                DocuSignAccountID = envelope.DocuSignAccountID,
                LastStatus = EnvelopeStatus.Completed,
                ActivityDate = envelope.LastModifiedDateTime,
                Theme = string.Empty,
                MessageBody = string.Empty,
                IsFinalVersion = true,
                EnvelopeID = envelope.EnvelopeID
            };

            envelope.CompletedFileID = docuSignFile.UID;
            envelope.CompletedFileName = docuSignFile.OriginalName;
            graphDsExt.Envelope.Update(envelope);

            graphDsExt.Envelope.Insert(envelopeInfo);

            return currentFile;
        }

        private string CreateFileWithUniqueName(UploadFileMaintenance uploadFileGraph,
            string fileName, int? fileRevisionId)
        {
            var fileExtension = GetFileExtension(fileName);
            var fileBase = fileName.Replace(fileExtension, string.Empty);
            return $"{fileBase} ({fileRevisionId}){fileExtension}";

        }

        private string GetFileExtension(string fileName)
        {
            var fileExtension = fileName
                .Substring(fileName
                    .IndexOf(".", StringComparison.Ordinal));
            return fileExtension;
        }

        private void VerifyStatus(DocuSignEnvelopeInfo existingEnvelope)
        {
            if (existingEnvelope.LastStatus != EnvelopeStatus.New &&
                existingEnvelope.LastStatus != EnvelopeStatus.Created)
            {
                throw new PXSetPropertyException(Messages.EnvelopeStatusChanged,
                    PXErrorLevel.Error, typeof(DocuSignEnvelopeInfo.docuSignAccountID).Name);
            }
        }

        private void CleanCache()
        {
            Envelope.Cache.Clear();
            Envelope.Cache.ClearQueryCache();
            Recipients.Cache.Clear();
            Recipients.Cache.ClearQueryCache();
        }

        private BaseRequestModel GetBaseRequestModel(DocuSignEnvelopeInfo envelope, DocuSignAccount dsAccount)
        {
            return new BaseRequestModel
            {
                EnvelopeId = envelope.EnvelopeID,
                DocuSignAccount = dsAccount
            };
        }

        private bool IsStatusChangeToComplete(DocuSignEnvelopeInfo envelope, GetEnvelopeHistoryResponseModel history)
        {
            return history.Envelope.Status == EnvelopeStatus.Completed
                   && envelope.LastStatus != EnvelopeStatus.Completed;
        }

        private void VerifyRecipients()
        {
            var recipients = Recipients.Select();
            if (recipients.Count == 0)
            {
                throw new PXException(Messages.DocuSignRecipientsRequired);
            }
            var groupBy = recipients.FirstTableItems.GroupBy(x => new { x.Position, x.Email, x.Name });
            if (groupBy.Any(x => x.Count() > 1))
            {
                throw new PXException(Messages.DocuSignRecipientsUnique);
            }
        }

        private void VerifyAccount(DocuSignAccount account)
        {
            if (account == null)
            {
                throw new PXException(Messages.DocuSignAccountNotExists);
            }
            if (account.Active != null && !account.Active.Value)
            {
                throw new PXException(Messages.DocuSignAccountInActive);
            }
        }

        private void PerformSendToDocuSignRequest()
        {
            var envelope = Envelope.Current;
            VerifyRecipients();

            PXLongOperation.StartOperation(Base, () =>
            {
                VerifyStatus(envelope);
                var dsService = new DocuSignService();
                var graph = PXGraph.CreateInstance<WikiFileMaintenance>();
                var actualEnvelope = GetActualEnvelope(graph, envelope);

                var graphExtension = graph.GetExtension<WikiFileMaintenanceDSExt>();
                graphExtension.Envelope.Current = actualEnvelope;

                var dsAccount = graphExtension.DocuSignAccount
                    .Search<DocuSignAccount.accountID>(envelope.DocuSignAccountID);
                var model = GetBaseRequestModel(actualEnvelope, dsAccount);

                VerifyAccount(dsAccount);
                VerifyStatus(actualEnvelope);

                if (actualEnvelope.LastStatus != EnvelopeStatus.Created || !dsService.IsFileExist(model))
                {
                    SendCreateEnvelopeRequest(dsService, actualEnvelope, dsAccount, graphExtension);
                }
                else
                {
                    SendUpdateEnvelopeRequest(dsService, actualEnvelope, dsAccount, graphExtension);
                }
            });
        }

        private void VoidDeletedDocument(DocuSignEnvelopeInfo envelope)
        {
            var account = DocuSignAccount.Search<DocuSignAccount.accountID>(envelope.DocuSignAccountID);
            VerifyAccount(account);

            try
            {
                var dsService = new DocuSignService();
                var request = new VoidEnvelopeRequestModel
                {
                    DocuSignAccount = account,
                    EnvelopeId = envelope.EnvelopeID,
                    VoidReason = Messages.DocuSignEnvelopeDefaultVoidReason
                };

                dsService.VoidEnvelope(request);
            }
            catch (PXException)
            {
            }

            Envelope.Cache.SetStatus(envelope, PXEntryStatus.Deleted);
        }

        private void VoidDraftDocument(DocuSignEnvelopeInfo envelope)
        {
            var account = DocuSignAccount.Search<DocuSignAccount.accountID>(envelope.DocuSignAccountID);
            VerifyAccount(account);

            try
            {
                var dsService = new DocuSignService();
                var request = new VoidEnvelopeRequestModel
                {
                    DocuSignAccount = account,
                    EnvelopeId = envelope.EnvelopeID,
                };

                dsService.VoidDraftEnvelope(request);
            }
            catch (PXException)
            {
            }


            Envelope.Cache.SetStatus(envelope, PXEntryStatus.Deleted);
        }

        private void CleanExisitingRecipients(DocuSignEnvelopeInfo envelope, WikiFileMaintenanceDSExt graphExtension)
        {
            var recipients = PXSelect<DocuSignRecipient,
                Where<DocuSignRecipient.envelopeInfoID,
                    Equal<Required<DocuSignRecipient.envelopeInfoID>>>>.Select(Base, envelope.EnvelopeInfoID);

            foreach (var pxResult in recipients)
            {
                graphExtension.Recipients.Delete(pxResult);
            }
        }

        private void CleanBaseEnvelope(DocuSignEnvelopeInfo envelope)
        {
            if (envelope.IsFinalVersion.HasValue && envelope.IsFinalVersion.Value)
            {
                DocuSignEnvelopeInfo baseEnvelope = PXSelect<DocuSignEnvelopeInfo,
                    Where<DocuSignEnvelopeInfo.completedFileID,
                        Equal<Required<DocuSignEnvelopeInfo.completedFileID>>>>
                    .Select(Base, envelope.FileID);

                baseEnvelope.CompletedFileName = null;
                baseEnvelope.CompletedFileID = null;

                Envelope.Update(baseEnvelope);
            }
        }

        #endregion
    }
}