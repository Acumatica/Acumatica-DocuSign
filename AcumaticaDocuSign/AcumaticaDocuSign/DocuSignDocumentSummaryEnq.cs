using System;
using System.Collections;
using PX.Data;
using PX.Objects.Common;
using PX.Objects.CR;
using PX.SM;

namespace AcumaticaDocuSign
{
    /// <summary>
    /// Represent inqury logic fo the <see cref="DocuSignEnvelopeInfo"/> envelopes
    /// Records of this type can be viewed through the Docusign Central (DS.40.10.00) screen.
    /// </summary>
    [DashboardType((int)DashboardTypeAttribute.Type.Default)]
    public class DocuSignDocumentSummaryEnq : PXGraph<DocuSignDocumentSummaryEnq>
    {
        #region Selects

        public PXFilter<EnvelopeFilter> Filter;

        [PXFilterable]
        public PXSelectJoinGroupBy<DocuSignEnvelopeInfo,
                InnerJoin<UploadFileRevision,
                    On<UploadFileRevision.fileID, Equal<DocuSignEnvelopeInfo.fileID>,
                    And<UploadFileRevision.fileRevisionID, Equal<DocuSignEnvelopeInfo.fileRevisionID>>>,
                InnerJoin<UploadFileWithIDSelector, On<UploadFileWithIDSelector.fileID, Equal<UploadFileRevision.fileID>>,
                InnerJoin<SiteMap, On<SiteMap.screenID, Equal<UploadFileWithIDSelector.primaryScreenID>>>>>,
                Where<DocuSignEnvelopeInfo.lastStatus, NotEqual<Empty>,
                    And<DocuSignEnvelopeInfo.isFinalVersion, NotEqual<True>>>,
                Aggregate<GroupBy<DocuSignEnvelopeInfo.envelopeInfoID>>> Envelopes;

        public PXFilter<VoidRequestFilter> VoidRequest;

        public PXSelect<DocuSignAccount,
            Where<DocuSignAccount.accountID, Equal<Required<DocuSignAccount.accountID>>>> DocuSignAccount;

        public PXSelect<SiteMap> SiteMap;

        public PXCancel<DocuSignEnvelopeInfo> Cancel;

        public virtual IEnumerable envelopes()
        {
            PXSelectBase<DocuSignEnvelopeInfo> query = new PXSelectJoinGroupBy<DocuSignEnvelopeInfo,
                InnerJoin<UploadFileRevision,
                    On<UploadFileRevision.fileID, Equal<DocuSignEnvelopeInfo.fileID>,
                    And<UploadFileRevision.fileRevisionID, Equal<DocuSignEnvelopeInfo.fileRevisionID>>>,
                InnerJoin<UploadFileWithIDSelector, On<UploadFileWithIDSelector.fileID, Equal<UploadFileRevision.fileID>>,
                InnerJoin<SiteMap, On<SiteMap.screenID, Equal<UploadFileWithIDSelector.primaryScreenID>>>>>,
                Where<DocuSignEnvelopeInfo.lastStatus, NotEqual<Empty>,
                    And<DocuSignEnvelopeInfo.isFinalVersion, NotEqual<True>>>,
                Aggregate<GroupBy<DocuSignEnvelopeInfo.envelopeInfoID>>>(this);

            EnvelopeFilter filter = Filter.Current;

            if (filter.OwnerID != null)
            {
                query.WhereAnd<Where<DocuSignEnvelopeInfo.createdByID, Equal<Current<EnvelopeFilter.ownerID>>>>();
            }

            return query.Select();
        }

        #endregion

        #region Events

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute),
                                  "DisplayName",
                                  Messages.SiteMapTitleDisplayName)]
        protected void SiteMap_Title_CacheAttached(PXCache sender)
        {
        }

        [PXMergeAttributes(Method = MergeMethod.Append)]
        [PXCustomizeBaseAttribute(typeof(PXUIFieldAttribute),
                                  "DisplayName",
                                  Messages.DocuSignEnvelopeCreatedDate)]
        protected void DocuSignEnvelopeInfo_CreatedDateTime_CacheAttached(PXCache sender)
        {
        }

        protected virtual void EnvelopeFilter_RowSelected(PXCache sender, PXRowSelectedEventArgs e)
        {
            var filter = e.Row as EnvelopeFilter;

            if (filter?.MyOwner != null)
            {
                PXUIFieldAttribute.SetEnabled<EnvelopeFilter.ownerID>(Filter.Cache, null, !filter.MyOwner.Value);
            }
        }

        public DocuSignDocumentSummaryEnq()
        {
            if (Envelopes.Any())
            {
                ViewDocuSign.SetEnabled(true);
                CheckStatus.SetEnabled(true);
            }
            else
            {
                ViewDocuSign.SetEnabled(false);
                CheckStatus.SetEnabled(false);
            }
        }

        #endregion

        #region Actions

        public PXAction<DocuSignEnvelopeInfo> Delete;
        [PXUIField(DisplayName = "", MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        [PXButton(ImageKey = PX.Web.UI.Sprite.Main.Remove)]
        public virtual IEnumerable delete(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                var envelope = Envelopes.Current;
                PXLongOperation.StartOperation(this, () =>
                {
                    var maintenanceGraph = CreateInstance<WikiFileMaintenance>();
                    var fileMaintenanceExtension = maintenanceGraph.GetExtension<WikiFileMaintenanceDSExt>();
                    var uploadGraph = CreateInstance<UploadFileMaintenance>();
                    var taskGraph = CreateInstance<CRTaskMaint>();
                    fileMaintenanceExtension.CheckStatus(maintenanceGraph, envelope, uploadGraph, taskGraph);

                    if (envelope.IsActionsAvailable != null
                        && envelope.IsActionsAvailable.Value
                        && envelope.EnvelopeInfoID.HasValue)
                    {
                        try
                        {
                            var account = DocuSignAccount.Select(envelope.DocuSignAccountID);
                            VerifyAccount(account);

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
                    }
                    else if (envelope.LastStatus == EnvelopeStatus.Created)
                    {
                        try
                        {
                            var account = DocuSignAccount.Select(envelope.DocuSignAccountID);
                            VerifyAccount(account);

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
                    }
                    Envelopes.Delete(envelope);
                    Actions.PressSave();
                });
            }
            return adapter.Get();
        }

        /// <summary>
        /// Redirect to <see cref="WikiFileMaintenance"/> screen with current file.
        /// </summary>
        public PXAction<DocuSignEnvelopeInfo> ViewFile;
        [PXUIField(DisplayName = Messages.ViewFile, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
        [PXButton]
        public virtual IEnumerable viewFile(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                WikiFileMaintenance graph = CreateInstance<WikiFileMaintenance>();
                graph.Files.Current = graph.Files.Search<UploadFileWithIDSelector.fileID>(Envelopes.Current.FileID);

                throw new PXRedirectRequiredException(graph, string.Empty) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            return adapter.Get();
        }

        /// <summary>
        /// Redirect to <see cref="DocuSignEnvelopeEnq"/> screen with current file.
        /// </summary>
        public PXAction<DocuSignEnvelopeInfo> ViewHistory;
        [PXUIField(DisplayName = Messages.ViewHistory, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select, Enabled = false)]
        [PXButton]
        public virtual IEnumerable viewHistory(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                DocuSignEnvelopeEnq graph = CreateInstance<DocuSignEnvelopeEnq>();
                graph.Envelopes.Current = graph.Envelopes.Search<DocuSignEnvelopeInfo.envelopeInfoID>(Envelopes.Current.EnvelopeInfoID);

                throw new PXRedirectRequiredException(graph, string.Empty) { Mode = PXBaseRedirectException.WindowMode.NewWindow };
            }
            return adapter.Get();
        }

        /// <summary>
        /// Open smart panel with <see cref="VoidRequest"/> void reason.
        /// </summary>
        public PXAction<DocuSignEnvelopeInfo> VoidEnvelope;
        [PXButton]
        [PXUIField(DisplayName = Messages.Void, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable voidEnvelope(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                VoidRequest.Cache.Clear();
                VoidRequest.Cache.ClearQueryCache();

                VoidRequest.AskExt(true);
            }
            return adapter.Get();
        }

        /// <summary>
        /// Send void request to <see cref="DocuSignService"/> service.
        /// </summary>
        public PXAction<DocuSignEnvelopeInfo> SendVoidRequest;
        [PXUIField(DisplayName = Messages.Ok, MapEnableRights = PXCacheRights.Select, MapViewRights = PXCacheRights.Select, Visible = false)]
        [PXButton(SpecialType = PXSpecialButtonType.Cancel)]
        public virtual IEnumerable sendVoidRequest(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                var envelope = Envelopes.Current;
                var voidReason = VoidRequest.Current;
                PXLongOperation.StartOperation(this, () =>
                {
                    var maintenanceGraph = CreateInstance<WikiFileMaintenance>();
                    var fileMaintenanceExtension = maintenanceGraph.GetExtension<WikiFileMaintenanceDSExt>();
                    var uploadGraph = CreateInstance<UploadFileMaintenance>();
                    var taskGraph = CreateInstance<CRTaskMaint>();
                    var actualEnvelope = fileMaintenanceExtension.CheckStatus(maintenanceGraph, envelope, uploadGraph, taskGraph);
                    if (!actualEnvelope.IsActionsAvailable.HasValue || !actualEnvelope.IsActionsAvailable.Value)
                    {
                        throw new PXException(Messages.EnvelopeVoidIsNotAvailable);
                    }

                    var account = DocuSignAccount.Select(envelope.DocuSignAccountID);
                    VerifyAccount(account);

                    var dsService = new DocuSignService();
                    var request = new VoidEnvelopeRequestModel
                    {
                        DocuSignAccount = account,
                        EnvelopeId = actualEnvelope.EnvelopeID,
                        VoidReason = voidReason.VoidReason
                    };

                    dsService.VoidEnvelope(request);
                    fileMaintenanceExtension.CheckStatus(maintenanceGraph, envelope, uploadGraph, taskGraph);
                });
            }
            return adapter.Get();
        }

        /// <summary>
        /// Send remind request to <see cref="DocuSignService"/> service.
        /// </summary>
        public PXAction<DocuSignEnvelopeInfo> Remind;
        [PXButton(SpecialType = PXSpecialButtonType.Cancel)]
        [PXUIField(DisplayName = Messages.Remind, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable remind(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                var envelope = Envelopes.Current;
                PXLongOperation.StartOperation(this, () =>
                {
                    var maintenanceGraph = CreateInstance<WikiFileMaintenance>();
                    var fileMaintenanceExtension = maintenanceGraph.GetExtension<WikiFileMaintenanceDSExt>();
                    var uploadGraph = CreateInstance<UploadFileMaintenance>();
                    var taskGraph = CreateInstance<CRTaskMaint>();
                    var actualEnvelope = fileMaintenanceExtension.CheckStatus(maintenanceGraph, envelope, uploadGraph, taskGraph);
                    if (!actualEnvelope.IsActionsAvailable.HasValue || !actualEnvelope.IsActionsAvailable.Value)
                    {
                        throw new PXException(Messages.EnvelopeRemindIsNotAvailable);
                    }

                    var account = DocuSignAccount.Select(envelope.DocuSignAccountID);
                    VerifyAccount(account);

                    var dsService = new DocuSignService();
                    var request = new BaseRequestModel
                    {
                        DocuSignAccount = account,
                        EnvelopeId = actualEnvelope.EnvelopeID
                    };

                    dsService.RemindEnvelope(request);
                });
            }
            return adapter.Get();
        }

        /// <summary>
        /// Raise new window with DocuSign Iframe.
        /// </summary>
        public PXAction<DocuSignEnvelopeInfo> ViewDocuSign;
        [PXButton]
        [PXUIField(DisplayName = Messages.ViewDocuSign, MapEnableRights = PXCacheRights.Update, MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable viewDocuSign(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                var account = DocuSignAccount.Select(Envelopes.Current.DocuSignAccountID);
                VerifyAccount(account);

                var dsService = new DocuSignService();
                var request = new BaseRequestModel
                {
                    DocuSignAccount = account,
                    EnvelopeId = Envelopes.Current.EnvelopeID
                };

                var url = dsService.Redirect(request);
                throw new PXRedirectToUrlException(url.Url, string.Empty);
            }
            return adapter.Get();
        }

        /// <summary>
        /// Send check status request to <see cref="WikiFileMaintenanceDSExt"/> graph.
        /// </summary>
        public PXAction<DocuSignEnvelopeInfo> CheckStatus;
        [PXButton(SpecialType = PXSpecialButtonType.Refresh)]
        [PXUIField(DisplayName = Messages.CheckStatus, MapEnableRights = PXCacheRights.Update,
            MapViewRights = PXCacheRights.Select)]
        public virtual IEnumerable checkStatus(PXAdapter adapter)
        {
            if (Envelopes.Current != null)
            {
                var envelope = Envelopes.Current;
                PXLongOperation.StartOperation(this, () =>
                {
                    var maintenanceGraph = CreateInstance<WikiFileMaintenance>();
                    var fileMaintenanceExtension = maintenanceGraph.GetExtension<WikiFileMaintenanceDSExt>();
                    var uploadGraph = CreateInstance<UploadFileMaintenance>();
                    var taskGraph = CreateInstance<CRTaskMaint>();
                    fileMaintenanceExtension.CheckStatus(maintenanceGraph, envelope, uploadGraph, taskGraph);
                });
            }
            return adapter.Get();
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

        #endregion
    }
}
