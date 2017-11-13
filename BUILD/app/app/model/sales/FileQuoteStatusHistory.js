Ext.define('CBH.model.sales.FileQuoteStatusHistory', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'QStatusKey', type:'int' },
    	{ name:'QStatusFileKey', type:'int' },
    	{ name:'QStatusQHdrKey', type:'int' },
    	{ name:'QStatusQuoteNumSnapshot', type:'string', useNull: true, defaultValue: null },
    	{ name:'QStatusDate', type:'date', defaultValue: new Date() },
    	{ name:'QStatusStatusKey', type:'int', useNull: true, defaultValue: null },
    	{ name:'QStatusMemo', type:'string', useNull: true, defaultValue: null },
    	{ name:'QStatusModifiedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName() },
    	{ name:'QStatusModifiedDate', type:'date' },
        { name:'x_Status', type:'string' }
	],

    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/FileQuoteStatusHistory',
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