Ext.define('CBH.model.vendors.Vendors', {
    extend: 'Ext.data.Model',
    alias: 'model.vendors',
    idProperty: 'VendorKey',

    requires:[
    //'CBH.model.vendors.Items',
    'CBH.model.vendors.LastQuoteMargin',
    'CBH.model.sales.FileQuoteVendorInfo'         /* rule 2 */
    ],

    fields: [
        { name:'VendorKey', type:'int' },
        { name:'VendorName', type:'string' },
        { name:'VendorPeachtreeID', type:'string' },
        { name:'VendorPeachtreeItemID', type:'string', useNull: true, defaultValue: null },
        { name:'VendorPeachtreeJobID', type:'string', useNull: true, defaultValue: null },
        { name:'VendorDisplayToCust', type:'string', useNull: true, defaultValue: null },
        { name:'VendorContact', type:'string', useNull: true, defaultValue: null },
        { name:'VendorAddress1', type:'string', useNull: true, defaultValue: null},
        { name:'VendorAddress2', type:'string', useNull: true, defaultValue: null},
        { name:'VendorCity', type:'string', useNull: true, defaultValue: null },
        { name:'VendorState', type:'string', useNull: true, defaultValue: null },
        { name:'VendorZip', type:'string', useNull: true, defaultValue: null },
        { name:'VendorCountryKey', type:'int', useNull: true },
        { name:'VendorPhone', type:'string', useNull: true, defaultValue: null },
        { name:'VendorFax', type:'string', useNull: true, defaultValue: null },
        { name:'VendorEmail', type:'string', useNull: true, defaultValue: null },
        { name:'VendorWebsite', type:'string', useNull: true, defaultValue: null },
        { name:'VendorLanguageCode', type:'string' },
        { name:'VendorAcctNum', type:'string', useNull: true, defaultValue: null },
        { name:'VendorCarrier', type:'boolean' },
        { name:'VendorModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'VendorModifiedDate', type:'date', useNull: true },
        { name:'VendorCreatedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
        { name:'VendorCreatedDate', type:'date' },
        { name:'VendorDefaultCommissionPercent', type:'float' },
        { name:'x_LastQuoteMargin', type:'float', useNull: true},
        { name:'x_VendorAddress', type:'string', useNull: true, defaultValue: null }
    ],

    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/vendors',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'VendorKey'
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

    hasMany: [{
        name: "Items",
        model: 'CBH.model.vendors.Items',
        primaryKey: 'VendorKey',
        foreignKey: 'ItemVendorKey',
        associationKey: 'items' // read child data from nested.child_groups
    },
    {
        name: "LastQuoteMargin",
        model: 'CBH.model.vendors.LastQuoteMargin',
        primaryKey: 'VendorKey',
        foreignKey: 'FVVendorKey',
        associationKey: 'lastquotemargin' // read child data from nested.child_groups
    },
    {
        name: "FileQuoteVendorInfo",
        model: 'CBH.model.sales.FileQuoteVendorInfo',
        primaryKey: 'VendorKey',
        foreignKey: 'FVVendorKey',
        associationKey: 'filequotevendorinfo' // read child data from nested.child_groups
    }
    ]
});