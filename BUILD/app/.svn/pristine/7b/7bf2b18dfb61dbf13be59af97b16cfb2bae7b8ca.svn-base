Ext.define('CBH.store.common.Languages', {
    extend: 'Ext.data.Store',

    requires: [
        'CBH.model.common.Languages'
    ],

    constructor: function(cfg) {
        var me = this;
        cfg = cfg || {};
        me.callParent([Ext.apply({
            autoLoad: false,
            model: 'CBH.model.common.Languages',
            //storeId: 'LanguageStore',
            proxy: {
                type: 'rest',
                url: CBH.GlobalSettings.webApiPath + '/api/languages',
                headers: {
                    'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
                },
                reader: {
                    type: 'json',
                    root: 'data',
                    totalProperty: 'total',
                    successProperty: 'success',
                    idProperty: 'LanguageCode'
                },
                writer: {
                    type: 'json',
                    writeAllFields: true
                }
            }
        }, cfg)]);
    }
});