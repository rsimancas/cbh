Ext.define('CBH.view.jobs.JobMenu', {
    extend: 'Ext.form.Panel',
    alias: 'widget.jobmenuform',
    //width: 790,
    layout: {
        type: 'column'
    },
    bodyPadding: 10,

    initComponent: function() {

        var me = this;

        var storeJobs = new CBH.store.jobs.JobList({
            autoLoad: false
        });

        var storeStatusHistory = null;

        Ext.applyIf(me, {
            items: [
                //Grid Jobs
                {
                    xtype: 'gridpanel',
                    itemId: 'gridjobs',
                    columnWidth: 0.85,
                    height: 420,
                    title: 'Jobs',
                    margin: '0 5 5 0',
                    autoScroll: false,
                    store: storeJobs,
                    columns: [{
                        xtype: 'rownumberer',
                        flex: 1,
                        text: ' # '
                    }, {
                        xtype: 'gridcolumn',
                        flex: 1,
                        dataIndex: 'JobNum',
                        text: 'Job Number'
                    }, {
                        xtype: 'gridcolumn',
                        flex: 1,
                        dataIndex: 'Date',
                        text: 'Date',
                        renderer: Ext.util.Format.dateRenderer('n/d/Y')
                    }, {
                        xtype: 'gridcolumn',
                        flex: 3,
                        dataIndex: 'Customer',
                        text: 'Customer'
                    }, {
                        xtype: 'gridcolumn',
                        flex: 3,
                        dataIndex: 'Reference',
                        text: 'Reference'
                    }, {
                        flex: 1,
                        xtype: 'gridcolumn',
                        dataIndex: 'Status',
                        text: 'Status'
                    }, {
                        xtype: 'actioncolumn',
                        width: 35,
                        items: [{
                            getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                            tooltip: 'view details Job',
                            handler: function(grid, rowIndex, colIndex) {
                                var me = this.up('form');
                                var record = grid.getStore().getAt(rowIndex);
                                me.onClickViewDetails(record);
                            }
                        }]
                    }],
                    bbar: new Ext.PagingToolbar({
                        itemId: 'pagingtoolbar',
                        store: storeJobs,
                        displayInfo: true,
                        displayMsg: 'Displaying records {0} - {1} of {2}',
                        emptyMsg: "No records to display"
                    }),
                    // Grid Jobs Toolbar
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
                            xtype: 'combo',
                            flex: 1,
                            margin: '0 0 5 0',
                            fieldLabel: 'Select from Options',
                            labelAlign: 'top',
                            labelWidth: 200,
                            value: 1,
                            displayField: 'name',
                            queryMode: 'local',
                            hidden: true,
                            store: {
                                fields: ['name'],
                                data: [{
                                    "name": "Current User",
                                    "id": 1
                                }, {
                                    "name": "All users (Current Jobs)",
                                    "id": 2
                                }, {
                                    "name": "All users (2 years)",
                                    "id": 3
                                }]
                            },
                            valueField: 'id',
                            listeners: {
                                beforequery: function(record) {
                                    record.query = new RegExp(record.query, 'i');
                                    record.forceAll = true;
                                }
                            }
                        }
                    ],
                    // Grid Job Listeners
                    listeners: {
                        celldblclick: function() {
                            var me = this.up('form'),
                                grid = me.down("#gridjobs");

                            if (!grid.getSelectionModel().getSelection()[0]) {
                                return;
                            }

                            record = grid.getSelectionModel().getSelection()[0];

                            me.onClickViewDetails(record);
                        },
                        selectionchange: {
                            fn: me.onSelectChange,
                            scope: me
                        },

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
                },
                // Buttons
                {
                    xtype: 'container',
                    columnWidth: 0.15,
                    height: 324,
                    layout: {
                        align: 'stretch',
                        type: 'vbox'
                    },
                    items: [{
                        xtype: 'button',
                        flex: 1,
                        margin: '0 5 5 5',
                        text: 'New Job',
                        listeners: {
                            click: {
                                fn: me.onClickNewJob,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        margin: '0 5 5 5',
                        text: 'Open Job',
                        listeners: {
                            click: function() {
                                var me = this.up('form'),
                                    grid = me.down("#gridjobs");

                                if (grid.getSelectionModel().selected.length === 0) {
                                    grid.getSelectionModel().select(0);
                                }

                                if (!grid.getSelectionModel().getSelection()[0]) {
                                    return;
                                }

                                record = grid.getSelectionModel().getSelection()[0];

                                me.onClickViewDetails(record);
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        margin: '0 5 5 5',
                        text: 'Find Job',
                        listeners: {
                            click: {
                                fn: me.onClickFindJob,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        margin: '0 5 5 5',
                        text: 'Job Information',
                        listeners: {
                            click: {
                                fn: me.onClickJobInformation,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        margin: '0 5 5 5',
                        text: 'Job Status History',
                        listeners: {
                            click: {
                                fn: me.onClickJobStatusHistory,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        height: 16,
                        margin: '10 5 5 5',
                        text: 'Ordering Form',
                        listeners: {
                            click: {
                                fn: me.onClickOrderingForm,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        height: 16,
                        margin: '0 5 5 5',
                        itemId: 'btnEditQuote',
                        text: 'Edit Quote',
                        listeners: {
                            click: {
                                fn: me.onClickEditCustomerQuote,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        height: 16,
                        margin: '10 5 5 5',
                        text: 'Add / Edit PO\'s',
                        listeners: {
                            click: {
                                fn: me.onClickAddEditPO,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        height: 16,
                        margin: '0 5 5 5',
                        text: 'Find PO',
                        listeners: {
                            click: {
                                fn: me.onClickFindPO,
                                scope: me
                            }
                        }
                    }, {
                        xtype: 'button',
                        flex: 1,
                        height: 16,
                        margin: '10 5 5 5',
                        text: 'Find Invoice',
                        handler: me.onClickFindJobInvoice
                    }, {
                        xtype: 'button',
                        flex: 1,
                        height: 16,
                        margin: '10 5 5 5',
                        text: 'Print Status Report',
                        listeners: {
                            click: {
                                fn: me.onClickPrintStatusReport,
                                scope: me
                            }
                        }
                    }]
                },
                // Status History
                {
                    margin: '5 0 5 0',
                    columnWidth: 1,
                    title: 'Status History',
                    xtype: 'gridpanel',
                    itemId: 'gridstatus',
                    store: storeStatusHistory,
                    minHeight: 140,
                    height: 140,
                    hideHeaders: false,
                    columns: [{
                            xtype: 'gridcolumn',
                            width: 120,
                            dataIndex: 'StatusModifiedDate',
                            text: 'Modified Date',
                            renderer: Ext.util.Format.dateRenderer('m/d/Y H:i')
                        },
                        /*{
                                               xtype: 'gridcolumn',
                                               width: 150,
                                               dataIndex: 'x_Status',
                                               text: 'Status'
                                           }, {
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
        me.down('#btnEditQuote').setDisabled(true);
    },

    onSearchFieldChange: function() {
        var form = this,
            field = form.down('#searchfield'),
            fieldValue = field.getRawValue(),
            grid = form.down('#gridjobs');

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

    onClickJobInformation: function() {
        var form = this.up('panel');

        var tabs = this.up('app_pageframe');

        var grid = form.down('#gridjobs');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        curJob = grid.getSelectionModel().getSelection()[0];

        var storeToNavigate = new CBH.store.jobs.JobHeader().load({
            params: {
                id: curJob.data.JobKey,
                page: 1,
                start: 0,
                limit: 8
            },
            callback: function() {
                var form = Ext.widget('jobinformation', {
                    storeNavigator: storeToNavigate,
                    currentJob: curJob
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Job: ' + curJob.data.JobNum,
                    items: [form],
                    listeners: {
                        activate: function() {
                            var form = this.down('form');
                            form.refreshData();
                        }
                    }
                });

                form.down('#FormToolbar').gotoAt(1);

                tab.show();
            }
        });
    },

    onClickNewJob: function() {
        var me = this;

        var tabs = me.up('app_pageframe');

        var storeToNavigate = new CBH.store.jobs.JobHeader();

        var form = Ext.widget('jobinformation', {
            storeNavigator: storeToNavigate
        });

        var tab = tabs.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'New Job',
            items: [form]
        });

        var model = Ext.create('CBH.model.jobs.JobHeader');

        tab.show();

        var btn = form.down('#FormToolbar').down('#add');
        btn.fireEvent('click', btn, null, null, model);
    },

    onClickJobStatusHistory: function() {
        var me = this.up('panel');

        var tabs = this.up('app_pageframe');

        var grid = me.down('#gridjobs');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        var selection = grid.getSelectionModel().getSelection()[0];
        var JobKey = selection.get('JobKey');
        var JobNum = selection.get('JobNum');
        var customer = selection.get('Customer');

        var tab = tabs.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Job Status History',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [
                    Ext.widget('jobstatushistorylist', {
                        JobKey: JobKey,
                        JobNum: JobNum,
                        Customer: customer,
                        JobStatus: selection.get('Status')
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

    onClickViewDetails: function(record) {
        var me = this.up('panel');

        var tabs = this.up('app_pageframe');

        var grid = me.down('#gridjobs');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        var curJob = grid.getSelectionModel().getSelection()[0];

        var storeJobOverview = new CBH.store.jobs.qJobOverview().load({
            params: {
                id: curJob.data.JobKey
            },
            callback: function(records, operation, success) {
                var tabs = me.up('app_pageframe');

                var form = Ext.widget('joboverview', {
                    currentRecord: records[0],
                    currentJob: curJob,
                    JobKey: curJob.data.JobKey,
                    JobNum: curJob.data.JobNum
                });

                form.loadRecord(this.getAt(0));

                var tab = tabs.add({
                    closable: true,
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
            }
        });
    },

    onClickFindJob: function() {
        var me = this,
            form = Ext.widget('JobFindJob', {
                modal: true,
                frameHeader: true,
                header: true,
                layout: {
                    type: 'column'
                },
                title: 'Find Job',
                bodyPadding: 10,
                closable: true,
                stateful: false,
                floating: true,
                callerForm: me,
                forceFit: true
            });

        form.show();
    },

    onClickFindPO: function() {
        var me = this,
            form = Ext.widget('JobFindPO', {
                modal: true,
                frameHeader: true,
                header: true,
                layout: {
                    type: 'column'
                },
                title: 'Find PO',
                bodyPadding: 10,
                closable: true,
                stateful: false,
                floating: true,
                callerForm: me,
                forceFit: true
            });

        form.show();
    },

    onClickFindJobInvoice: function() {
        var me = this,
            form = Ext.widget('JobFindJobInvoice', {
                modal: true,
                frameHeader: true,
                header: true,
                layout: {
                    type: 'column'
                },
                title: 'Find Invoice',
                bodyPadding: 10,
                closable: true,
                stateful: false,
                floating: true,
                callerForm: me,
                forceFit: true
            });

        form.show();
    },

    onSelectChange: function(model, record) {
        var me = this;
        var gridStatus = me.down("#gridstatus");

        if (record && record.length > 0) {
            var storeStatusHistory = new CBH.store.jobs.JobStatusHistorySubDetails({
                autoLoad: false
            }).load({
                params: {
                    JobKey: record[0].data.JobKey,
                    page: 0,
                    limit: 0,
                    start: 0
                },
                callback: function(records, operation, success) {
                    gridStatus.reconfigure(this);
                }
            });
        }

        var grid = me.down('#gridjobs');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];
        me.down('#btnEditQuote').setDisabled(selection.get('JobQHdrKey') === null);
    },

    onClickAddEditPO: function() {
        var me = this,
            tabs = me.up('app_pageframe'),
            grid = me.down('#gridjobs');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var jobnum = selection.data.JobNum,
            curJobKey = selection.data.JobKey;

        var storeToNavigate = new CBH.store.jobs.JobPurchaseOrders({ pageSize: 1 }).load({
            params: {
                POJobKey: curJobKey
            },
            callback: function(records, operation, success) {

                var form = Ext.widget('jobpurchaseordermaintenance', {
                    JobKey: curJobKey,
                    JobNum: jobnum,
                    currentJob: selection,
                    storeNavigator: storeToNavigate
                });

                var tab = tabs.add({
                    layout: {
                        type: 'vbox',
                        align: 'stretch',
                        pack: 'start',
                    },
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Add / Edit PO',
                    padding: '0 5 0 5',
                    items: [form]
                });

                /*if (records && records.length > 0) {
                    form.down('#FormToolbar').gotoAt(1);
                    tab.show();
                } else {*/
                    var model = new CBH.model.jobs.JobPurchaseOrders({
                        POJobKey: curJobKey,
                        POCurrencyCode: selection.data.CustCurrencyCode,
                        POCurrencyRate: selection.data.CustCurrencyRate,
                        POWarehouseKey: !selection.data.JobWarehouseKey ? null : selection.data.JobWarehouseKey,
                        PODefaultProfitMargin: 0.15,
                        POCustShipKey: selection.data.JobCustShipKey,
                        PODate: new Date(),
                        POGoodThruDate: Ext.Date.add(new Date(), Ext.Date.DAY, 1),
                        POVendorPaymentTerms: 5
                    });

                    tab.show();

                    var btn = form.down('#FormToolbar').down('#add');
                    btn.fireEvent('click', btn, null, null, model);
                /*}*/
            },
            scope: this
        });
    },

    onClickEditCustomerQuote: function() {
        var me = this,
            tabs = me.up('app_pageframe'),
            grid = me.down('#gridjobs');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var filenum = selection.get('FileNum'),
            customer = selection.get('Customer'),
            curFileKey = selection.get('QHdrFileKey'),
            quote = selection.get('Quote'),
            quotekey = selection.get('JobQHdrKey');

        var storeToNavigate = new CBH.store.sales.FileQuoteHeader().load({
            params: {
                id: quotekey
            },
            callback: function(records, operation, success) {

                var form = Ext.widget('filequotemaintenance', {
                    QuoteNum: quote,
                    FileKey: curFileKey,
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

    onClickPrintStatusReport: function() {
        var pageframe = this.up('app_pageframe');
        tab = pageframe.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Open Job Summary Report',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [Ext.widget("FormReportCriteria", {
                    reportName: 'rptJobSummary'
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

    onClickOrderingForm: function() {
        var me = this,
            userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey(),
            tabs = me.up('app_pageframe'),
            grid = me.down('#gridjobs');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var jobkey = selection.get('JobKey');

        var formData = {
                id: jobkey,
                employeeKey: userKey
            };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptQuoteOrderingForm", params);
        window.open(pathReport, 'CBH - File Status History', false);
    }
});