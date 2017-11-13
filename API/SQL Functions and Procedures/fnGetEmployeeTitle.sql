/****** Object:  UserDefinedFunction [dbo].[fnGetReportText]    Script Date: 11/24/2014 13:02:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rony Simancas
-- Create date: 05/11/2014
-- Description:	Get Employee Title
-- =============================================
CREATE FUNCTION [dbo].[fnGetEmployeeTitle]
(
	@EmployeeTitle int,
	@LangCode char(2)
)
RETURNS NVARCHAR(MAX)
AS
BEGIN
	DECLARE @ResultVar NVARCHAR(MAX)

	SELECT @ResultVar = Text
	FROM tsysEmployeeCodes 
	WHERE TextExpression = @EmployeeTitle and TextLanguageCode = @LangCode

	-- Return the result of the function
	RETURN @ResultVar
END
