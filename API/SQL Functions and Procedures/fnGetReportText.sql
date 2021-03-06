/****** Object:  UserDefinedFunction [dbo].[fnGetReportText]    Script Date: 11/24/2014 13:02:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rony Simancas
-- Create date: 13/11/2014
-- Description:	Get Report Text
-- =============================================
CREATE FUNCTION [dbo].[fnGetReportText]
(
	@TextKey int,
	@LanguageCode char(2)
)
RETURNS NVARCHAR(4000)
AS
BEGIN
	DECLARE @ResultVar NVARCHAR(4000)

	IF(@LanguageCode = '') 
		SET @LanguageCode = 'en'
		
	SELECT TOP 1 @ResultVar = Text
    FROM tsysReportText
    WHERE (TextExpression = @TextKey) AND (TextLanguageCode = @LanguageCode)

	-- Return the result of the function
	RETURN @ResultVar

END
