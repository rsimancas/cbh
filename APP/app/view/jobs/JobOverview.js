Ext.define('CBH.view.jobs.JobOverview', {
    extend: 'Ext.form.Panel',
    alias: 'widget.joboverview',
    layout: {
        type: 'column'
    },
    bodyPadding: 10,
    currentRecord: null,
    modelJobOverView: null,
    currentJob: null,

    initComponent: function() {

        var me = this;

        var storeVendorPOInformation = new CBH.store.jobs.qLstJobPurchaseOrders().load({
            params: {
                JobKey: me.currentRecord.data.JobKey
            },
            scope: me
        });

        var storeInvoiceInformation = new CBH.store.jobs.qLstJobInvoices().load({
            params: {
                JobKey: me.currentRecord.data.JobKey
            },
            scope: me
        });

        storeStatus = new CBH.store.jobs.JobStatusHistorySubDetails({
            autoLoad: false
        }).load({
            params: {
                JobKey: me.currentRecord.data.JobKey,
                page: 0,
                limit: 0,
                start: 0
            }
        });

        var filterQHdr = new Ext.util.Filter({
            property: 'ListCategory',
            value: 2
        });

        var storetlkpGL = new CBH.store.sales.tlkpGenericLists().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            },
            filters: [filterQHdr],
            callback: function() {
                var field = me.down('field[name=JobShipmentCarrier]').bindStore(storetlkpGL);
                field.setValue(me.currentRecord.data.JobShipmentCarrier);
            }
        });

        Ext.applyIf(me, {
            items: [{
                    xtype: 'fieldset',
                    padding: '5 5',
                    margin: '5 0 5 0',
                    columnWidth: 1,
                    layout: {
                        type: 'hbox'
                    },
                    items: [{
                        xtype: 'component',
                        flex: 1
                    }, {
                        xtype: 'displayfield',
                        name: 'QuoteNum',
                        hideTrigger: true,
                        labelAlign: 'top',
                        fieldLabel: 'QuoteNum',
                        editable: false,
                        fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: right'
                    }, {
                        margin: '0 0 0 10',
                        xtype: 'displayfield',
                        name: 'JobNum',
                        fieldLabel: 'Reference Num',
                        labelAlign: 'top',
                        editable: false,
                        fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: right'
                    }]
                },
                // panel left
                {
                    xtype: 'container',
                    columnWidth: 0.80,
                    layout: {
                        type: 'column'
                    },
                    items: [
                        // Vendors Grid
                        {
                            xtype: 'gridpanel',
                            itemId: 'gridvendors',
                            title: 'Vendor PO/Information',
                            columnWidth: 0.75,
                            height: 200,
                            margin: '0 5 5 0',
                            store: storeVendorPOInformation,
                            features: [{
                                ftype: 'summary'
                            }],
                            columns: [{
                                xtype: 'rownumberer'
                            }, {
                                xtype: 'gridcolumn',
                                width: 80,
                                dataIndex: 'Date',
                                text: 'Date',
                                renderer: Ext.util.Format.dateRenderer('m/d/Y')
                            }, {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'Vendor',
                                text: 'Vendor'
                            }, {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'PONum',
                                text: 'PO Num'
                            }, {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'Status',
                                text: 'Status',
                                summaryRenderer: function(value, summaryData, dataIndex) {
                                    var lastIndex = this.store.getCount() - 1,
                                        currency = (lastIndex > -1) ? this.store.getAt(lastIndex).data.CurrencyCode : 0;
                                    return Ext.String.format('Total {0}', currency);
                                },
                            }, {
                                xtype: 'numbercolumn',
                                width: 120,
                                dataIndex: 'Cost',
                                text: 'Cost',
                                align: 'right',
                                format: '00,000.00',
                                summaryType: 'sum'
                            }, {
                                xtype: 'gridcolumn',
                                width: 60,
                                dataIndex: 'CurrencyCode',
                                text: 'CUR'
                            }],
                            listeners: {
                                celldblclick: {
                                    fn: me.onClickEditPurchaseOrder,
                                    scope: me
                                }
                            }
                        },
                        // PO Buttons
                        {
                            xtype: 'container',
                            columnWidth: 0.25,
                            layout: {
                                align: 'stretch',
                                type: 'vbox'
                            },
                            defaults: {
                                margin: '0 10 10 5'
                            },
                            items: [{
                                xtype: 'button',
                                flex: 1,
                                text: 'New Purchase Order',
                                handler: function() {
                                    var me = this.up('panel');

                                    me.onClickNewPurchaseOrder();
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Edit Purchase Order',
                                handler: function() {
                                    var me = this.up('panel');

                                    me.onClickEditPurchaseOrder();
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Print Purchase Order',
                                handler: function() {
                                    var me = this.up('panel');

                                    me.onClickPrintPurchaseOrder();
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Edit Status History',
                                handler: me.onClickEditStatusHistory
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Export PO To Peachtree',
                                handler: me.onClickExportPOToPeachtree
                            }]
                        },
                        // Invoices
                        {
                            xtype: 'gridpanel',
                            itemId: 'gridInvoices',
                            store: storeInvoiceInformation,
                            title: 'Invoice Information',
                            columnWidth: 0.75,
                            minHeight: 200,
                            margin: '0 5 5 0',
                            columns: [{
                                xtype: 'rownumberer'
                            }, {
                                xtype: 'gridcolumn',
                                width: 85,
                                dataIndex: 'Date',
                                text: 'Date',
                                renderer: Ext.util.Format.dateRenderer('m/d/Y')
                            }, {
                                xtype: 'gridcolumn',
                                width: 80,
                                dataIndex: 'Invoice',
                                text: 'Invoice'
                            }, {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'BillTo',
                                text: 'Bill To'
                            }, {
                                xtype: 'numbercolumn',
                                dataIndex: 'Price',
                                text: 'Price',
                                width: 120,
                                align: 'right',
                                format: '00,000.00'
                            }, {
                                xtype: 'gridcolumn',
                                width: 80,
                                dataIndex: 'CurrencyCode',
                                text: 'CUR'
                            }],
                            listeners: {
                                celldblclick: {
                                    fn: me.onClickEditInvoice,
                                    scope: me
                                }
                            }

                        },
                        // Invoice Buttons
                        {
                            xtype: 'container',
                            columnWidth: 0.25,
                            layout: {
                                align: 'stretch',
                                type: 'vbox'
                            },
                            defaults: {
                                margin: '0 10 10 5'
                            },
                            items: [{
                                xtype: 'button',
                                flex: 1,
                                text: 'New Invoice',
                                listeners: {
                                    click: function() {
                                        me.onClickJobNewInvoice();
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Edit Invoice',
                                listeners: {
                                    click: function() {
                                        me.onClickEditInvoice();
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Print Invoice',
                                listeners: {
                                    click: function() {
                                        me.onClickPrintInvoice();
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Print Packing List',
                                listeners: {
                                    click: {
                                        fn: me.onClickPackingList,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Export Inv. To Peachtree',
                                handler: me.onClickExportInvToPeachtree
                            }]
                        },
                        // Status History
                        {
                            margin: '5 0 5 0',
                            columnWidth: 1,
                            title: 'Status History',
                            xtype: 'gridpanel',
                            itemId: 'gridstatus',
                            store: storeStatus,
                            minHeight: 140,
                            height: 140,
                            hideHeaders: true,
                            columns: [{
                                    xtype: 'gridcolumn',
                                    width: 120,
                                    dataIndex: 'StatusModifiedDate',
                                    text: 'Modified Date',
                                    renderer: Ext.util.Format.dateRenderer('m/d/Y H:i')
                                },
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
                            selType: 'rowmodel'
                        }
                    ],
                    listeners: {
                        render: {
                            fn: me.onRenderForm,
                            scope: me
                        }
                    }
                },
                // panel right
                {
                    margin: '0 0 0 5',
                    xtype: 'container',
                    columnWidth: 0.20,
                    layout: {
                        type: 'column'
                    },
                    items: [{
                        xtype: 'container',
                        columnWidth: 1,
                        layout: {
                            align: 'stretch',
                            type: 'vbox'
                        },
                        defaults: {
                            margin: '0 10 10 5'
                        },
                        items: [{
                            xtype: 'button',
                            flex: 1,
                            text: 'Job Information',
                            handler: function() {
                                var me = this.up('form');
                                me.onClickJobInformation();

                            }
                        }, {
                            xtype: 'button',
                            flex: 1,
                            text: 'Update Currency Rates',
                            handler: function() {
                                var me = this.up("form");
                                me.onClickUpdateCurrencyRates();
                            }
                        }, {
                            xtype: 'button',
                            itemId: 'btnEditQuote',
                            flex: 1,
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
                            text: 'Print Ordering Form',
                            listeners: {
                                click: {
                                    fn: me.onClickPrintOrderingForm,
                                    scope: me
                                }
                            }
                        }, {
                            xtype: 'button',
                            flex: 1,
                            text: 'Job Status History',
                            listeners: {
                                click: {
                                    fn: me.onClickJobStatusHistory,
                                    scope: me
                                }
                            }
                        }, {
                            xtype: 'textareafield',
                            margin: '20 0 0 0',
                            columnWidth: 1,
                            labelAlign: 'top',
                            fieldLabel: 'Payment Terms',
                            name: 'x_JobCustPaymentTerms',
                            editable: false,
                            readOnly: true
                        }, {
                            xtype: 'textareafield',
                            margin: '20 0 0 0',
                            columnWidth: 1,
                            labelAlign: 'top',
                            fieldStyle: 'color:red; font-weight: bold; font-size: 13px;',
                            //fieldLabel: 'Payment Terms',
                            name: 'x_Info',
                            editable: false,
                            readOnly: true
                        }]
                    }]
                },
                // Cust Info
                {
                    xtype: 'fieldset',
                    columnWidth: 0.5,
                    fieldDefaults: {
                        labelWidth: 50,
                        labelAlign: 'top',
                        readOnly: true
                    },
                    padding: '0 10 10 10',
                    layout: 'column',
                    collapsible: true,
                    title: 'Customer Information',
                    items: [{
                        xtype: 'textfield',
                        name: 'CustName',
                        fieldLabel: 'Customer',
                        columnWidth: 1
                    }, {
                        margin: '0 0 0 5',
                        xtype: 'textfield',
                        name: 'x_Phone',
                        fieldLabel: 'Phone',
                        columnWidth: 0.5
                    }, {
                        margin: '0 0 0 5',
                        xtype: 'textfield',
                        name: 'x_Email',
                        fieldLabel: 'Email',
                        columnWidth: 0.5
                    }, {
                        xtype: 'textfield',
                        name: 'x_ContactName',
                        fieldLabel: '- Contact',
                        columnWidth: 0.5
                    }, {
                        margin: '0 0 0 5',
                        xtype: 'textfield',
                        name: 'x_Fax',
                        fieldLabel: 'Fax',
                        columnWidth: 0.5
                    }]
                },
                // Shipping Info
                {
                    margin: '0 0 0 5',
                    xtype: 'fieldset',
                    columnWidth: 0.5,
                    fieldDefaults: {
                        labelWidth: 50,
                        labelAlign: 'top'
                    },
                    padding: '0 10 10 10',
                    layout: 'column',
                    collapsible: true,
                    title: 'Shipping Information',
                    items: [
                    {
                        xtype: 'datefield',
                        name: 'JobShipDate',
                        fieldLabel: 'Ship Date',
                        columnWidth: 0.2
                    }, /*{
                        margin: '0 0 0 5',
                        xtype: 'textfield',
                        name: 'x_JobShipmentCarrierText',
                        fieldLabel: 'Carrier',
                        columnWidth: 0.4
                    },*/ {
                        margin: '0 0 0 5',
                        columnWidth: 0.4,
                        xtype: 'combo',
                        fieldLabel: 'Carrier',
                        displayField: 'ListText',
                        valueField: 'ListKey',
                        name: 'JobShipmentCarrier',
                        queryMode: 'local',
                        minChars: 2,
                        allowBlank: false,
                        forceSelection: true,
                        emptyText: 'Choose Location',
                        autoSelect: false,
                        anyMatch: true,
                        selectOnFocus: true
                    }, {
                        margin: '0 0 0 5',
                        xtype: 'textfield',
                        name: 'JobCarrierVessel',
                        fieldLabel: 'Vessel',
                        columnWidth: 0.4
                    }, {
                        xtype: 'datefield',
                        name: 'JobArrivalDate',
                        fieldLabel: 'Est Arrival Date',
                        columnWidth: 0.2
                    }, {
                        margin: '0 0 0 5',
                        xtype: 'textfield',
                        name: 'JobCarrierRefNum',
                        fieldLabel: 'Ref (AWB / BL)',
                        columnWidth: 0.4
                    }, {
                        margin: '0 0 0 5',
                        xtype: 'textfield',
                        name: 'JobInspectionCertificateNum',
                        fieldLabel: 'Certificated',
                        columnWidth: 0.4
                    }, {
                        margin: '15 0 6 0',
                        xtype: 'container',
                        layout: 'hbox',
                        columnWidth: 1,
                        items: [{
                            xtype: 'component',
                            flex: 1
                        }, {
                            xtype: 'button',
                            text: 'Save Changes',
                            handler: me.onShippingInformationChange
                        }]
                    }]
                },
                // footer
                {
                    columnWidth: 1,
                    xtype: 'toolbar',
                    dock: 'bottom',
                    ui: 'footer',
                    items: [{
                        xtype: 'textfield',
                        name: 'JobCreatedBy',
                        readOnly: true,
                        editable: false,
                        fieldLabel: 'Created By',
                        labelAlign: 'top'
                    }, {
                        xtype: 'datetimefield',
                        name: 'JobCreatedDate',
                        readOnly: true,
                        editable: false,
                        fieldLabel: 'Created Date',
                        hideTrigger: true,
                        labelAlign: 'top'
                    }, {
                        xtype: 'textfield',
                        name: 'JobModifiedBy',
                        readOnly: true,
                        editable: false,
                        fieldLabel: 'Modified By',
                        labelAlign: 'top'
                    }, {
                        xtype: 'datetimefield',
                        name: 'JobModifiedDate',
                        readOnly: true,
                        editable: false,
                        fieldLabel: 'Modified Date',
                        hideTrigger: true,
                        labelAlign: 'top'
                    }, {
                        xtype: 'component',
                        flex: 1
                    }]
                }
            ]
        });

        me.callParent(arguments);
    },

    onRenderForm: function() {
        var me = this;

        me.down('#btnEditQuote').setDisabled(me.currentRecord.get('JobQHdrKey') === null);
    },

    onClickLineDetails: function() {
        var tabs = this.up('app_pageframe');

        var existsTab = Ext.getCmp('lineentry');

        if (existsTab) {
            tabs.remove(existsTab, true);
        }

        var jobdetails;

        var tab = tabs.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Line Entry',
            id: 'lineentry',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [
                    jobdetails = Ext.widget("joblineentry")
                ]
            }]
        });

        tabs.setActiveTab(tab.getId());
    },

    onClickOrderDetails: function() {
        var me = this,
            tabs = Ext.getCmp('cbh_pageframe'),
            grid = me.down('#gridvendors');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        var selection = grid.getSelectionModel().getSelection()[0];
        var jobKey = selection.get('QuoteJobKey');
        var jobNum = me.JobNum;
        var customer = Ext.getCmp('joboverviewcustname').getValue();

        var form = Ext.widget('joborderentry', {
            JobKey: jobKey,
            JobNum: jobNum,
            Customer: customer
        });

        var tab = tabs.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Order Entry',
            items: [form],
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

    onClickEditSupplierQuotes: function() {
        var me = this,
            tabs = Ext.getCmp('cbh_pageframe'),
            grid = me.down('#gridvendors');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var jobnum = me.JobNum;
        var customer = me.down("field[name=CustName]").getValue();
        var curJobKey = selection.get('QuoteJobKey');

        var storeToNavigate = new CBH.store.jobs.JobQuoteVendorInfo().load({
            params: {
                jobkey: curJobKey,
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function(records, operation, success) {

                var form = Ext.widget('jobquoteentry', {
                    JobKey: curJobKey,
                    Customer: customer,
                    JobNum: jobnum,
                    storeNavigator: storeToNavigate
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Quote Entry',
                    items: [{
                        xtype: 'container',
                        layout: {
                            type: 'anchor'
                        },
                        items: [form]
                    }]
                });

                form.down('#FormToolbar').gotoAt(1);
                tab.show();
            },
            scope: this
        });
    },

    onClickJobNewInvoice: function() {
        var me = this;

        Ext.Msg.show({
            title: 'Create New Invoice?',
            msg: 'Are you sure you want to create a new invoice for this job?',
            buttons: Ext.Msg.YESNO,
            fn: me.onCreateNewInvoice,
            scope: me,
            icon: Ext.Msg.QUESTION
        });
    },

    onCreateNewInvoice: function(buttonId, text, opt) {

        if (buttonId !== 'yes') return;

        var me = this,
            cr = me.currentRecord.data,
            grid = me.down('#gridvendors'),
            selection = null;

        if(grid.getStore().getCount() > 0 && grid.getSelectionModel().selected.length == 0) {
            grid.getSelectionModel().select(0);
            selection = grid.getSelectionModel().getSelection()[0];
        }

        if(!selection) return;

        var curJobKey = selection.get('JobKey');

        var newCommissionInvoice = Ext.create('CBH.model.jobs.NewCommissionInvoice', {
            CommissionPCT: 0,
            CommissionTotal: 0,
            CommissionTotalCurrencyCode: cr.CustCurrencyCode,
            CommissionTotalCurrencyRate: cr.CustCurrencyRate,
            TotalSale: 0,
            TotalSaleCurrencyCode: cr.CustCurrencyCode,
            TotalSaleCurrencyRate: cr.CustCurrencyRate,
            VendorContactKey: null,
            VendorKey: null
        });
        form = Ext.widget('jobnewinvoice', {
            JobKey: curJobKey,
            callerForm: me,
            commissionModel: newCommissionInvoice
        });
        form.loadRecord(newCommissionInvoice);
        form.center();
        form.show();
    },

    onClickVendorSelection: function() {
        var me = this,
            grid = me.down('#gridInvoices'),
            modelJob = Ext.create('CBH.model.jobs.JobOverview', {
                JobKey: me.modelJobOverView.data.JobKey
            });

        var selected = null;

        if (grid.store.getCount() === 0) return;


        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        record = grid.getSelectionModel().getSelection()[0].data;

        var storeJobQuoteVendorInfo = new CBH.store.jobs.JobQuoteVendorInfo().load({
            params: {
                jobKey: record.QHdrJobKey
            },
            callback: function() {
                var storeVendors = modelJob.Vendors().load({
                    callback: function() {
                        var form = Ext.widget('jobquotevendorselection', {
                            storeVendors: storeVendors,
                            modal: true,
                            frameHeader: true,
                            header: true,
                            title: 'Vendor Selection for Quote: ' + record.Quote,
                            bodyPadding: 5,
                            closable: true,
                            stateful: false,
                            floating: true,
                            callerForm: me,
                            forceFit: true,
                            selectedQuote: record,
                            storeJobQuoteVendorInfo: storeJobQuoteVendorInfo
                        });

                        form.show();
                    }
                });
            }
        });
    },

    setDisabledQuoteButtons: function(disabled) {
        var me = this;
        me.down('#VendorSelectionBtn').setDisabled(disabled);
        me.down('#EditCustomerQuoteBtn').setDisabled(disabled);
        me.down('#CBHOrderingFormBtn').setDisabled(disabled);
        me.down('#PrintInternalQuoteBtn').setDisabled(disabled);
        me.down('#PrintCustomerQuoteBtn').setDisabled(disabled);
    },

    onClickPrintCustomerQuoteBtn: function() {
        var me = this,
            grid = me.down('#gridInvoices');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var record = selection;

        var userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey();
        var formData = {
            id: record.get('QHdrKey'),
            employeeKey: userKey
        };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptQuoteCustomer", params);
        window.open(pathReport, 'CBH - Quote Customer', false);
    },

    onClickEditPurchaseOrder: function() {
        var me = this,
            tabs = me.up('app_pageframe'),
            grid = me.down('#gridvendors');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var jobnum = me.JobNum,
            curJobKey = me.currentRecord.data.JobKey,
            POKey = selection.data.POKey;

        var storeToNavigate = new CBH.store.jobs.JobPurchaseOrders().load({
            params: {
                id: POKey
            },
            callback: function(records, operation, success) {

                var form = Ext.widget('jobpurchaseordermaintenance', {
                    JobKey: curJobKey,
                    JobNum: jobnum,
                    storeNavigator: storeToNavigate,
                    currentJob: me.currentJob
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
                    title: 'Purchase Order Maintenance',
                    padding: '0 5 0 5',
                    items: [form]
                });

                form.down('#FormToolbar').gotoAt(1);
                tab.show();
            },
            scope: this
        });
    },

    onClickEditInvoice: function() {
        var me = this,
            tabs = me.up('viewport').down('app_pageframe'),
            grid = me.down('#gridInvoices');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var jobnum = me.JobNum,
            curJobKey = me.currentRecord.data.JobKey,
            InvoiceKey = selection.data.InvoiceKey;

        var storeToNavigate = new CBH.store.jobs.qfrmInvoiceMaintenance().load({
            params: {
                id: InvoiceKey
            },
            callback: function(records, operation, success) {

                var form = Ext.widget('invoicemaintenance', {
                    JobKey: curJobKey,
                    JobNum: jobnum,
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
                    title: 'Invoice Maintenance',
                    padding: '0 5 0 5',
                    items: [form]
                });

                form.down('#FormToolbar').gotoAt(1);
                form.down('#FormToolbar').on('aftersavechanges', me.refreshOverview, me);
                tab.show();
            },
            scope: this
        });
    },

    onClickJobStatusHistory: function() {
        var me = this;

        var tabs = this.up('app_pageframe');

        var grid = me.down('#gridjobs');

        var selection = me.currentRecord;

        var JobKey = selection.data.JobKey;
        var JobNum = selection.data.JobNum;
        var customer = selection.data.CustName;

        CBH.AppEvents.on("jobclosed", function() {
            me.refreshOverview(me, me.currentRecord);
        });


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
                        JobStatus: null
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

    onClickEditStatusHistory: function() {
        var me = this.up("form");

        var tabs = this.up('app_pageframe');

        var grid = me.down('#gridvendors');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        var selection = grid.getSelectionModel().getSelection()[0];
        var POKey = selection.data.POKey;
        var JobKey = selection.data.JobKey;
        var JobNum = me.down('field[name=JobNum]').getValue();
        var customer = me.down('field[name=CustName]').getValue();
        var vendor = selection.data.Vendor;
        var PONum = selection.data.PONum;

        var storeqfrmJobPurchaseOrderStatusHistory = new CBH.store.jobs.qfrmJobPurchaseOrderStatusHistory().load({
            params: {
                id: POKey
            },
            callback: function(records, success, eOpts) {
                
                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Job Purchase Order Status History',
                    items: [{
                        xtype: 'container',
                        layout: {
                            type: 'anchor'
                        },
                        items: [
                            Ext.widget('JobPurchaseOrderStatusHistoryList', {
                                JobKey: JobKey,
                                JobNum: JobNum,
                                Customer: customer,
                                Vendor: vendor,
                                JobStatus: null,
                                POKey: POKey,
                                PONum: PONum,
                                POShipETA: records[0].data.POShipETA,
                                currentRecord: records[0]
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
            }
        });
    },

    onClickNewPurchaseOrder: function() {
        var me = this,
            tabs = me.up('app_pageframe');

        selection = me.currentJob;

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
                    title: 'New Purchase Order',
                    padding: '0 5 0 5',
                    items: [form]
                });

                /*if(records && records.length > 0) {
                    form.down('#FormToolbar').gotoAt(1);
                    tab.show();
                } else {*/
                var model = new CBH.model.jobs.JobPurchaseOrders({
                    POJobKey: curJobKey,
                    POCurrencyCode: selection.data.CustCurrencyCode,
                    POCurrencyRate: selection.data.CustCurrencyRate,
                    POWarehouseKey: (!selection.data.JobWarehouseKey) ? null : selection.data.JobWarehouseKey,
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

    onClickExportInvToPeachtree: function() {
        var me = this.up('form'),
            today = new Date(),
            t = Ext.util.Cookies.get('CBH.UserAuth'),
            grid = me.down('#gridInvoices');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var invoiceKey = selection.data.InvoiceKey;

        var url = "{0}/ExportInvoiceToPeachTree?_dc={1}&InvoiceKey={2}&t={3}".format(CBH.GlobalSettings.webApiPath, today.getTime(), invoiceKey, t);
        window.open(url, '_blank');
    },

    onClickExportPOToPeachtree: function() {
        var me = this.up('form'),
            today = new Date(),
            t = Ext.util.Cookies.get('CBH.UserAuth'),
            grid = me.down('#gridvendors');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var POKey = selection.data.POKey;

        var url = "{0}/ExportPurchaseOrderToPeachTree?_dc={1}&POKey={2}&t={3}".format(CBH.GlobalSettings.webApiPath, today.getTime(), POKey, t);
        window.open(url, '_blank');
    },

    onClickEditCustomerQuote: function() {
        var me = this,
            tabs = me.up('app_pageframe');

        selection = me.currentJob;

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

    onClickJobInformation: function() {
        var me = this;

        var tabs = this.up('app_pageframe');

        curJob = me.currentRecord;

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
                    currentJob: me.currentJob
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Job: ' + curJob.data.JobNum,
                    items: [form]
                });

                form.down('#FormToolbar').gotoAt(1);

                tab.show();
            }
        });
    },

    onClickUpdateCurrencyRates: function() {
        var me = this,
            JobKey = me.currentRecord.data.JobKey;

        me.getEl().mask('Updating....');
        Ext.Ajax.request({
            url: CBH.GlobalSettings.webApiPath + '/api/UpdateJobCurrencyMaster',
            method: 'GET',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                JobKey: JobKey
            },
            success: function(response) {

                var store = new CBH.store.jobs.qfrmJobOverviewPopupUpdateCurrency().load({
                    scope: me,
                    params: {
                        JobKey: JobKey
                    },
                    callback: function(records, operation, success) {
                        me.getEl().unmask();

                        if(!records[0]) return;
                        // Open Job Overview Update Currency
                        var form = new CBH.view.jobs.JobOverviewPopupUpdateCurrency({
                            JobKey: me.currentRecord.get('JobKey'),
                            currentJob: me.currentRecord
                        });
                        form.modal = true;
                        form.loadRecord(records[0]);
                        form.callerForm = me;
                        form.show();        
                    }
                });
            }
        });
    },

    refreshData: function() {
        var me = this;
        me.down("#gridInvoices").getStore().reload();
        me.down("#gridvendors").getStore().reload();
        me.down("#gridstatus").getStore().reload();
    },

    onClickPrintOrderingForm: function() {
        var me = this,
            userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey(),
            tabs = me.up('app_pageframe');

        var jobkey = me.currentRecord.get('JobKey');

        var formData = {
                id: jobkey,
                employeeKey: userKey
            };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptQuoteOrderingForm", params);
        window.open(pathReport, 'CBH - File Status History', false);
    },

    onClickPackingList: function() {
        var me = this,
            userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey();

        var today = new Date(),
            grid = me.down('#gridInvoices');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var invoiceKey = selection.data.InvoiceKey;

        var formData = {
                id: invoiceKey,
                employeeKey: userKey
            };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptJobInvoicePackingList", params);
        window.open(pathReport, 'CBH - Packing List', false);
    },

    onClickPrintPurchaseOrder: function() {
        var me = this,
            userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey(),
            tabs = me.up('app_pageframe'),
            grid = me.down('#gridvendors');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        var selection = grid.getSelectionModel().getSelection()[0],
            POKey = selection.data.POKey;

        var formData = {
                id: POKey,
                employeeKey: userKey
            };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptJobPurchaseOrder", params);
        window.open(pathReport, 'CBH - Purchase Order', false);
    },

    onClickPrintInvoice: function() {
        var me = this,
            userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey();

        var today = new Date(),
            grid = me.down('#gridInvoices');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var invoiceKey = selection.data.InvoiceKey;

        var formData = {
                id: invoiceKey,
                employeeKey: userKey
            };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptJobInvoice", params);
        window.open(pathReport, 'CBH - Invoice', false);
    },

    refreshOverview: function(toolbar, record) {
        var me = this;
        me.setLoading("Loading...");
        var storeJobOverview = new CBH.store.jobs.qJobOverview().load({
            params: {
                id: me.currentJob.get("JobKey")
            },
            callback: function(records, operation, success) {
                
                if(records && records.length)
                    me.loadRecord(this.getAt(0));

                me.setLoading(false);
            }
        });
    },

    onShippingInformationChange: function(component, e) {
        var me = component.up("form");
        me.saveChanges();
    },

    saveChanges: function() {
        var me = this,
            form = me.getForm();

        if (!form.isValid()) {
            Ext.Msg.alert("Validation", "Check data for valid input!!!");
            return false;
        }

        form.updateRecord();

        var savedRecord = form.getRecord();

       Ext.getBody().mask("Saving Changes...")

        savedRecord.save({
            callback: function(records, operation, success) {
                Ext.getBody().unmask();
            }
        });
    }
});