Ext.define('CBH.model.common.PaymentTerms', {
    extend: 'Ext.data.Model',
    alias: 'model.paymentterms',
    //idProperty: 'TermKey',

    fields: [
    { name:'TermKey', type:'int' },
    { name:'TermPercentPrepaid', type:'float' },
    { name:'TermPercentWithOrder', type:'float' },
    { name:'TermPercentPriorToShip', type:'float' },
    { name:'TermPercentAgainstShipDocs', type:'float' },
    { name:'TermPercentNet', type:'float' },
    { name:'TermPercentDays', type:'int' },
    { name:'TermWarningFlag', type:'boolean' },
    { name:'x_Description', type:'string'}
    ],

    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/PaymentTerms',
        headers: {
           'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data',
            totalProperty: 'total',
            successProperty: 'success',
            messageProperty: 'message',
            //idProperty: 'TermKey'
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