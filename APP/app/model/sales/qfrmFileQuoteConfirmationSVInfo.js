Ext.define('CBH.model.sales.qfrmFileQuoteConfirmationSVInfo', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'FVFileKey', type:'int', defaultValue: null },
        { name:'FVQHdrKey', type:'int', useNull: true, defaultValue: null },
        { name:'FVVendorKey', type:'int', defaultValue: null },
        { name:'FVVendorContactKey', type:'int', useNull: true, defaultValue: null },
        { name:'FVProfitMargin', type:'float', defaultValue: null },
        { name:'FVPaymentTerms', type:'int', useNull: true, defaultValue: null },
        { name:'FVFreightDirect', type:'boolean', defaultValue: null },
        { name:'FVFreightDestination', type:'int', defaultValue: null },
        { name:'FVFreightDestinationZip', type:'string', useNull: true, defaultValue: null },
        { name:'FVFreightShipmentType', type:'int', defaultValue: null },
        { name:'FVWarehouseKey', type:'int', useNull: true, defaultValue: null },
        { name:'FVFreightCost', type:'float', defaultValue: null },
        { name:'FVFreightPrice', type:'float', defaultValue: null },
        { name:'FVLeadTime', type:'string', useNull: true, defaultValue: null },
        { name:'FVDiscount', type:'float', defaultValue: null },
        { name:'FVDiscountPercent', type:'float', defaultValue: null },
        { name:'FVDiscountCurrencyCode', type:'string' },
        { name:'FVDiscountCurrencyRate', type:'float', defaultValue: null },
        { name:'FVExcelFilename', type:'string', useNull: true, defaultValue: null },
        { name:'FVTotalWeight', type:'float', useNull: true, defaultValue: null },
        { name:'FVTotalVolume', type:'float', useNull: true, defaultValue: null },
        { name:'FVRFQFileName', type:'string', useNull: true, defaultValue: null },
        { name:'FVRFQDate', type:'date', useNull: true, defaultValue: null },
        { name:'FVSentDate', type:'date', useNull: true, defaultValue: null },
        { name:'FVQuoteInfoConfirmed', type:'boolean', defaultValue: null },
        { name:'FVQuotePONotes', type:'string', useNull: true, defaultValue: null },
        { name:'FVPOCurrencyCode', type:'string' },
        { name:'FVPOCurrencyRate', type:'float', defaultValue: null },
        { name:'ContactPhone', type:'string', useNull: true, defaultValue: null },
        { name:'ContactEmail', type:'string', useNull: true, defaultValue: null },
        { name:'VendorFax', type:'string', useNull: true, defaultValue: null },
        { name:'FVTotalWeightTag', type:'float', useNull: true, defaultValue: null },
        { name:'FVTotalVolumeTag', type:'float', useNull: true, defaultValue: null }
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/qfrmFileQuoteConfirmationSVInfo',
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