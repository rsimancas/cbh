/****** Object:  View [dbo].[qlstJobInvoices]    Script Date: 29-04-2016 06:46:47 a.m. ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[qryJobInvoiceSearch]
AS
SELECT a.InvoiceKey, a.InvoiceJobKey AS JobKey, a.InvoiceDate AS Date, dbo.fnGetInvoiceNum(a.InvoiceKey) AS InvoiceNum, 
    a.InvoiceBillingName AS [BillTo], a.InvoiceCurrencyCode,  
    ISNULL(b.InvoiceSummaryPrice, ISNULL(d.InvoiceItemsPrice, 0)) 
    + ISNULL(c.IChargePrice, 0) AS Price,
	dbo.fnGetJobNum(a.InvoiceJobKey) as JobNum
FROM dbo.tblInvoiceHeader AS a LEFT OUTER JOIN
    dbo.qsumInvoiceSummaryItems AS b ON a.InvoiceKey = b.InvoiceKey LEFT OUTER JOIN
    dbo.qsumInvoiceCharges AS c ON a.InvoiceKey = c.InvoiceKey LEFT OUTER JOIN
    dbo.qsumInvoiceJobItems AS d ON a.InvoiceKey = d.InvoiceKey
WHERE     (a.InvoiceJobKey IS NOT NULL)