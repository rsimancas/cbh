Ext.define('CBH.model.jobs.JobPurchaseOrderInstructions', {
    extend: 'Ext.data.Model',
    alias: 'model.JobPurchaseOrderInstructions',
    idProperty: 'POInstructionsKey',

    fields: [
    { name:'POInstructionsKey', type:'int', defaultValue: null },
    { name:'POInstructionsPOKey', type:'int', defaultValue: null },
    { name:'POInstructionsStep', type:'int', defaultValue: null },
    { name:'POInstructionsInstructionKey', type:'int', useNull: true, defaultValue: null },
    { name:'POInstructionsMemo', type:'string', useNull: true, defaultValue: null },
    { name:'POInstructionsMemoFontColor', type:'int', defaultValue: null },
    { name:'x_ITextMemo', type:'string', defaultValue:null},
    { name:'x_NotesFontColor', type:'string', defaultValue:null}
    ],
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/JobPurchaseOrderInstructions',
        headers: {
             'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            idProperty: 'POInstructionsKey'
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
        })
    }
});