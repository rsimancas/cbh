Ext.define('CBH.model.common.Status', {
    extend: 'Ext.data.Model',
    alias: 'model.status',
    idProperty: 'StatusKey',

    fields: [
    { name:'StatusKey', type:'int' },
    { name:'StatusCategory', type:'int' },
    { name:'StatusSort', type:'int' },
    { name:'StatusText', type:'string' },
    { name:'StatusPublicDefault', type:'boolean' },
    { name:'StatusCustEntry', type:'boolean' },
    { name:'StatusClosed', type:'boolean' },
    { name:'StatusCompleted', type:'boolean' },
    { name:'StatusStatusKey', type:'int' }
    ],

    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/Status',
        headers: {
           'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'StatusKey'
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
    }
});