Ext.define('CBH.model.jobs.qryJobInvoiceSearch', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'InvoiceKey', type:'int', defaultValue: null },
        { name:'JobKey', type:'int', useNull: true, defaultValue: null },
        { name:'JobNum', type:'string', useNull: true, defaultValue: null },
        { name:'Date', type:'date', defaultValue: null },
        { name:'InvoiceNum', type:'string', useNull: true, defaultValue: null },
        { name:'BillTo', type:'string', useNull: true, defaultValue: null },
        { name:'InvoiceCurrencyCode', type:'string' },
        { name:'Price', type:'float', useNull: true, defaultValue: null }
	],

    proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/qryJobInvoiceSearch',
        headers: {
            'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
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