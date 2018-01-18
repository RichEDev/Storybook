CREATE PROCEDURE [dbo].[GetEmailTemplate]
AS
BEGIN
select emailtemplateid,templatename,subject,bodyhtml,priority,basetableid,systemtemplate,createdon,createdby,modifiedon,modifiedby,sendEmail,
	emailDirection,sendNote,note, templateId, CanSendMobileNotification, MobileNotificationMessage from dbo.emailTemplates 
END