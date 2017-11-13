Ext.define('CBH.model.customers.CustomerShipAddress', {
    extend: 'Ext.data.Model',
    alias: 'model.customershipaddress',
    idProperty: 'ShipKey',

    fields: [
        { name:'ShipKey', type: 'int'},
        { name:'ShipCustKey', type: 'int'},
        { name:'ShipName'},
        { name:'ShipComments'},
        { name:'ShipAddress1'},
        { name:'ShipAddress2'},
        { name:'ShipCity'},
        { name:'ShipState'},
        { name:'ShipZip'},
        { name:'ShipCountryKey', type: 'int'},
        { name:'ShipPhone'},
        { name:'ShipDefault', type: 'boolean'},
        { name:'ShipModifiedBy'},
        { name:'ShipModifiedDate', type: 'date'},
        { name: 'x_ShipAddress', type: 'string',
            convert: function(val,row) {
                var shipAddress = row.data.ShipAddress1 !== null ? row.data.ShipAddress1 : row.data.ShipName;
                shipAddress += row.data.ShipAddress2 !== null ? ' ' + row.data.ShipAddress2 : "";

                return shipAddress;
            }   
        },
        { name: 'x_CountryName', type: 'string'},
        { name: 'x_StateName', type: 'string'},
        { name: 'x_FullShipAddress', type: 'string'}
    ],
    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/CustomerShipAddress',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'ContactKey'
        },
        afterRequest: function(request, success) {
            if (request.action == 'read') {
                //this.readCallback(request);
            } else if (request.action == 'create') {
                if (!request.operation.success) {
                    Ext.popupMsg.msg("Warning", "Record was not created");
                    Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    Ext.popupMsg.msg("Success", "Created Successfully");
                }
            } else if (request.action == 'update') {
                if (!request.operation.success) {
                    Ext.popupMsg.msg("Warning", "Record was not saved");
                    Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    Ext.popupMsg.msg("Success", "Updated Successfully");
                }
            } else if (request.action == 'destroy') {
                if (!request.operation.success) {
                    Ext.popupMsg.msg("Warning", "Record was not deleted");
                    //Ext.global.console.warn(request.proxy.reader.jsonData.message);
                } else {
                    Ext.popupMsg.msg("Success", "Deleted Successfully");
                }
            }
        }
    },

    belongsTo: 'CBH.model.customers.Customers'
});