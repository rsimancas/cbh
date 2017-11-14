Ext.define('CBH.view.jobs.JobFindJobInvoice', {
    extend: 'Ext.form.Panel',
    alias: 'widget.JobFindJobInvoice',
    title: 'Find PO',
    modal: true,
    width: 1280,
    minHeight: 400,
    callerForm: null,

    initComponent: function() {
        var me = this;

        var storePO = new CBH.store.jobs.qryJobInvoiceSearch({
            pageSize: Math.round((screen.height * (60 / 100)) / 40),
            autoLoad: false
        });

        Ext.applyIf(me, {
            items: [
                // Grid
                {
                    xtype: 'gridpanel',
                    itemId: 'gridInvoices',
                    scrollable: true,
                    columnWidth: 1,
                    viewConfig: {
                        stripeRows: true
                    },
                    minHeight: screen.height * (60 / 100),
                    maxHeight: screen.height * (60 / 100),
                    forceFit: true,
                    store: storePO,
                    columns: [
                        // Columns
                        {
                            xtype: 'gridcolumn',
                            width: 80,
                            dataIndex: 'JobNum',
                            text: 'Job Num'
                        }, {
                            xtype: 'gridcolumn',
                            width: 80,
                            dataIndex: 'InvoiceNum',
                            text: 'Invoice'
                        }, {
                            xtype: 'gridcolumn',
                            width: 80,
                            dataIndex: 'Date',
                            text: 'Date',
                            renderer: Ext.util.Format.dateRenderer('m/d/Y')
                        }, {
                            xtype: 'gridcolumn',
                            flex: 1,
                            dataIndex: 'BillTo',
                            text: 'Bill To'
                        }, {
                            xtype: 'numbercolumn',
                            width: 120,
                            dataIndex: 'Price',
                            text: 'Price',
                            align: 'right',
                            format: '00,000.00',
                        }, {
                            xtype: 'gridcolumn',
                            width: 60,
                            dataIndex: 'InvoiceCurrencyCode',
                            text: 'CUR'
                        }, {
                            xtype: 'actioncolumn',
                            width: 30,
                            items: [{
                                handler: function(grid, rowIndex, colIndex) {
                                    var me = this.up("form");
                                    var record = grid.getStore().getAt(rowIndex);
                                    me.onClickEdit(record);
                                },
                                getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                                tooltip: 'Edit'
                            }]
                        }
                    ],
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
                        store: storePO,
                        displayInfo: true,
                        displayMsg: 'Show {0} - {1} of {2}',
                        emptyMsg: "No records to show"
                    }),
                    listeners: {
                        celldblclick: {
                            fn: function(view, td, cellIndex, record, tr, rowIndex, e, eOpts) {
                                var me = this;
                                me.onClickEdit(record);
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
        var me = this;
        me.onSearchFieldChange();

        /*var grid = me.down('gridpanel');

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

    onClickEdit: function(record) {
        var me = this,
            tabs = me.callerForm.up('app_pageframe');

        var invoice = record;

        Ext.Msg.wait('Loading....', 'Wait');
        var storeJobs = new CBH.store.jobs.JobList().load({
            params: {
                id: invoice.data.JobKey
            },
            callback: function(records, operation, success) {
                var curJob = records[0];

                var storeJobOverview = new CBH.store.jobs.qJobOverview().load({
                    params: {
                        id: invoice.data.JobKey
                    },
                    callback: function(records, operation, success) {
                        var jobOverview = records[0];

                        me.destroy();

                        var form = Ext.widget('joboverview', {
                            currentRecord: jobOverview,
                            currentJob: curJob,
                            JobKey: invoice.data.InvoiceJobKey,
                            JobNum: invoice.data.JobNum
                        });

                        form.loadRecord(jobOverview);

                        var tab = tabs.add({
                            closable: true,
                            iconCls: 'tabs',
                            autoScroll: true,
                            title: 'Job Overview: ' + invoice.data.JobNum,
                            items: [form]
                        });
                        
                        tab.show();
                        Ext.Msg.hide();
                    }
                });
            }
        });
    }
});
