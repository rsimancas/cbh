Ext.define('CBH.view.sales.FileFindQuote', {
    extend: 'Ext.form.Panel',
    alias: 'widget.FileFindQuote',
    title: 'Find Quote',
    modal: true,
    width: 1260,
    minHeight: 400,
    callerForm: null,

    initComponent: function() {
        var me = this;

        var storeQuoteSearch = new CBH.store.sales.qryFileQuoteSearch({
            pageSize: Math.round((screen.height * (60 / 100)) / 32)
        }).load({
            callback: function() {
                //me.RecalcTotals();
            }
        });

        Ext.applyIf(me, {
            items: [
                // Grid
                {
                    xtype: 'gridpanel',
                    scrollable: true,
                    columnWidth: 1,
                    viewConfig: {
                        stripeRows: true
                    },
                    minHeight: screen.height * (60 / 100),
                    maxHeight: screen.height * (60 / 100),
                    //forceFit: true,
                    store: storeQuoteSearch,
                    columns: [{
                        xtype: 'gridcolumn',
                        sortable: true,
                        width: 150,
                        dataIndex: 'QHdrDate',
                        text: 'Date',
                        renderer: Ext.util.Format.dateRenderer('m/d/Y')
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'QHdrNum',
                        text: 'Quote Num'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'FileNum',
                        text: 'File Num'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'JobNum',
                        text: 'Job Num'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'FileReference',
                        text: 'File Reference'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'QHdrCustRefNum',
                        text: 'Cust. Reference'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'QHdrProdDescription',
                        text: 'Product Description'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'CustName',
                        text: 'Customer'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'CustContact',
                        text: 'Contact'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'WharehouseName',
                        text: 'Ship To'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'QHdrShipType',
                        text: 'Ship Type'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'CustShipName',
                        text: 'Ship To'
                    }, {
                        xtype: 'gridcolumn',
                        width: 80,
                        dataIndex: 'QHdrCurrencyCode',
                        text: 'CUR'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'QHdrInspectionNum',
                        text: 'Inspection Num'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'QHdrDUINum',
                        text: 'DUI Num'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'SalesEmployee',
                        text: 'Sales Employee'
                    }, {
                        xtype: 'gridcolumn',
                        width: 150,
                        dataIndex: 'Order Employee',
                        text: 'Order Employee'
                    }, {
                        xtype: 'actioncolumn',
                        width: 25,
                        items: [{
                            handler: function(grid, rowIndex, colIndex) {
                                var me = this.up("form");
                                var record = grid.getStore().getAt(rowIndex);
                                me.onClickEditCustomerQuote(record);
                            },
                            getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                            tooltip: 'Edit'
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
                    }],
                    selType: 'rowmodel',
                    bbar: new Ext.PagingToolbar({
                        itemId: 'pagingtoolbar',
                        store: storeQuoteSearch,
                        displayInfo: true,
                        displayMsg: 'Show {0} - {1} of {2}',
                        emptyMsg: "No records to show"
                    }),
                    listeners: {
                        viewready: function(grid) {
                            var view = grid.view;

                            // record the current cellIndex
                            grid.mon(view, {
                                uievent: function(type, view, cell, recordIndex, cellIndex, e) {
                                    grid.cellIndex = cellIndex;
                                    grid.recordIndex = recordIndex;
                                }
                            });

                            grid.tip = Ext.create('Ext.tip.ToolTip', {
                                target: view.el,
                                delegate: '.x-grid-cell',
                                trackMouse: true,
                                renderTo: Ext.getBody(),
                                listeners: {
                                    beforeshow: function updateTipBody(tip) {
                                        if (!Ext.isEmpty(grid.cellIndex) && grid.cellIndex !== -1) {
                                            header = grid.headerCt.getGridColumns()[grid.cellIndex];
                                            tip.update(grid.getStore().getAt(grid.recordIndex).get(header.dataIndex));
                                        }
                                    }
                                }
                            });
                        },
                        celldblclick: {
                            fn: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                                var me = this;
                                me.onClickEditCustomerQuote(record);
                            },
                            scope: me
                        }
                    }
                }
            ],
            listeners: {
                render: {
                    fn: me.onRenderForm,
                    scope: me
                },
                afterrender: {
                    fn: me.registerKeyBindings,
                    scope: me
                }
            }

        });

        me.callParent(arguments);
    },

    onRenderForm: function() {
        /*var me = this;

        var grid = me.down('gridpanel');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }*/
    },

    registerKeyBindings: function(view, options) {
        var me = this;
        Ext.EventManager.on(view.getEl(), 'keyup', function(evt, t, o) {
                if (evt.keyCode === Ext.EventObject.INSERT) {
                    evt.stopEvent();
                    var btn = me.down('#addline');
                    btn.fireHandler();
                }
            },
            this);

        me.down('#searchfield').focus(true, 300);
    },

    onSearchFieldChange: function() {
        var me = this,
            field = me.down('#searchfield'),
            fieldValue = field.getRawValue(),
            grid = me.down('gridpanel');

        grid.store.removeAll();

        if (!String.isNullOrEmpty(fieldValue)) {
            grid.store.loadPage(1, {
                params: {
                    query: fieldValue
                },
                callback: function() {
                    me.down('#pagingtoolbar').bindStore(this);
                }
            });
        } else {
            grid.store.loadPage(1, {
                callback: function() {
                    me.down('#pagingtoolbar').bindStore(this);
                }
            });
        }
    },

    onClickEditCustomerQuote: function(record) {
        var me = this,
            tabs = me.callerForm.up('app_pageframe');

        selection = record;

        var filenum = selection.data.FileNum,
            customer = selection.data.CustName,
            filekey = selection.data.FileKey,
            quote = selection.get('QHdrNum'),
            quotekey = selection.get('QHdrKey');

        var storeToNavigate = new CBH.store.sales.FileQuoteHeader().load({
            params: {
                id: quotekey
            },
            callback: function(records, operation, success) {

                var form = Ext.widget('filequotemaintenance', {
                    QuoteNum: quote,
                    FileKey: filekey,
                    Customer: customer,
                    FileNum: filenum,
                    storeNavigator: storeToNavigate
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Quote Maintenance',
                    padding: '0 5 0 5',
                    items: [form]
                });

                me.destroy();
                form.down('#FormToolbar').gotoAt(1);
                tab.show();
            },
            scope: this
        });
    }
});