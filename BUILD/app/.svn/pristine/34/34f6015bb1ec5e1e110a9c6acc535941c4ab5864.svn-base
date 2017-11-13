Ext.define('CBH.model.jobs.qfrmJobPurchaseOrderStatusHistory', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'POKey', type:'int', defaultValue: null },
        { name:'POJobKey', type:'int', defaultValue: null },
        { name:'VendorName', type:'string', useNull: true, defaultValue: null },
        { name:'POShipETA', type:'date', useNull: true, defaultValue: null },
        { name:'EmployeeKey', type:'int', useNull: true, defaultValue: null },
        { name:'EmployeeEmail', type:'string', useNull: true, defaultValue: null },
        { name:'CustEmail', type:'string' },
        { name:'ForwarderEmail', type:'string', useNull: true, defaultValue: null },
        { name:'JobProdDescription', type:'string', useNull: true, defaultValue: null },
        { name:'JobCustRefNum', type:'string', useNull: true, defaultValue: null },
        { name:'VendorDisplayToCust', type:'string', useNull: true, defaultValue: null },
        { name:'QuoteNum', type:'string', useNull: true, defaultValue: null }
	],

	proxy: {
        type: 'rest',
        url: CBH.GlobalSettings.webApiPath + '/api/qfrmJobPurchaseOrderStatusHistory',
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