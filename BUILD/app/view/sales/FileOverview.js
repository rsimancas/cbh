Ext.define('CBH.view.sales.FileOverview', {
    extend: 'Ext.form.Panel',
    alias: 'widget.fileoverview',
    layout: {
        type: 'column'
    },
    padding: '5 10 10 10',
    FileKey: 0,
    modelFileOverView: null,
    FileNum: null,
    CustKey: 0,

    initComponent: function() {

        var me = this;

        storeVendorSummary = me.modelFileOverView.Vendors().load({
            callback: function() {
                var grid = this.down('#gridvendors');

                if (grid.getSelectionModel().selected.length === 0) {
                    grid.getSelectionModel().select(0);
                }
            },
            scope: me
        });

        storeQuoteSummary = me.modelFileOverView.Quotes().load({
            callback: function() {
                var grid = this.down('#gridquotes');

                if (grid.getSelectionModel().selected.length === 0) {
                    grid.getSelectionModel().select(0);
                }

                if (grid.store.getCount() === 0) {
                    me.setDisabledQuoteButtons(true);
                }
            },
            scope: me
        });

        var storeFileEmployeeRoles = new CBH.store.sales.FileEmployeeRoles().load({
            params: {
                filekey: me.FileKey
            },
            callback: function() {
                var grid = me.down('#gridroles');
                grid.reconfigure(this);
                //Ext.Msg.hide();
                storeFileEmployeeRoles.lastOptions.callback = null;
            }
        });

        var storeStatus = new CBH.store.sales.FileStatusHistorySubDetails({
            autoLoad: false
        }).load({
            params: {
                page: 0,
                start: 0,
                limit: 0,
                filekey: me.FileKey
            }
        });

        Ext.applyIf(me, {
            items: [
                // header form
                {
                    xtype: 'fieldset',
                    padding: '5',
                    margin: '0 10 5 0',
                    columnWidth: 1,
                    layout: {
                        type: 'hbox'
                    },
                    items: [{
                        xtype: 'displayfield',
                        name: 'CustPeachtreeID',
                        flex: 1,
                        hideTrigger: true,
                        fieldLabel: 'Customer Code',
                        editable: false,
                        labelWidth: 80,
                        //labelStyle: 'text-align: right',
                        fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: left'
                    }, {
                        margin: '0 0 0 10',
                        xtype: 'displayfield',
                        width: '20%',
                        labelWidth: 80,
                        name: 'x_FileNum',
                        fieldLabel: 'Reference Num',
                        editable: false,
                        labelStyle: 'text-align: right',
                        fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: right'
                    }]
                },
                // Panel Left GRIDS
                {
                    xtype: 'container',
                    columnWidth: 0.7,
                    layout: {
                        type: 'column'
                    },
                    items: [
                        // Vendors Grid
                        {
                            xtype: 'gridpanel',
                            itemId: 'gridvendors',
                            title: 'Vendors',
                            columnWidth: 0.75,
                            height: 150,
                            margin: '0 5 5 0',
                            store: storeVendorSummary,
                            columns: [{
                                xtype: 'rownumberer'
                            }, {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'Vendor',
                                text: 'Vendor'
                            }, {
                                xtype: 'numbercolumn',
                                width: 80,
                                dataIndex: 'Qty',
                                text: 'Qty',
                                format: '00,000.00',
                                align: 'right'
                            }, {
                                xtype: 'numbercolumn',
                                width: 80,
                                dataIndex: 'Cost',
                                text: 'Cost',
                                format: '00,000.00',
                                align: 'right'
                            }, {
                                xtype: 'numbercolumn',
                                width: 80,
                                dataIndex: 'Price',
                                text: 'Price',
                                format: '00,000.00',
                                align: 'right'
                            }, {
                                xtype: 'gridcolumn',
                                dataIndex: 'Currency',
                                text: 'Currency'
                            }],
                            // Grid vendors Listeners
                            listeners: {
                                selectionchange: {
                                    fn: me.onGridVendorSelectChange,
                                    scope: me
                                }
                            }
                        },
                        // File Buttons
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
                                text: 'Edit Supplier Quotes',
                                listeners: {
                                    click: {
                                        fn: me.onClickEditSupplierQuotes,
                                        scope: me
                                    }
                                }
                            }]
                        },
                        // Quotes grid
                        {
                            margin: '5 0 0 0',
                            xtype: 'gridpanel',
                            itemId: 'gridquotes',
                            store: storeQuoteSummary,
                            title: 'Quotes',
                            columnWidth: 0.75,
                            minHeight: 150,
                            //margin: '0 5 10 0',
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
                                dataIndex: 'Quote',
                                text: 'Quote'
                            }, {
                                xtype: 'gridcolumn',
                                width: 80,
                                dataIndex: 'Vendors',
                                text: 'Vendors'
                            }, {
                                xtype: 'gridcolumn',
                                dataIndex: 'Status',
                                text: 'Status',
                                flex: 1
                            }],
                            listeners: {
                                celldblclick: {
                                    fn: me.onClickEditCustomerQuote,
                                    scope: me
                                },
                                selectionchange: {
                                    fn: me.onGridQuotesSelectChange,
                                    scope: me
                                }
                            }
                        },
                        // Quotes Buttons
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
                                text: 'Create New Cust Quote',
                                listeners: {
                                    click: {
                                        fn: me.onClickNewQuote,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                itemId: 'VendorSelectionBtn',
                                flex: 1,
                                text: 'Edit Vendor Selection',
                                listeners: {
                                    click: {
                                        fn: me.onClickVendorSelection,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                itemId: 'EditCustomerQuoteBtn',
                                flex: 1,
                                text: 'Edit Customer Quote',
                                listeners: {
                                    click: {
                                        fn: me.onClickEditCustomerQuote,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                itemId: 'PrintCustomerQuoteBtn',
                                flex: 1,
                                text: 'Print Customer Quote',
                                listeners: {
                                    click: {
                                        fn: me.onClickPrintCustomerQuoteBtn,
                                        scope: me
                                    }
                                }
                            }, {
                                xtype: 'button',
                                itemId: 'CBHOrderingFormBtn',
                                flex: 1,
                                text: 'CBH Ordering Form',
                                disabled: true
                            }, {
                                xtype: 'button',
                                itemId: 'PrintInternalQuoteBtn',
                                flex: 1,
                                text: 'Print Internal Quote',
                                disabled: true
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
                    ]
                },
                // Panel Right
                {
                    margin: '0 0 0 5',
                    xtype: 'container',
                    columnWidth: 0.3,
                    layout: {
                        type: 'column'
                    },
                    items: [
                        // Buttons Right
                        {
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
                                text: 'Edit File/Customer Info',
                                handler: function() {
                                    var me = this.up("form");
                                    me.onClickCustomerInformation();
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
                                flex: 1,
                                text: 'Edit Status History',
                                handler: function() {
                                    var me = this.up("form");
                                    me.onClickStatusHistory();
                                }
                            }, {
                                xtype: 'button',
                                flex: 1,
                                text: 'Print Status History',
                                handler: function() {
                                    var me = this.up('form');
                                    me.onClickPrintStatusHistory();
                                }
                            }]
                        },
                        // Customer Information
                        {
                            xtype: 'fieldset',
                            columnWidth: 1,
                            margin: '0 0 10 5',
                            padding: '0 5 5 5',
                            layout: {
                                type: 'column'
                            },
                            fieldDefaults: {
                                labelAlign: 'top',
                                labelWidth: 60,
                                msgTarget: 'side',
                                fieldStyle: 'font-size:11px',
                                labelStyle: 'font-size:11px'
                            },
                            defaults: {
                                columnWidth: 1
                            },
                            collapsible: false,
                            title: 'Customer Info',
                            items: [{
                                xtype: 'textfield',
                                name: 'CustName',
                                readOnly: true,
                                fieldLabel: 'Customer'
                            }, {
                                xtype: 'textfield',
                                name: 'ContactFullName',
                                readOnly: true,
                                fieldLabel: '- Contact'
                            }, {
                                xtype: 'textfield',
                                name: 'Phone',
                                columnWidth: 0.5,
                                readOnly: true,
                                fieldLabel: 'Phone'
                            }, {
                                margin: '0 0 0 5',
                                xtype: 'textfield',
                                name: 'Fax',
                                columnWidth: 0.5,
                                readOnly: true,
                                fieldLabel: 'Fax'
                            }, {
                                xtype: 'textfield',
                                name: 'Email',
                                columnWidth: 1,
                                readOnly: true,
                                fieldLabel: 'Email'
                            }, {
                                //margin: '0 0 0 5',
                                xtype: 'textfield',
                                name: 'FileReference',
                                columnWidth: 1,
                                readOnly: true,
                                fieldLabel: 'File Reference'
                            }]
                        },
                        // Employee Roles
                        {
                            margin: '10 0 5 5',
                            columnWidth: 1,
                            xtype: 'gridpanel',
                            itemId: 'gridroles',
                            title: 'CBH Contacts',
                            store: storeFileEmployeeRoles,
                            minHeight: 140,
                            height: 140,
                            hideHeaders: true,
                            columns: [{
                                xtype: 'gridcolumn',
                                dataIndex: 'x_RoleName',
                                //text: 'Job Role',
                                flex: 1
                            }, {
                                xtype: 'gridcolumn',
                                flex: 1,
                                dataIndex: 'x_EmployeeName',
                                //text: 'Employee'
                            }]
                        }
                    ]
                },
                // footer
                {
                    columnWidth: 1,
                    xtype: 'toolbar',
                    dock: 'bottom',
                    ui: 'footer',
                    items: [{
                        xtype: 'textfield',
                        name: 'FileCreatedBy',
                        readOnly: true,
                        editable: false,
                        fieldLabel: 'Created By',
                        labelAlign: 'top'
                    }, {
                        xtype: 'datetimefield',
                        name: 'FileCreatedDate',
                        readOnly: true,
                        editable: false,
                        fieldLabel: 'Created Date',
                        hideTrigger: true,
                        labelAlign: 'top'
                    }, {
                        xtype: 'textfield',
                        name: 'FileModifiedBy',
                        readOnly: true,
                        editable: false,
                        fieldLabel: 'Modified By',
                        labelAlign: 'top'
                    }, {
                        xtype: 'datetimefield',
                        name: 'FileModifiedDate',
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

    refreshData: function() {
        var me = this;
        me.down("#gridvendors").getStore().reload();
        me.down("#gridquotes").getStore().reload();
        me.down("#gridroles").getStore().reload();
        me.down("#gridstatus").getStore().reload();
    },

    onClickLineDetails: function() {
        var tabs = this.up('app_pageframe');

        var existsTab = Ext.getCmp('lineentry');

        if (existsTab) {
            tabs.remove(existsTab, true);
        }

        var filedetails;

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
                items: [
                    filedetails = Ext.widget("filelineentry")
                ]
            }]
        });

        tabs.setActiveTab(tab.getId());
    },

    onClickOrderDetails: function() {
        var me = this,
            tabs = me.up('viewport').down('app_pageframe'),
            grid = me.down('#gridvendors');

        var fileKey = me.modelFileOverView.data.FileKey;
        var fileNum = me.FileNum;
        var customer = me.down("field[name=CustName]").getValue();
        var custkey = me.modelFileOverView.data.FileCustKey;

        var form = Ext.widget('fileorderentry', {
            FileKey: fileKey,
            FileNum: fileNum,
            Customer: customer,
            CustKey: custkey
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
            tabs = me.up('viewport').down('app_pageframe'),
            grid = me.down('#gridvendors');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var filenum = me.FileNum;
        var customer = me.down("field[name=CustName]").getValue();
        var curFileKey = selection.get('QuoteFileKey');

        var storeToNavigate = new CBH.store.sales.FileQuoteVendorInfo().load({
            params: {
                filekey: selection.get("QuoteFileKey"),
                /*vendorkey: selection.get("VendorKey"),*/
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function(records, operation, success) {
                var index = this.find("FVVendorKey", selection.get("VendorKey"));

                var form = Ext.widget('filequoteentry', {
                    FileKey: curFileKey,
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

                index = (index === -1) ? 1 : index + 1;
                form.down('#FormToolbar').gotoAt(index);
                tab.show();
            }
        });
    },

    onClickNewQuote: function() {
        var me = this;

        Ext.Msg.show({
            title: 'Create New Quote?',
            msg: 'Are you sure you want to create a new quote for this file?',
            buttons: Ext.Msg.YESNO,
            fn: me.onCreateNewQuote,
            scope: me,
            icon: Ext.Msg.QUESTION
        });
    },

    onCreateNewQuote: function(buttonId, text, opt) {

        if (buttonId !== 'yes') return;

        var me = this,
            curFile = me.modelFileOverView.data,
            quote = Ext.create('CBH.model.sales.FileQuoteHeader', {
                QHdrFileKey: curFile.FileKey,
                QHdrCurrencyCode: curFile.FileDefaultCurrencyCode,
                QHdrCurrencyRate: curFile.FileDefaultCurrencyRate,
                QHdrDate: new Date()
            });

        var gridquotes = me.down('#gridquotes');

        Ext.Msg.wait('Creating Quote', 'Wait');

        quote.save({
            callback: function(record) {
                var params = gridquotes.store.lastOptions.params,
                    lastParams = (params) ? params : null;

                me.down('#gridquotes').store.reload({
                    params: lastParams,
                    callback: function() {
                        Ext.Msg.show({
                            title: 'New Quote Created',
                            msg: 'Quote ' + record.data.QHdrPrefix + Ext.String.leftPad(record.data.QHdrNum, 4, '0') + ' has been successfully created.',
                            buttons: Ext.Msg.OK,
                            icon: Ext.Msg.Info
                        });
                        Ext.Msg.hide();
                    }
                });
            }
        });
    },

    onClickVendorSelection: function() {
        var me = this,
            grid = me.down('#gridquotes'),
            modelFile = Ext.create('CBH.model.sales.FileOverview', {
                FileKey: me.modelFileOverView.data.FileKey
            });

        var selected = null;

        if (grid.store.getCount() === 0) return;


        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        record = grid.getSelectionModel().getSelection()[0].data;

        var storeFileQuoteVendorInfo = new CBH.store.sales.FileQuoteVendorInfo().load({
            params: {
                fileKey: record.QHdrFileKey
            },
            callback: function() {
                var storeVendors = modelFile.Vendors().load({
                    callback: function() {
                        var form = Ext.widget('filequotevendorselection', {
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
                            storeFileQuoteVendorInfo: storeFileQuoteVendorInfo
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
        //me.down('#PrintInternalQuoteBtn').setDisabled(disabled);
        //me.down('#PrintCustomerQuoteBtn').setDisabled(disabled);
    },

    onClickEditCustomerQuote: function() {
        var me = this,
            tabs = me.up('app_pageframe'),
            grid = me.down('#gridquotes');

        if (grid.getSelectionModel().selected.length === 0) {
            grid.getSelectionModel().select(0);
        }

        if (!grid.getSelectionModel().getSelection()[0]) {
            return;
        }

        selection = grid.getSelectionModel().getSelection()[0];

        var filenum = me.FileNum,
            customer = me.down("field[name=CustName]").getValue(),
            curFileKey = selection.get('QHdrFileKey'),
            quote = selection.get('Quote'),
            quotekey = selection.get('QHdrKey');

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

    onClickPrintCustomerQuoteBtn: function() {
        var me = this,
            grid = me.down('#gridquotes');

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

    onClickCustomerInformation: function() {
        var me = this;

        var tabs = this.up('app_pageframe');

        var storeToNavigate = new CBH.store.sales.FileHeader().load({
            params: {
                id: me.FileKey,
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

    onClickUpdateCurrencyRates: function() {
        var me = this,
            grid = me.down("gridpanel");

        me.getEl().mask('Please wait...');

        Ext.Ajax.request({
            method: 'GET',
            type: 'json',
            url: CBH.GlobalSettings.webApiPath + '/api/UpdateFileQuoteCurrencyRates',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                FileKey: me.FileKey,
                CurrentUser: CBH.GlobalSettings.getCurrentUser().UserName
            },
            success: function(rsp) {
                var data = Ext.JSON.decode(rsp.responseText);
                me.getEl().unmask();
                grid.store.reload();
                Ext.Msg.alert('Status', 'Updated successfully.');
            },
            failure: function(response, opts) {
                console.info('server-side failure with status code ' + response.status);
                me.getEl().unmask();
            }
        });
    },

    onClickStatusHistory: function() {
        var me = this;

        var tabs = this.up('app_pageframe');

        var fileKey = me.FileKey;
        var fileNum = me.FileNum;
        var status = me.FileStatus;
        var customer = me.Customer;

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
                        FileStatus: status
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

    onGridQuotesSelectChange: function(model, record) {
        var me = this;
        me.down('#EditCustomerQuoteBtn').setDisabled(false);
        me.down('#VendorSelectionBtn').setDisabled(false);
        me.down('#PrintCustomerQuoteBtn').setDisabled(false);
        //me.down('#CBHOrderingFormBtn').setDisabled(false);
        //me.down('#PrintInternalQuoteBtn').setDisabled(false);
    },

    onGridVendorSelectChange: function(model, record) {
        
    },

    onClickPrintStatusHistory: function() {
        var me = this,
            userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey(),
            formData = {
                id: me.FileKey,
                employeeKey: userKey
            };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptFileStatusHistory", params);
        window.open(pathReport, 'CBH - File Status History', false);
    }
});