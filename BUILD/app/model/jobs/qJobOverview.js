Ext.define('CBH.model.jobs.qJobOverview', {
    extend: 'Ext.data.Model',
    alias: 'model.qJobOverview',

    fields: [
    { name:'JobKey', type:'int', defaultValue: null },
    { name:'JobQHdrKey', type:'int', useNull: true, defaultValue: null },
    { name:'QuoteNum', type:'string', useNull: true, defaultValue: null },
    { name:'JobNum', type:'string', useNull: true, defaultValue: null },
    { name:'JobShipDate', type:'date', useNull: true, defaultValue: null },
    { name:'JobArrivalDate', type:'date', useNull: true, defaultValue: null },
    { name:'JobShipmentCarrier', type:'int', useNull: true, defaultValue: null },
    { name:'JobCarrierRefNum', type:'string', useNull: true, defaultValue: null },
    { name:'JobCarrierVessel', type:'string', useNull: true, defaultValue: null },
    { name:'JobInspectionCertificateNum', type:'string', useNull: true, defaultValue: null },
    { name:'JobCustPaymentTerms', type:'int', defaultValue: null },
    { name:'JobClosed', type:'date', useNull: true, defaultValue: null },
    { name:'JobComplete', type:'date', useNull: true, defaultValue: null },
    { name:'JobModifiedBy', type:'string', useNull: true, defaultValue: null },
    { name:'JobModifiedDate', type:'date', useNull: true, defaultValue: null },
    { name:'JobCreatedBy', type:'string' },
    { name:'JobCreatedDate', type:'date', defaultValue: null },
    { name:'JobExemptFromProfitReport', type:'boolean', defaultValue: null },
    { name:'CustName', type:'string' },
    { name:'ContactLastName', type:'string', useNull: true, defaultValue: null },
    { name:'ContactFirstName', type:'string', useNull: true, defaultValue: null },
    { name:'CustPhone', type:'string', useNull: true, defaultValue: null },
    { name:'CustFax', type:'string', useNull: true, defaultValue: null },
    { name:'CustEmail', type:'string', useNull: true, defaultValue: null },
    { name:'ContactPhone', type:'string', useNull: true, defaultValue: null },
    { name:'ContactFax', type:'string', useNull: true, defaultValue: null },
    { name:'ContactEmail', type:'string', useNull: true, defaultValue: null },
    { name:'x_Phone', type:'string', useNull: true, defaultValue: null },
    { name:'x_Email', type:'string', useNull: true, defaultValue: null },
    { name:'x_Fax', type:'string', useNull: true, defaultValue: null },
    { name:'x_ContactName', type:'string', useNull: true, defaultValue: null }, 
    { name:'x_JobShipmentCarrierText', type:'string', useNull: true, defaultValue: null },
    { name:'CustCurrencyCode', type:'string', useNull: true, defaultValue: null },
    { name:'CustCurrencyRate', type:'float', useNull: true, defaultValue: null },
    { name:'x_JobCustPaymentTerms', type:'string', useNull: true, defaultValue: null },
    { name:'x_Info', type:'string', useNull: true, defaultValue: null }
    ],
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/qJobOverview',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data'
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
    },
    load: function(id, config) {
        config = Ext.apply({}, config);
        config = Ext.applyIf(config, {
            model: this,   //this line is necessary
            action: 'read',
            params: {
                id: id
            }
        })
    }
});