Ext.define('CBH.model.common.CurrencyRates', {
    extend: 'Ext.data.Model',

    fields: [
    { name:'CurrencyCode', type:'string' },
    { name:'CurrencyRate', type:'float' },
    { name:'CurrencyDescription', type:'string' },
    { name:'CurrencySymbol', type:'string' },
    { name:'CurrencyFormat', type:'string' },
    { name:'CurrencyModifiedDate', type:'date' },
    { name:'x_CurrencyCodeDesc', type:'string'},
    {
        name: 'CurrencyCodeDesc', 
        type: 'string',
            convert: function(val,row) {
                return (row.data.CurrencyCode !== null) ? row.data.CurrencyCode +', '+ row.data.CurrencyDescription : '';
            }   
    }
    ],

    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/currencyrates',
        headers: {
           'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
        },
        reader:{
            type:'json',
            root:'data'
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