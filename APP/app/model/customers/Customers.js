Ext.define('CBH.model.customers.Customers', {
    extend: 'Ext.data.Model',
    alias: 'model.customers',
    idProperty: 'CustKey',
    autoLoad: false,

    requires:[
    'CBH.model.customers.CustomerContacts',
    'CBH.model.customers.CustomerShipAddress'          /* rule 2 */
    ],


    fields: [
    { name:'CustKey', type:'int' },
    { name:'CustPeachtreeID', type:'string' },
    { name:'CustPeachtreeIndex', type:'int', useNull: true },
    { name:'CustName', type:'string', defaultValue: "" },
    { name:'CustAddress1', type:'string', useNull: true, defaultValue: null },
    { name:'CustAddress2', type:'string', useNull: true, defaultValue: null },
    { name:'CustCity', type:'string', useNull: true, defaultValue: null },
    { name:'CustState', type:'string', useNull: true, defaultValue: null },
    { name:'CustZip', type:'string', useNull: true, defaultValue: null },
    { name:'CustCountryKey', type:'int', useNull: true },
    { name:'CustPhone', type:'string', useNull: true, defaultValue: null },
    { name:'CustFax', type:'string', useNull: true, defaultValue: null },
    { name:'CustEmail', type:'string', useNull: true, defaultValue: null },
    { name:'CustWebsite', type:'string', useNull: true, defaultValue: null },
    { name:'CustSalesRepKey', type:'int', useNull: true },
    { name:'CustOrdersRepKey', type:'int', useNull: true },
    { name:'CustLanguageCode', type:'string' },
    { name:'CustStatus', type:'int', convert: null },
    { name:'CustModifiedBy', type:'string', useNull: true, defaultValue: null },
    { name:'CustModifiedDate', type:'date', useNull: true },
    {   name:'CustCreatedBy', 
        type:'string',
        defaultValue: CBH.GlobalSettings.getCurrentUserName()
    },
    { name:'CustCreatedDate', type:'date', defaultValue: new Date()  },
    { name:'CustCreditLimit', type:'float' },
    { name:'CustCurrencyCode', type:'string', defaultValue: 'USD' },
    { name:'CustMemo', type:'string', useNull: true, defaultValue: null },
    ],

    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/Customers',
        headers: {
           'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'CustKey'
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
        name: "Contacts",
        model: 'CBH.model.customers.CustomerContacts',
        primaryKey: 'CustKey',
        foreignKey: 'ContactCustKey',
        //autoLoad: true,
        associationKey: 'contacts' // read child data from nested.child_groups
    },
    {
        name: "ShipAddress",
        model: 'CBH.model.customers.CustomerShipAddress',
        primaryKey: 'CustKey',
        foreignKey: 'ShipCustKey',
        //autoLoad: true,
        associationKey: 'shipaddress' // read child data from nested.child_groups
    }]
});