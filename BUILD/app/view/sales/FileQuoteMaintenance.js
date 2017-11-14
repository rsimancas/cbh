Ext.define('CBH.view.sales.FileQuoteMaintenance', {
    extend: 'Ext.form.Panel',
    alias: 'widget.filequotemaintenance',
    layout: {
        type: 'column'
    },
    bodyPadding: 5,
    FileKey: 0,
    frameHeader: false,
    header: false,
    enableKeyEvents: true,
    storeNavigator: null,
    Customer: null,
    FileNum: null,
    QuoteNum: null,

    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;

        var me = this;

        var rowEditingCharges = new Ext.grid.plugin.RowEditing({
            clicksToMoveEditor: 2,
            autoCancel: false,
            errorSummary: false,
            listeners: {
                beforeedit: {
                    delay: 100,
                    fn: function(item, context) {
                        var me = this.getEditor().up('form'),
                            formEditing = this.getEditor(),
                            toolbar = me.down('#FormToolbar'),
                            masterRecord = toolbar.getCurrentRecord();

                        var record = context.record;

                        formEditing.onFieldChange();

                        Ext.Msg.wait('Loading', 'Loading Data...');

                        var storeFreightCompanies = new CBH.store.vendors.Vendors().load({
                            params: {
                                id: (record.data.QChargeFreightCompany) ? record.data.QChargeFreightCompany : 0
                            },
                            callback: function(records, operation, success) {
                                var combo = formEditing.down("field[name=QChargeFreightCompany]");
                                    combo.bindStore(storeFreightCompanies);

                                if(records[0]) {
                                    combo.setValue(record.data.QChargeFreightCompany);
                                }

                                Ext.Msg.hide();
                            }
                        });
                    }
                },
                cancelEdit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('gridpanel');
                        // Canceling editing of a locally added, unsaved record: remove it
                        if (context.record.phantom) {
                            grid.store.remove(context.record);
                        }
                    }
                },
                edit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('gridpanel'),
                            record = context.record,
                            fromEdit = true,
                            isPhantom = record.phantom,
                            fieldCurrency = this.editor.down("field[name=QChargeCurrencyCode]");

                        var index = fieldCurrency.store.find("CurrencyCode", fieldCurrency.getValue()),
                            currency = fieldCurrency.store.getAt(index);

                        record.set("QChargeCurrencyRate", currency.data.CurrencyRate);

                        record.save({
                            callback: function() {
                                grid.store.reload();
                            }
                        });
                    }
                }
            }
        });

        var rowEditingChargesST = new Ext.grid.plugin.RowEditing({
            clicksToMoveEditor: 2,
            autoCancel: false,
            errorSummary: false,
            listeners: {
                beforeedit: {
                    delay: 100,
                    fn: function(item, context) {
                        var me = this.getEditor().up('form'),
                            formEditing = this.getEditor(),
                            toolbar = me.down('#FormToolbar'),
                            masterRecord = toolbar.getCurrentRecord();

                        //if(toolbar.isEditing === false) {
                        //    return false;
                        //}

                        var record = context.record;

                        formEditing.onFieldChange();

                        var filterQHdr = new Ext.util.Filter({
                            property: 'ListCategory',
                            value: 1
                        });

                        Ext.Msg.wait('Loading', 'Loading Data...');

                        storetlkpGL = new CBH.store.sales.tlkpGenericLists().load({
                            params: {
                                page: 0,
                                start: 0,
                                limit: 0
                            },
                            filters: [filterQHdr],
                            callback: function() {
                                field = formEditing.down('field[name=QSTLocation]').bindStore(storetlkpGL);
                                field.setValue(record.data.QSTLocation);

                                /*var filterQHdr = new Ext.util.Filter({
                                    property: 'STDescriptionLanguageCode',
                                    value: masterRecord.data.x_CustLanguageCode
                                });*/

                                storetlkpCategories = new CBH.store.sales.tlkpInvoiceSubTotalCategories().load({
                                    params: {
                                        page: 0,
                                        start: 0,
                                        limit: 0
                                    },
                                    //filters: [filterQHdr],
                                    callback: function(records, operation, success) {
                                        var field = formEditing.down('field[name=QSTSubTotalKey]');
                                        field.bindStore(storetlkpCategories);
                                        field.setValue(record.data.QSTSubTotalKey);
                                        Ext.Msg.hide();
                                    }
                                });
                            }
                        });
                    }
                },
                cancelEdit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('gridpanel');
                        // Canceling editing of a locally added, unsaved record: remove it
                        if (context.record.phantom) {
                            grid.store.remove(context.record);
                        }
                    }
                },
                edit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('gridpanel'),
                            record = context.record,
                            fromEdit = true,
                            isPhantom = record.phantom;

                        record.save({
                            callback: function() {
                                grid.store.reload();
                            }
                        });
                    }
                }
            }
        });

        var storeCurRatesCharges = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        var storeCurRatesItems = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        var storeCurRatesChargesDiscount = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        var storeShipmentTypes = new CBH.store.common.ShipmentTypes().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        var storePaymentTerms = new CBH.store.common.PaymentTerms().load({
            params: {
                page: 0,
                start: 0,
                limit: 0,
                ddl: 1
            }
        });
        var storeLeadTimes = new CBH.store.sales.LeadTime().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        var storeWarehouseType = null,
            storeCharges = null,
            storeChargesST = null,
            storeItemsSummary = null,
            storeItems = null,
            storetlkpGL = null,
            storetlkpCategories = null,
            storeStatus = null;

        var storeVendors = new CBH.store.vendors.Vendors().load({
            params: {
                QuoteFileKey: me.FileKey,
                page: 0,
                start: 0,
                limit: 0
            }
        });

        var storeChargesKeys = new CBH.store.sales.ChargeCategories().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        var storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        var storeCurrencyRatesHeader = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        Ext.applyIf(me, {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items: [
                // Header Container
                {
                    xtype: 'fieldcontainer',
                    columnWidth: 1,
                    layout: {
                        type: 'hbox'
                    },
                    items: [{
                        xtype: 'textfield',
                        fieldLabel: 'Customer',
                        readOnly: true,
                        value: me.Customer,
                        flex: 8,
                        fieldStyle: 'font-size: 12px; color: #157fcc;font-weight:bold;'
                    }, {
                        xtype: 'textfield',
                        margin: '0 0 0 5',
                        fieldLabel: 'File Num',
                        //labelStyle: 'text-align: right',
                        readOnly: true,
                        value: me.FileNum,
                        flex: 1,
                        fieldStyle: 'font-size: 12px; color: #157fcc;font-weight:bold;'
                    }, {
                        xtype: 'textfield',
                        margin: '0 0 0 5',
                        //labelStyle: 'text-align: right',
                        fieldLabel: 'Quote Number',
                        readOnly: true,
                        flex: 1,
                        value: me.QuoteNum,
                        fieldStyle: 'font-size: 12px; color: #157fcc;font-weight:bold;'
                    }, {
                        xtype: 'hidden',
                        name: 'QHdrKey'
                    }]
                },
                // Tab Panel
                {
                    xtype: 'tabpanel',
                    columnWidth: 1,
                    margin: '5 0 0 0',
                    activeTab: 0,
                    items: [
                        // General Panel
                        {
                            xtype: 'panel',
                            title: 'General Information',
                            margin: '0 0 10 0',
                            layout: 'column',
                            items: [
                                // 1rst row
                                {
                                    xtype: 'container',
                                    layout: 'column',
                                    columnWidth: 1,
                                    items: [
                                        // inside left panel
                                        {
                                            xtype: 'fieldcontainer',
                                            columnWidth: 0.5,
                                            layout: 'column',
                                            items: [
                                                // General Info FieldSet
                                                {
                                                    xtype: 'fieldset',
                                                    title: 'General Info',
                                                    padding: '0 5 5 5',
                                                    columnWidth: 1,
                                                    layout: 'column',
                                                    items: [{
                                                        xtype: 'fieldcontainer',
                                                        columnWidth: 1,
                                                        layout: 'hbox',
                                                        items: [{
                                                            xtype: 'textfield',
                                                            fieldLabel: 'Revision',
                                                            name: 'QHdrRevision'
                                                        }, {
                                                            xtype: 'component',
                                                            flex: 1
                                                        }]
                                                    }, {
                                                        xtype: 'datefield',
                                                        name: 'QHdrDate',
                                                        fieldLabel: 'Quote Date',
                                                        columnWidth: 0.5,
                                                        listeners: {
                                                            blur: function(field) {
                                                                if (field.value > field.next().getValue()) {
                                                                    Ext.Msg.show({
                                                                        title: 'Warning',
                                                                        msg: 'The expiration date cannot be before the quote date!',
                                                                        buttons: Ext.Msg.OK,
                                                                        icon: Ext.Msg.WARNING
                                                                    });
                                                                }
                                                            },
                                                            change: function(field) {
                                                                field.next().setValue(Ext.Date.add(field.value, Ext.Date.DAY, 30));
                                                            }
                                                        }
                                                    }, {
                                                        xtype: 'datefield',
                                                        name: 'QHdrGoodThruDate',
                                                        fieldLabel: 'Expires:',
                                                        margin: '0 0 0 5',
                                                        columnWidth: 0.5,
                                                        listeners: {
                                                            blur: function(field) {
                                                                if (field.value < field.previousNode().getValue()) {
                                                                    Ext.Msg.show({
                                                                        title: 'Warning',
                                                                        msg: 'The expiration date cannot be before the quote date!',
                                                                        buttons: Ext.Msg.OK,
                                                                        icon: Ext.Msg.WARNING
                                                                    });
                                                                }
                                                            }
                                                        }
                                                    }, {
                                                        columnWidth: 1,
                                                        xtype: 'combo',
                                                        name: 'QHdrWarehouseKey',
                                                        fieldLabel: 'Carrier / Warehouse',
                                                        valueField: 'WarehouseKey',
                                                        displayField: 'CarrierWarehouse',
                                                        store: storeWarehouseType,
                                                        queryMode: 'local',
                                                        typeAhead: false,
                                                        minChars: 2,
                                                        forceSelection: false,
                                                        beforequery: function(record) {
                                                            record.query = new RegExp(record.query, 'i');
                                                            record.forceAll = true;
                                                        },
                                                        listeners: {
                                                            select: function(field) {
                                                                field.next().setValue(field.value);
                                                            }
                                                        }
                                                    }, {
                                                        xtype: 'hidden',
                                                        name: 'QHdrCarrierKey'
                                                    }, {
                                                        columnWidth: 0.5,
                                                        xtype: 'combo',
                                                        name: 'QHdrShipType',
                                                        fieldLabel: 'Shipment Type',
                                                        valueField: 'ShipTypeExpression',
                                                        displayField: 'ShipTypeText',
                                                        store: storeShipmentTypes,
                                                        queryMode: 'local',
                                                        typeAhead: false,
                                                        minChars: 2,
                                                        forceSelection: true,
                                                        listeners: {
                                                            beforequery: function(record) {
                                                                record.query = new RegExp(record.query, 'i');
                                                                record.forceAll = true;
                                                            }
                                                        }
                                                    }, {
                                                        xtype: 'combo',
                                                        margin: '0 0 0 5',
                                                        columnWidth: 0.5,
                                                        name: 'QHdrCustPaymentTerms',
                                                        fieldLabel: 'Payment Terms',
                                                        valueField: 'TermKey',
                                                        displayField: 'x_Description',
                                                        store: storePaymentTerms,
                                                        queryMode: 'local',
                                                        typeAhead: false,
                                                        minChars: 2,
                                                        forceSelection: true,
                                                        listeners: {
                                                            beforequery: function(record) {
                                                                record.query = new RegExp(record.query, 'i');
                                                                record.forceAll = true;
                                                            }
                                                        }
                                                    }, {
                                                        xtype: 'combo',
                                                        margin: '0 0 0 0',
                                                        columnWidth: 0.5,
                                                        name: 'QHdrLeadTime',
                                                        fieldLabel: 'Lead Time',
                                                        valueField: 'FVLeadTime',
                                                        displayField: 'FVLeadTime',
                                                        store: storeLeadTimes,
                                                        queryMode: 'local',
                                                        typeAhead: false,
                                                        minChars: 2,
                                                        forceSelection: false,
                                                        listeners: {
                                                            beforequery: function(record) {
                                                                record.query = new RegExp(record.query, 'i');
                                                                record.forceAll = true;
                                                            }
                                                        }
                                                    }, {
                                                        xtype: 'textfield',
                                                        fieldLabel: 'Job Number',
                                                        columnWidth: 0.5,
                                                        margin: '0 0 20 5',
                                                        name:'JobNum',
                                                        editable: false
                                                    }]
                                                },
                                                // task container
                                                {
                                                    xtype: 'fieldset',
                                                    columnWidth: 0.5,
                                                    title: 'Tasks',
                                                    layout: 'column',
                                                    items: [{
                                                        xtype: 'button',
                                                        columnWidth: 0.5,
                                                        margin: '0 0 5 0',
                                                        text: 'Export Quote to PT',
                                                        handler: me.onClickExportQuoteToPeachtree
                                                    }, {
                                                        xtype: 'button',
                                                        columnWidth: 0.5,
                                                        margin: '0 0 5 5',
                                                        text: 'Status History',
                                                        listeners: {
                                                            click: {
                                                                fn: me.onClickQuoteStatusHistory,
                                                                scope: me
                                                            }
                                                        }
                                                    }, {
                                                        xtype: 'button',
                                                        columnWidth: 0.5,
                                                        margin: '0 0 5 0',
                                                        text: 'Ordering Form',
                                                        handler: function() {
                                                            var me = this.up('form');
                                                            me.onClickOrderingForm();
                                                        }
                                                    }, {
                                                        xtype: 'button',
                                                        columnWidth: 0.5,
                                                        margin: '0 0 5 5',
                                                        text: 'Print Quote w/Sch B',
                                                        handler: function() {
                                                            var me = this.up('form');
                                                            me.onClickPrintQuoteWSch();
                                                        }
                                                    }]
                                                }
                                            ]
                                        },
                                        // inside right panel
                                        {
                                            xtype: 'fieldcontainer',
                                            columnWidth: 0.5,
                                            layout: 'column',
                                            items: [
                                                // Reporte Note
                                                {
                                                    xtype: 'fieldset',
                                                    margin: '0 0 0 10',
                                                    padding: '0 5 5 5',
                                                    columnWidth: 1,
                                                    title: 'Report Note',
                                                    layout: 'column',
                                                    items: [{
                                                        xtype: 'textareafield',
                                                        //fieldLabel: 'Report Note',
                                                        columnWidth: 1,
                                                        height: 100,
                                                        name: 'QHdrMemo'
                                                    }]
                                                },
                                                // Customer Information
                                                {
                                                    xtype: 'fieldset',
                                                    columnWidth: 1,
                                                    padding: '0 5 5 5',
                                                    margin: '5 0 0 10',
                                                    title: 'Customer Information',
                                                    layout: 'column',
                                                    items: [{
                                                        xtype: 'textfield',
                                                        fieldLabel: 'Customer',
                                                        columnWidth: 1,
                                                        name: 'x_CustName',
                                                        readOnly: true
                                                    }, {
                                                        xtype: 'textfield',
                                                        fieldLabel: '- Contact',
                                                        columnWidth: 1,
                                                        name: 'x_CustContactName',
                                                        readOnly: true
                                                    }, {
                                                        xtype: 'textfield',
                                                        fieldLabel: 'Reference',
                                                        columnWidth: 1,
                                                        name: 'x_FileReference',
                                                        readOnly: true
                                                    }]
                                                }
                                            ]
                                        }
                                    ]
                                },
                                // Currency Rate
                                {
                                    xtype: 'combo',
                                    name: 'QHdrCurrencyCode',
                                    fieldLabel: 'Currency',
                                    store: storeCurrencyRates,
                                    labelWidth: 50,
                                    listConfig: {
                                        minWidth: null
                                    },
                                    columnWidth: 0.5,
                                    valueField: 'CurrencyCode',
                                    displayField: 'CurrencyCodeDesc',
                                    queryMode: 'local',
                                    autoSelect: false,
                                    //typeAhead: true,
                                    minChars: 2,
                                    allowBlank: false,
                                    forceSelection: true,
                                    listeners: {
                                        beforequery: function(record) {
                                            record.query = new RegExp(record.query, 'i');
                                            record.forceAll = true;
                                        }
                                    },
                                    tpl: Ext.create('Ext.XTemplate',
                                        '<tpl for=".">',
                                        '<div class="x-boundlist-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                                        '</tpl>')
                                },
                                // Grid Totals
                                {
                                    xtype: 'fieldset',
                                    columnWidth: 0.5,
                                    margin: '0 0 0 5',
                                    layout: 'column',
                                    title: 'Vendor Information',
                                    items: [
                                        // Grid Totals
                                        {
                                            margin: '5 0 5 0',
                                            columnWidth: 1,
                                            //title: 'Vendor Total',
                                            xtype: 'gridpanel',
                                            itemId: 'gridVendorSummary',
                                            store: null,
                                            minHeight: 100,
                                            height: 100,
                                            features: [{
                                                ftype: 'summary'
                                            }],
                                            //hideHeaders: true,
                                            columns: [{
                                                xtype: 'gridcolumn',
                                                flex: 1,
                                                dataIndex: 'Vendor',
                                                text: 'Vendor',
                                                summaryType: 'count',
                                                summaryRenderer: function(value, summaryData, dataIndex) {
                                                    return Ext.String.format('Total for {0} items', value, value !== 1 ? value : 0);
                                                }
                                            }, {
                                                xtype: 'numbercolumn',
                                                width: 70,
                                                format: '00,000',
                                                dataIndex: 'Qty',
                                                text: 'Qty',
                                                align: 'right',
                                                summaryType: 'sum'
                                            }, {
                                                xtype: 'numbercolumn',
                                                width: 120,
                                                renderer: Ext.util.Format.usMoney,
                                                dataIndex: 'Cost',
                                                text: 'Cost',
                                                align: 'right',
                                                summaryType: 'sum'
                                            }, {
                                                xtype: 'numbercolumn',
                                                width: 120,
                                                renderer: Ext.util.Format.usMoney,
                                                dataIndex: 'Price',
                                                text: 'Price',
                                                align: 'right',
                                                summaryType: 'sum'
                                            }, {
                                                xtype: 'gridcolumn',
                                                width: 70,
                                                dataIndex: 'Currency',
                                                text: 'Currency',
                                            }],
                                            selType: 'rowmodel'
                                        }
                                    ]
                                }
                            ]
                        },
                        // Charges Panel
                        {
                            xtype: 'panel',
                            title: 'Charges',
                            margin: '0 0 10 0',
                            layout: {
                                type: 'column'
                            },
                            items: [
                                // Charges SubTotals
                                {
                                    xtype: 'fieldset',
                                    title: 'Charges Sub Totals',
                                    columnWidth: 1,
                                    padding: '5 5 15 5',
                                    layout: 'column',
                                    items: [
                                        // Grid Charges SubTotals
                                        {
                                            xtype: 'grid',
                                            itemId: 'gridchargesst',
                                            autoScroll: true,
                                            columnWidth: 1,
                                            minHeight: 120,
                                            margin: '0 5 5 0',
                                            store: storeChargesST,
                                            columns: [{
                                                xtype: 'gridcolumn',
                                                flex: 1,
                                                dataIndex: 'x_Category',
                                                text: 'Category',
                                                editor: {
                                                    xtype: 'combo',
                                                    displayField: 'STDescriptionText',
                                                    valueField: 'SubTotalKey',
                                                    name: 'QSTSubTotalKey',
                                                    fieldStyle: 'font-size:11px',
                                                    queryMode: 'local',
                                                    minChars: 2,
                                                    allowBlank: false,
                                                    forceSelection: true,
                                                    emptyText: 'Choose Category',
                                                    autoSelect: false,
                                                    matchFieldWidth: false,
                                                    listConfig: {
                                                        width: 400
                                                    },
                                                    listeners: {
                                                        change: function(field) {
                                                            var form = field.up('panel');
                                                            form.onFieldChange();
                                                        },
                                                        select: function(field, records, eOpts) {
                                                            var form = field.up('panel'),
                                                                record = form.context.record;

                                                            if (records.length > 0) {
                                                                record.set('x_Category', field.getRawValue());
                                                            }
                                                        },
                                                        beforequery: function(record) {
                                                            record.query = new RegExp(record.query, 'i');
                                                            record.forceAll = true;
                                                        }
                                                    },
                                                    selectOnFocus: true,
                                                    store: storetlkpCategories
                                                }
                                            }, {
                                                xtype: 'gridcolumn',
                                                flex: 1,
                                                dataIndex: 'x_Location',
                                                text: 'Location',
                                                editor: {
                                                    xtype: 'combo',
                                                    displayField: 'ListText',
                                                    valueField: 'ListKey',
                                                    name: 'QSTLocation',
                                                    fieldStyle: 'font-size:11px',
                                                    queryMode: 'local',
                                                    minChars: 2,
                                                    allowBlank: false,
                                                    forceSelection: true,
                                                    emptyText: 'Choose Location',
                                                    autoSelect: false,
                                                    matchFieldWidth: false,
                                                    listConfig: {
                                                        width: 400
                                                    },
                                                    listeners: {
                                                        change: function(field) {
                                                            var form = field.up('panel');
                                                            form.onFieldChange();
                                                        },
                                                        select: function(field, records, eOpts) {
                                                            var form = field.up('panel'),
                                                                record = form.context.record;

                                                            if (records.length > 0) {
                                                                record.set('x_Location', field.getRawValue());
                                                            }
                                                        },
                                                        beforequery: function(record) {
                                                            record.query = new RegExp(record.query, 'i');
                                                            record.forceAll = true;
                                                        }
                                                    },
                                                    selectOnFocus: true,
                                                    store: storetlkpGL
                                                }
                                            }],
                                            tbar: [{
                                                text: 'Add',
                                                itemId: 'addchargest',
                                                handler: function() {
                                                    var me = this.up('gridpanel').up('form'),
                                                        grid = this.up('gridpanel'),
                                                        count = grid.getStore().count(),
                                                        qhdrKey = me.down('field[name=QHdrKey]').getValue();

                                                    rowEditingCharges.cancelEdit();

                                                    var r = Ext.create('CBH.model.sales.FileQuoteChargesSubTotals', {
                                                        QSTQHdrKey: qhdrKey
                                                    });

                                                    grid.store.insert(count, r);
                                                    rowEditingChargesST.startEdit(count, 1);
                                                },
                                                disabled: true
                                            }, {
                                                itemId: 'removechargest',
                                                text: 'Delete',
                                                //hidden: accLevel === 3,
                                                handler: function() {
                                                    var grid = this.up('gridpanel');
                                                    var sm = grid.getSelectionModel();

                                                    rowEditingChargesST.cancelEdit();

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
                                                                            grid.store.reload({
                                                                                callback: function() {
                                                                                    sm.select(0);
                                                                                }
                                                                            });
                                                                        }
                                                                    });
                                                                }
                                                            }
                                                        }).defaultButton = 2;
                                                    }
                                                },
                                                disabled: true
                                            }],
                                            selType: 'rowmodel',
                                            plugins: [rowEditingChargesST],
                                            listeners: {
                                                selectionchange: function(view, records, eOpts) {
                                                    var me = this.up('form'),
                                                        toolbar = me.down('toolbar');

                                                    if (toolbar.isEditing === true) {
                                                        this.down('#removechargest').setDisabled(!records.length);
                                                    }
                                                }
                                            }
                                        }
                                    ]
                                },
                                // Grid Charges
                                {
                                    xtype: 'grid',
                                    itemId: 'gridcharges',
                                    autoScroll: true,
                                    columnWidth: 1,
                                    minHeight: 150,
                                    margin: '0 5 5 0',
                                    store: storeCharges,
                                    features: [{
                                        ftype: 'summary'
                                    }],
                                    columns: [{
                                        xtype: 'numbercolumn',
                                        width: 60,
                                        dataIndex: 'QChargeSort',
                                        text: 'Sort',
                                        format: '00,000'
                                    }, {
                                        xtype: 'numbercolumn',
                                        width: 80,
                                        dataIndex: 'QChargeQty',
                                        text: 'Qty',
                                        align: 'right',
                                        format: '00,000',
                                        editor: {
                                            xtype: 'numericfield',
                                            name: 'QChargeQty',
                                            hideTrigger: false,
                                            allowBlank: false,
                                            useThousandSeparator: true,
                                            minValue: 1,
                                            alwaysDisplayDecimals: false,
                                            allowNegative: false,
                                            alwaysDecimals: false,
                                            thousandSeparator: ',',
                                            fieldStyle: 'font-size:11px,text-align: right;',
                                            selectOnFocus: true,
                                            listeners: {
                                                change: function(field) {
                                                    var form = field.up('panel');
                                                    form.onFieldChange();
                                                }
                                            }
                                        }
                                    }, {
                                        xtype: 'gridcolumn',
                                        flex: 1,
                                        dataIndex: 'x_ChargeDescription',
                                        text: 'Description',
                                        editor: {
                                            xtype: 'combo',
                                            displayField: 'x_DescriptionText',
                                            valueField: 'ChargeKey',
                                            name: 'QChargeChargeKey',
                                            fieldStyle: 'font-size:11px',
                                            queryMode: 'local',
                                            minChars: 2,
                                            allowBlank: false,
                                            forceSelection: true,
                                            emptyText: 'Choose Charge',
                                            autoSelect: false,
                                            matchFieldWidth: false,
                                            listConfig: {
                                                width: 400
                                            },
                                            listeners: {
                                                change: function(field) {
                                                    var form = field.up('panel');
                                                    form.onFieldChange();
                                                },
                                                select: function(field, records, eOpts) {
                                                    var form = field.up('panel'),
                                                        record = form.context.record;

                                                    if (records.length > 0) {
                                                        record.set('x_ChargeDescription', field.getRawValue());
                                                        field.next().next().setValue(records[0].data.x_DescriptionMemo);
                                                    }
                                                },
                                                beforequery: function(record) {
                                                    record.query = new RegExp(record.query, 'i');
                                                    record.forceAll = true;
                                                }
                                            },
                                            selectOnFocus: true,
                                            store: storeChargesKeys
                                        }
                                    }, {
                                        xtype: 'checkcolumn',
                                        dataIndex: 'QChargePrint',
                                        width: 60,
                                        text: 'Print',
                                        editor: {
                                            xtype: 'checkbox',
                                            name: 'QChargePrint',
                                            listeners: {
                                                change: function(field) {
                                                    var form = field.up('panel');
                                                    form.onFieldChange();
                                                }
                                            }
                                        },
                                        listeners: {
                                            beforecheckchange: function() {
                                                return false;
                                            }
                                        }
                                    }, {
                                        xtype: 'gridcolumn',
                                        flex: 1,
                                        dataIndex: 'QChargeMemo',
                                        text: 'Notes & Comments',
                                        summaryType: 'count',
                                        summaryRenderer: function(value, summaryData, dataIndex) {
                                            return Ext.String.format('Total for {0} items', value, value !== 1 ? value : 0);
                                        },
                                        editor: {
                                            xtype: 'textfield',
                                            name: 'QChargeMemo',
                                            allowBlank: false,
                                            listeners: {
                                                change: function(field) {
                                                    var form = field.up('panel');
                                                    form.onFieldChange();
                                                }
                                            }
                                        }
                                    }, {
                                        xtype: 'checkcolumn',
                                        dataIndex: 'QChargeCostEstimated',
                                        width: 70,
                                        text: 'Estimate',
                                        editor: {
                                            xtype: 'checkbox',
                                            name: 'QChargeCostEstimated',
                                            listeners: {
                                                change: function(field) {
                                                    var form = field.up('panel');
                                                    form.onFieldChange();
                                                }
                                            }
                                        },
                                        listeners: {
                                            beforecheckchange: function() {
                                                return false;
                                            }
                                        }
                                    }, {
                                        xtype: 'numbercolumn',
                                        width: 120,
                                        dataIndex: 'QChargeCost',
                                        text: 'Cost',
                                        align: 'right',
                                        renderer: Ext.util.Format.usMoney,
                                        summaryType: 'sum',
                                        editor: {
                                            xtype: 'numericfield',
                                            name: 'QChargeCost',
                                            minValue: 0,
                                            hideTrigger: true,
                                            allowBlank: false,
                                            useThousandSeparator: true,
                                            decimalPrecision: 2,
                                            alwaysDisplayDecimals: true,
                                            allowNegative: false,
                                            alwaysDecimals: false,
                                            thousandSeparator: ',',
                                            fieldStyle: 'text-align: right;',
                                            enableKeyEvents: true,
                                            listeners: {
                                                change: function(field) {
                                                    var form = field.up('panel');
                                                    form.onFieldChange();
                                                },
                                                keydown: function(field, e) {
                                                    // e.HOME, e.END, e.PAGE_UP, e.PAGE_DOWN,
                                                    // e.TAB, e.ESC, arrow keys: e.LEFT, e.RIGHT, e.UP, e.DOWN
                                                    if (e.getKey() == e.F4) {
                                                        qty = field.previousNode('field[name=QChargeQty]').getValue();
                                                        value = field.value / qty;
                                                        var msgbox = Ext.create('Ext.ux.window.NumberPrompt');
                                                        msgbox.prompt('New Cost', 'Enter the Per-Unit cost:',
                                                            function(btn, value) {
                                                                field.setValue(value * qty);
                                                                field.focus(true, 200);
                                                            },
                                                            msgbox,
                                                            false,
                                                            value
                                                        );
                                                    }
                                                }
                                            }
                                        }
                                    }, {
                                        xtype: 'numbercolumn',
                                        width: 120,
                                        dataIndex: 'QChargePrice',
                                        text: 'Price',
                                        align: 'right',
                                        renderer: Ext.util.Format.usMoney,
                                        summaryType: 'sum',
                                        editor: {
                                            xtype: 'numericfield',
                                            name: 'QChargePrice',
                                            minValue: 0,
                                            hideTrigger: true,
                                            allowBlank: false,
                                            useThousandSeparator: true,
                                            decimalPrecision: 2,
                                            alwaysDisplayDecimals: true,
                                            allowNegative: false,
                                            alwaysDecimals: false,
                                            thousandSeparator: ',',
                                            fieldStyle: 'text-align: right;',
                                            enableKeyEvents: true,
                                            listeners: {
                                                change: function(field) {
                                                    var form = field.up('panel');
                                                    form.onFieldChange();
                                                },
                                                keydown: function(field, e) {
                                                    // e.HOME, e.END, e.PAGE_UP, e.PAGE_DOWN,
                                                    // e.TAB, e.ESC, arrow keys: e.LEFT, e.RIGHT, e.UP, e.DOWN
                                                    if (e.getKey() == e.F4) {
                                                        qty = field.previousNode('field[name=QChargeQty]').getValue();
                                                        value = field.value / qty;
                                                        var msgbox = Ext.create('Ext.ux.window.NumberPrompt');
                                                        msgbox.prompt('New Price', 'Enter the Per-Unit price:',
                                                            function(btn, value) {
                                                                field.setValue(value * qty);
                                                                field.focus(true, 200);
                                                            },
                                                            msgbox,
                                                            false,
                                                            value
                                                        );
                                                    }
                                                }
                                            }
                                        }
                                    }, {
                                        xtype: 'gridcolumn',
                                        width: 100,
                                        dataIndex: 'QChargeCurrencyCode',
                                        text: 'CUR',
                                        editor: {
                                            xtype: 'combo',
                                            name: 'QChargeCurrencyCode',
                                            fieldStyle: 'font-size:11px',
                                            store: storeCurRatesCharges,
                                            valueField: 'CurrencyCode',
                                            displayField: 'CurrencyCodeDesc',
                                            queryMode: 'local',
                                            minChars: 2,
                                            allowBlank: false,
                                            forceSelection: true,
                                            matchFieldWidth: false,
                                            listConfig: {
                                                width: 250
                                            },
                                            tpl: Ext.create('Ext.XTemplate',
                                                '<tpl for=".">',
                                                '<div class="x-boundlist-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                                                '</tpl>'),
                                            listeners: {
                                                change: function(field) {
                                                    var form = field.up('panel');
                                                    form.onFieldChange();
                                                },
                                                select: function(field, records, eOpts) {
                                                    var form = field.up('panel'),
                                                        record = form.context.record;
                                                    if (records.length > 0) {
                                                        record.set('QChargeCurrencyCode', field.value);
                                                        record.set('QChargeCurrencyRate', records[0].data.CurrencyRate);
                                                    }
                                                },
                                                beforequery: function(record) {
                                                    record.query = new RegExp(record.query, 'i');
                                                    record.forceAll = true;
                                                }
                                            }
                                        }
                                    }, {
                                        xtype: 'gridcolumn',
                                        flex: 1,
                                        dataIndex: 'x_FreightCompany',
                                        text: 'Freight Company',
                                        editor: {
                                            xtype: 'combo',
                                            name: 'QChargeFreightCompany',
                                            fieldStyle: 'font-size:11px',
                                            valueField: 'VendorKey',
                                            displayField: 'VendorName',
                                            queryMode: 'remote',
                                            pageSize: 11,
                                            triggerAction: '',
                                            forceSelection: false,
                                            queryCaching: false,
                                            emptyText: 'Choose Vendor',
                                            autoSelect: false,
                                            selectOnFocus: true,
                                            minChars: 2,
                                            allowBlank: false,
                                            matchFieldWidth: false,
                                            listConfig: {
                                                width: 250
                                            },
                                            /*tpl: Ext.create('Ext.XTemplate',
                                                '<tpl for=".">',
                                                '<div class="x-boundlist-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                                                '</tpl>'),*/
                                            listeners: {
                                                change: function(field) {
                                                    var form = field.up('panel');
                                                    form.onFieldChange();
                                                },
                                                select: function(field, records, eOpts) {
                                                    var form = field.up('panel'),
                                                        record = form.context.record;
                                                    if (records.length > 0) {
                                                        //record.set('QChargeCurrencyCode', field.value);
                                                        record.set('x_FreightCompany', records[0].data.VendorName);
                                                    }
                                                }
                                            }
                                        }
                                    }, {
                                        xtype: 'actioncolumn',
                                        draggable: false,
                                        width: 25,
                                        resizable: false,
                                        hideable: false,
                                        stopSelection: false,
                                        items: [{
                                            handler: function(view, rowIndex, colIndex, item, e, record, row) {
                                                view.editingPlugin.startEdit(rowIndex, 0);
                                                setTimeout(function() {
                                                    view.editingPlugin.editor.down("field[name=QChargeQty]").focus(true, 200);
                                                }, 500);
                                            },
                                            getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                                            tootip: 'view line detail'
                                        }]
                                    }],
                                    tbar: [{
                                        text: 'Add',
                                        itemId: 'addcharge',
                                        handler: function() {
                                            var me = this.up('gridpanel').up('form'),
                                                grid = this.up('gridpanel'),
                                                count = grid.getStore().count();

                                            rowEditingCharges.cancelEdit();

                                            var r = Ext.create('CBH.model.sales.FileQuoteCharges', {
                                                QChargeQty: 1,
                                                QChargeCurrencyCode: 'USD',
                                                QChargeSort: (count + 1) * 100,
                                                QChargeHdrKey: me.down('field[name=QHdrKey]').getValue(),
                                                QChargeFileKey: me.FileKey,
                                                QChargePrint: true
                                            });

                                            grid.store.insert(count, r);
                                            rowEditingCharges.startEdit(count, 1);
                                        },
                                        disabled: true
                                    }, {
                                        itemId: 'removecharge',
                                        text: 'Delete',
                                        //hidden: accLevel === 3,
                                        handler: function() {
                                            var grid = this.up('gridpanel');
                                            var sm = grid.getSelectionModel();

                                            rowEditingCharges.cancelEdit();

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
                                                                    grid.store.reload({
                                                                        callback: function() {
                                                                            sm.select(0);
                                                                        }
                                                                    });
                                                                }
                                                            });
                                                        }
                                                    }
                                                }).defaultButton = 2;
                                            }
                                        },
                                        disabled: true
                                    }],
                                    selType: 'rowmodel',
                                    plugins: [rowEditingCharges],
                                    listeners: {
                                        selectionchange: function(view, records, eOpts) {
                                            this.down('#removecharge').setDisabled(!records.length);
                                        }
                                    }
                                }
                            ]
                        },
                        // Items Summary Panel
                        {
                            xtype: 'panel',
                            title: 'Items Summary',
                            items: [{
                                xtype: 'gridpanel',
                                itemId: 'griditemssummary',
                                store: storeItemsSummary,
                                minHeight: 150,
                                features: [{
                                    ftype: 'summary'
                                }],
                                columns: [{
                                    xtype: 'numbercolumn',
                                    width: 80,
                                    dataIndex: 'QSummarySort',
                                    text: 'Sort',
                                    format: '00,000'
                                }, {
                                    xtype: 'numbercolumn',
                                    width: 80,
                                    dataIndex: 'QSummaryQty',
                                    text: 'Quantity',
                                    align: 'right',
                                    format: '00,000'
                                }, {
                                    xtype: 'gridcolumn',
                                    width: 180,
                                    dataIndex: 'x_VendorName',
                                    text: 'Vendor'
                                }, {
                                    xtype: 'gridcolumn',
                                    width: 80,
                                    dataIndex: 'QSummaryItemNum',
                                    text: 'Item Num.'
                                }, {
                                    xtype: 'gridcolumn',
                                    flex: 1,
                                    dataIndex: 'QSummaryDescription',
                                    text: 'Description',
                                    totalsText: 'First column is sum, second max',
                                    summaryType: 'count',
                                    summaryRenderer: function(value, summaryData, dataIndex) {
                                        return Ext.String.format('Total for {0} items', value, value !== 1 ? value : 0);
                                    }
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Unit Price',
                                    align: 'right',
                                    dataIndex: 'QSummaryPrice',
                                    renderer: Ext.util.Format.usMoney,
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'Total Price',
                                    align: 'right',
                                    dataIndex: 'QSummaryLinePrice',
                                    renderer: Ext.util.Format.usMoney,
                                    summaryType: 'sum'
                                }, {
                                    xtype: 'gridcolumn',
                                    text: 'CUR',
                                    align: 'right',
                                    dataIndex: 'QSummaryCurrencyCode'
                                }, {
                                    xtype: 'actioncolumn',
                                    draggable: false,
                                    width: 35,
                                    resizable: false,
                                    hideable: false,
                                    stopSelection: false,
                                    handler: function(view, rowIndex, colIndex, item, e, record, row) {
                                        var me = this.up('form'),
                                            curRec = me.down('formtoolbar').getCurrentRecord();

                                        var form = Ext.widget('FileQuoteEditItemSummary', {
                                            currentRecord: record
                                        });
                                        form.modal = true;
                                        form.loadRecord(record);
                                        form.callerForm = this.up('form');
                                        form.show();
                                    },
                                    getGlyph: function(itemScope, rowIdx, colIdx, item, rec) { return 'xf00e@FontAwesome';},
                                    tootip: 'view details'
                                }],
                                tbar: [{
                                    xtype: 'component',
                                    flex: 1
                                }, {
                                    text: 'Add',
                                    itemId: 'additem',
                                    handler: function() {
                                        var me = this.up('gridpanel').up('form'),
                                            grid = this.up('gridpanel'),
                                            count = grid.getStore().count();


                                        var record = Ext.create('CBH.model.sales.FileQuoteItemsSummary', {
                                            QSummaryQty: 1,
                                            QSummaryCurrencyCode: 'USD',
                                            QSummarySort: (count + 1) * 100,
                                            QSummaryQHdrKey: me.down('field[name=QHdrKey]').getValue()
                                        });


                                        var form = Ext.widget('FileQuoteEditItemSummary', {
                                            currentRecord: record
                                        });

                                        form.modal = true;
                                        form.loadRecord(record);
                                        form.callerForm = this.up('form');
                                        form.show();
                                    }
                                }, {
                                    itemId: 'deleteline',
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
                                                        selection[0].destroy();
                                                    }
                                                }
                                            }).defaultButton = 2;
                                        }
                                    },
                                    disabled: true
                                }],
                                selType: 'rowmodel',
                                /*plugins: [rowEditingItems],*/
                                listeners: {
                                    selectionchange: function(view, records, eOpts) {
                                        this.down('#deleteline').setDisabled(!records.length);
                                    },
                                    validateedit: {
                                        fn: me.onValidateEdit,
                                        scope: me
                                    }
                                }
                            }]
                        }
                    ]
                },
                // Delimited 2
                {
                    xtype: 'container',
                    columnWidth: 1,
                    layout: 'anchor',
                    items: [{
                        xtype: 'component',
                        flex: 1
                    }]
                },
                // Currency Rate General
                {
                    xtype: 'fieldset',
                    layout: 'column',
                    columnWidth: 0.5,
                    margin: '10 0 0 5',
                    padding: '0 10 10 10',
                    items: [{
                        xtype: 'combo',
                        name: 'x_QHdrCurrencyCode',
                        fieldLabel: 'Currency',
                        formBind: false,
                        store: storeCurrencyRatesHeader,
                        labelWidth: 50,
                        listConfig: {
                            minWidth: null
                        },
                        columnWidth: 0.5,
                        valueField: 'CurrencyCode',
                        displayField: 'CurrencyCodeDesc',
                        queryMode: 'local',
                        typeAhead: false,
                        autoSelect: false,
                        minChars: 2,
                        allowBlank: true,
                        forceSelection: true,
                        listeners: {
                            select: function(field, records, eOpts) {
                                if (records.length > 0) {
                                    var me = this.up('panel');
                                    field.next().setValue(records[0].data.CurrencyRate);
                                    me.onSelectCurrencyCode(records[0]);
                                }
                            },
                            change: function(field, newValue, oldValue) {
                                if (newValue === null) {
                                    field.next().setValue(0);
                                }
                            },
                            blur: function(field, The, eOpts) {
                                if (field.value !== null) {
                                    var me = this.up('panel');
                                    copyToField = field.valueModels[0].data.CurrencyRate;
                                    copyField = me.down('field[name=QHdrCurrencyRate]');
                                    copyField.setValue(copyToField);
                                }
                            },
                            beforequery: function(record) {
                                record.query = new RegExp(record.query, 'i');
                                record.forceAll = true;
                            }
                        },
                        tpl: Ext.create('Ext.XTemplate',
                            '<tpl for=".">',
                            '<div class="x-boundlist-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                            '</tpl>')
                    }, {
                        xtype: 'numericfield',
                        name: 'QHdrCurrencyRate',
                        columnWidth: 0.5,
                        margin: '0 0 0 10',
                        hideTrigger: true,
                        useThousandSeparator: true,
                        decimalPrecision: 3,
                        alwaysDisplayDecimals: true,
                        allowNegative: false,
                        currencySymbol: '$',
                        alwaysDecimals: true,
                        thousandSeparator: ',',
                        fieldLabel: 'Rate',
                        labelAlign: 'top',
                        fieldStyle: 'text-align: right;',
                        allowBlank: false,
                        formBind: false,
                        editable: false,
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
                    //hideHeaders: true,
                    columns: [{
                            xtype: 'rownumberer'
                        }, {
                            xtype: 'gridcolumn',
                            width: 120,
                            dataIndex: 'QStatusModifiedDate',
                            text: 'Modified Date',
                            renderer: Ext.util.Format.dateRenderer('m/d/Y H:i')
                        }, {
                            xtype: 'gridcolumn',
                            width: 80,
                            dataIndex: 'QStatusQuoteNum',
                            text: 'Quote Num'
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
                            dataIndex: 'QStatusMemo',
                            text: 'Description',
                        }, {
                            xtype: 'gridcolumn',
                            width: 100,
                            dataIndex: 'QStatusModifiedBy',
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
            dockedItems: [{
                xtype: 'formtoolbar',
                itemId: 'FormToolbar',
                dock: 'top',
                store: me.storeNavigator,
                printEnabled: true,
                listeners: {
                    addrecord: {
                        fn: me.onAddClick,
                        scope: me
                    },
                    savechanges: {
                        fn: me.onSaveClick,
                        scope: me
                    },
                    deleterecord: {
                        fn: me.onDeleteClick,
                        scope: me
                    },
                    afterloadrecord: {
                        fn: me.onAfterLoadRecord,
                        scope: me
                    },
                    printrecord: {
                        fn: me.onPrintRecord,
                        scope: me
                    },
                    beginedit: {
                        fn: me.onBeginEdit,
                        scope: me
                    },
                }
            }, {
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [{
                    xtype: 'textfield',
                    itemId: 'QHdrCreatedBy',
                    name: 'QHdrCreatedBy',
                    readOnly: true,
                    fieldLabel: 'Created By'
                }, {
                    xtype: 'datefield',
                    name: 'QHdrCreatedDate',
                    itemId: 'QHdrCreatedDate',
                    readOnly: true,
                    fieldLabel: 'Created Date',
                    hideTrigger: true
                }, {
                    xtype: 'textfield',
                    name: 'QHdrModifiedBy',
                    itemId: 'QHdrModifiedBy',
                    readOnly: true,
                    fieldLabel: 'Modified By'
                }, {
                    xtype: 'datefield',
                    name: 'QHdrModifiedDate',
                    itemId: 'QHdrModifiedDate',
                    readOnly: true,
                    fieldLabel: 'Modified Date',
                    hideTrigger: true
                }, {
                    xtype: 'component',
                    flex: 1
                }]
            }],
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

    registerKeyBindings: function(view, options) {
        var me = this;
        Ext.EventManager.on(view.getEl(), 'keyup', function(evt, t, o) {
                if (evt.ctrlKey && evt.keyCode === Ext.EventObject.F8) { 
                    evt.stopEvent();
                    var toolbar = me.down('#FormToolbar');
                    if(toolbar.isEditing) {
                        var btn = toolbar.down('#save');
                        btn.fireEvent('click');
                    }
                }
            },
            this);
    },

    onRenderForm: function() {
        var me = this;
    },

    onAfterLoadRecord: function(tool, record) {
        var me = this;

        if (!record.phantom) {
            me.down('#addchargest').setDisabled(false);
            me.down('#addcharge').setDisabled(false);
        } else {
            me.down('#addchargest').setDisabled(true);
            me.down('#addcharge').setDisabled(true);
        }

        var filterQHdr = new Ext.util.Filter({
            property: 'QChargeHdrKey',
            value: record.data.QHdrKey
        });

        Ext.Msg.wait('Loading data...', 'Wait');

        var storeCharges = new CBH.store.sales.FileQuoteCharges({
            remoteFilter: true
        }).load({
            filters: [filterQHdr],
            callback: function() {
                var grid = me.down('#gridcharges');

                grid.reconfigure(storeCharges);

                filterQHdr = new Ext.util.Filter({
                    property: 'QSTQHdrKey',
                    value: record.data.QHdrKey
                });

                var storeChargesST = new CBH.store.sales.FileQuoteChargesSubTotals({
                    remoteFilter: true
                }).load({
                    filters: [filterQHdr],
                    callback: function() {
                        var grid = me.down('#gridchargesst');

                        grid.reconfigure(storeChargesST);

                        var storeWarehouseType = new CBH.store.vendors.WarehouseTypes().load({
                            callback: function() {
                                field = me.down('field[name=QHdrWarehouseKey]').bindStore(storeWarehouseType);
                                field.setValue(record.data.QHdrWarehouseKey);

                                var filterQHdr = new Ext.util.Filter({
                                    property: 'QSummaryQHdrKey',
                                    value: record.data.QHdrKey
                                });

                                var storeItemsSummary = new CBH.store.sales.FileQuoteItemsSummary().load({
                                    params: {
                                        page: 1,
                                        start: 0,
                                        limit: 8
                                    },
                                    filters: [filterQHdr],
                                    callback: function() {
                                        var grid = me.down('#griditemssummary');
                                        grid.reconfigure(storeItemsSummary);

                                        var storeStatus = new CBH.store.sales.FileQuoteStatusHistorySubDetails({
                                            autoLoad: false
                                        }).load({
                                            params: {
                                                FileKey: record.data.QHdrFileKey,
                                                page: 0,
                                                start: 0,
                                                limit: 0
                                            },
                                            callback: function() {
                                                me.down('#gridstatus').reconfigure(storeStatus);

                                                var storeVendorSummary = new CBH.store.sales.qsumFileQuoteVendorSummary({
                                                    autoLoad: false
                                                }).load({
                                                    params: {
                                                        QHdrKey: record.data.QHdrKey
                                                    },
                                                    callback: function() {
                                                        me.down('#gridVendorSummary').reconfigure(storeVendorSummary);
                                                        Ext.Msg.hide();
                                                    }
                                                });
                                            }
                                        });
                                    }
                                });
                            }
                        });
                    }
                });
            }
        });
    },

    onSaveClick: function(toolbar, record) {
        var me = this;
        var form = me.getForm();

        if (!form.isValid()) {
            Ext.Msg.alert("Validation", "Check data for valid input!!!");
            return false;
        }

        form.updateRecord();

        record = form.getRecord();
        var isdirty = record.dirty;

        var saveRecord = function() {
            Ext.Msg.wait('Saving Record...', 'Wait');
            record.save({
                callback: function(records, operation, success) {
                    if (success) {
                        Ext.Msg.hide();
                        toolbar.doRefresh();
                    } else {
                        Ext.Msg.hide();
                    }
                }
            });
        };

        if(!record.phantom && !String.isNullOrEmpty(record.get("QHdrRevision"))) {
            Ext.Msg.show({
                title: 'Increment Revision?',
                msg: 'Do you want to update the revision number?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(btn) {
                    if (btn === "yes") {
                        var value = record.get("QHdrRevision") || "";
                        var next = GetNewRevisionNum(value);
                        record.set("QHdrRevision", next);
                    }
                    saveRecord();

                }
            }).defaultButton = 1;
        } else {
            saveRecord();
        }
    },

    onClickOrderingForm: function() {
        var me = this,
            tabs = me.up('app_pageframe');

        var quotekey = me.down('field[name=QHdrKey]').getValue();

        var storeToNavigate = new CBH.store.sales.qfrmFileQuoteConfirmation().load({
            params: {
                QHdrKey: quotekey
            },
            callback: function(records, operation, success) {

                var form = Ext.widget('filequoteconfirmation', {
                    QuoteNum: me.QuoteNum,
                    Customer: me.Customer,
                    FileNum: me.FileNum,
                    currentRecord: records[0]
                });

                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Quote Confirmation',
                    padding: '0 5 0 5',
                    items: [form]
                });

                //form.down('#FormToolbar').gotoAt(1);
                form.loadRecord(records[0]);
                form.onAfterLoadRecord(records[0]);
                tab.show();
            },
            scope: this
        });
    },

    onClickQuoteStatusHistory: function() {
        var me = this,
            tabs = me.up('app_pageframe'),
            curRec = me.down('#FormToolbar').getCurrentRecord();

        var fileKey = me.FileKey;
        var fileNum = me.FileNum;
        var customer = me.Customer;

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
                        FileStatus: curRec.data.x_Status,
                        QHdrKey: curRec.data.QHdrKey,
                        QuoteNum: me.QuoteNum
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

    onClickPrintQuoteWSch: function() {
        var me = this,
            quotekey = me.down('field[name=QHdrKey]').getValue();
        
        var userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey();
        var formData = {
            id: quotekey,
            employeeKey: userKey,
            wSch: 1
        };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptQuoteCustomer", params);
        window.open(pathReport, 'CBH - Quote Customer', false);
    },

    onSelectCurrencyCode: function(record) {
        var me = this,
            curRec = me.down('formtoolbar').getCurrentRecord();

        Ext.Msg.wait('Please wait....', 'Wait');

        Ext.Ajax.request({
            method: 'GET',
            type: 'json',
            url: CBH.GlobalSettings.webApiPath + '/api/ChangeFileQuoteCurrency',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                QHdrKey: curRec.data.QHdrKey,
                CurrentUser: CBH.GlobalSettings.getCurrentUser().UserName,
                CurrencyCode: record.data.CurrencyCode,
                CurrencyRate: record.data.CurrencyRate
            },
            success: function(rsp) {
                var data = Ext.JSON.decode(rsp.responseText);
                me.down("field[name=QHdrCurrencyCode]").setValue(record.data.CurrencyCode);
                me.down("field[name=QHdrCurrencyRate]").setValue(record.data.CurrencyRate);
                Ext.Msg.hide();
            }
        });
    },

    onClickExportQuoteToPeachtree: function() {
        var me = this.up('form'),
            today = new Date(),
            t = Ext.util.Cookies.get('CBH.UserAuth');

        var QHdrKey = me.down("field[name=QHdrKey]").getValue();

        var url = "{0}/ExportQuoteToPeachTree?_dc={1}&QHdrKey={2}&t={3}".format(CBH.GlobalSettings.webApiPath, today.getTime(), QHdrKey, t);
        window.open(url, '_blank');
    },

    onPrintRecord: function(toolbar, record) {
        var userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey();
        var formData = {
            id: record.get('QHdrKey'),
            employeeKey: userKey
        };

        var params = Serialize(formData),
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format("rptQuoteCustomer", params);
        window.open(pathReport, 'CBH - Quote Customer', false);
    },

    onBeginEdit: function(toolbar, record) {
        var me  = this;

        me.down("field[name=QHdrModifiedBy]").setValue(CBH.GlobalSettings.getCurrentUserName());
        me.down("field[name=QHdrModifiedDate]").setValue(new Date());
    }
});