DECLARE @ApprovedEmailGuid UNIQUEIDENTIFIER = '2D929DDA-C076-4AAD-A1A7-5531A9CA57EF'
DECLARE @ChangeDate DATETIME = getdate()
DECLARE @SystemAdminGrrenlightUserId INT

SELECT @SystemAdminGrrenlightUserId = employeeid
FROM employees
WHERE username = 'adminsystemgreenlight'

MERGE [emailTemplates] AS target
USING (
	VALUES (
		N'Send to the claimant if the approver has approved the vehicle document.'
		,N'Your vehicle document has passed the approval process'
		,N'<p>Dear <span class="merge" contenteditable="false" node="kefbd0c7f-0ecc-406f-be6c-ed9978604439_k5ddbf0ef-fa06-4e7c-a45a-54e50e33307e_n6614acad-0a43-4e30-90ec-84de0792b1d6" title=" Vehicle Documents:Vehicles(Vehicle):Employees(Employee ID):">[First Name]</span>,<br />
		<br />
		Your vehicle <span class="merge" contenteditable="false" node="n0107c206-185d-4cb6-8bb3-9b561ac80cfa" title=" Vehicle Documents:">[Type]</span> document details have been approved.<br />
		<br />
		Any changes made to this information will invalidate the approval.<br />
		<br />
		Yours sincerely,<br />
		Duty of Care Team</p>'
		,0
		,N'28d592d7-4596-49c4-96b8-45655d8df797'
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
		SET target.[templatename] = source.[templatename]
			,target.[subject] = source.[subject]
			,target.[bodyhtml] = source.[bodyhtml]
			,target.[priority] = source.[priority]
			,target.[basetableid] = source.[basetableid]
			,target.[systemtemplate] = source.[systemtemplate]
			,target.[archived] = source.[archived]
			,target.[modifiedon] = source.[modifiedon]
			,target.[modifiedby] = source.[modifiedby]
			,target.[sendEmail] = source.[sendEmail]
			,target.[emailDirection] = source.[emailDirection]
			,target.[sendNote] = source.[sendNote]
			,target.[note] = source.[note]
			,target.[templateId] = source.[templateId]
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

DECLARE @employeejoinviaid INT = dbo.getjoinviaid('84A27D86DA64450525171CB39872B928')

DELETE
FROM emailTemplateBodyFields
WHERE emailtemplateid = @emailtemplateid

INSERT INTO dbo.emailTemplateBodyFields
VALUES (
	@emailtemplateid
	,'6614ACAD-0A43-4E30-90EC-84DE0792B1D6'
	,0
	,@employeejoinviaid
	)
	,(
	@emailtemplateid
	,'0107C206-185D-4CB6-8BB3-9B561AC80CFA'
	,0
	,0
	)
GO