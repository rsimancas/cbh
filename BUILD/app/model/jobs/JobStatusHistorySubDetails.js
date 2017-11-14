Ext.define('CBH.model.jobs.JobStatusHistorySubDetails', {
	extend: 'Ext.data.Model',

	fields: [
        { name:'StatusKey', type:'int' },
		{ name:'StatusJobKey', type:'int' },
		{ name:'StatusPONum', type:'string', useNull: true, defaultValue: null },
		{ name:'StatusDate', type:'date' },
		{ name:'StatusStatusKey', type:'int' },
		{ name:'StatusMemo', type:'string', useNull: true, defaultValue: null },
		{ name:'StatusModifiedBy', type:'string' },
		{ name:'StatusModifiedDate', type:'date' },
		{ name:'StatusKey', type:'int' },
		{ name:'x_Status', type:'string' },
		{ name:'x_JobClosed', type:'date'}	
	],

    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/JobStatusHistorySubDetails',
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