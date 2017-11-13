Ext.define('CBH.model.vendors.VendorWarehouse', {
    extend: 'Ext.data.Model',
    alias: 'model.vendorwarehouse',
    idProperty: 'WarehouseKey',

    fields: [
    { name:'WarehouseKey', type:'int' },
    { name:'WarehouseVendorKey', type:'int' },
    { name:'WarehouseName', type:'string' },
    { name:'WarehouseAddress1', type:'string', useNull: true, defaultValue: null },
    { name:'WarehouseAddress2', type:'string', useNull: true, defaultValue: null },
    { name:'WarehouseCity', type:'string', useNull: true, defaultValue: null },
    { name:'WarehouseState', type:'string', useNull: true, defaultValue: null },
    { name:'WarehouseZip', type:'string', useNull: true, defaultValue: null },
    { name:'WarehouseCountryKey', type:'int', useNull: true },
    { name:'WarehousePhone', type:'string', useNull: true, defaultValue: null },
    { name:'WarehouseModifiedBy', type:'string' },
    { name:'WarehouseModifiedDate', type:'date' },
    { 
        name: 'x_WarehouseAddress', 
        type: 'string',
        convert: function(val,row) {
            var shipAddress = row.data.Warehouse1 !== null ? row.data.Warehouse1 : "";
            shipAddress += row.data.Warehouse2 !== null ? ' ' + row.data.Warehouse2 : "";

            return shipAddress;
        }   
    },
    { name: 'x_CountryName', type: 'string'},
    { name: 'x_StateName', type: 'string'}
    ]
    ,
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/VendorWarehouse',
        headers: {
           'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'WarehouseKey'
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