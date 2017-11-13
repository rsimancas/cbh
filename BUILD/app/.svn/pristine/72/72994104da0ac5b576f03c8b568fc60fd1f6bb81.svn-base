Ext.define('CBH.model.sales.FileQuoteStatusHistorySubDetails', {
	extend: 'Ext.data.Model',

	fields: [
		{ name:'QStatusKey', type:'int', defaultValue: null },
		{ name:'QStatusFileKey', type:'int', defaultValue: null },
		// { name:'QStatusQHdrKey', type:'int', defaultValue: null },
		{ name:'QStatusQuoteNum', type:'string', useNull: true, defaultValue: null },
		{ name:'QStatusDate', type:'date', defaultValue: null },
		{ name:'QStatusStatusKey', type:'int', defaultValue: null },
		{ name:'QStatusMemo', type:'string', useNull: true, defaultValue: null },
		{ name:'QStatusModifiedBy', type:'string' },
		{ name:'QStatusModifiedDate', type:'date', defaultValue: null },
		{ name:'x_Status', type:'string' },
		{ name:'x_FileClosed', type:'date'}	
	],
    proxy:{
        type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/FileQSHSubDetails',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'QStatusKey'
        },
        writer: {
            type:'json',
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
    }
});