Ext.define('CBH.model.jobs.tlkpChargeCategories', {
    extend: 'Ext.data.Model',
    alias: 'model.tlkpChargeCategories',
    //idProperty: 'ChargeKey',

    fields: [
    { name:'ChargeKey', type:'int', defaultValue: null },
    { name:'ChargePeachtreeID', type:'string', useNull: true, defaultValue: null },
    { name:'ChargePeachtreeJobPhaseID', type:'string', useNull: true, defaultValue: null },
    { name:'ChargePeachtreeJobCostIndex', type:'int', useNull: true, defaultValue: null },
    { name:'ChargeGLAccount', type:'int', useNull: true, defaultValue: null },
    { name:'ChargeSubTotalCategory', type:'int', defaultValue: null },
    { name:'ChargeAPAccount', type:'int', useNull: true, defaultValue: null },
    { name:'x_DescriptionText', type:'string', useNull: true, defaultValue: null },
    { name:'x_DescriptionLanguageCode', type:'string', useNull: true, defaultValue: null }
    ],
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/tlkpChargeCategories',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            //idProperty: 'ChargeKey'
        },
        writer: {
            type: 'json',
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
        });
    }
});