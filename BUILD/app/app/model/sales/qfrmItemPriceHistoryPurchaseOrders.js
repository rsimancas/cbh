Ext.define('CBH.model.sales.qfrmItemPriceHistoryPurchaseOrders', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'POKey', type:'int', defaultValue: null },
        { name:'ItemKey', type:'int', defaultValue: null },
        { name:'CustKey', type:'int', defaultValue: null },
        { name:'PONum', type:'string', useNull: true, defaultValue: null },
        { name:'CostFromSupplier', type:'float', defaultValue: null },
        { name:'PricePaidByCustomer', type:'float', defaultValue: null },
        { name:'Customer', type:'string' },
        { name:'Date', type:'date', useNull: true, defaultValue: null }
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/qfrmItemPriceHistoryPurchaseOrders',
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