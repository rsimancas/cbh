Ext.define('CBH.model.sales.FileStatusHistory', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'FileStatusKey', type:'int' },
    	{ name:'FileStatusFileKey', type:'int' },
    	{ name:'FileStatusDate', type:'date', defaultValue: new Date()  },
    	{ name:'FileStatusStatusKey', type:'int', useNull: true, defaultValue: null  },
    	{ name:'FileStatusMemo', type:'string', useNull: true, defaultValue: null },
    	{ name:'FileStatusModifiedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
    	{ name:'FileStatusModifiedDate', type:'date' },
    	{ name:'x_Status', type:'string'}
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/FileStatusHistory',
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