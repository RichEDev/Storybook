CREATE PROCEDURE [dbo].[UpdateRechargeCPAnnualCost] @contractId int, @acv int
AS
	DECLARE @cpId int
	DECLARE @sumval float
	DECLARE @total float
	SET @total = 0

	-- Same as normal annual update value except that it checks whether the recharge is within it's support dates before including it
	IF(@contractId > 0)
	BEGIN
		DECLARE cploop_cursor CURSOR FOR
		SELECT [contractProductId] FROM contract_productdetails WHERE [contractId] = @contractId
		OPEN cploop_cursor
		FETCH NEXT FROM cploop_cursor INTO @cpId
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF dbo.IsInSupport((SELECT TOP 1 [rechargeId] FROM recharge_associations WHERE [contractProductId] = @cpId),getdate()) = 1
			BEGIN
				SET @sumval = (SELECT [maintenanceValue] FROM contract_productdetails WHERE [contractProductId] = @cpId)
			END
			ELSE
			BEGIN
				SET @sumval = 0
			END

			SET @total = @total + @sumval

			FETCH NEXT FROM cploop_cursor INTO @cpId
		END
	
		CLOSE cploop_cursor
		DEALLOCATE cploop_cursor

		UPDATE contract_details SET [annualContractValue] = @total WHERE [contractId] = @contractId
	END
	ELSE
	BEGIN
		-- Update for ALL contracts
		DECLARE @conId INT
		DECLARE contract_loop_cursor CURSOR FOR
		SELECT [contractId] FROM contract_details WHERE [Archived] = 'N'
		OPEN contract_loop_cursor
		FETCH NEXT FROM contract_loop_cursor INTO @conId
		WHILE @@FETCH_STATUS = 0
		BEGIN
			DECLARE loop_cursor CURSOR FOR
			SELECT [contractProductId] FROM contract_productdetails
			OPEN loop_cursor
			FETCH NEXT FROM loop_cursor INTO @cpId
			WHILE @@FETCH_STATUS = 0
			BEGIN
				IF dbo.IsInSupport((SELECT TOP 1 [rechargeId] FROM recharge_associations WHERE [contractProductId] = @cpId),getdate()) = 1
				BEGIN
					SET @sumval = (SELECT [maintenanceValue] FROM contract_productdetails WHERE [contractProductId] = @cpId)
				END
				ELSE
				BEGIN
					SET @sumval = 0
				END
	
				SET @total = @total + @sumval

				FETCH NEXT FROM loop_cursor INTO @cpId
			END
			CLOSE loop_cursor
			DEALLOCATE loop_cursor		
			
			FETCH NEXT FROM contract_loop_cursor INTO @conId
		END
		CLOSE contract_loop_cursor
		DEALLOCATE contract_loop_cursor
	END

	IF @acv = 1
	BEGIN
		UPDATE contract_details SET [totalMaintenanceValue] = [annualContractValue] WHERE [contractId] = @contractId
	END
