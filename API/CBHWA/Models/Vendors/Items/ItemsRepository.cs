using Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Utilidades;

namespace CBHWA.Models
{
    public class ItemRepository : IItemRepository
    {
        #region ITEMS
        public IList<Item> GetList(string query, string ItemNum, int vendorkey, Sort sort, Filter filter, string[] queryBy, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = string.IsNullOrEmpty(ItemNum) ? "1 = 1" : String.Format("ItemNum = '{0}'", ItemNum);
            if (vendorkey > 0) where = "ItemVendorKey = @vendorkey";

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                where += String.Format(" and {0} = {1}", filter.property, filter.value);
            }
            #endregion Filtros

            #region Query By Settings
            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "";

                if (queryBy.Length == 0)
                {
                    fieldName = "STR(ItemKey) + ' ' + ISNULL(ItemNum,'') + ' ' + ISNULL(x_ItemName,'') + ' ' + ISNULL(x_VendorName, '')";
                }
                else
                {
                    foreach (string field in queryBy)
                    {
                        switch (field)
                        {
                            case "ItemNum":
                                fieldName += (!String.IsNullOrWhiteSpace(fieldName)) ? " + " : "";
                                fieldName += "ISNULL(ItemNum,'')";
                                break;
                            case "x_ItemName":
                                fieldName += (!String.IsNullOrWhiteSpace(fieldName)) ? " + " : "";
                                fieldName += "ISNULL(x_ItemName,'')";
                                break;
                            case "x_ItemNumName":
                                fieldName += (!String.IsNullOrWhiteSpace(fieldName)) ? " + " : "";
                                fieldName += "ISNULL(ItemNum,'') + ' ' + ISNULL(x_ItemName,'')";
                                break;
                            default:
                                fieldName += (!String.IsNullOrWhiteSpace(fieldName)) ? " + " : "";
                                fieldName += field;
                                break;
                        }
                    }
                }

                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                        EnumExtension.generateLikeWhere(query, fieldName);

            }
            #endregion Query By Settings

            #region Ordenamiento
            string order = "ItemKey";
            string direction = "DESC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }

            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                order = "ItemNum";
                direction = "ASC";
            }
            #endregion Ordenamiento

            string sql = @"WITH qSELECT 
                          AS 
                          ( 
                         	SELECT a.ItemKey,a.ItemVendorKey,a.ItemNum,a.ItemCost,a.ItemPrice,a.ItemCurrencyCode,a.ItemWeight,a.ItemVolume,a.ItemActive,
                            ISNULL(c.DescriptionText,'') as x_ItemName, 
                            IsNull(b.VendorName, '') as x_VendorName,
                            ISNULL(d.CurrencyRate, 0) as ItemCurrencyRate, ISNULL(d.CurrencySymbol,'') as ItemCurrencySymbol 
                         	FROM tblItems a LEFT OUTER JOIN tblVendors b on a.ItemVendorKey = b.VendorKey 
                                    LEFT OUTER JOIN tblItemDescriptions c on a.ItemKey = c.DescriptionItemKey and c.DescriptionLanguageCode = 'en'
                                    LEFT OUTER JOIN tblCurrencyRates d on a.ItemCurrencyCode = d.CurrencyCode
                         	WHERE a.ItemActive = 1 
                          )  
                         SELECT * FROM ( 
                            SELECT *,ISNULL(ItemNum,'') +' => ' + IsNull(x_ItemName,'') as x_ItemNumName, 
                         	    ROW_NUMBER() OVER (ORDER BY {2} {3}) as row,   
                         	    IsNull((SELECT count(*) FROM qSELECT WHERE {0}),0)  as TotalRecords  
                             FROM qSELECT
                            WHERE {0}) a 
                          WHERE {1} 
                          ORDER BY row";

            sql = String.Format(sql, where, wherepage, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            if (vendorkey > 0) da.SelectCommand.Parameters.Add("@vendorkey", SqlDbType.Int).Value = Convert.ToInt32(vendorkey);

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<Item> data = EnumExtension.ToList<Item>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public Item Get(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            Item item;
            try
            {
                item = Get(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return item;
        }

        private Item Get(int id, SqlConnection oConn)
        {
            string sql = @"SELECT a.*,
                            ISNULL(c.DescriptionText,'') as x_ItemName, 
                            IsNull(b.VendorName, '') as x_VendorName,
                            ISNULL(d.CurrencyRate, 0) as ItemCurrencyRate, ISNULL(d.CurrencySymbol,'') as ItemCurrencySymbol  
                            FROM tblItems a 
                                LEFT OUTER JOIN tblVendors b ON a.ItemVendorKey = b.VendorKey
                                LEFT OUTER JOIN tblItemDescriptions c on a.ItemKey = c.DescriptionItemKey and c.DescriptionLanguageCode = 'en'
                                LEFT OUTER JOIN tblCurrencyRates d on a.ItemCurrencyCode = d.CurrencyCode
                         WHERE (a.ItemKey = @itemkey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<Item> data = dt.ToList<Item>();
                return data.FirstOrDefault<Item>();
            }
            else
            {
                return null;
            }
        }

        public Item Add(Item model)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string sql = @"DECLARE @ErrorMessage NVARCHAR(4000);
                           DECLARE @ErrorSeverity INT;
                           DECLARE @ErrorState INT;
                           DECLARE @insertedKey INT = 0;

                            SET XACT_ABORT ON;
                            BEGIN TRY
	                            BEGIN TRANSACTION;

	                            INSERT INTO tblItems ({0}) VALUES ({1});
                                SET @insertedKey = SCOPE_IDENTITY();

                                INSERT INTO tblItemDescriptions (DescriptionItemKey, DescriptionLanguageCode, DescriptionText)
                                    VALUES (@insertedKey, 'en', @DescriptionText);

	                            COMMIT TRANSACTION;
                            END TRY

                            BEGIN CATCH
                                IF (XACT_STATE()) = -1
                                BEGIN
                                    ROLLBACK TRANSACTION;
                                    SELECT 
                                        @ErrorMessage = ERROR_MESSAGE(),
                                        @ErrorSeverity = ERROR_SEVERITY(),
                                        @ErrorState = ERROR_STATE();
                                    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
                                END;
                            END CATCH;
                            select @insertedKey";

            model.ItemCreatedDate = DateTime.Now;
            EnumExtension.setListValues(model, "ItemKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@DescriptionText", SqlDbType.VarChar).Value = model.x_ItemName;

            int identityGenerated = 0;

            try
            {
                identityGenerated = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw new Exception(ex.Message);
            }

            model = Get(identityGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return model;
        }

        public Item Update(Item model)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            //string sql = "UPDATE tblItems SET {0} WHERE ItemKey = @ItemKey";
            string sql = @"DECLARE @ErrorMessage NVARCHAR(4000);
                                    DECLARE @ErrorSeverity INT;
                                    DECLARE @ErrorState INT;
                           SET XACT_ABORT ON;
                            BEGIN TRY
	                            BEGIN TRANSACTION;

	                            UPDATE tblItems SET {0} WHERE ItemKey = @ItemKey;

                                IF(NOT EXISTS(SELECT DescriptionItemKey FROM tblItemDescriptions WHERE DescriptionItemKey = @ItemKey AND DescriptionLanguageCode = 'en'))
	                                INSERT INTO tblItemDescriptions (DescriptionItemKey, DescriptionLanguageCode, DescriptionText)
                                        VALUES (@ItemKey, 'en', @DescriptionText);
                                ELSE
	                                UPDATE tblItemDescriptions SET DescriptionText = @DescriptionText
                                        WHERE DescriptionItemKey = @ItemKey AND DescriptionLanguageCode = 'en';

	                            COMMIT TRANSACTION;
                            END TRY

                            BEGIN CATCH
                                IF (XACT_STATE()) = -1
                                BEGIN
                                    ROLLBACK TRANSACTION;
                                    SELECT 
                                        @ErrorMessage = ERROR_MESSAGE(),
                                        @ErrorSeverity = ERROR_SEVERITY(),
                                        @ErrorState = ERROR_STATE();
                                    RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
                                END;
                            END CATCH;";

            EnumExtension.setUpdateValues(model, "ItemKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);
            cmd.Parameters.Add("@ItemKey", SqlDbType.Int).Value = model.ItemKey;
            cmd.Parameters.Add("@DescriptionText", SqlDbType.VarChar).Value = model.x_ItemName;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw new Exception(ex.Message);
            }

            model = Get(model.ItemKey, oConn);

            ConnManager.CloseConn(oConn);

            return model;
        }

        public bool Remove(Item item)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            bool result;
            try
            {
                result = Remove(item, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(Item item, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblItems " +
                         " WHERE (ItemKey = {0})";

            sql = String.Format(sql, item.ItemKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }

        public IList<Item> GetByVendor(int vendorkey)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            IList<Item> data;
            try
            {
                data = GetByVendor(vendorkey, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return data;
        }

        private IList<Item> GetByVendor(int vendorkey, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tblItems " +
                         " WHERE (ItemVendorKey = {0})";

            sql = String.Format(sql, vendorkey);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<Item> data = EnumExtension.ToList<Item>(dt);

            return data;
        }
        #endregion "ITEMS"

        #region Descriptions
        public IList<ItemDescription> GetDescriptions(string query, int itemkey, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = (itemkey==0) ? "1=1" : String.Format("a.DescriptionItemKey = {0}",itemkey) ;

            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "a.DescriptionText";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                    EnumExtension.generateLikeWhere(query, fieldName);
            }

//            string sql = @"SELECT * FROM ( 
//                         SELECT a.DescriptionKey, a.DescriptionItemKey, a.DescriptionText, a.DescriptionLanguageCode, b.LanguageName as x_Language, 
//                         	ROW_NUMBER() OVER (ORDER BY DescriptionKey) as row,   
//                         	IsNull((SELECT count(*) FROM tblItemDescriptions a WHERE {0}),0)  as TotalRecords 
//                         FROM tblItemDescriptions a left join tblLanguages b on a.DescriptionLanguageCode=b.LanguageCode WHERE {0}) a 
//                          WHERE {1} 
//                          ORDER BY row ";

            string sql = @"WITH qSELECT 
                          AS 
                          ( 
                            SELECT a.DescriptionKey, a.DescriptionItemKey, a.DescriptionText, a.DescriptionLanguageCode, b.LanguageName as x_Language
                            FROM tblItemDescriptions a 
                            LEFT OUTER JOIN tblLanguages b on a.DescriptionLanguageCode=b.LanguageCode 
                            WHERE {0}
                          )  
                         SELECT * 
                            FROM ( 
                                SELECT *, 
                         	        ROW_NUMBER() OVER (ORDER BY DescriptionKey) as row,   
                         	        IsNull((SELECT count(*) FROM qSELECT),0)  as TotalRecords  
                                 FROM qSELECT
                            ) a 
                          WHERE {1} 
                          ORDER BY row";

            sql = String.Format(sql, where, wherepage);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<ItemDescription> data = EnumExtension.ToList<ItemDescription>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public ItemDescription GetItemDescription(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            ItemDescription desc;
            try
            {
                desc = GetItemDescription(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return desc;
        }

        private ItemDescription GetItemDescription(int id, SqlConnection oConn)
        {
            string sql = "SELECT * FROM tblItemDescriptions " +
                         " WHERE (DescriptionItemKey = @itemkey)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@itemkey", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            IList<ItemDescription> data = EnumExtension.ToList<ItemDescription>(dt);

            return data.FirstOrDefault<ItemDescription>();
        }

        public ItemDescription Add(ItemDescription desc)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string sql = "INSERT INTO tblItemDescriptions ({0}) VALUES ({1}) " +
                "SELECT SCOPE_IDENTITY()";

            EnumExtension.setListValues(desc, "DescriptionKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int identityGenerated = 0;

            try
            {
                identityGenerated = Convert.ToInt32(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ItemDescription data = GetItemDescription(identityGenerated, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public ItemDescription Update(ItemDescription desc)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string sql = "UPDATE tblItemDescriptions SET {0} WHERE DescriptionKey = " + desc.DescriptionKey.ToString();

            EnumExtension.setUpdateValues(desc, "DescriptionKey", ref sql);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ItemDescription data = GetItemDescription(desc.DescriptionKey, oConn);

            ConnManager.CloseConn(oConn);

            return data;
        }

        public bool Remove(ItemDescription desc)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            bool result;
            try
            {
                result = Remove(desc, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return false;
            }

            ConnManager.CloseConn(oConn);

            return result;
        }

        private bool Remove(ItemDescription desc, SqlConnection oConn)
        {
            string sql = "DELETE FROM tblItemDescriptions " +
                         " WHERE (DescriptionKey = {0})";

            sql = String.Format(sql, desc.DescriptionKey);

            SqlCommand cmd = new SqlCommand(sql, oConn);

            int number = Convert.ToInt32(cmd.ExecuteNonQuery());

            if (number > 0) return true;

            return false;
        }
        #endregion

        #region qlstScheduleBImports
        public IList<qlstScheduleBImport> GetListScheduleB(string query, Sort sort, Filter filter, string[] queryBy, int page, int start, int limit, ref int totalRecords)
        {
            limit = limit + start;

            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            string wherepage = (page != 0) ? String.Format("row>{0} and row<={1} ", start, limit) : "1=1";
            string where = string.IsNullOrEmpty(query) ? "LEN(a.SchBNum) < 5" : "1=1";

            #region Filtros
            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                where += String.Format(" and {0} = {1}", filter.property, filter.value);
            }
            #endregion Filtros

            #region Query By Settings
            if (!string.IsNullOrEmpty(query))
            {
                string fieldName = "";
                fieldName = "a.x_Description";
                where += (!string.IsNullOrEmpty(where) ? " and " : "") +
                        EnumExtension.generateLikeWhere(query, fieldName);

            }
            #endregion Query By Settings

            #region Ordenamiento
            string order = "SBLanguageText";
            string direction = "ASC";

            if (!string.IsNullOrWhiteSpace(sort.property))
            {
                order = sort.property;
                direction = sort.direction;
            }

            if (!string.IsNullOrWhiteSpace(filter.property))
            {
                order = "SBLanguageText";
                direction = "ASC";
            }
            #endregion Ordenamiento

            string sql = @"WITH qData
                           AS
                           ( 
                                SELECT a.* FROM 
                                (
                                    SELECT  *
                                        ,dbo.fn_FormatUsingMask(ISNULL(SchBNum,''), '####.##.####') + ISNULL(SBLanguageSchBSubNum,'')+' '+ISNULL(SBLanguageText,'') as x_Description
                                        ,dbo.fn_FormatUsingMask(ISNULL(SchBNum,''), '####.##.####') + ISNULL(SBLanguageSchBSubNum,'') as x_SchBNumFormatted
                                    FROM dbo.qlstScheduleBImports
                                ) a
                                WHERE {0}
                           ) 
                           SELECT * FROM (
                                SELECT a.*
                                    ,ROW_NUMBER() OVER (ORDER BY {2} {3}) as row
                                    , t.TotalRecords
                                FROM qData a 
									cross apply (select COUNT(*) as TotalRecords FROM qData) as t
                           ) a
                           WHERE {1} 
                           ORDER BY row";

            sql = String.Format(sql, where, wherepage, order, direction);

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            DataSet ds = new DataSet();

            try
            {
                da.Fill(ds);
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
            }

            ConnManager.CloseConn(oConn);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qlstScheduleBImport> data = EnumExtension.ToList<qlstScheduleBImport>(dt);
                totalRecords = Convert.ToInt32(dt.Rows[0]["totalRecords"]);
                return data;
            }
            else
            {
                return null;
            }
        }

        public qlstScheduleBImport GetScheduleB(int id)
        {
            SqlConnection oConn = null;

            try
            {
                oConn = ConnManager.OpenConn();
            }
            catch (Exception ex)
            {
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                throw;
            };

            qlstScheduleBImport item;
            try
            {
                item = GetScheduleB(id, oConn);
            }
            catch (Exception ex)
            {
                ConnManager.CloseConn(oConn);
                LogManager.Write("ERROR:" + Environment.NewLine + "\tMETHOD = " + this.GetType().FullName + "." + MethodBase.GetCurrentMethod().Name + Environment.NewLine + "\tMESSAGE = " + ex.Message);
                return null;
            }

            ConnManager.CloseConn(oConn);

            return item;
        }

        private qlstScheduleBImport GetScheduleB(int id, SqlConnection oConn)
        {
            string sql = @"SELECT *, 
                                dbo.fn_FormatUsingMask(ISNULL(SchBNum,''), '####.##.####') + ISNULL(SBLanguageSchBSubNum,'')+' '+ISNULL(SBLanguageText,'') as x_Description,
                                dbo.fn_FormatUsingMask(ISNULL(SchBNum,''), '####.##.####') + ISNULL(SBLanguageSchBSubNum,'') as x_SchBNumFormatted
                            FROM dbo.qlstScheduleBImports
                         WHERE (SBLanguageKey = @key)";

            SqlDataAdapter da = new SqlDataAdapter(sql, oConn);

            da.SelectCommand.Parameters.Add("@key", SqlDbType.Int).Value = Convert.ToInt32(id);

            DataSet ds = new DataSet();

            da.Fill(ds);

            DataTable dt;
            dt = ds.Tables[0];

            if (dt.Rows.Count > 0)
            {
                IList<qlstScheduleBImport> data = dt.ToList<qlstScheduleBImport>();
                return data.FirstOrDefault();
            }
            else
            {
                return null;
            }

        }
        #endregion qlstScheduleBImports
    }
}