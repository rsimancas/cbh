Ext.define('CBH.store.common.PrintQueue', {
    extend: 'Ext.data.Store',
    alias: 'store.PrintQueue',
    autoLoad: false,
    pageSize: 11,

    requires: [
        'CBH.model.common.PrintQueue'
    ],

    constructor: function(cfg) {
        var me = this;
        cfg = cfg || {};
        me.callParent([Ext.apply({
            model: 'CBH.model.common.PrintQueue',
            remoteSort: true,
            proxy: {
                type: 'rest',
                url: CBH.GlobalSettings.webApiPath + '/api/PrintQueue',
                headers: {
                    'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
                },
                reader: {
                    type: 'json',
                    root: 'data',
                    totalProperty: 'total',
                    successProperty: 'success'
                },
                writer: {
                    type: 'json',
                    writeAllFields: true
                },

                afterRequest: function(request, success) {
                    if (request.action == 'read') {
                        //this.readCallback(request);
                    } else if (request.action == 'create') {
                        if (!request.operation.success) {
                            Ext.popupMsg.msg("Warning", "Record was not created");
                            Ext.global.console.warn(request.proxy.reader.jsonData.message);
                        } else {
                            Ext.popupMsg.msg("Success", "Created Successfully");
                        }
                    } else if (request.action == 'update') {
                        if (!request.operation.success) {
                            Ext.popupMsg.msg("Warning", "Record was not saved");
                            Ext.global.console.warn(request.proxy.reader.jsonData.message);
                        } else {
                            Ext.popupMsg.msg("Success", "Updated Successfully");
                        }
                    } else if (request.action == 'destroy') {
                        if (!request.operation.success) {
                            Ext.popupMsg.msg("Warning", "Record was not deleted");
                            //Ext.global.console.warn(request.proxy.reader.jsonData.message);
                        } else {
                            Ext.popupMsg.msg("Success", "Deleted Successfully");
                        }
                    }
                }
            }
        }, cfg)]);
    }
});
