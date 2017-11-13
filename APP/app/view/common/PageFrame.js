Ext.define('CBH.view.common.PageFrame', {
    extend: 'Ext.tab.Panel',

    xtype: 'app_pageframe',

    //forceFit: true,
    
    layout:'column',

    initComponent: function() {
        var me = this;

        Ext.applyIf(me, {
            items: [{
                title: 'Sales Menu',
                columnWidth: 1,
                autoScroll: true,
                //height: '100%',
                // layout: {
                //     type: 'vbox',
                //     align: 'center',
                //     pack: 'center'
                // },
                items: [Ext.widget('salesmenu')],
                listeners: {
                    activate: {
                        scope: this,
                        fn: me.onActivatePage
                    }
                }
            }]
        });

        me.callParent(arguments);
    },

    onActivatePage: function() {
        var me = this,
            grid = me.down('gridpanel');
        grid.store.reload({
            scope: grid,
             callback: function() {
                var selModel = this.getSelectionModel(),
                    selected = selModel.getSelection();
                this.fireEvent('selectionchange', selModel, selected);
            }
        });
    },

    getCount: function() {
        return this.items.items.length;
    }
});
