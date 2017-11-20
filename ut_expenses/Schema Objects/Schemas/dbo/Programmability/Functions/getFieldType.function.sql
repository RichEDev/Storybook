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
		IF @format = 4
			RETURN 'D'
		ELSE IF @format = 3
			RETURN 'DT'
		ELSE IF @format = 5
			RETURN 'T'
		ELSE
			RETURN 'DT'
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
	ELSE IF @fieldtype = 15
		RETURN 'O'
	ELSE IF @fieldtype = 16
		RETURN 'S'
	ELSE IF @fieldtype = 17
		RETURN 'CL'
	ELSE IF @fieldtype = 19
		RETURN 'S'
	ELSE IF @fieldtype = 21
		RETURN 'L'
	ELSE IF @fieldtype = 22
		RETURN 'AT'
	ELSE IF @fieldtype = 23
		RETURN 'CO'
	RETURN ''
END
