/****** Object:  UserDefinedFunction [dbo].[fnGetPaymentTerms]    Script Date: 11/24/2014 13:03:43 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Rony Simancas
-- Create date: 13/11/2014
-- Description:	Get PaymentTerms
-- =============================================
CREATE FUNCTION [dbo].[fnGetPaymentTerms]
(
	@TermKey int,
	@LanguageCode char(2)
)
RETURNS NVARCHAR(4000)
AS
BEGIN
	DECLARE @ResultVar NVARCHAR(4000)

	IF(@LanguageCode = '') 
		SET @LanguageCode = 'en'
		
	SELECT TOP 1 @ResultVar = PTDescription
    FROM tlkpPaymentTermsDescriptions
    WHERE (PTTermKey = @TermKey) AND (PTLanguageCode = @LanguageCode)

	-- Return the result of the function
	RETURN @ResultVar

END
