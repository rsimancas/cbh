Ext.define('CBH.store.common.Countries', {
    extend: 'Ext.data.Store',
    requires: [
        'CBH.model.common.Countries'
    ],

    constructor: function(cfg) {
        var me = this;
        cfg = cfg || {};
        me.callParent([Ext.apply({
            autoLoad: false,
            model: 'CBH.model.common.Countries',
            //storeId: 'CountryStore',
            proxy: {
                type: 'rest',
                url: CBH.GlobalSettings.webApiPath + '/api/countries',
                headers: {
                    'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
                },
                reader: {
                    type: 'json',
                    root: 'data',
                    totalProperty: 'total',
                    successProperty: 'success',
                    idProperty: 'CountryKey'
                },
                writer: {
                    type: 'json'
                }
            }
        }, cfg)]);
    }
});