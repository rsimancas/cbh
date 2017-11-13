Ext.define('CBH.model.customers.CustomerContacts', {
    extend: 'Ext.data.Model',
    alias: 'model.customercontacts',
    idProperty: 'ContactKey',

    fields: [
    { name:'ContactKey', type:'int' },
    { name:'ContactCustKey', type:'int' },
    { name:'ContactTitle', type:'string', useNull: true, defaultValue: null },
    { name:'ContactFirstName', type:'string', useNull: true, defaultValue: null },
    { name:'ContactLastName', type:'string', useNull: true, defaultValue: null },
    { name:'ContactPhone', type:'string', useNull: true, defaultValue: null },
    { name:'ContactFax', type:'string', useNull: true, defaultValue: null },
    { name:'ContactEmail', type:'string', useNull: true, defaultValue: null },
    { name:'ContactMemo', type:'string', useNull: true, defaultValue: null },
    { name:'ContactAllowedWebAccess', type:'boolean' },
    { name:'ContactPassword', type:'string', useNull: true, defaultValue: null },
    { name:'ContactPasswordReset', type:'boolean' },
    { 
        name:'ContactModifiedBy', 
        type:'string',
        defaultValue: CBH.GlobalSettings.getCurrentUserName()
    },
    { name:'ContactModifiedDate', type:'date' },
    { name: 'x_ContactFullName', type: 'string'}
    ],

    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/CustomerContacts',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'ContactKey'
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

    belongsTo: 'CBH.model.customers.Customers'
});