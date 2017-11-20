DECLARE @ApprovedEmailGuid UNIQUEIDENTIFIER = '74403B3D-0C6C-40E9-830F-A9140C8E84FF'
DECLARE @ChangeDate DATETIME = getdate()
DECLARE @SystemAdminGrrenlightUserId INT

SELECT @SystemAdminGrrenlightUserId = employeeid
FROM employees
WHERE username = 'adminsystemgreenlight'

MERGE [emailTemplates] WITH (HOLDLOCK) AS target
USING (
	VALUES (
		N'Send to the claimant if the approver is has approved driving licence document.'
		,N'Your Driving Licence has passed the approval process'
		,N'<p>Dear <span class="merge" contenteditable="false" node="n6614acad-0a43-4e30-90ec-84de0792b1d6" title="Employees:">[To:First Name],</span><br />
		<br />
		The uploaded Driving Licence document has been approved.<br />
		<br />
		Any changes made to this information will invalidate the approval.<br />
		<br />
		Yours sincerely,<br />
		Duty of Care Team<br />
		</p>'
		,0
		,N'f066ef8f-705f-4cd7-8dd7-fdefbf8f3821'
		,1
		,0
		,@ChangeDate
		,@SystemAdminGrrenlightUserId
		,@ChangeDate
		,@SystemAdminGrrenlightUserId
		,1
		,1
		,0
		,NULL
		,@ApprovedEmailGuid
		)
	) AS source([templatename], [subject], [bodyhtml], [priority], [basetableid], [systemtemplate], [archived], [createdon], [createdby], [modifiedon], [modifiedby], [sendEmail], [emailDirection], [sendNote], [note], [templateId])
	ON target.[templateId] = @ApprovedEmailGuid
WHEN MATCHED
	THEN
		UPDATE
		SET [templatename] = source.[templatename]
			,[subject] = source.[subject]
			,[bodyhtml] = source.[bodyhtml]
			,[priority] = source.[priority]
			,[basetableid] = source.[basetableid]
			,[systemtemplate] = source.[systemtemplate]
			,[archived] = source.[archived]
			,[modifiedon] = source.[modifiedon]
			,[modifiedby] = source.[modifiedby]
			,[sendEmail] = source.[sendEmail]
			,[emailDirection] = source.[emailDirection]
			,[sendNote] = source.[sendNote]
			,[note] = source.[note]
			,[templateId] = source.[templateId]
WHEN NOT MATCHED
	THEN
		INSERT (
			[templatename]
			,[subject]
			,[bodyhtml]
			,[priority]
			,[basetableid]
			,[systemtemplate]
			,[archived]
			,[createdon]
			,[createdby]
			,[sendEmail]
			,[emailDirection]
			,[sendNote]
			,[note]
			,[templateId]
			)
		VALUES (
			source.[templatename]
			,source.[subject]
			,source.[bodyhtml]
			,source.[priority]
			,source.[basetableid]
			,source.[systemtemplate]
			,source.[archived]
			,source.[createdon]
			,source.[createdby]
			,source.[sendEmail]
			,source.[emailDirection]
			,source.[sendNote]
			,source.[note]
			,source.[templateId]
			);
DECLARE @emailtemplateid INT

SELECT @emailtemplateid = emailtemplateid
FROM emailtemplates
WHERE templateid = @ApprovedEmailGuid

DELETE
FROM emailTemplateBodyFields
WHERE emailtemplateid = @emailtemplateid

INSERT INTO dbo.emailTemplateBodyFields
VALUES (@emailtemplateid,	'6614ACAD-0A43-4E30-90EC-84DE0792B1D6',	2,	0)
GO