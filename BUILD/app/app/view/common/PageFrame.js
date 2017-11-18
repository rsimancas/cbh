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
                items: [Ext.widget('salesmenu')],
                listeners: {
                    activate: {
                        scope: this,
                        fn: me.onActivatePage
                    }
                }
            }],
            plugins: Ext.create('Ext.ux.TabCloseMenu', {
                extraItemsTail: [
                    '-',
                    {
                        text: 'Closable',
                        checked: true,
                        hideOnClick: true,
                        handler: function (item) {
                            currentItem.tab.setClosable(item.checked);
                        }
                    },
                    '-',
                    {
                        text: 'Enabled',
                        checked: true,
                        hideOnClick: true,
                        handler: function(item) {
                            currentItem.tab.setDisabled(!item.checked);
                        }
                    }
                ],
                listeners: {
                    beforemenu: function (menu, item) {
                        var enabled = menu.child('[text="Enabled"]'); 
                        menu.child('[text="Closable"]').setChecked(item.closable);
                        if (item.tab.active) {
                            enabled.disable();
                        } else {
                            enabled.enable();
                            enabled.setChecked(!item.tab.isDisabled());
                        }

                        currentItem = item;
                    }
                }
            })
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

        var cm = new Ext.ux.TabCloseMenu();
        cm.init(this);
    },

    getCount: function() {
        return this.items.items.length;
    }
});
