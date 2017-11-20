CREATE FUNCTION [dbo].[getFieldType]
(
	@fieldtype tinyint,
	@format tinyint
)
RETURNS NVARCHAR(2)
AS
BEGIN
	IF @fieldtype = 1
		RETURN 'S'
	ELSE IF @fieldtype = 10 AND @format = 6
		RETURN 'LT'
	ELSE IF @fieldtype = 10 AND @format <> 6
		RETURN 'S'
	ELSE IF @fieldtype = 2 OR @fieldtype = 9
		RETURN 'N'
	ELSE IF @fieldtype = 3
		RETURN 'D'
	ELSE IF @fieldtype = 4
		RETURN 'N'
	ELSE IF @fieldtype = 5
		RETURN 'X'
	ELSE IF @fieldtype = 6
		RETURN 'C'
	ELSE IF @fieldtype = 7
		RETURN 'M'
	ELSE IF @fieldtype = 8
		RETURN 'S'
	ELSE IF @fieldtype = 11
		RETURN 'RW'
	ELSE IF @fieldtype = 12
		RETURN 'R'
	ELSE IF @fieldtype = 14
		RETURN 'S'
	ELSE IF @fieldtype = 15
		RETURN 'N'
	ELSE IF @fieldtype = 16
		RETURN 'S'

	RETURN ''
END


