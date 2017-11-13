Ext.define('CBH.view.common.MainMenu', {
    extend: 'Ext.form.Panel',
    alias: 'widget.MainMenu',
    
    forceFit: true,
    
    layout: {
        type: 'vbox',
        align: 'center',
        pack: 'center'
    },
    
    padding: '50 10 10 10',

    initComponent: function() {

        var me = this;

        var storeMenu = me.loadStoreMenu();

        var imageTpl = new Ext.XTemplate(
            '<tpl for=".">',
            '<div class="thumb-wrap" id="{MenuDesc}">',
            '<div class="thumb"><a href="#"><img src="app/resources/images/{ImageMenu}" title="{MenuDesc}"></a></div>',
            '<div><h2><p style="width:inherit;text-align:center;">{MenuDesc}</p></h2></div>',
            '</div>',
            '</tpl>',
            '<div class="x-clear"></div>'
        );

        Ext.applyIf(me, {
            items: [{
                cls:'app-menu-view',
                collapsible: false,
                items: [
                    Ext.create('Ext.view.View', {
                        itemId: 'data-view',
                        store: storeMenu,
                        tpl: imageTpl,
                        multiSelect: false,
                        //renderTo: ,
                        width: 680,
                        minWidth: 680,
                        //height: 310,
                        trackOver: true,
                        overItemCls: 'x-item-over',
                        itemSelector: 'div.thumb-wrap',
                        emptyText: 'No images to display',
                        prepareData: function(data) {
                            Ext.apply(data, {
                                shortName: Ext.util.Format.ellipsis(data.caption, 15),
                                sizeString: Ext.util.Format.fileSize(data.size),
                                dateString: Ext.util.Format.date(data.lastmod, "m/d/Y g:i a")
                            });
                            return data;
                        },
                        listeners: {
                            selectionchange: function(dv, nodes) {
                                var l = nodes.length,
                                    s = l !== 1 ? 's' : '';
                            },
                            itemclick: function(dv, record, item, index, e, eOpts) {
                                var me = this.up('form');
                                me.onDataViewDblClick(record);
                            }
                        }
                    })
                ]
            }],
            // Grid Quotes Listeners
            listeners: {
                render: {
                    fn: me.onRenderForm,
                    scope: me
                }
            }
        });

        me.callParent(arguments);
    },

    onRenderForm: function() {
        //var me = this;
        //var field = me.down('#searchfield').focus(true, 200);
    },

    loadStoreMenu: function() {
        return {
            fields: ['MenuDesc', 'ImageMenu', "Widget"],
            data: [{
                "MenuDesc": "Sales",
                "ImageMenu": "sales.png",
                "Widget": "salesmenu"
            }, {
                "MenuDesc": "Jobs",
                "ImageMenu": "jobs.png",
                "Widget": "jobmenuform"
            }, {
                "MenuDesc": "Customers",
                "ImageMenu": "customers.png",
                "Widget": "customerslist"
            }, {
                "MenuDesc": "Vendors",
                "ImageMenu": "vendor.png",
                "Widget": "vendorslist"
            }, {
                "MenuDesc": "Items",
                "ImageMenu": "items.png",
                "Widget": "itemslist"
            }, {
                "MenuDesc": "Reports",
                "ImageMenu": "reports.png",
                "Widget": "reportmenu"
            }]
        };
    },

    onDataViewDblClick: function(record) {
        var pageframe = this.up('app_pageframe');
        var tab = null;

        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: record.data.MenuDesc,
            /*layout: 'fit',
            forceFit: true,*/
            items: [Ext.widget(record.data.Widget)],
            listeners: {
                activate: function() {
                    var grid = this.down('form').down('gridpanel');
                    if (grid)
                        grid.store.reload();
                }
            }
        });

        tab.show();
    }
});
