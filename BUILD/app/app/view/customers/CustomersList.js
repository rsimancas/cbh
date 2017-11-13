Ext.define('CBH.view.customers.CustomersList', {
    extend: 'Ext.form.Panel',
    alias: 'widget.customerslist',

    bodyPadding: 10,
    layout: {
        type: 'anchor'
    },

    initComponent: function() {

        var me = this;

        var storeCustomers = new CBH.store.customers.Customers().load();

        Ext.applyIf(me, {
            items: [{
                xtype: 'gridpanel',
                title: 'Customers',
                minHeight: 350,
                forceFit: true,
                viewConfig: {
                    stripeRows: true
                },
                itemId: 'gridcustomers',
                store: storeCustomers,
                columns: [{
                    xtype: 'rownumberer',
                    width: 50
                }, {
                    xtype: 'numbercolumn',
                    width: 60,
                    dataIndex: 'CustKey',
                    text: 'ID',
                    format: '000'
                }, {
                    xtype: 'gridcolumn',
                    flex: 3,
                    dataIndex: 'CustName',
                    text: 'Name'
                }, {
                    xtype: 'gridcolumn',
                    flex: 3,
                    dataIndex: 'CustAddress1',
                    text: 'Address 1'
                }, {
                    xtype: 'gridcolumn',
                    flex: 3,
                    dataIndex: 'CustAddress2',
                    text: 'Address 2'
                }, {
                    xtype: 'gridcolumn',
                    flex: 3,
                    dataIndex: 'CustCity',
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
                            var me = this.up('form'),
                                tabs = me.up('app_pageframe'),
                                grid = me.down('#gridcustomers');

                            var storeCust = new CBH.store.customers.Customers().load({
                                params: {
                                    id: record.data.CustKey
                                },
                                callback: function(records, operation, success) {

                                    var form = Ext.widget('customers', {
                                        storeNavigator: storeCust
                                    });

                                    var tab = tabs.add({
                                        closable: true,
                                        iconCls: 'tabs',
                                        autoScroll: true,
                                        title: 'Customer Maintenance',
                                        items: [form]
                                    });

                                    form.down('#FormToolbar').gotoAt(1);

                                    tab.show();
                                }
                            });
                        },
                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf075@FontAwesome';},
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
                        text: 'Create New Customer',
                        handler: function() {
                            var tabs = this.up('app_pageframe');

                            storeCust = new CBH.store.customers.Customers();
                            var form = Ext.widget('customers', {
                                storeNavigator: storeCust
                            });

                            var tab = tabs.add({
                                closable: true,
                                iconCls: 'tabs',
                                autoScroll: true,
                                title: 'Customer Maintenance',
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
                    store: storeCustomers,
                    displayInfo: true,
                    displayMsg: 'Displaying records {0} - {1} of {2}',
                    emptyMsg: "No records to display"
                }),
                listeners: {
                    celldblclick: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                        var me = this.up('form'),
                            tabs = me.up('app_pageframe'),
                            grid = me.down('#gridcustomers');

                        var storeCust = new CBH.store.customers.Customers().load({
                            params: {
                                id: record.data.CustKey
                            },
                            callback: function(records, operation, success) {

                                var form = Ext.widget('customers', {
                                    storeNavigator: this
                                });

                                var tab = tabs.add({
                                    closable: true,
                                    iconCls: 'tabs',
                                    autoScroll: true,
                                    title: 'Customer Maintenance',
                                    items: [form]
                                });

                                form.down('#FormToolbar').gotoAt(1);
                                tab.show();
                            }
                        });
                    }
                }
            }],
            listeners: {
                render: {
                    fn: me.onRenderForm,
                    scope: me
                },
                keypress: function(event, target) {
                    if (event.ctrlKey && !event.shiftKey) {
                        event.stopEvent();

                        switch (event.getKey()) {

                            //     case event.LEFT :
                            //         this.shiftTabs(-1);
                            //         break;

                            //     case event.RIGHT :
                            //         this.shiftTabs(1);
                            //         break;

                            //     case event.DELETE :
                            //         this.closeActiveTab();
                            //         break;

                            case event.F12: // this is actually the "S" key
                                //this.saveAll(); // handler
                                break;

                                // other cases...
                        }
                    }
                }
            }
        });
        me.callParent(arguments);
    },

    onRenderForm: function() {
        var me = this;

        var grid = me.down('#gridcustomers');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        var field = me.down('#searchfield').focus(true, 200);
    },

    onSearchFieldChange: function() {
        var form = this,
            field = form.down('#searchfield'),
            fieldValue = field.getRawValue(),
            grid = form.down('#gridcustomers');

        grid.store.removeAll();

        if (!String.isNullOrEmpty(fieldValue)) {
            grid.store.loadPage(1, {
                params: {
                    query: fieldValue
                },
                callback: function(records, operation, success) {
                    form.down('#pagingtoolbar').bindStore(this);
                }
            });
        } else {
            grid.store.loadPage(1, {
                callback: function(records, operation, success) {
                    form.down('#pagingtoolbar').bindStore(this);
                }
            });
        }
    }
});
