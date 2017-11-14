Ext.define('CBH.store.jobs.qryPOSearch', {
    extend: 'Ext.data.Store',
    autoLoad: false,
    filterOnLoad: false,

    requires: [
        'CBH.model.jobs.qryPOSearch'
    ],

    constructor: function(cfg) {
        var me = this;
        cfg = cfg || {};
        me.callParent([Ext.apply({
            model: 'CBH.model.jobs.qryPOSearch',
            remoteSort: true,
            proxy: {
                type: 'rest',
                url: CBH.GlobalSettings.webApiPath + '/api/qryPOSearch',
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