CREATE PROCEDURE [dbo].[AddOrUpdateLogonMessages] @messageid INT
	,@CategoryTitle NVARCHAR(40)
	,@CategoryTitleColourCode NVARCHAR(6)
	,@HeaderText NVARCHAR(80)
	,@HeaderTextColourCode NVARCHAR(200)
	,@BodyText NVARCHAR(200)
	,@BodyTextColourCode NVARCHAR(6)
	,@BackgroundImage NVARCHAR(80)
	,@Icon NVARCHAR(80)=null
	,@ButtonText NVARCHAR(40)
	,@ButtonLink NVARCHAR(2000)
	,@ButtonForeColour NVARCHAR(6)
	,@ButtonBackGroundColour NVARCHAR(6)
	,@Archived INT
	,@moduleIds varchar(50)=null
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @pos INT
	DECLARE @len INT
	DECLARE @module INT

	IF @messageid = 0
	BEGIN
		INSERT INTO LogonMessages (
			CategoryTitle
			,CategoryTitleColourCode
			,HeaderText
			,HeaderTextColourCode
			,BodyText
			,BodyTextColourCode
			,BackgroundImage
			,Icon
			,ButtonText
			,ButtonLink
			,ButtonForeColour
			,ButtonBackGroundColour
			,Archived
			)
		VALUES (
			@CategoryTitle
			,@CategoryTitleColourCode
			,@HeaderText
			,@HeaderTextColourCode
			,@BodyText
			,@BodyTextColourCode
			,@BackgroundImage
			,@Icon
			,@ButtonText
			,@ButtonLink
			,@ButtonForeColour
			,@ButtonBackGroundColour
			,@Archived
			)

		SET @messageid = SCOPE_IDENTITY();
	END
	ELSE
	BEGIN
		UPDATE LogonMessages
		SET CategoryTitle = @CategoryTitle
			,CategoryTitleColourCode = @CategoryTitleColourCode
			,HeaderText = @HeaderText
			,HeaderTextColourCode = @HeaderTextColourCode
			,BodyText = @BodyText
			,BodyTextColourCode = @BodyTextColourCode
			,BackgroundImage = @BackgroundImage
			,Icon = @Icon
			,ButtonText = @ButtonText
			,ButtonForeColour = @ButtonForeColour
			,ButtonBackGroundColour = @ButtonBackGroundColour
			,ButtonLink=@ButtonLink
		WHERE MessageId = @messageid
		END
		DELETE
		FROM MessageModuleBase
		WHERE MessageId = @messageid

		SET @pos = 0
		SET @len = 0

		WHILE CHARINDEX(',', @moduleIds, @pos + 1) > 0
		BEGIN
			SET @len = CHARINDEX(',', @moduleIds, @pos + 1) - @pos
			SET @module = SUBSTRING(@moduleIds, @pos, @len)

			INSERT INTO MessageModuleBase (
				ModuleId
				,MessageId
				)
			VALUES (
				@module
				,@messageid
				)

			SET @pos = CHARINDEX(',', @moduleIds, @pos + @len) + 1
		END

		RETURN @messageid	
END
	
