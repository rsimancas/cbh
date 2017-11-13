Ext.define('CBH.view.vendors.ItemsList', {
    extend: 'Ext.form.Panel',
    alias: 'widget.itemslist',

    bodyPadding: 10,

    layout: {
        type: 'anchor'
    },

    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;

        var me = this;

        var store = new CBH.store.vendors.Items({
            remoteSort: true,
            sorters: new Ext.util.Sorter({property : 'x_VendorName', direction: 'ASC' })
        });

        Ext.applyIf(me, {
            items: [{
                xtype: 'gridpanel',
                forceFit: true,
                itemId: 'griditems',
                title: 'Items',
                minHeight: 380,
                store: store,
                columns: [{
                    xtype: 'rownumberer',
                    format: '00,000',
                    width: 70
                }, {
                    xtype: 'numbercolumn',
                    width: 70,
                    dataIndex: 'ItemKey',
                    text: 'ID',
                    format: '000'
                }, {
                    xtype: 'gridcolumn',
                    text: 'Vendor',
                    dataIndex: 'x_VendorName',
                    flex: 3
                }, {
                    xtype: 'gridcolumn',
                    text: '(Item Number) / Description',
                    dataIndex: 'x_ItemNumName',
                    flex: 3
                }, {
                    xtype: 'numbercolumn',
                    text: 'Cost',
                    dataIndex: 'ItemCost',
                    format: '00,000.00',
                    align: 'right'
                }, {
                    xtype: 'gridcolumn',
                    text: 'CUR',
                    dataIndex: 'ItemCurrencyCode'
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
                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) {
                            return 'xf00e@FontAwesome'; },
                        tootip: 'view details'
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
                        text: 'Add',
                        itemId: 'additem',
                        handler: function() {
                            var tabs = this.up('app_pageframe');

                            storeToNavigate = new CBH.store.vendors.Items();

                            var form = Ext.widget('items', {
                                storeNavigator: storeToNavigate,
                                //modal: true,
                                //width: 720,
                                //frameHeader: true,
                                //header: true,
                                layout: {
                                    type: 'column'
                                },
                                //title: 'New Item',
                                //bodyPadding: 10,
                                //closable: true,
                                //constrain: true,
                                //stateful: false,
                                //floating: true,
                                callerForm: me,
                                forceFit: true
                            });

                            var tab = tabs.add({
                                closable: true,
                                iconCls: 'tabs',
                                autoScroll: true,
                                title: 'New Item',
                                items: [form]
                            });

                            tab.show();

                            //form.show();

                            var btn = form.down('#FormToolbar').down('#add');
                            btn.fireEvent('click', btn);
                        }
                    }, {
                        itemId: 'deleteitem',
                        text: 'Delete',
                        hidden: accLevel === 3,
                        handler: function() {
                            var grid = this.up('gridpanel');
                            var sm = grid.getSelectionModel();

                            selection = sm.getSelection();

                            if (selection) {
                                Ext.Msg.show({
                                    title: 'Delete',
                                    msg: 'Do you want to delete?',
                                    buttons: Ext.Msg.YESNO,
                                    icon: Ext.Msg.QUESTION,
                                    fn: function(btn) {
                                        if (btn === "yes") {
                                            selection[0].destroy({
                                                success: function() {
                                                    grid.store.remove(selection[0]);
                                                    if (grid.store.getCount() > 0) {
                                                        sm.select(0);
                                                    }
                                                }
                                            });
                                        }
                                    }
                                }).defaultButton = 2;
                            }
                        },
                        disabled: true
                    }
                ],
                bbar: new Ext.PagingToolbar({
                    itemId: 'pagingtoolbar',
                    store: store,
                    displayInfo: true,
                    displayMsg: 'Displaying records {0} - {1} of {2}',
                    emptyMsg: "No records to display"
                }),
                selType: 'rowmodel',
                listeners: {
                    selectionchange: function(view, records) {
                        this.down('#deleteitem').setDisabled(!records.length);
                    },
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

        var grid = me.down('#griditems');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        var field = me.down('#searchfield').focus(true, 200);
    },

    onSearchFieldChange: function() {
        var form = this,
            field = form.down('#searchfield'),
            fieldValue = field.getRawValue(),
            grid = form.down('gridpanel');

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
        var me = this;
        var tabs = this.up('app_pageframe');
        var grid = me.down('#griditems');

        var itemkey = record.data.ItemKey;

        storeToNavigate = new CBH.store.vendors.Items().load({
            params: {
                id: itemkey
            },
            callback: function() {
                var form = Ext.widget('items', {
                    storeNavigator: storeToNavigate,
                    ItemKey: itemkey
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Item Maintenance',
                    items: [form]
                });

                form.down('#FormToolbar').gotoAt(1);
                tab.show();
            }
        });
    }
});
