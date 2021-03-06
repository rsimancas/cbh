/****** Object:  Trigger [dbo].[tblFileQuoteHeader_NewRecord]    Script Date: 12/01/2014 17:05:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[tblFileQuoteHeader_NewRecord]
ON [dbo].[tblFileQuoteHeader]
FOR INSERT
AS
DECLARE @QHdrKey int, @FileKey int, @InsertedBy NVARCHAR(50)

SELECT @QHdrKey = QHdrKey, @FileKey = QHdrFileKey, @InsertedBy = ISNULL(QHdrModifiedBy, QHdrCreatedBy) FROM Inserted

	INSERT INTO tblFileQuoteStatusHistory (QStatusFileKey, QStatusQHdrKey, QStatusStatusKey, QStatusModifiedBy)
	VALUES (@FileKey, @QHdrKey, 25, @InsertedBy)
