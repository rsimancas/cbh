Ext.define('CBH.model.jobs.qryPOSearch', {
	extend: 'Ext.data.Model',

	fields: [
    	{ name:'POKey', type:'int', defaultValue: null },
        { name:'PONum', type:'int', defaultValue: null },
        { name:'PORevisionNum', type:'int', defaultValue: null },
        { name:'PONumShipment', type:'int', defaultValue: null },
        { name:'POJobKey', type:'int', defaultValue: null },
        { name:'POInvoiceKey', type:'int', useNull: true, defaultValue: null },
        { name:'POVendorKey', type:'int', defaultValue: null },
        { name:'POVendorContactKey', type:'int', useNull: true, defaultValue: null },
        { name:'POVendorReference', type:'string', useNull: true, defaultValue: null },
        { name:'POVendorOriginAddress', type:'int', useNull: true, defaultValue: null },
        { name:'PODate', type:'date', defaultValue: null },
        { name:'POGoodThruDate', type:'date', defaultValue: null },
        { name:'POLeadTime', type:'string', useNull: true, defaultValue: null },
        { name:'PODefaultProfitMargin', type:'float', useNull: true, defaultValue: null },
        { name:'POVendorPaymentTerms', type:'int', useNull: true, defaultValue: null },
        { name:'POCurrencyCode', type:'string' },
        { name:'POCurrencyRate', type:'float', useNull: true, defaultValue: null },
        { name:'POShipmentType', type:'int', defaultValue: null },
        { name:'POFreightDestination', type:'int', defaultValue: null },
        { name:'POFreightDestinationZip', type:'string', useNull: true, defaultValue: null },
        { name:'POCustShipKey', type:'int', useNull: true, defaultValue: null },
        { name:'POWarehouseKey', type:'int', useNull: true, defaultValue: null },
        { name:'POShipETA', type:'date', useNull: true, defaultValue: null },
        { name:'POSubmittedDate', type:'date', useNull: true, defaultValue: null },
        { name:'POStatusReport', type:'string', useNull: true, defaultValue: null },
        { name:'POClosed', type:'boolean', defaultValue: null },
        { name:'POModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'POModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'POCreatedBy', type:'string' },
        { name:'POCreatedDate', type:'date', defaultValue: null },
        { name:'POPeachtreeNRecord', type:'int', useNull: true, defaultValue: null },
        { name:'POUseOriginAddress', type:'boolean', defaultValue: null },
        { name:'JobNum', type:'string', useNull: true, defaultValue: null },
        { name:'PONumFormatted', type:'string', useNull: true, defaultValue: null },
        { name:'VendorName', type:'string', useNull: true, defaultValue: null }
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/qryPOSearch',
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