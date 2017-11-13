Ext.define('CBH.view.vendors.VendorsList', {
    extend: 'Ext.form.Panel',
    alias: 'widget.vendorslist',

    bodyPadding: 10,
    layout: {
        type: 'anchor'
    },

    requires: [
        'CBH.view.vendors.Vendors'
    ],


    initComponent: function() {

        var me = this;

        var storeVendors = new CBH.store.vendors.Vendors({
            remoteSort: true,
            sorters: new Ext.util.Sorter({property : 'VendorName', direction: 'ASC' })
        });

        Ext.applyIf(me, {
            items: [{
                xtype: 'gridpanel',
                title: 'Vendors',
                forceFit: true,
                minHeight: 350,
                viewConfig: {
                    stripeRows: true
                },
                itemId: 'gridvendor',
                store: storeVendors,
                columns: [{
                    xtype: 'rownumberer',
                    width: 70,
                    format: '00,000'
                }, {
                    xtype: 'numbercolumn',
                    width: 60,
                    dataIndex: 'VendorKey',
                    text: 'ID',
                    format: '000'
                }, {
                    xtype: 'gridcolumn',
                    flex: 3,
                    dataIndex: 'VendorName',
                    text: 'Name'
                }, {
                    xtype: 'gridcolumn',
                    flex: 3,
                    dataIndex: 'VendorAddress1',
                    text: 'Address 1'
                }, {
                    xtype: 'gridcolumn',
                    flex: 3,
                    dataIndex: 'VendorAddress2',
                    text: 'Address 2'
                }, {
                    xtype: 'gridcolumn',
                    flex: 3,
                    dataIndex: 'VendorCity',
                    text: 'City'
                }, {
                    xtype: 'actioncolumn',
                    draggable: false,
                    width: 35,
                    resizable: false,
                    hideable: false,
                    stopSelection: false,
                    items: [{
                        handler: function(view, rowIndex, colIndex, item, e, record, row) {
                            this.up('form').onViewDetails(record);
                        },
                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                    }]
                }],
                tbar: [
                    // Search Field
                    {
                        xtype: 'searchfield',
                        width: '50%',
                        itemId: 'searchfield',
                        name: 'searchField',
                        listeners: {
                            'triggerclick': function(field) {
                                me.onSearchFieldChange();
                            }
                        }
                    }, {
                        xtype: 'component',
                        flex: 1
                    }, {
                        text: 'Create New Vendor',
                        handler: function() {
                            var tabs = this.up('app_pageframe');

                            var grid = this.up('form').down('#gridvendor');

                            storeToNavigate = new CBH.store.vendors.Vendors();
                            var form = Ext.widget('vendors', {
                                storeNavigator: storeToNavigate
                            });

                            var tab = tabs.add({
                                closable: true,
                                iconCls: 'tabs',
                                autoScroll: true,
                                title: 'New Vendor',
                                items: [form]
                            });

                            tab.show();

                            var btn = form.down('#FormToolbar').down('#add');
                            btn.fireEvent('click', btn);
                        }
                    }
                ],
                bbar: new Ext.PagingToolbar({
                    itemId: 'pagingtoolbar',
                    store: storeVendors,
                    displayInfo: true,
                    displayMsg: 'Displaying records {0} - {1} of {2}',
                    emptyMsg: "No records to display"
                }),
                listeners: {
                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                        this.up('form').onViewDetails(record);
                    }
                }
            }],
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
        var me = this;

        var grid = me.down('#gridvendor');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        var field = me.down('#searchfield').focus(true, 200);
    },

    onSearchFieldChange: function() {
        var form = this,
            field = form.down('#searchfield'),
            fieldValue = field.getRawValue(),
            grid = form.down('#gridvendor');

        grid.store.removeAll();

        if (!String.isNullOrEmpty(fieldValue)) {
            grid.store.loadPage(1, {
                params: {
                    query: fieldValue
                },
                callback: function() {
                    form.down('#pagingtoolbar').bindStore(this);
                }
            });
        } else {
            grid.store.loadPage(1, {
                callback: function() {
                    form.down('#pagingtoolbar').bindStore(this);
                }
            });
        }
    },

    onViewDetails: function(record) {
        var me = this,
            tabs = me.up('app_pageframe'),
            grid = me.down('gridpanel');

        storeToNavigate = new CBH.store.vendors.Vendors().load({
            params: {
                id: record.data.VendorKey
            },
            callback: function(records) {
                var form = Ext.widget('vendors', {
                    storeNavigator: storeToNavigate,
                    callerForm: me
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Vendor Maintenance',
                    items: [form],
                });

                //This javascript replaces all 3 types of line breaks with a space
                if(records[0].data.VendorAddress1) 
                    records[0].data.VendorAddress1 = records[0].data.VendorAddress1.replace(/(\r\n|\n|\r)/gm," ");
                if(records[0].data.VendorAddress2)
                    records[0].data.VendorAddress2 = records[0].data.VendorAddress2.replace(/(\r\n|\n|\r)/gm," ");
                
                form.down('#FormToolbar').gotoAt(1);
                tab.show();
            }
        });
    }
});