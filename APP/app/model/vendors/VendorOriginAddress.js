Ext.define('CBH.model.vendors.VendorOriginAddress', {
    extend: 'Ext.data.Model',
    alias: 'model.vendororiginaddress',
    idProperty: 'OriginKey',

    fields: [
    { name:'OriginKey', type:'int' },
    { name:'OriginVendorKey', type:'int' },
    { name:'OriginName', type:'string' },
    { name:'OriginComments', type:'string', useNull: true, defaultValue: null },
    { name:'OriginAddress1', type:'string', useNull: true, defaultValue: null },
    { name:'OriginAddress2', type:'string', useNull: true, defaultValue: null },
    { name:'OriginCity', type:'string', useNull: true, defaultValue: null },
    { name:'OriginState', type:'string', useNull: true, defaultValue: null },
    { name:'OriginZip', type:'string', useNull: true, defaultValue: null },
    { name:'OriginCountryKey', type:'int' },
    { name:'OriginDefault', type:'boolean' },
    { name:'OriginPhone', type:'string', useNull: true, defaultValue: null },
    { name:'OriginModifiedBy', type:'string' },
    { name:'OriginModifiedDate', type:'date' },
    { 
        name: 'x_OriginAddress', 
        type: 'string',
        convert: function(val,row) {
            var shipAddress = row.data.OriginAddress1 !== null ? row.data.OriginAddress1 : "";
            shipAddress += row.data.OriginAddress2 !== null ? ' ' + row.data.OriginAddress2 : "";

            return shipAddress;
        }   
    },
    { name: 'x_CountryName', type: 'string'},
    { name: 'x_StateName', type: 'string'}
    ]
    ,
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/VendorOriginAddress',
        headers: {
           'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'OriginKey'
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