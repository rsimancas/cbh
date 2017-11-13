/****** Object:  UserDefinedFunction [dbo].[fnPronacaProfitMargin]    Script Date: 09/15/2017 22:08:46 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Rony Simancas
-- Create date: 26-03-2017
-- Update: 15-09-2017
-- Description:	Calculate Profit Margin Pronaca Report
-- =============================================
ALTER FUNCTION [dbo].[fnPronacaProfitMargin]
(
	-- Add the parameters for the function here
	@JobNum varchar(MAX)
	,@QuotePrice DECIMAL(18,2)
    ,@QuoteCost DECIMAL(18,2)
    ,@QuoteProfit DECIMAL(18,2)
    ,@JobPriceUSD DECIMAL(18,2)
    ,@JobCostUSD DECIMAL(18,2)
    ,@JobProfit DECIMAL(18,2)
)
RETURNS DECIMAL(18,2)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @Price DECIMAL(18,2) = 0
	,@Cost DECIMAL(18,2) = 0
	,@Profit DECIMAL(18,2) = 0
	,@Percent DECIMAL(18,2) = 0

	IF(@JobNum IS NULL)
	BEGIN
		SET @Price = @QuotePrice
		SET @Cost = @QuoteCost
		SET @Profit = @QuoteProfit
	END
	ELSE
	BEGIN
		SET @Price = @JobPriceUSD
		SET @Cost = @JobCostUSD
		SET @Profit = @JobProfit
	END
	
	IF(@Price <> 0)
	BEGIN
		SET @Percent = @Profit / @Price
	END
	
	-- Return the result of the function
	RETURN @Percent

END


GO


