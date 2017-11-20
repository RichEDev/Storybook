CREATE procedure [dbo].[saveTask]
@taskId int,
@subAccountId int,
@taskCmdType smallint,
@regardingId int,
@regardingArea smallint,
@subject nvarchar(150),
@description nvarchar(max),
@taskTypeId int,
@statusId smallint,
@startDate datetime,
@dueDate datetime,
@endDate datetime,
@taskOwnerId int,
@taskOwnerType smallint,
@userId int,
@escalated bit,
@escalationDate datetime,
@CUemployeeID INT,
@CUdelegateID INT
as
begin
	declare @retTaskId int

	if @taskId = 0
	begin
		insert into tasks (subAccountId, regardingId, regardingArea, taskCmdType, taskCreatorId, taskCreationDate, taskTypeId, taskOwnerId, taskOwnerType, subject, description, startDate, dueDate, endDate, statusId, escalated, escalationDate) 
		values (@subAccountId, @regardingId, @regardingArea, @taskCmdType, @userId, getutcdate(), @taskTypeId, @taskOwnerId, @taskOwnerType, @subject, @description, @startDate, @dueDate, @endDate, @statusId, @escalated, @escalationDate)

		set @retTaskId = scope_identity();

		if(@CUemployeeID > 0)
		Begin
			exec addInsertEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, @subject, @subAccountId;
		end
	end
	else
	begin
		declare @oldsubAccountId int;
		declare @oldtaskCmdType smallint;
		declare @oldregardingId int;
		declare @oldregardingArea smallint;
		declare @oldsubject nvarchar(150);
		declare @olddescription nvarchar(max);
		declare @oldtaskTypeId int;
		declare @oldstatusId smallint;
		declare @oldstartDate datetime;
		declare @olddueDate datetime;
		declare @oldendDate datetime;
		declare @oldtaskOwnerId int;
		declare @oldtaskOwnerTypeId int;
		declare @oldescalated bit;
		declare @oldescalationDate datetime;
		select @oldsubAccountId = subAccountId, @oldtaskCmdType = taskCmdType, @oldregardingId = regardingId, @oldregardingArea = regardingArea, @oldsubject = subject, @olddescription = description, @oldtaskTypeId = taskTypeId, @oldstatusId = statusId, @oldstartDate = startDate, @olddueDate = dueDate, @oldendDate = endDate, @oldtaskOwnerId = taskOwnerId, @oldescalated = escalated, @oldescalationDate = escalationDate from tasks where taskId = @taskId;

		update tasks set subAccountId = @subAccountId, regardingId = @regardingId, regardingArea = @regardingArea, taskTypeId = @taskTypeId, taskOwnerId = @taskOwnerId, taskOwnerType = @taskOwnerType, subject = @subject, description = @description, startDate = @startDate, dueDate = @dueDate, endDate = @endDate, statusId = @statusId, escalated = @escalated, escalationDate = @escalationDate, taskCmdType = @taskCmdType where taskId = @taskId;

		set @retTaskId = @taskId;
		
		if(@CUemployeeID > 0)
		Begin
			if @oldsubAccountId <> @subAccountId
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, 'cb449c35-669f-4845-a3dd-7de1b30ee091', @oldsubAccountId, @subAccountId, @subject, @subAccountId;
			if @oldtaskCmdType <> @taskCmdType
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, '651a8731-72de-498d-acf4-0b2171f2e522', @oldtaskCmdType, @taskCmdType, @subject, @subAccountId;
			if @oldregardingId <> @regardingId
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, '6ccf18b0-6333-4e3f-ad5a-b5624b4d6bc7', @oldregardingId, @regardingId, @subject, @subAccountId;
			if @oldregardingArea <> @regardingArea
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, '3c8e960b-5ff2-43d0-a4e0-96e357545ada', @oldregardingArea, @regardingArea, @subject, @subAccountId;
			if @oldsubject <> @subject
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, '56578805-3f20-48ec-b158-632ed5a4d0ee', @oldsubject, @subject, @subject, @subAccountId;
			if @olddescription <> @description
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, 'a22803ed-95a8-45ee-aa18-12d622cafdb3', @olddescription, @description, @subject, @subAccountId;
			if @oldtaskTypeId <> @taskTypeId
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, '86743eee-4d0a-4c95-aaa8-b1093f844600', @oldtaskTypeId, @taskTypeId, @subject, @subAccountId;
			if @oldtaskOwnerTypeId <> @taskOwnerType
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, '4B5A3F0D-6E47-4020-9388-A1CFBE3FE820', @oldtaskOwnerTypeId, @taskOwnerType, @subject, @subAccountId;
			if @oldstatusId <> @statusId
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, 'd4f3dd3a-f614-4446-ac56-6ffa46b05344', @oldstatusId, @statusId, @subject, @subAccountId;
			if @oldstartDate <> @startDate
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, 'c038bae8-a6f1-4984-817c-83c931ed5759', @oldstartDate, @startDate, @subject, @subAccountId;
			if @oldendDate <> @endDate
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, '6a52e48d-fcda-4f37-ba32-c4d687b63fad', @oldendDate, @endDate, @subject, @subAccountId;
			if @olddueDate <> @dueDate
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, '09CC1B73-93DB-44FE-85E8-7874719C5F6B', @olddueDate, @dueDate, @subject, @subAccountId;
			if @oldtaskOwnerId <> @taskOwnerId
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, 'e5309530-9949-4ec9-864c-0acc03d50ca2', @oldtaskOwnerId, @taskOwnerId, @subject, @subAccountId;
			if @oldescalated <> @escalated
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, 'd09a8c1a-f8e0-4a73-ac83-58c13231ae52', @oldescalated, @escalated, @subject, @subAccountId;
			if @oldescalationDate <> @escalationDate
				exec addUpdateEntryToAuditLog @CUemployeeID, @CUdelegateID, 92, @retTaskId, '3f1b5017-32e9-4e55-936d-fc169c38d34e', @oldescalationDate, @escalationDate, @subject, @subAccountId;
		end

	end

	return @retTaskId
end
