Ext.define('CBH.view.common.PaymentTermsList', {
    extend: 'Ext.form.Panel',
    alias: 'widget.paymenttermslist',

    bodyPadding: 10,
    layout: {
        type: 'anchor'
    },

    requires: [
        'CBH.view.common.PaymentTerms'
    ],


    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;

        var me = this;

        var store = new CBH.store.common.PaymentTerms();

        Ext.applyIf(me, {
            items: [{
                xtype: 'gridpanel',
                itemId: 'gridpayment',
                title: 'Payment Terms',
                minHeight: 400,
                store: store,
                columns: [{
                    xtype: 'gridcolumn',
                    text: 'ID',
                    dataIndex: 'TermKey',
                    flex: 2
                }, {
                    xtype: 'gridcolumn',
                    text: 'Description',
                    dataIndex: 'x_Description',
                    flex: 6
                }, {
                    xtype: 'numbercolumn',
                    text: '% Prepaid',
                    dataIndex: 'TermPercentPrepaid',
                    align: 'right',
                    format: '00,000.00',
                    flex: 2
                }, {
                    xtype: 'actioncolumn',
                    draggable: false,
                    width: 35,
                    resizable: false,
                    hideable: false,
                    stopSelection: false,
                    items: [{
                        handler: function(view, rowIndex, colIndex, item, e, record, row) {
                            var tabs = this.up('app_pageframe');

                            storeToNavigate = new CBH.store.common.PaymentTerms().load({
                                params: {
                                    id: record.data.TermKey
                                },
                                callback: function() {
                                    var form = Ext.widget('paymentterms', {
                                        storeNavigator: storeToNavigate
                                    });
                                    var tab = tabs.add({
                                        closable: true,
                                        iconCls: 'tabs',
                                        autoScroll: true,
                                        title: 'Payment Terms',
                                        items: [form]
                                    });

                                    form.down('#FormToolbar').gotoAt(1);

                                    tab.show();
                                }
                            });

                        },
                        getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                        tootip: 'view details'
                    }]
                }],
                tbar: [{
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

                        storeToNavigate = new CBH.store.common.PaymentTerms();

                        var form = Ext.widget('paymentterms', {
                            storeNavigator: storeToNavigate
                        });

                        var tab = tabs.add({
                            closable: true,
                            iconCls: 'tabs',
                            autoScroll: true,
                            title: 'New Payment Term',
                            items: [form]
                        });

                        tab.show();

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
                                            success: function() {}
                                        });
                                    }
                                }
                            }).defaultButton = 2;
                        }
                    },
                    disabled: true
                }],
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
                        var tabs = this.up('app_pageframe');

                        storeToNavigate = new CBH.store.common.PaymentTerms().load({
                            params: {
                                id: record.data.TermKey
                            },
                            callback: function() {
                                var form = Ext.widget('paymentterms', {
                                    storeNavigator: storeToNavigate
                                });

                                var tab = tabs.add({
                                    closable: true,
                                    iconCls: 'tabs',
                                    autoScroll: true,
                                    title: 'Payment Terms',
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
                }
            }
        });
        me.callParent(arguments);
    },

    onRenderForm: function() {
        var me = this;

        var grid = me.down('#gridpayment');

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
    }
});
