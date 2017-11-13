Ext.define('CBH.model.vendors.VendorContacts', {
    extend: 'Ext.data.Model',
    alias: 'model.vendorcontacts',
    idProperty: 'ContactKey',

    fields: [
    { name:'ContactKey', type:'int' },
    { name:'ContactVendorKey', type:'int' },
    { name:'ContactTitle', type:'string', useNull: true, defaultValue: null },
    { name:'ContactFirstName', type:'string', useNull: true, defaultValue: null },
    { name:'ContactLastName', type:'string', useNull: true, defaultValue: null },
    { name:'ContactPhone', type:'string', useNull: true, defaultValue: null },
    { name:'ContactFax', type:'string', useNull: true, defaultValue: null },
    { name:'ContactEmail', type:'string', useNull: true, defaultValue: null },
    { 
        name:'ContactModifiedBy', 
        type:'string',
        defaultValue: CBH.GlobalSettings.getCurrentUserName()
    },
    { name:'ContactModifiedDate', type:'date' },
    { name:'ContactMemo', type:'string', useNull: true, defaultValue: null },
    { name:'ContactCarrierNotify', type:'boolean' },
    { name:'ContactModifiedDate', type:'date' },
    { 
        name: 'x_ContactFullName', 
        type: 'string',
        convert: function(val,row) {
            var fullname = row.data.ContactLastName !== null ? row.data.ContactLastName : "";
            fullname += row.data.ContactFirstName !== null ? ' '+ row.data.ContactFirstName : "";
            return fullname;
        }   
    }
    ],

    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/VendorContacts',
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

    belongsTo: 'CBH.model.vendors.Vendors'
});