Ext.define('CBH.model.sales.qfrmFileQuoteDetailsSub', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'QuoteKey', type:'int', defaultValue: null },
        { name:'QuoteFileKey', type:'int', defaultValue: null },
        { name:'QuoteSort', type:'int', defaultValue: null },
        { name:'QuoteQty', type:'int', defaultValue: null },
        { name:'QuoteVendorKey', type:'int', defaultValue: null },
        { name:'QuoteItemKey', type:'int', defaultValue: null },
        { name:'QuoteItemCost', type:'float', defaultValue: null },
        { name:'QuoteItemPrice', type:'float', defaultValue: null },
        { name:'QuoteItemLineCost', type:'float', defaultValue: null },
        { name:'QuoteItemLinePrice', type:'float', defaultValue: null },
        { name:'QuoteItemCurrencyCode', type:'string' },
        { name:'QuoteItemCurrencyRate', type:'float', defaultValue: null },
        { name:'QuoteItemWeight', type:'float', useNull: true, defaultValue: null },
        { name:'QuoteItemVolume', type:'float', useNull: true, defaultValue: null },
        { name:'QuoteItemMemoCustomer', type:'string', useNull: true, defaultValue: null },
        { name:'QuoteItemMemoCustomerMoveBottom', type:'boolean', defaultValue: null },
        { name:'QuoteItemMemoSupplier', type:'string', useNull: true, defaultValue: null },
        { name:'QuoteItemMemoSupplierMoveBottom', type:'boolean', defaultValue: null },
        { name:'LineCost', type:'float', useNull: true, defaultValue: null },
        { name:'LinePrice', type:'float', useNull: true, defaultValue: null },
        { name:'LineWeight', type:'float', useNull: true, defaultValue: null },
        { name:'LineVolume', type:'float', useNull: true, defaultValue: null },
        { name:'FileDefaultCurrencyCode', type:'string'},
        { name:'FileDefaultCurrencyRate', type:'float', useNull:true, defaultValue: null},
        { name:'FileDefaultCurrencySymbol', type:'string'},
        { name:'QuoteItemCurrencySymbol', type:'string'}
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/qfrmFileQuoteDetailsSub',
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