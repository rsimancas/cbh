/****** Object:  View [dbo].[qfrmFileQuoteDetailsSub]    Script Date: 07-12-2015 02:04:08 p.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER VIEW [dbo].[qfrmFileQuoteDetailsSub]
AS
SELECT        QuoteKey, QuoteFileKey, QuoteSort, QuoteQty, QuoteVendorKey, QuoteItemKey, QuoteItemCost, QuoteItemPrice, QuoteItemLineCost, QuoteItemLinePrice, QuoteItemCurrencyCode, QuoteItemCurrencyRate, 
                         QuoteItemWeight, QuoteItemVolume, QuoteItemMemoCustomer, QuoteItemMemoCustomerMoveBottom, QuoteItemMemoSupplier, QuoteItemMemoSupplierMoveBottom, QuotePOItemsKey, 
                         CAST(QuoteItemLineCost * QuoteItemCurrencyRate AS money) AS LineCost, CAST(QuoteItemLinePrice * QuoteItemCurrencyRate AS money) AS LinePrice, CAST(QuoteItemWeight * QuoteQty AS decimal) 
                         AS LineWeight, CAST(QuoteItemVolume * QuoteQty AS decimal) AS LineVolume
FROM            dbo.tblFileQuoteDetail

GO


