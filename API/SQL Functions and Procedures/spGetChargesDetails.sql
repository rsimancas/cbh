/****** Object:  StoredProcedure [dbo].[spGetChargesDetails]    Script Date: 11/24/2014 13:01:18 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		RONY SIMANCAS
-- Create date: 12/11/2014
-- Description:	Devuelve Charges Detail para Quote Customer Report
-- =============================================
CREATE PROCEDURE [dbo].[spGetChargesDetails]
	-- Add the parameters for the stored procedure here
	@QHdrKey INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    IF OBJECT_ID('tempdb..#tempcharges') IS NOT NULL
		DROP TABLE #tempcharges
    
	DECLARE @total DECIMAL(18,2)
			
	SELECT @total = SUM(a.QuoteItemLinePrice) FROM qrptFileQuoteCustomerItemDetail a where a.FVQHdrKey=@QHdrKey;

	WITH qCharges (QChargeFileKey, QChargeHdrKey, QChargeSort, QChargeMemo, QChargeCost, 
				   QChargePrice, QCDLanguageCode, QCDDescription, SubTotalCategory) 
	AS 
	( 
		SELECT a.QChargeFileKey, a.QChargeHdrKey, a.QChargeSort,a.QChargeMemo,   
		a.QChargeCost * a.QChargeCurrencyRate / b.QHdrCurrencyRate AS QChargeCost,  
		a.QChargePrice * a.QChargeCurrencyRate / b.QHdrCurrencyRate AS QChargePrice,  
		d.DescriptionLanguageCode AS QCDLanguageCode,   
		d.DescriptionText AS QCDDescription, c.ChargeSubTotalCategory AS SubTotalCategory
		FROM tblFileQuoteCharges a   
		 INNER JOIN tblFileQuoteHeader b ON a.QChargeHdrKey = b.QHdrKey   
		 INNER JOIN tlkpChargeCategories c ON a.QChargeChargeKey = c.ChargeKey  
		 INNER JOIN tlkpChargeCategoryDescriptions d ON a.QChargeChargeKey = d.DescriptionChargeKey  
		 INNER JOIN qrptQuoteCustomer e ON a.QChargeHdrKey=e.QHdrKey and d.DescriptionLanguageCode=e.CustLanguageCode  
		 WHERE a.QChargeHdrKey=@QHdrKey and (a.QChargePrint = 1)  
	)  
	SELECT b.SubTotalKey AS SubTotalKey, b.SubTotalSort AS SubTotalSort,  
		   a.STDescriptionLanguageCode AS SubTotalLanguageCode,   
		   a.STDescriptionText AS SubTotalDescription,  
		   e.Location AS SubTotalLocation, d.QHdrKey AS ShowFooter, c.QHdrKey, 
		   f.*,@total as TotalWCharges, ROW_NUMBER() OVER (ORDER BY b.SubTotalSort) as row
	INTO #tmpcharges FROM tlkpInvoiceSubTotalCategoriesDescriptions a   
		INNER JOIN tlkpInvoiceSubTotalCategories b ON a.STDescriptionSubTotalKey = b.SubTotalKey  
		INNER JOIN qrptQuoteCustomer c ON c.QHdrKey = @QHdrKey and a.STDescriptionLanguageCode = c.CustLanguageCode  
	  INNER JOIN qrptFileQuoteCustomerChargeDetailLocations d ON d.QHdrKey=c.QHdrKey and d.SubTotalKey=b.SubTotalKey  
	  INNER JOIN qrptFileQuoteCustomerChargeDetailLocations e ON e.QHdrKey=c.QHdrKey and e.SubTotalKey=b.SubTotalKey 
	  LEFT JOIN qCharges f ON 
	  ((f.SubTotalCategory=b.SubTotalKey) or (f.SubTotalCategory=3 AND b.SubTotalKey = 8) or (f.SubTotalCategory=2 AND b.SubTotalKey = 5))
	ORDER BY b.SubTotalSort

	DECLARE @row INT, @price DECIMAL(18,2)

	DECLARE temp_cursor CURSOR FOR
	select SubTotalSort,QChargePrice
	from #tmpcharges
	OPEN temp_cursor

	FETCH NEXT FROM temp_cursor INTO @row,@price

	WHILE @@FETCH_STATUS = 0
	BEGIN
		SET @total = @total + @price
		UPDATE #tmpcharges SET TotalWCharges = @total WHERE SubTotalSort=@row
		
	FETCH NEXT FROM temp_cursor INTO @row,@price
	END

	CLOSE temp_cursor

	DEALLOCATE temp_cursor

	select * from #tmpcharges
END
