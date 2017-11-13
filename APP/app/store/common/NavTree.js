Ext.define('CBH.store.common.NavTree', {
    extend: 'Ext.data.TreeStore',

    constructor: function(cfg) {
        var me = this;
        cfg = cfg || {};
        me.callParent([Ext.apply({
            storeId: 'MyTreeStore',
            root: {
                expanded: true,
                children: [{
                    text: 'Database',
                    expanded: true,
                    children: [{
                        id: 'find-cus',
                        text: 'Customers',
                        iconCls: 'task',
                        leaf: true,
                        //iconCls: 'task-folder'
                    }, {
                        text: 'Vendors',
                        id: 'find-ven',
                        iconCls: 'task',
                        leaf: true
                    }, {
                        text: 'Items',
                        id: 'find-item',
                        iconCls: 'task',
                        leaf: true
                    }, {
                        text: 'Edit Schedule B',
                        id: 'find-sched-b',
                        iconCls: 'task',
                        leaf: true
                    }, {
                        text: 'Currency Rates',
                        id: 'find-currate',
                        iconCls: 'task',
                        leaf: true
                    }, {
                        text: 'Payment Terms',
                        id: 'find-payterms',
                        iconCls: 'task',
                        leaf: true
                    }, {
                        text: 'Print Queue',
                        id: 'find-print-queue',
                        iconCls: 'task',
                        leaf: true
                    }]
                }, {
                    text: 'Sales Menu',
                    leaf: true

                }, {
                    text: 'Job Menu',
                    id:'job-menu',
                    leaf: true

                }, {
                    text: 'Manager Menu',
                    id: 'manager-menu',
                    leaf: true
                }]
            }
        }, cfg)]);
    }
});
