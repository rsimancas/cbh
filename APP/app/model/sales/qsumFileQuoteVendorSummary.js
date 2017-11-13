Ext.define('CBH.model.sales.qsumFileQuoteVendorSummary', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'FileKey', type:'int', defaultValue: null },
        { name:'QHdrKey', type:'int', useNull: true, defaultValue: null },
        { name:'VendorKey', type:'int', defaultValue: null },
        { name:'Vendor', type:'string' },
        { name:'Qty', type:'int', useNull: true, defaultValue: null },
        { name:'Cost', type:'float', useNull: true, defaultValue: null },
        { name:'Price', type:'float', useNull: true, defaultValue: null },
        { name:'Currency', type:'string', useNull: true, defaultValue: null }
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/qsumFileQuoteVendorSummary',
		headers: {
			'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
		},
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
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