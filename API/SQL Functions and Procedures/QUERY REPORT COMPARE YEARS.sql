DECLARE @from datetime = '20170101'
--DECLARE @to datetime = DATEADD(DD, -1, DATEADD(YEAR, 1, @from))
DECLARE @to datetime = DATEADD(DD, -1, DATEADD(MONTH, 10, @from))

WITH qData
AS
(
select a.FileNum, ISNULL(a.CustRefNum,'') CustRefNum, a.QuotePrice, a.InvoiceNumbers, a.Price
, ISNULL(fq.QHdrCurrencyCode, 'USD') QuoteCurrencyCode
, ISNULL(fq.QHdrCurrencyRate, 1) QuoteCurrencyRate
, ISNULL(iv.InvoiceCurrencyCode, 'USD') InvoiceCurrencyCode
, ISNULL(iv.InvoiceCurrencyRate, 1) InvoiceCurrencyRate
from dbo.qrptPronacaReport a 
outer apply (select TOP 1 fqh.QHdrCurrencyCode, fqh.QHdrCurrencyRate FROM dbo.tblFileQuoteHeader fqh WHERE a.FileKey = fqh.QHdrFileKey) as fq
outer apply (select TOP 1 fih.InvoiceCurrencyCode, fih.InvoiceCurrencyRate FROM dbo.tblInvoiceHeader fih WHERE a.JobKey = fih.InvoiceJobKey) as iv
where CustKey=1340 AND JobKey IS NOT NULL AND CAST(rptDate as DATE) BETWEEN @from AND @to
)
SELECT ISNULL(FileNum,'') as FileNum, CustRefNum, ISNULL(QuotePrice,0) as QuotePrice, ISNULL(InvoiceNumbers,'') as InvoiceNumbers
	,ISNULL(Price,0) as Price
	,QuoteCurrencyCode, QuoteCurrencyRate
	,InvoiceCurrencyCode, InvoiceCurrencyRate
	,ISNULL(QuotePrice/QuoteCurrencyRate, 0) as QuotePriceUSD
	,ISNULL(Price/InvoiceCurrencyRate,0) as InvoicePriceUSD
FROM qData
ORDER BY 1