Ext.define('CBH.view.sales.FileFindFile', {
    extend: 'Ext.form.Panel',
    alias: 'widget.FileFindFile',
    title: 'Find File',
    modal: true,
    width: 1360,
    minHeight: 400,
    callerForm: null,

    initComponent: function() {
        var me = this;

        var storeQuoteSearch = new CBH.store.sales.qryFileSearch({
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
                    forceFit: true,
                    store: storeQuoteSearch,
                    columns: [/*{
                        xtype: 'gridcolumn',
                        sortable: true,
                        width: 80,
                        dataIndex: 'QHdrDate',
                        text: 'Date',
                        renderer: Ext.util.Format.dateRenderer('m/d/Y')
                    },*/ {
                        xtype: 'gridcolumn',
                        width: 75,
                        dataIndex: 'FileNum',
                        text: 'File Num'
                    }, {
                        xtype: 'gridcolumn',
                        width: 100,
                        dataIndex: 'SalesEmployee',
                        text: 'Sales Employee'
                    }, {
                        xtype: 'gridcolumn',
                        width: 100,
                        dataIndex: 'OrderEmployee',
                        text: 'Order Employee'
                    }, {
                        xtype: 'gridcolumn',
                        width: 120,
                        dataIndex: 'CustName',
                        text: 'Customer'
                    }, {
                        xtype: 'gridcolumn',
                        width: 120,
                        dataIndex: 'CustContact',
                        text: 'Cust. Contact'
                    }, {
                        xtype: 'gridcolumn',
                        width: 120,
                        dataIndex: 'CustShipName',
                        text: 'Ship To'
                    }, {
                        xtype: 'gridcolumn',
                        width: 120,
                        dataIndex: 'FileReference',
                        text: 'File Reference'
                    }, {
                        xtype: 'gridcolumn',
                        width: 60,
                        dataIndex: 'FileCurrencyCode',
                        text: 'CUR'
                    }, {
                        xtype: 'gridcolumn',
                        width: 85,
                        dataIndex: 'QHdrNum',
                        text: 'Quote Num'
                    }, {
                        xtype: 'gridcolumn',
                        width: 120,
                        dataIndex: 'VendorName',
                        text: 'Vendor'
                    }, {
                        xtype: 'gridcolumn',
                        width: 120,
                        dataIndex: 'VendorContact',
                        text: 'Contact'
                    }, {
                        xtype: 'gridcolumn',
                        width: 90,
                        dataIndex: 'ShipType',
                        text: 'Ship Type'
                    }, {
                        xtype: 'gridcolumn',
                        width: 120,
                        dataIndex: 'WarehouseName',
                        text: 'Ship To'
                    }, {
                        xtype: 'gridcolumn',
                        width: 60,
                        dataIndex: 'VendorCurrencyCode',
                        text: 'CUR'
                    }, {
                        xtype: 'actioncolumn',
                        width: 30,
                        items: [{
                            handler: function(grid, rowIndex, colIndex) {
                                    var me = this.up("form");
                                    var record = grid.getStore().getAt(rowIndex);
                                    me.onClickView(record);
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
                        celldblclick: {
                            fn: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                                var me = this;
                                me.onClickView(record);
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
                },
            }

        });

        me.callParent(arguments);
    },

    onRenderForm: function() {
        
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

    onClickView: function(record) {
        var me = this,
            tabs = me.callerForm.up('app_pageframe'); 
            
        selection = record;

        var filenum = selection.data.FileNum,
            customer = selection.data.CustName,
            filekey = selection.data.FileKey;

        var storeToNavigate = new CBH.store.sales.FileOverview().load({
            params: {
                id: filekey,
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function(records, operation, success) {

                var curModel = records[0];

                var form;

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'File Overview',
                    items: [{
                        xtype: 'container',
                        layout: {
                            type: 'anchor'
                        },
                        items: [
                            form = Ext.widget('fileoverview', {
                                FileKey: filekey,
                                modelFileOverView: curModel,
                                FileNum: filenum
                            })
                        ]
                    }],
                    listeners: {
                        activate: function() {
                            var form = this.down('form');
                            form.refreshData();
                        }
                    }
                });

                me.destroy();
                form.loadRecord(curModel);
                tabs.setActiveTab(tab.getId());
            },
            scope: this
        });
    }
});