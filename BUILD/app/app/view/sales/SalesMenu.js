Ext.define('CBH.view.sales.SalesMenu', {
    extend: 'Ext.form.Panel',
    alias: 'widget.salesmenu',

    layout: {
        type: 'column'
    },

    bodyPadding: 10,

    //forceFit: true,

    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;

        var storeFileList = new CBH.store.sales.FileList({
            pageSize: Math.round(((screen.height * 0.40) - 40) / 35)
        });

        var me = this;

        var storeStatusHistory = null;

        var pluginExpanded = true;

        Ext.applyIf(me, {
            items: [
                // Grid Sales
                {
                    xtype: 'gridpanel',
                    itemId: 'gridsales',
                    autoScroll: true,
                    viewConfig: {
                        stripeRows: true
                    },
                    title: 'Files',
                    columnWidth: 0.80,
                    minHeight: 340,
                    height: (screen.height * 0.40).toFixed(0),
                    margin: '0 5 5 0',
                    store: storeFileList,
                    columns: [{
                        xtype: 'rownumberer',
                        flex: 0.3
                    }, {
                        xtype: 'gridcolumn',
                        flex: 0.5,
                        dataIndex: 'Date',
                        text: 'Date',
                        renderer: Ext.util.Format.dateRenderer('m/d/Y')
                    }, {
                        xtype: 'gridcolumn',
                        flex: 0.5,
                        dataIndex: 'FileNum',
                        text: 'File Num'
                    }, {
                        xtype: 'gridcolumn',
                        dataIndex: 'Customer',
                        text: 'Customer',
                        flex: 2.5
                    }, {
                        xtype: 'gridcolumn',
                        flex: 1,
                        dataIndex: 'Reference',
                        text: 'Reference'
                    }, {
                        xtype: 'gridcolumn',
                        dataIndex: 'Status',
                        text: 'Status',
                        flex: 0.7
                    }, {
                        xtype: 'actioncolumn',
                        width: 35,
                        items: [{
                            getGlyph: function() { return 'xf00e@FontAwesome';},
                            tooltip: 'view details',
                            handler: function(view, rowIndex, colIndex, item, e, record, row) {
                                var me = view.up('form');
                                me.onClickViewDetails(record);
                            }
                        }]
                    }],
                    plugins: [{
                        ptype: 'rowexpander',
                        rowBodyTpl: [
                            '<div class="ux-row-expander-comment"><p><b>Modified By:</b> {ModifiedBy}<b>,  Modified Date:</b> {ModifiedDate:date("m/d/Y")}</p></div>',
                            '<div class="ux-row-expander-box"></div>'
                        ],
                        expandOnRender: true,
                        expandOnDblClick: false
                    }],
                    bbar: new Ext.PagingToolbar({
                        itemId: 'pagingtoolbar',
                        store: storeFileList,
                        displayInfo: true,
                        displayMsg: 'Displaying records {0} - {1} of {2}',
                        emptyMsg: "No records to display"
                    }),
                    // Grid Files Toolbar
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
                            text: 'Create New File',
                            handler: function() {
                                var tabs = this.up('app_pageframe');

                                var storeToNavigate = new CBH.store.sales.FileHeader();
                                var form = Ext.widget('fileform', {
                                    storeNavigator: storeToNavigate
                                });

                                var tab = tabs.add({
                                    closable: true,
                                    iconCls: 'tabs',
                                    autoScroll: true,
                                    title: 'New File',
                                    items: [form]
                                });

                                tab.show();

                                var btn = form.down('#FormToolbar').down('#add');
                                btn.fireEvent('click', btn);
                            }
                        }
                    ],
                    // Grid Files Listeners
                    listeners: {
                        selectionchange: {
                            fn: me.onSelectChange,
                            scope: me
                        },
                        celldblclick: {
                            fn: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts ) {
                                this.onClickViewDetails(record);
                            },
                            scope: me
                        }
                    }
                },
                // Buttons Sales
                {
                    xtype: 'tabpanel',
                    columnWidth: 0.20,
                    height: 'auto',
                    items: [
                        {
                            title: 'Files/Quotes',
                            layout: {
                                align: 'stretch',
                                type: 'vbox'
                            },
                            defaults: {
                                margin: '0 10 10 10'
                            },
                            items: [{
                                xtype: 'button',
                                margin: '15 10 10 10',
                                flex: 1,
                                text: 'Open File',
                                listeners: {
                                    click: {
                                        fn: function() {
                                            var me = this.up("form");
                                                grid = me.down("#gridsales");
                                        
                                            if(!grid.getStore().getCount())
                                                return;

                                            if(!grid.getSelectionModel().getSelection().length) {
                                                grid.getSelectionModel().select(0);
                                            }
                                            
                                            me.onClickViewDetails(grid.getSelectionModel().getSelection()[0]);
                                        }
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Find File',
                                listeners: {
                                    click: {
                                        fn: me.onClickFindFile,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Customer Information',
                                listeners: {
                                    click: {
                                        fn: me.onClickCustomerInformation,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Order Details',
                                listeners: {
                                    click: {
                                        fn: me.onClickOrderDetails,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Line Details',
                                listeners: {
                                    click: {
                                        fn: me.onClickLineDetails,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Supplier Quotes',
                                listeners: {
                                    click: {
                                        fn: me.onClickSupplierQuotes,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Status History',
                                listeners: {
                                    click: {
                                        fn: me.onClickStatusHistory,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Delete File',
                                hidden: accLevel === 3,
                                listeners: {
                                    click: {
                                        fn: me.onClickDeleteFile,
                                        scope: me
                                    }
                                }
                            }]
                        },
                        {
                            title: 'Reports',
                            layout: 'column',
                            cls: 'app-custom-panel-background',
                            items: [{
                                xtype: 'button',
                                margin: '15 10 10 10',
                                columnWidth: 1,
                                text: 'Print Status Report',
                                listeners: {
                                    click: {
                                        fn: me.onClickPrintStatusReport,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                margin: '0 10 10 25',
                                columnWidth: 1,
                                text: 'Group By Contacts',
                                handler: function(btn) {
                                    var me = btn.up("form");
                                    me.onClickPrintStatusReport(true);
                                }
                            },  {
                                xtype: 'button',
                                margin: '10 10 10 10',
                                text: 'Quote Reports',
                                ui: 'flat',
                                /*disabled: true,*/
                                cls: 'custom-app-btn',
                                /*style: {
                                    fontSize:'18px;'

                                },*/
                                textAlign: 'left',
                                columnWidth: 1
                            }, {
                                xtype: 'button',
                                margin: '0 10 10 25',
                                columnWidth: 1,
                                text: 'Closed and Shipped',
                                handler: function(btn) {
                                    var me = btn.up("form");
                                    me.onClickPrintClosedAndShipped();
                                }
                            }, {
                                xtype: 'button',
                                margin: '0 10 10 25',
                                columnWidth: 1,
                                text: 'Transit Orders',
                                handler: function(btn) {
                                    var me = btn.up("form");
                                    me.onClickPrintTransitOrders();
                                }
                            }, {
                                xtype: 'button',
                                margin: '0 10 10 25',
                                columnWidth: 1,
                                text: 'Open Quotes',
                                handler: function(btn) {
                                    var me = btn.up("form");
                                    me.onClickPrintPronacaOpenQuotes();
                                }
                            }
                            ]
                        }
                    ]
                },
                // Grid Quotes
                {
                    xtype: 'gridpanel',
                    itemId: 'gridquotes',
                    title: 'Quotes',
                    columnWidth: 0.80,
                    height: 180,
                    margin: '0 5 5 0',
                    columns: [{
                        xtype: 'rownumberer'
                    }, {
                        xtype: 'gridcolumn',
                        flex: 1,
                        dataIndex: 'Date',
                        text: 'Date',
                        renderer: Ext.util.Format.dateRenderer('m/d/Y')
                    }, {
                        xtype: 'gridcolumn',
                        flex: 1,
                        dataIndex: 'Quote',
                        text: 'Quote'
                    }, {
                        xtype: 'gridcolumn',
                        flex: 1,
                        dataIndex: 'Vendors',
                        text: 'Vendors'
                    }, {
                        xtype: 'gridcolumn',
                        dataIndex: 'Status',
                        text: 'Status',
                        flex: 1
                    }, {
                        xtype: 'actioncolumn',
                        flex: 0.2,
                        items: [{
                            getGlyph: function(itemScope, rowIdx, colIdx, item, rec) {
                                return 'xf00e@FontAwesome';
                            },
                            tooltip: 'view details'
                        }],
                        listeners: {
                            click: {
                                fn: me.onClickEditCustomerQuote,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'actioncolumn',
                        flex: 0.2,
                        items: [{
                            handler: function(view, rowIndex, colIndex, item, e, record) {
                                var me = view.up('form');
                                me.onClickPrintQuote(record);
                            },
                            getGlyph: function(itemScope, rowIdx, colIdx, item, rec) {
                                return 'xf02f@FontAwesome';
                            },
                            tooltip: 'print quote'
                        }]
                    }],
                    listeners: {
                        celldblclick: {
                            fn: me.onClickEditCustomerQuote,
                            scope: me
                        }
                    }
                },
                // Buttons Quotes
                {
                    xtype: 'container',
                    columnWidth: 0.20,
                    layout: {
                        align: 'stretch',
                        type: 'vbox'
                    },
                    defaults: {
                        margin: '0 10 10 10'
                    },
                    items: [{
                        xtype: 'button',
                        flex: 1,
                        text: 'Open Quote',
                        listeners: {
                            click: {
                                fn: me.onClickEditCustomerQuote,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        text: 'Find Quote',
                        listeners: {
                            click: {
                                fn: me.onClickFindQuote,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        text: 'Print Quote',
                        listeners: {
                            click: function() {
                                var me = this.up('form'),
                                    grid = me.down('#gridquotes');

                                if (grid.store.getCount() === 0) return;

                                if (grid.getSelectionModel().selected.length === 0) {
                                    grid.getSelectionModel().select(0);
                                }

                                var record = grid.getSelectionModel().getSelection()[0];
                                me.onClickPrintQuote(record);
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        text: 'Status History',
                        listeners: {
                            click: {
                                fn: me.onClickQuoteStatusHistory,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        text: 'Delete Quote',
                        hidden: accLevel === 3,
                        listeners: {
                            click: {
                                fn: me.onClickDeleteQuote,
                                scope: me
                            }
                        }
                    }]
                },
                // Status History
                {
                    margin: '5 10 5 0',
                    columnWidth: 1,
                    title: 'Status History',
                    xtype: 'gridpanel',
                    itemId: 'gridstatus',
                    store: storeStatusHistory,
                    minHeight: 140,
                    height: 140,
                    hideHeaders: false,
                    columns: [{
                            xtype: 'rownumberer',
                            width: 50
                        }, {
                            xtype: 'gridcolumn',
                            width: 120,
                            dataIndex: 'StatusModifiedDate',
                            text: 'Modified Date',
                            renderer: Ext.util.Format.dateRenderer('m/d/Y H:i')
                        }, {
                            xtype: 'gridcolumn',
                            width: 80,
                            dataIndex: 'StatusQuoteNum',
                            text: 'Quote Num'
                        },
                        /*{
                                               xtype: 'gridcolumn',
                                               width: 80,
                                               dataIndex: 'StatusQuoteNum',
                                               text: 'Quote Num.'
                                           },*/
                        {
                            xtype: 'gridcolumn',
                            flex: 1,
                            dataIndex: 'StatusMemo',
                            text: 'Description',
                        }, {
                            xtype: 'gridcolumn',
                            width: 100,
                            dataIndex: 'StatusModifiedBy',
                            text: 'Modified By'
                        }
                    ],
                    selType: 'rowmodel',
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
                        }
                    }
                }
            ],
            // Grid Quotes Listeners
            listeners: {
                render: {
                    fn: me.onRenderForm,
                    scope: me
                }
            }
        });

        /*storeFileList.loadPage(1, {
            callback: function() {
                var grid = me.down('gridpanel');

                if (grid && grid.getSelectionModel().selected.length === 0) {
                    grid.getSelectionModel().select(0);
                }
            }
        });*/
        me.callParent(arguments);
    },

    onRenderForm: function() {

    },

    onSearchFieldChange: function() {
        var form = this,
            field = form.down('#searchfield'),
            fieldValue = field.getRawValue(),
            grid = form.down('#gridsales');

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

    onClickViewDetails: function(record) {
        var me = this;
        var tabs = this.up('app_pageframe');

        var grid = me.down('#gridsales');

        var filekey = record.get('FileKey');
        var filenum = record.get('FileNum');
        var filestatus = record.get('Status');
        var customer = record.get('Customer');

        new CBH.store.sales.FileOverview().load({
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
                                FileNum: filenum,
                                FileStatus: filestatus,
                                Customer: customer
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

                form.loadRecord(curModel);
                tabs.setActiveTab(tab.getId());
            },
            scope: this
        });
    },

    onClickLineDetails: function() {
        var me = this.up('panel');
        var tabs = this.up('app_pageframe');
        var grid = me.down('#gridsales');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        var selection = grid.getSelectionModel().getSelection()[0];

        var r = Ext.create('CBH.model.sales.FileQuoteDetail', {
            QuoteFileKey: selection.get('FileKey'),
            QuoteVendorKey: selection.get('FileVendorKey'),
            x_FileNum: selection.get('FileNum'),
            QuoteSort: '100'
        });

        Ext.Msg.wait('Loading...', 'Wait');

        var storeNavEmpty = new CBH.store.sales.FileQuoteDetail();

        var storeToNavigate = new CBH.store.sales.FileQuoteDetail().load({
            scope: storeToNavigate,
            params: {
                fileKey: selection.get('FileKey')
            },
            callback: function(records, operation, success) {
                var form;

                var vendor = (this.getCount() !== 0) ? this.getAt(0).get('QuoteVendorKey') : 0;
                var filekey = selection.get('FileKey');
                var filenum = selection.get('FileNum');
                var itemkey = (this.getCount() !== 0) ? this.getAt(0).get('QuoteItemKey') : 0;

                form = Ext.widget("filelineentry", {
                    storeNavigator: (this.totalCount === 0) ? storeNavEmpty : this,
                    VendorKey: vendor,
                    ItemKey: itemkey,
                    FileKey: filekey,
                    FileNum: filenum
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Line Entry',
                    //id: 'lineentry',
                    items: [{
                        xtype: 'container',
                        layout: {
                            type: 'anchor'
                        },
                        items: [form]
                    }]
                });

                Ext.Msg.hide();
                tab.show();

                if (this.getCount() === 0) {
                    var btn = form.down('#FormToolbar').down('#add');
                    btn.fireEvent('click', btn, null, null, r);
                } else {
                    form.down('#FormToolbar').gotoAt(1);
                }

                this.lastOptions.callback = null;
            }
        });
    },

    onClickOrderDetails: function() {
        var tabs = this.up('app_pageframe');

        var grid = this.up('panel').down('gridpanel');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        var selection = grid.getSelectionModel().getSelection()[0];
        var fileKey = selection.get('FileKey');
        var fileNum = selection.get('FileNum');
        var customer = selection.get('Customer');
        var custkey = selection.get('FileCustKey');

        var form;

        var tab = tabs.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Order Entry',
            items: [form = Ext.widget('fileorderentry', {
                FileKey: fileKey,
                FileNum: fileNum,
                Customer: customer,
                CustKey: custkey
            })],
            listeners: {
                activate: function() {
                    var grid = this.down('form').down('gridpanel'),
                        lastOptions = grid.store.lastOptions,
                        lastParams = (lastOptions) ? lastOptions.params : null;
                    if (lastParams)
                        grid.store.reload({
                            params: lastParams
                        });
                }
            }
        });

        tabs.setActiveTab(tab.getId());
    },

    onClickStatusHistory: function() {
        var tabs = this.up('app_pageframe');

        var grid = this.up('panel').down('gridpanel');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        var selection = grid.getSelectionModel().getSelection()[0];
        var fileKey = selection.get('FileKey');
        var fileNum = selection.get('FileNum');
        var customer = selection.get('Customer');

        var form;

        var tab = tabs.add({
            closable: true,
            forceFit: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Status History',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [
                    form = Ext.widget('filestatushistorylist', {
                        FileKey: fileKey,
                        FileNum: fileNum,
                        Customer: customer,
                        FileStatus: selection.get('Status')
                    })
                ]
            }],
            listeners: {
                activate: function() {
                    var grid = this.down('form').down('gridpanel'),
                        lastOptions = grid.store.lastOptions,
                        lastParams = (lastOptions) ? lastOptions.params : null;
                    if (lastParams)
                        grid.store.reload({
                            params: lastParams
                        });
                }
            }
        });

        tabs.setActiveTab(tab.getId());
    },

    onClickCustomerInformation: function() {
        var form = this.up('panel');

        var tabs = this.up('app_pageframe');

        var grid = form.down('#gridsales');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        curFile = grid.getSelectionModel().getSelection()[0];

        var storeToNavigate = new CBH.store.sales.FileHeader().load({
            params: {
                id: curFile.data.FileKey,
                page: 1,
                start: 0,
                limit: 8
            },
            callback: function() {
                var form = Ext.widget('fileform', {
                    storeNavigator: storeToNavigate
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'File: ' + this.getAt(0).data.x_FileNumFormatted,
                    items: [form]
                });

                form.down('#FormToolbar').gotoAt(1);

                tab.show();
            }
        });
    },

    onClickSupplierQuotes: function() {
        var me = this;
        var tabs = this.up('app_pageframe');

        var grid = me.down('#gridsales');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var filenum = selection.get('FileNum');
        var customer = selection.get('Customer');
        var filekey = selection.get('FileKey');

        me.getEl().mask('Loading...');

        var storeToNavigate = new CBH.store.sales.FileQuoteVendorInfo().load({
            params: {
                filekey: filekey,
                page: 0,
                start: 0,
                limit: 0,
                ShowOnlyWithQuotes: 1
            },
            callback: function(records, operation, success) {

                if (storeToNavigate.getCount() === 0) {
                    me.getEl().unmask();
                    Ext.Msg.alert('Warning', 'This file doesn\'t have quotes');
                } else {

                    var form = Ext.widget('filequoteentry', {
                        FileKey: filekey,
                        Customer: customer,
                        FileNum: filenum,
                        storeNavigator: storeToNavigate
                    });

                    var tab = tabs.add({
                        closable: true,
                        iconCls: 'tabs',
                        autoScroll: true,
                        title: 'Quote Entry',
                        items: [form],
                        listeners: {
                            activate: function() {
                                var form = this.down('form');
                                form.refreshData();
                            }
                        }
                    });

                    me.getEl().unmask();
                    form.down('#FormToolbar').gotoAt(1);
                    tab.show();
                }
            },
            scope: this
        });
    },

    onSelectChange: function(model, record) {
        var me = this;
        var grid = me.down('#gridquotes');
        var gridStatus = me.down("#gridstatus");

        grid.store.removeAll();

        if (record && record.length > 0) {
            var quotes = new CBH.model.sales.FileList(record[0].data).Quotes().load({
                callback: function() {
                    grid.reconfigure(quotes);
                    if (grid.getSelectionModel().selected.length === 0) {
                        grid.getSelectionModel().select(0);
                    }
                }
            });

            var storeStatusHistory = new CBH.store.sales.FileStatusHistorySubDetails({
                autoLoad: false
            }).load({
                params: {
                    page: 0,
                    start: 0,
                    limit: 0,
                    filekey: record[0].data.FileKey
                },
                callback: function(records, operation, success) {
                    gridStatus.reconfigure(this);
                }
            });
        }
    },

    onClickEditCustomerQuote: function() {
        var me = this,
            tabs = me.up('app_pageframe'),
            grid = me.down('#gridquotes'),
            gridFiles = me.down('#gridsales'),
            fileSelected = gridFiles.getSelectionModel().getSelection()[0];

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var filenum = fileSelected.data.FileNum,
            customer = fileSelected.data.Customer,
            filekey = fileSelected.data.FileKey,
            quote = selection.get('Quote'),
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

                form.down('#FormToolbar').gotoAt(1);
                tab.show();
            },
            scope: this
        });
    },

    onClickPrintQuote: function(record) {
        var userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey();
        var formData = {
            id: record.get('QHdrKey'),
            employeeKey: userKey
        };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptQuoteCustomer", params);
        window.open(pathReport, 'CBH - Quote Customer', false);
    },

    onClickFindFile: function() {
        var tabs = this.up('app_pageframe');

        var me = this,
            form = Ext.widget('FileFindFile', {
                modal: true,
                frameHeader: true,
                header: true,
                layout: 'column',
                title: 'Find File',
                bodyPadding: 10,
                closable: true,
                stateful: false,
                floating: true,
                callerForm: me,
                forceFit: true,
                constrain: true
            });

        form.center().show();
    },

    onClickFindQuote: function() {
        var me = this,
            form = Ext.widget('FileFindQuote', {
                modal: true,
                frameHeader: true,
                header: true,
                layout: {
                    type: 'column'
                },
                title: 'Find Quote',
                bodyPadding: 10,
                closable: true,
                stateful: false,
                floating: true,
                callerForm: me,
                forceFit: true
            });

        form.center().show();
    },

    onClickQuoteStatusHistory: function() {
        var tabs = this.up('app_pageframe'),
            grid = this.up('panel').down('#gridquotes'),
            gridFiles = this.up('panel').down('gridpanel');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        var fileSelection = gridFiles.getSelectionModel().getSelection()[0];

        var selection = grid.getSelectionModel().getSelection()[0];
        var fileKey = selection.get('QHdrFileKey');
        var fileNum = fileSelection.get('FileNum');
        var customer = fileSelection.get('Customer');

        var form;

        var tab = tabs.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Quote Status History',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [
                    form = Ext.widget('filequotestatushistorylist', {
                        FileKey: fileKey,
                        FileNum: fileNum,
                        Customer: customer,
                        FileStatus: fileSelection.get('Status'),
                        QHdrKey: selection.get('QHdrKey'),
                        QuoteNum: selection.get('Quote')
                    })
                ]
            }],
            listeners: {
                activate: function() {
                    var grid = this.down('form').down('gridpanel'),
                        lastOptions = grid.store.lastOptions,
                        lastParams = (lastOptions) ? lastOptions.params : null;
                    if (lastParams)
                        grid.store.reload({
                            params: lastParams
                        });
                }
            }
        });

        tabs.setActiveTab(tab.getId());
    },

    onClickDeleteFile: function() {
        var me = this;
        var tabs = this.up('app_pageframe');

        var grid = me.down('#gridsales');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        if (selection) {
            Ext.Msg.show({
                title: 'Delete',
                msg: 'Do you want to delete file {0}?'.format(selection.data.FileNum),
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(btn) {
                    if (btn === "yes") {
                        me.getEl().mask('Deleting...');
                        selection.destroy({
                            success: function() {
                                me.getEl().unmask();
                            },
                            failure: function() {
                                me.getEl().unmask();
                            }
                        });
                    }
                }
            }).defaultButton = 2;
        }
    },

    onClickDeleteQuote: function() {
        var me = this;
        var tabs = this.up('app_pageframe');

        var grid = me.down('#gridquotes');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        if (selection) {
            Ext.Msg.show({
                title: 'Delete',
                msg: 'Do you want to delete quote {0}?'.format(selection.data.Quote),
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(btn) {
                    if (btn === "yes") {
                        me.getEl().mask('Deleting...');
                        selection.destroy({
                            success: function() {
                                me.getEl().unmask();
                            },
                            failure: function() {
                                me.getEl().unmask();
                            }
                        });
                    }
                }
            }).defaultButton = 2;
        }
    },

    onClickPrintStatusReport: function(groupByContact) {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Status Report',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria", {
                    reportName: (groupByContact === true) ? 'rptFileSummaryByContacts' : 'rptFileSummary'
                })]
            }],
            listeners: {
                activate: function() {
                    /*var grid = this.down('form').down('gridpanel');
                    grid.store.reload();*/
                }
            }
        });

        tab.show();
    },

    onClickPrintClosedAndShipped: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Closed and Shipped Report....',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria", {
                    reportName: 'rptPronacaReportClosedShipped'
                })]
            }],
            listeners: {
                activate: function() {
                    /*var grid = this.down('form').down('gridpanel');
                    grid.store.reload();*/
                }
            }
        });

        tab.show();
    },

    onClickPrintTransitOrders: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Open Quote Report....',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria", {
                    reportName: 'rptPronacaTransitOrders'
                })]
            }],
            listeners: {
                activate: function() {
                    /*var grid = this.down('form').down('gridpanel');
                    grid.store.reload();*/
                }
            }
        });

        tab.show();
    },

    onClickPrintPronacaOpenQuotes: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Open Quote Report....',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria", {
                    reportName: 'rptPronacaReportQuotes NoProfit'
                })]
            }],
            listeners: {
                activate: function() {
                    /*var grid = this.down('form').down('gridpanel');
                    grid.store.reload();*/
                }
            }
        });

        tab.show();
    }
});