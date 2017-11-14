Ext.define('CBH.store.sales.qryFileQuoteVendorSummaryWithDiscount', {
    extend: 'Ext.data.Store',
    autoLoad: false,
    filterOnLoad: false,

    requires: [
        'CBH.model.sales.qryFileQuoteVendorSummaryWithDiscount'
    ],

    constructor: function(cfg) {
        var me = this;
        cfg = cfg || {};
        me.callParent([Ext.apply({
            model: 'CBH.model.sales.qryFileQuoteVendorSummaryWithDiscount',
            remoteSort: true,
            proxy: {
                type: 'rest',
                url: CBH.GlobalSettings.webApiPath + '/api/qryFileQuoteVendorSummaryWithDiscount',
                headers: {
                    'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
                },
                reader: {
                    type: 'json',
                    root: 'data',
                    totalProperty: 'total',
                    successProperty: 'success',
                    messageProperty: 'message',
                },
                writer: {
                    type: 'json',
                    writeAllFields: false
                }
            }
        }, cfg)]);
    }
});