Ext.define('CBH.view.jobs.JobMenuTab', {
    extend: 'Ext.panel.Panel',
    alias: 'widget.jobmenutab',

    requires: [
        'CBH.view.jobs.JobMenu'
    ],

    title: 'Job Menu',

    initComponent: function() {
        var me = this;

        Ext.applyIf(me, {
            items: [
                {
                    xtype: 'container',
                    layout: {
                        type: 'anchor'
                    },
                    items: [
                        {
                            xtype: 'jobmenuform'
                        }
                    ]
                }
            ]
        });

        me.callParent(arguments);
    }

});