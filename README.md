[![Project Status](http://opensource.box.com/badges/active.svg)](http://opensource.box.com/badges)

DocuSign for Acumatica
==================================
DocuSign provides a complete solution to send, sign and manage documents accelerating the time from lead to deal. With Docusign, users can quickly and securely review and sign documents.
With the integration in Acumatica, users can now take advantage of the Docusign capability from within Acumatica Cloud ERP. Users working on a sales order or an SOW or a contract in an opportunity, can now send it for signing from within Acumatica’s Document management module.
With DocuSign for Acumatica, users can:
- Create envelopes and prepare documents for signing
- Send documents for signing and manage documents
- Check status of document sent for signing
- Send documents from CRM, Financials, Distribution and other modules using Document Management

Note: DocuSign account is required to use this integration.
More info here: https://www.docusign.com/b/products-and-pricing

Please visit https://www.docusign.com/developer-center/api-overview#go-live to get Integrator Key.

###Prerequisites
* Acumatica 6.0 or higher

Getting Started
-----------

### Install DocuSign Customization Project
1. Download AcumaticaDocuSignPackage.zip from https://portal.acumatica.com/downloads/docusignintegration/
2. In your Acumatica ERP instance, import this as a customization project
3. Publish the customization project

![Screenshot](/_ReadMeImages/Image1.png)

### Configure DocuSign Account
Next step is to configure the DocuSign Account from Configuration -> Document Management -> DocuSign Accounts.

![Screenshot](/_ReadMeImages/Image2.png)

1. Provide a name for this Docusign Account (e.g. MYDOCUSIGN).
2. Select either “Individual” or “Shared” (Corporate) account type
3. Assing the account to Acumatica user (This user can send documents for signing using the Docusign account)
4. Specify the Email and Password for the Docusign account (If you don’t have an DocuSign account, create one first at http://www.docusign.com)
5. Click “Test Connection” to verify the configuration

###	Prepare and Send Document
With the configuration completed, user can now send documents for signing from within Acumatica. This can be initiated from any Acumatica module (Financials, CRM, Distribution) where files are handled. (See Image below)
![Screenshot](/_ReadMeImages/Image3.png)

Clicking Edit in the above window, launches the “File Maintenance” dialog with the “DocuSign” option.
![Screenshot](/_ReadMeImages/Image4.png)

Click on “DocuSign” to prepare the envelope. In this dialog, specify subject and message and specify the receipients to sign the document and the ones to receive copy. You can manually add the receipents or use the contact selector to pick a contact from Acumatica. 
If the underlying entity had a contact specified (e.g. Contact in Opportunity), that contact will be loaded automatically.

![Screenshot](/_ReadMeImages/Image5.png)

Click “SEND” to get the document over to DocuSign for signing. This will popup a DocuSign screen to place Docusign tags (e.g. name, company info, signature) and to send it for signing.
Once the document is sent for signing, the status will be updated in the “File Maintenance” dialog. 

![Screenshot](/_ReadMeImages/Image6.png)

### Checking Document Status
User can check the status the documents in the new “DocuSign Central”. Here you will see list of all documents that were sent and with the status. See image below for an example.

![Screenshot](/_ReadMeImages/Image7.png)

Users can check the status of documents either manually or by scheduling a synch from the “DocuSign Sync” screen in Configuration -> Document Management”. In this screen, users can manually get the status update by either using “PROCESS” or “PROCESS ALL” options. This action will perform a synch with DocuSign and retrieve latest status information for the documents in the queue.
Alternatively, users can setup a synch frequency using the  ”Schedules” option to automaticaly retrieve status at specified schedule.

![Screenshot](/_ReadMeImages/Image8.png)

### DocuSign Demo
[![Screenshot](/_ReadMeImages/Image9.png)](https://www.youtube.com/watch?v=Mv-b8_iwLiE&feature=youtu.be)

Known Issues
------------
None at the moment.

## Copyright and License

Copyright © `2017` `Acumatica`

This component is licensed under the MIT License, a copy of which is available online [here](LICENSE.md)
