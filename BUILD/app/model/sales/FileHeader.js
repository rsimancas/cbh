Ext.define('CBH.model.sales.FileHeader', {
    extend: 'Ext.data.Model',
    alias: 'model.fileheader',

    requires:[
    'CBH.model.sales.FileQuoteDetail',
    'CBH.model.sales.FileOverview',
    'CBH.model.sales.FileEmployeeRoles'         /* rule 2 */
    ],

    fields: [
        { name: 'FileKey', type: 'int' },
        { name: 'FileYear', type: 'int', 
            defaultValue: new Date().getFullYear()
        },
        { name: 'FileNum', type: 'int'},
        { name: 'FileStatusKey', type: 'int' },
        { name: 'FileQuoteEmployeeKey', 
            type: 'int', 
            defaultValue: CBH.GlobalSettings.getCurrentUserEmployeeKey()
        },
        { name: 'FileOrderEmployeeKey', type: 'int', useNull: true, defaultValue: null},
        { name: 'FileCustKey', type: 'int', useNull: true, defaultValue: null },
        { name: 'FileContactKey', type: 'int', useNull: true, defaultValue: null  },
        { name: 'FileCustShipKey', type: 'int', useNull: true, defaultValue: null  },
        { name: 'FileReference' },
        { name: 'FileProfitMargin', type: 'float' },
        { name: 'FileCurrentVendor', type: 'int', useNull: true },
        { name: 'FileModifiedBy' },
        { name: 'FileModifiedDate', type: 'date' },
        { name: 'FileCreatedBy',
           defaultValue: CBH.GlobalSettings.getCurrentUserName() },
        { name: 'FileCreatedDate', type: 'date', defaultValue: new Date()},
        { name: 'FileDateCustRequired', type: 'date', useNull: true},
        { name: 'FileDateCustRequiredNote' },
        { name: 'FileDefaultCurrencyCode', type: 'string', defaultValue: "USD"},
        { name: 'FileDefaultCurrencyRate', type: 'float', defaultValue: 1 },
        { name: 'FileClosed', type: 'date' },
        { name: 'x_FileNumFormatted', 
            type: 'string',
            convert: function(val,row) {
                return (row.data.FileKey !== null && row.data.FileKey !==0) ? 'F' + 
                    Ext.util.Format.substr(row.data.FileYear,2) + '-' + 
                    Ext.String.leftPad(row.data.FileNum, 4, '0') : '(NEW)';
            }   
        },
        { name: 'FileDefaultCurrencySymbol', type: 'string'}
    ],
    hasMany: [
    {
        name: "QuoteDetails",
        model: 'CBH.model.sales.FileQuoteDetail',
        primaryKey: 'FileKey',
        foreignKey: 'QuoteFileKey',
        associationKey: 'quotedetails'
    },
    {
        name: "FileOverview",
        model: 'CBH.model.sales.FileOverview',
        primaryKey: 'FileKey',
        foreignKey: 'FileKey',
        associationKey: 'fileoverview'
    },
    {
        name: "FileEmployeeRoles",
        model: 'CBH.model.sales.FileEmployeeRoles',
        primaryKey: 'FileKey',
        foreignKey: 'FileEmployeeFileKey',
        associationKey: 'fileemployeeroles'
    }
    ],
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/File',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data'
        },
        writer: {
            type:'json',
            writeAllFields: true
        },
        afterRequest: function (request, success) {
            if (request.action == 'read') {
                //this.readCallback(request);
            }
            else if (request.action == 'create') {
                if (!request.operation.success)
                {
                    Ext.popupMsg.msg("Warning", "Record was not created");
                    Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    Ext.popupMsg.msg("Success","Created Successfully");
                }
            }
            else if (request.action == 'update') {
                if (!request.operation.success)
                {
                    Ext.popupMsg.msg("Warning", "Record was not saved");
                    Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    Ext.popupMsg.msg("Success","Updated Successfully");
                }
            }
            else if (request.action == 'destroy') {
                if (!request.operation.success)
                {
                    Ext.popupMsg.msg("Warning", "Record was not deleted");
                    //Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    Ext.popupMsg.msg("Success","Deleted Successfully");
                }
            }
        }
    },
    load: function(id, config) {
        config = Ext.apply({}, config);
        config = Ext.applyIf(config, {
            model: this,   //this line is necessary
            action: 'read',
            params: {
                id: id
            }
        })
    }
});