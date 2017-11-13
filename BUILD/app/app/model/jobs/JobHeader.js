Ext.define('CBH.model.jobs.JobHeader', {
    extend: 'Ext.data.Model',
    alias: 'model.jobheader',

    fields: [
        { name:'JobKey', type:'int', defaultValue: null },
        { name:'JobQHdrKey', type:'int', useNull: true, defaultValue: null },
        { name:'JobYear', type:'int', defaultValue: null },
        { name:'JobNum', type:'int', defaultValue: null },
        { name:'JobProdDescription', type:'string', useNull: true, defaultValue: null },
        { name:'JobShippingDescription', type:'string', useNull: true, defaultValue: null },
        { name:'JobReference', type:'string', useNull: true, defaultValue: null },
        { name:'JobSalesEmployeeKey', type:'int', useNull: true, defaultValue: null },
        { name:'JobOrderEmployeeKey', type:'int', defaultValue: CBH.GlobalSettings.getCurrentUserEmployeeKey() },
        { name:'JobCustKey', type:'int', useNull: true, defaultValue: null },
        { name:'JobContactKey', type:'int', useNull: true, defaultValue: null },
        { name:'JobCustShipKey', type:'int', useNull: true, defaultValue: null },
        { name:'JobCustRefNum', type:'string', useNull: true, defaultValue: null },
        { name:'JobDateCustRequired', type:'date', useNull: true, defaultValue: null },
        { name:'JobDateCustRequiredNote', type:'string', useNull: true, defaultValue: null },
        { name:'JobCustCurrencyCode', type:'string' },
        { name:'JobCustCurrencyRate', type:'float', defaultValue: null },
        { name:'JobCustPaymentTerms', type:'int', defaultValue: null },
        { name:'JobCarrierKey', type:'int', useNull: true, defaultValue: null },
        { name:'JobWarehouseKey', type:'int', useNull: true, defaultValue: null },
        { name:'JobShipType', type:'int', defaultValue: null },
        { name:'JobShipDate', type:'date', useNull: true, defaultValue: null },
        { name:'JobArrivalDate', type:'date', useNull: true, defaultValue: null },
        { name:'JobShipmentCarrier', type:'int', useNull: true, defaultValue: null },
        { name:'JobCarrierRefNum', type:'string', useNull: true, defaultValue: null },
        { name:'JobCarrierVessel', type:'string', useNull: true, defaultValue: null },
        { name:'JobInspectorKey', type:'int', useNull: true, defaultValue: null },
        { name:'JobInspectionNum', type:'string', useNull: true, defaultValue: null },
        { name:'JobInspectionCertificateNum', type:'string', useNull: true, defaultValue: null },
        { name:'JobDUINum', type:'string', useNull: true, defaultValue: null },
        { name:'JobClosed', type:'date', useNull: true, defaultValue: null },
        { name:'JobComplete', type:'date', useNull: true, defaultValue: null },
        { name:'JobPTIndex', type:'int', useNull: true, defaultValue: null },
        { name:'JobExemptFromProfitReport', type:'boolean', defaultValue: null },
        { name:'JobExemptFromPronacaReport', type:'boolean', defaultValue: null },
        { name:'JobCurrencyLockedRate', type:'float', useNull: true, defaultValue: null },
        { name:'JobCurrencyLockedDate', type:'date', useNull: true, defaultValue: null },
        { name:'JobStatusKey', type:'int', defaultValue: null },
        { name:'JobModifiedBy', type:'string', useNull: true, defaultValue: null },
        { name:'JobModifiedDate', type:'date', useNull: true, defaultValue: null },
        { name:'JobCreatedBy', type:'string', defaultValue: CBH.GlobalSettings.getCurrentUserName()  },
        { name:'JobCreatedDate', type:'date', defaultValue: new Date() },
        { name:'x_JobNumFormatted', type:'string' }
    ],
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/JobHeader',
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