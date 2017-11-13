Ext.define('CBH.model.jobs.JobRoles', {
    extend: 'Ext.data.Model',
    alias: 'model.jobroles',
    idProperty: 'JobRoleKey',

    fields: [
    { name:'JobRoleKey', type:'int' },
    { name:'JobRoleSort', type:'int', useNull: true },
    { name:'JobRoleDescription', type:'string' },
    { name:'JobRoleModifiedBy', type:'string', useNull: true, defaultValue: null },
    { name:'JobRoleModifiedDate', type:'date', useNull: true },
    { name:'JobRoleCreatedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
    { name:'JobRoleCreatedDate', type:'date' }
    ],
    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/JobRoles',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'JobRoleKey'
        },
        writer: {
            type: 'json',
            writeAllFields: true
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
    }
});