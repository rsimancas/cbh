Ext.define('CBH.model.sales.qfrmFileQuoteConfirmation', {
	extend: 'Ext.data.Model',
    idProperty: 'QHdrKey',

	fields: [
    	{ name:'FileKey', type:'int', defaultValue: null },
        { name:'FileYear', type:'int', defaultValue: null },
        { name:'FileNum', type:'int', defaultValue: null },
        { name:'FileStatusKey', type:'int', defaultValue: null },
        { name:'FileQuoteEmployeeKey', type:'int', defaultValue: null },
        { name:'FileOrderEmployeeKey', type:'int', useNull: true, defaultValue: null },
        { name:'FileCustKey', type:'int', useNull: true, defaultValue: null },
        { name:'FileContactKey', type:'int', useNull: true, defaultValue: null },
        { name:'FileCustShipKey', type:'int', useNull: true, defaultValue: null },
        { name:'FileReference', type:'string', useNull: true, defaultValue: null },
        { name:'FileProfitMargin', type:'float', defaultValue: null },
        { name:'FileCurrentVendor', type:'int', useNull: true, defaultValue: null },
        { name:'FileModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'FileModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'FileCreatedBy', type:'string' },
        { name:'FileCreatedDate', type:'date', defaultValue: null },
        { name:'FileDateCustRequired', type:'date', useNull: true, defaultValue: null },
        { name:'FileDateCustRequiredNote', type:'string', useNull: true, defaultValue: null },
        { name:'FileDefaultCurrencyCode', type:'string' },
        { name:'FileDefaultCurrencyRate', type:'float', defaultValue: null },
        { name:'FileClosed', type:'date', useNull: true, defaultValue: null },
        { name:'CustName', type:'string', useNull: true, defaultValue: null },
        { name:'CustFax', type:'string', useNull: true, defaultValue: null },
        { name:'ContactPhone', type:'string', useNull: true, defaultValue: null },
        { name:'ContactEmail', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrKey', type:'int', defaultValue: null },
        { name:'QHdrPrefix', type:'string' },
        { name:'QHdrFileKey', type:'int', defaultValue: null },
        { name:'QHdrNum', type:'int', defaultValue: null },
        { name:'QHdrRevision', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrDate', type:'date', defaultValue: null },
        { name:'QHdrGoodThruDate', type:'date', useNull: true, defaultValue: null },
        { name:'QHdrCustPaymentTerms', type:'int', defaultValue: null },
        { name:'QHdrMemo', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrExFactoryOption', type:'boolean', defaultValue: null },
        { name:'QHdrExFactoryLocation', type:'int', useNull: true, defaultValue: null },
        { name:'QHdrFOBOption', type:'boolean', defaultValue: null },
        { name:'QHdrFOBLocation', type:'int', useNull: true, defaultValue: null },
        { name:'QHdrCIFOption', type:'int', defaultValue: null },
        { name:'QHdrCIFLocation', type:'int', useNull: true, defaultValue: null },
        { name:'QHdrFreightDirect', type:'boolean', defaultValue: null },
        { name:'QHdrLeadTime', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrCarrierKey', type:'int', useNull: true, defaultValue: null },
        { name:'QHdrWarehouseKey', type:'int', useNull: true, defaultValue: null },
        { name:'QHdrShipType', type:'int', defaultValue: null },
        { name:'QHdrSentDate', type:'date', useNull: true, defaultValue: null },
        { name:'QHdrCurrencyCode', type:'string' },
        { name:'QHdrCurrencyRate', type:'float', defaultValue: null },
        { name:'QHdrProdDescription', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrShippingDescription', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrCustRefNum', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'QHdrCreatedBy', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrCreatedDate', type:'date', useNull: true, defaultValue: null },
        { name:'QHdrQuoteConfirmationDate', type:'date', useNull: true, defaultValue: null },
        { name:'QHdrInsurance', type:'boolean', useNull: true, defaultValue: null },
        { name:'QHdrInspectorKey', type:'int', useNull: true, defaultValue: null },
        { name:'QHdrInspectionNum', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrDUINum', type:'string', useNull: true, defaultValue: null },
        { name:'QHdrJobKey', type:'int', useNull: true, defaultValue: null }
	],

	proxy:{
		type:'rest',
		url:CBH.GlobalSettings.webApiPath + '/api/qfrmFileQuoteConfirmation',
		headers: {
			'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
		},
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'QHdrKey'
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