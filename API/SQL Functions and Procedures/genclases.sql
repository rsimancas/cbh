declare @tablename nvarchar (50)
set @tablename = 'Users'

SELECT '{ name:'+CHAR(39)+ RTRIM(Column_Name)+CHAR(39)+
       ', type:'+
       CHAR(39) + 
       (CASE WHEN DATA_TYPE in ('char','nvarchar','text','nchar','varchar','nchar') 
			THEN 'string' 
			ELSE (CASE WHEN DATA_TYPE IN ('int','smallint','tinyint','bigint')
						THEN 'int'
						ELSE (CASE WHEN DATA_TYPE='bit' 
									THEN 'boolean'
									ELSE (CASE WHEN DATA_TYPE  in ('datetime','smalldatetime')
												THEN 'date'
												ELSE (CASE WHEN DATA_TYPE IN ('decimal','money')
															THEN 'float'
															ELSE DATA_TYPE
															END)
											END)
								END) 
					END) 
		END) + 
       CHAR(39)+ 
       (CASE WHEN IS_NULLABLE='NO' THEN '' ELSE ', useNull: true' END) +
       (CASE WHEN IS_NULLABLE='NO' AND DATA_TYPE in ('char','nvarchar','text','nchar','varchar','nchar')
             THEN '' ELSE ', defaultValue: null' END)
       + ' },'
FROM INFORMATION_SCHEMA.COLUMNS a
WHERE TABLE_NAME = @tablename and COLUMN_NAME <> 'rowguid'


SELECT 'public ' + 
	(CASE WHEN IS_NULLABLE<>'NO' AND DATA_TYPE not in ('char','nvarchar','text','nchar','varchar','int') 
			THEN 'Nullable<' ELSE '' END) +
	(CASE WHEN DATA_TYPE in ('char','nvarchar','text','nchar','varchar') 
	THEN 'string' 
	ELSE (CASE WHEN DATA_TYPE IN ('int','smallint','tinyint','bigint')
				THEN 'int'
				ELSE (CASE WHEN DATA_TYPE='bit' 
							THEN 'bool'
							ELSE (CASE WHEN DATA_TYPE in ('datetime','smalldatetime')
										THEN 'DateTime'
										ELSE (CASE WHEN DATA_TYPE  IN ('decimal','money','numeric')
													THEN 'decimal'
													ELSE DATA_TYPE
													END)
										END)
							END) 
				END) 
	END) + 
	(CASE WHEN IS_NULLABLE<>'NO' AND DATA_TYPE not in ('char','nvarchar','text','nchar','varchar') 
		THEN (CASE WHEN IS_NULLABLE<>'NO' AND DATA_TYPE='int' THEN '?' ELSE '>' END) ELSE '' END) +
  ' ' + RTRIM(COLUMN_NAME)+' ' +
  '{ get; set; }'
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = @tablename  and COLUMN_NAME <> 'rowguid'