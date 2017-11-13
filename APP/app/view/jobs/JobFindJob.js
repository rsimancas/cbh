Ext.define('CBH.view.jobs.JobFindJob', {
    extend: 'Ext.form.Panel',
    alias: 'widget.JobFindJob',
    title: 'Find Job',
    modal: true,
    width: 1280,
    minHeight: 400,
    callerForm: null,

    initComponent: function() {
        var me = this;

        var storeQuoteSearch = new CBH.store.jobs.qryJobSearch({
            pageSize: Math.round((screen.height * (60 / 100)) / 40),
            autoLoad: false
        });

        Ext.applyIf(me, {
            items: [
                // Grid
                {
                    xtype: 'gridpanel',
                    itemId: 'gridjobs',
                    scrollable: true,
                    columnWidth: 1,
                    viewConfig: {
                        stripeRows: true
                    },
                    minHeight: screen.height * (60 / 100),
                    maxHeight: screen.height * (60 / 100),
                    forceFit: true,
                    store: storeQuoteSearch,
                    columns: [
                        // columns
                        {
                            xtype: 'gridcolumn',
                            width: 80,
                            dataIndex: 'JobNum',
                            text: 'Job Num'
                        }, {
                            xtype: 'gridcolumn',
                            width: 80,
                            dataIndex: 'QHdrNum',
                            text: 'Quote Num'
                        }, {
                            xtype: 'gridcolumn',
                            width: 120,
                            dataIndex: 'JobOrderEmployee',
                            text: 'Order Employee'
                        }, {
                            xtype: 'gridcolumn',
                            width: 120,
                            dataIndex: 'CustName',
                            text: 'Customer'
                        }, {
                            xtype: 'gridcolumn',
                            width: 100,
                            dataIndex: 'CustContact',
                            text: 'Contact'
                        }, {
                            xtype: 'gridcolumn',
                            width: 100,
                            dataIndex: 'CustShipName',
                            text: 'Ship To'
                        }, {
                            xtype: 'gridcolumn',
                            width: 120,
                            dataIndex: 'JobReference',
                            text: 'Job Reference'
                        }, {
                            xtype: 'gridcolumn',
                            width: 120,
                            dataIndex: 'JobCustRefNum',
                            text: 'Cust. Reference'
                        }, {
                            xtype: 'gridcolumn',
                            width: 120,
                            dataIndex: 'JobProdDescription',
                            text: 'Product Description'
                        }, {
                            xtype: 'gridcolumn',
                            width: 60,
                            dataIndex: 'JobCustCurrencyCode',
                            text: 'CUR'
                        }, {
                            xtype: 'gridcolumn',
                            width: 100,
                            dataIndex: 'ShipType',
                            text: 'Ship Type'
                        }, {
                            xtype: 'gridcolumn',
                            width: 100,
                            dataIndex: 'InspectionNum',
                            text: 'Insp. Num'
                        }, {
                            xtype: 'gridcolumn',
                            width: 80,
                            dataIndex: 'JobDUINum',
                            text: 'DUI Num'
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
                    }, {
                        margin: '20 10 0 10',
                        width: 115,
                        xtype: 'checkbox',
                        name: 'ShowClosed',
                        labelSeparator: '',
                        hideLabel: true,
                        boxLabel: 'Show Closed',
                        listeners: {
                            change: function(field, newValue, oldValue, eOpts) {
                                var me = this.up('form');
                                me.onSearchFieldChange();
                            }
                        }
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
            showClosed = me.down('field[name=ShowClosed]').getValue(),
            field = me.down('#searchfield'),
            fieldValue = field.getRawValue(),
            grid = me.down('gridpanel');

        grid.store.removeAll();

        if (!String.isNullOrEmpty(fieldValue)) {
            grid.store.loadPage(1, {
                params: {
                    query: fieldValue,
                    ShowClosed: showClosed ? 1 : 0
                },
                callback: function() {
                    me.down('#pagingtoolbar').bindStore(this);
                }
            });
        } else {
            grid.store.loadPage(1, {
                params: {
                    ShowClosed: showClosed ? 1 : 0
                },
                callback: function() {
                    me.down('#pagingtoolbar').bindStore(this);
                }
            });
        }
    },

    onClickEdit: function(record) {
        var me = this;

        var tabs = me.callerForm.up('app_pageframe');

        curJob = record; //grid.getSelectionModel().getSelection()[0];

        var storeJobOverview = new CBH.store.jobs.qJobOverview().load({
            params: {
                id: curJob.data.JobKey
            },
            callback: function(records, operation, success) {
                //var tabs = me.up('app_pageframe');

                var form = Ext.widget('joboverview', {
                    currentRecord: records[0]
                });

                form.loadRecord(this.getAt(0));

                var tab = tabs.add({
                    closable: true,
                    autoScroll: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Job Overview: ' + curJob.data.JobNum,
                    items: [form],
                    listeners: {
                        activate: function() {
                            var form = this.down('form');
                            form.refreshData();
                        }
                    }
                });

                tab.show();
                me.destroy();
            }
        });
    }
});
