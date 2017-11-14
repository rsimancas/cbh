Ext.define('CBH.view.appViewPort', {
    extend: 'Ext.container.Viewport',

    xtype: 'app_viewport',

    layout: {
        type: 'border'
    },

    requires: [
        'CBH.view.common.ContentPanel',
        'CBH.view.common.MainHeader',
        'CBH.view.common.TreeViewMain',
        'CBH.view.common.PageFrame'
    ],

    initComponent: function() {
        var me = this;

        var storeTreeView = new CBH.store.common.NavTree();


        Ext.applyIf(me, {
            items: [{
                    region: 'north',
                    xtype: 'app_header'
                }, {
                    region: 'west',
                    title: 'Navigation Panel',
                    collapsed: false,
                    collapsible: true,
                    split: true,
                    stateful: false,
                    titleCollapse: true,
                    items:[ 
                    {
                        xtype: 'container',
                        width: 240,
                        html: '<div style="width:100%; padding:10px;"><p style="text-align:center;"><img src="images/logo_cbh_logon.png" style="width:130px;"/></p></div>'
                    },
                    {
                        /*margin: '5 0 0 0',*/
                        /*padding: '10 0 0 0',*/
                        xtype: 'app_treeview',
                        width: 240,
                        height: Math.round(screen.height * 0.40),
                        store: storeTreeView
                    }]
                }, {
                    region: 'center',
                    xtype: 'app_contentpanel',
                    items: [{
                        xtype: 'app_pageframe'
                    }]
                } //,
                // {
                //     region: 'east',
                //     hidden: true
                // },
                // {
                //     region: 'south',
                //     hidden: true
                // }
            ]
        });

        me.callParent(arguments);
    },
});
