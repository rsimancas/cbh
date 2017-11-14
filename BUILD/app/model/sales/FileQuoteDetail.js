Ext.define('CBH.model.sales.FileQuoteDetail', {
    extend: 'Ext.data.Model',
    alias: 'model.filequotedetail',
    idProperty: 'QuoteKey',

    fields: [
        { name:'QuoteKey', type:'int', defaultValue: null },
        { name:'QuoteFileKey', type:'int', defaultValue: null },
        { name:'QuoteSort', type:'int', defaultValue: null },
        { name:'QuoteQty', type:'float', defaultValue: null },
        { name:'QuoteVendorKey', type:'int', useNull: true, defaultValue: null },
        { name:'QuoteItemKey', type:'int', useNull: true, defaultValue: null },
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
        { name:'QuotePOItemsKey', type:'int', useNull: true, defaultValue: null },
        { name:'x_ProfitMargin', type:'float',
            convert: function(val,row) {
                return GetProfitMargin(row.data.QuoteItemLineCost, row.data.QuoteItemLinePrice);
            }   
        },
        { name:'x_ItemNum', type:'string'},
        { name:'x_LineWeight', type:'float', useNull:true, defaultValue:null,
            convert: function(val,row) {
                return row.data.QuoteQty * row.data.QuoteItemWeight;
            }
        },
        { name:'x_LineVolume', type:'float', useNull:true, defaultValue:null,
            convert: function(val,row) {
                return row.data.QuoteQty * row.data.QuoteItemVolume;
            }
        },
        { name:'x_VendorName', type:'string', useNull:true, defaultValue:null },
        { name:'x_ItemName', type:'string', useNull:true, defaultValue:null },
        { name:'x_FileNum', type:'string', useNull:true, defaultValue: null },
        { name:'QuoteItemLineCostInUSD', type:'float', defaultValue: null,
            convert: function(val,row) {
                return row.data.QuoteItemLineCost * row.data.QuoteItemCurrencyRate;
            }
        },
        { name:'QuoteItemLinePriceInUSD', type:'float', defaultValue: null,
            convert: function(val,row) {
                return row.data.QuoteItemLinePrice * row.data.QuoteItemCurrencyRate;
            }
        },
        { name:'FileDefaultCurrencyCode', type:'string'},
        { name:'FileDefaultCurrencyRate', type:'float', useNull:true, defaultValue: null},
        { name:'FileDefaultCurrencySymbol', type:'string'},
        { name:'QuoteItemCurrencySymbol', type:'string'}
    ],
    proxy:{
        type:'rest',
        url:CBH.GlobalSettings.webApiPath + '/api/FileQuoteDetails',
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
                    //Ext.global.console.warn(request.proxy.reader.jsonData.message);
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

    belongsTo: [
        'CBH.model.sales.FileHeader',
        'CBH.model.sales.FileList'
    ]
});