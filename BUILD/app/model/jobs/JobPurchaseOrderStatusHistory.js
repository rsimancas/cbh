Ext.define('CBH.model.jobs.JobPurchaseOrderStatusHistory', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'POStatusKey', type:'int' },
    	{ name:'POStatusJobKey', type:'int' },
    	{ name:'POStatusPOKey', type:'int' },
    	{ name:'POStatusDate', type:'date' },
    	{ name:'POStatusStatusKey', type:'int', useNull: true, defaultValue: null },
    	{ name:'POStatusMemo', type:'string', useNull: true, defaultValue: null },
        { name:'POStatusPublic', type:'boleean', defaultValue: false},
    	{ name:'POStatusModifiedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
    	{ name:'POStatusModifiedDate', type:'date' },
        { name:'x_Status', type:'string' },
        { name:'x_JobClosed', type:'date'}  
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/JobPurchaseOrderStatusHistory',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
		reader:{
			type:'json',
			root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message'
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