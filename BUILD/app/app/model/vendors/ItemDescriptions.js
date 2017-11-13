Ext.define('CBH.model.vendors.ItemDescriptions', {
    extend: 'Ext.data.Model',
    alias: 'model.itemdescriptions',
    idProperty: 'DescriptionKey',

    fields: [
    { name:'DescriptionKey', type:'int' },
    { name:'DescriptionItemKey', type:'int' },
    { name:'DescriptionLanguageCode', type:'string', defaulValue: 'en' },
    { name:'DescriptionText', type:'string' },
    { name:'x_Language', type:'string'}
    ],

    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/ItemDescriptions',
        headers: {
           'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'DescriptionKey'
        },
        writer: {
            type: 'json',
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

    belongsTo: 'CBH.model.vendors.Items'
});