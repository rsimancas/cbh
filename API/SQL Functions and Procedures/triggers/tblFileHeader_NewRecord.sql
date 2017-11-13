/****** Object:  Trigger [dbo].[tblFileHeader_NewRecord]    Script Date: 12/01/2014 17:03:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER TRIGGER [dbo].[tblFileHeader_NewRecord]
ON [dbo].[tblFileHeader]
FOR INSERT
AS
DECLARE @FileKey int, @InsertedBy NVARCHAR(50)

SELECT @FileKey = FileKey, @InsertedBy = ISNULL(FileModifiedBy, FileCreatedBy) FROM Inserted

	INSERT INTO tblFileStatusHistory (FileStatusFileKey, FileStatusStatusKey, FileStatusModifiedBy)
	VALUES (@FileKey, 26, @InsertedBy)




