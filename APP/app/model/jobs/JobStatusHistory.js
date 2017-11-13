Ext.define('CBH.model.jobs.JobStatusHistory', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'JobStatusKey', type:'int', defaultValue: null },
        { name:'JobStatusJobKey', type:'int', defaultValue: null },
        { name:'JobStatusDate', type:'date', defaultValue: null },
        { name:'JobStatusStatusKey', type:'int', useNull: true, defaultValue: null },
        { name:'JobStatusMemo', type:'string', useNull: true, defaultValue: null },
        { name:'JobStatusPublic', type:'boolean', defaultValue: null },
        { name:'JobStatusModifiedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
        { name:'JobStatusModifiedDate', type:'date', defaultValue: null },
    	{ name:'x_Status', type:'string'}
	],

    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/JobStatusHistory',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message'
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