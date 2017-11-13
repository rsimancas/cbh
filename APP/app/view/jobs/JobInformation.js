Ext.define('CBH.view.jobs.JobInformation', {
    extend: 'Ext.form.Panel',
    alias: 'widget.jobinformation',

    layout: {
        type: 'column'
    },
    bodyPadding: 10,
    frameHeader: false,
    header: false,
    title: 'Job Information',

    requires: [
        'CBH.view.customers.Customers',
        'CBH.view.customers.CustomerContacts',
        'CBH.view.customers.CustomerShipAddress'
    ],

    JobKey: 0,
    JobCustKey: 0,
    storeNavigator: null,

    initComponent: function() {
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;
        
        var me = this;

        rowEditing = new Ext.grid.plugin.RowEditing({
            clicksToMoveEditor: 2,
            autoCancel: false,
            errorSummary: false,
            listeners: {
                beforeedit: {
                    //delay: 100,
                    fn: function(item, e) {
                        this.getEditor().onFieldChange();
                    }
                },
                cancelEdit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('#gridroles');
                        // Canceling editing of a locally added, unsaved record: remove it
                        if (context.record.phantom) {
                            grid.store.remove(context.record);
                        }
                    }
                },
                edit: {
                    fn: function(rowEditing, context) {
                        var grid = this.editor.up('#gridroles'),
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

        Ext.Msg.wait('Loading data...', 'Wait');
        storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function() {
                Ext.Msg.hide();
            }
        });

        var storeCustomerContacts = null;
        var storeCustomerShipAddress = null;
        var storeEmployeeRoles = null;
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
                limit: 0
            }
        });
        var storeInspectionCompanies = new CBH.store.common.InspectionCompanies().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        var storeWarehouseType = null;

        Ext.applyIf(me, {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items: [{
                xtype: 'fieldset',
                columnWidth: 1,
                defaults: {
                    anchor: '100%',

                },
                collapsible: false,
                //title: 'Job Information',
                items: [{
                    xtype: 'fieldcontainer',
                    layout: {
                        type: 'hbox'
                    },
                    items: [{
                        xtype: 'component',
                        flex: 1
                    }, {
                        xtype: 'displayfield',
                        name: 'x_JobCustKey',
                        hideTrigger: true,
                        labelAlign: 'left',
                        labelWidth: 80,
                        fieldLabel: 'Customer Code',
                        editable: false,
                        fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: right'
                    }, {
                        margin: '0 0 0 10',
                        xtype: 'displayfield',
                        name: 'x_JobNumFormatted',
                        fieldLabel: 'Reference Num',
                        labelAlign: 'left',
                        labelWidth: 80,
                        editable: false,
                        fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: right'
                    },{
                        xtype:'hiddenfield',
                        name:'JobKey'
                    }]
                }]
            },
            //General Information
            {
                xtype: 'fieldset',
                title: 'General Information',
                minHeight: 350,
                columnWidth: 0.5,
                padding: '5 10 10 10',
                layout: {
                    type: 'column'
                },
                items: [{
                    xtype: 'textfield',
                    columnWidth: 0.5,
                    name: 'JobProdDescription',
                    fieldLabel: 'Product Desc'

                }, {
                    margin: '0 0 0 5',
                    xtype: 'textfield',
                    columnWidth: 0.5,
                    name: 'JobShippingDescription',
                    fieldLabel: 'BL/AWB Desc'

                }, {
                    xtype: 'textfield',
                    columnWidth: 1,
                    name: 'JobReference',
                    fieldLabel: 'CBH Reference'

                }, {
                    xtype: 'combo',
                    name: 'JobCustKey',
                    fieldLabel: 'Customer',
                    labelWidth: 50,
                    columnWidth: 1,
                    valueField: 'CustKey',
                    displayField: 'CustName',
                    queryMode: 'remote',
                    autoSelect: false,
                    minChars: 2,
                    allowBlank: false,
                    triggerAction: '',
                    forceSelection: false,
                    queryCaching: false, // set for after add a new customer, this control recognize the new customer added
                    emptyText: 'Choose Customer',
                    pageSize: 11,
                    //queryBy: 'CustName', //Custom property for optimize remote query
                    listeners: {
                        buffer: 100,
                        blur: {
                            fn: me.onCustomerBlur,
                            scope: this
                        }
                    },
                    store: new CBH.store.customers.Customers({
                        params: {
                            page: 1,
                            start: 0,
                            limit: 8
                        }
                    })
                }, {
                    xtype: 'combo',
                    name: 'JobContactKey',
                    fieldLabel: '- Contact',
                    labelWidth: 50,
                    columnWidth: 1,
                    valueField: 'ContactKey',
                    displayField: 'x_ContactFullName',
                    store: storeCustomerContacts,
                    queryMode: 'local',
                    typeAhead: false,
                    minChars: 2,
                    allowBlank: false,
                    forceSelection: false,
                    queryCaching: false, // set for after add a new contact, this control recognize the new contact added
                    listeners: {
                        beforequery: function(record) {
                            record.query = new RegExp(record.query, 'i');
                            record.forceAll = true;
                        },
                        blur: {
                            fn: me.onContactBlur,
                            scope: this
                        }
                    }
                }, {
                    xtype: 'textfield',
                    columnWidth: 1,
                    fieldLabel: 'Cust Reference',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'JobCustRefNum',
                }, {
                    xtype: 'combo',
                    name: 'JobCustCurrencyCode',
                    fieldLabel: 'Default Currency',
                    store: storeCurrencyRates,
                    labelWidth: 50,
                    columnWidth: 0.5,
                    valueField: 'CurrencyCode',
                    displayField: 'CurrencyCode',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    allowBlank: false,
                    forceSelection: true,
                    tpl: Ext.create('Ext.XTemplate',
                        '<tpl for=".">',
                        '<div class="x-boundlist-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                        '</tpl>'),
                    listeners: {
                        blur: function(field, The, eOpts) {
                            if (field.value !== null) {
                                var form = field.up('panel');

                                copyToField = field.valueModels[0].data.CurrencyRate;
                                copyField = form.down('field[name=JobCustCurrencyRate]');
                                copyField.setValue(copyToField);
                            }
                        },
                        beforequery: function(record) {
                            record.query = new RegExp(record.query, 'i');
                            record.forceAll = true;
                        }
                    }
                }, {
                    xtype: 'numericfield',
                    name: 'JobCustCurrencyRate',
                    columnWidth: 0.5,
                    margin: '0 0 0 10',
                    hideTrigger: false,
                    useThousandSeparator: true,
                    decimalPrecision: 5,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    currencySymbol: '$',
                    alwaysDecimals: true,
                    thousandSeparator: ',',
                    fieldLabel: 'Rate',
                    labelAlign: 'top',
                    fieldStyle: 'font-size:11px;text-align:right;',
                    allowBlank: false
                }, {
                    xtype: 'datefield',
                    columnWidth: 0.3,
                    fieldLabel: 'Date Rqd. by Customer',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'JobDateCustRequired',
                    //format: 'm/d/Y',
                    //allowBlank: false
                }, {
                    xtype: 'textfield',
                    columnWidth: 0.3,
                    margin: '0 0 0 10',
                    fieldLabel: 'Date Rqd. by Cust. Note',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'JobDateCustRequiredNote',
                    //allowBlank: false
                    listeners: {
                        blur: function() {
                            //me.onSaveClick();
                        }
                    }
                }, {
                    xtype: 'combo',
                    margin: '0 0 0 5',
                    columnWidth: 0.4,
                    name: 'JobCustPaymentTerms',
                    fieldLabel: 'Payment Terms',
                    valueField: 'TermKey',
                    displayField: 'x_Description',
                    store: storePaymentTerms,
                    queryMode: 'local',
                    typeAhead: false,
                    allowBlank: false,
                    minChars: 2,
                    forceSelection: true,
                    matchFieldWidth: false,
                    listConfig: {
                        width: 400
                    },
                    listeners: {
                        beforequery: function(record) {
                            record.query = new RegExp(record.query, 'i');
                            record.forceAll = true;
                        }
                    }
                }]
            },
            // Job Roles
            {
                margin: '0 0 0 10',
                xtype: 'fieldset',
                title: 'Roles',
                minHeight: 350,
                columnWidth: 0.5,
                layout: 'column',
                padding: '5 10 10 10',
                items: [{
                    xtype: 'gridpanel',
                    columnWidth: 1,
                    itemId: 'gridroles',
                    store: storeEmployeeRoles,
                    minHeight: 200,
                    columns: [{
                        xtype: 'rownumberer'
                    }, {
                        xtype: 'gridcolumn',
                        dataIndex: 'x_RoleName',
                        text: 'Job Role',
                        flex: 1,
                        editor: {
                            xtype: 'combo',
                            displayField: 'JobRoleDescription',
                            valueField: 'JobRoleKey',
                            name: 'JobEmployeeRoleKey',
                            enableKeyEvents: true,
                            forceSelection: true,
                            queryMode: 'local',
                            selectOnFocus: true,
                            allowBlank: false,
                            listeners: {
                                // specialkey: function(field, e){
                                //     // e.HOME, e.END, e.PAGE_UP, e.PAGE_DOWN,
                                //     // e.TAB, e.ESC, arrow keys: e.LEFT, e.RIGHT, e.UP, e.DOWN

                                //     e.stopEvent();

                                //     if(e.getKey() == e.TAB && field.isExpanded) {
                                //         e.stopEvent();
                                //         return;
                                //     };
                                //     // if (e.getKey() == e.ENTER) {
                                //     //     var form = field.up('form').getForm();
                                //     //     form.submit();
                                //     // }
                                // },
                                change: function(field) {
                                    var form = field.up('panel');
                                    form.onFieldChange();
                                },
                                select: function(field, records, eOpts) {
                                    var form = field.up('panel'),
                                        record = form.context.record;
                                    if (records.length > 0) {
                                        record.set('x_RoleName', records[0].data.JobRoleDescription);
                                    }
                                },
                                beforequery: function(record) {
                                    record.query = new RegExp(record.query, 'i');
                                    record.forceAll = true;
                                }
                            },
                            store: new CBH.store.jobs.JobRoles().load({
                                params: {
                                    page: 0,
                                    start: 0,
                                    limit: 0
                                }
                            })
                        }
                    }, {
                        xtype: 'gridcolumn',
                        flex: 1,
                        dataIndex: 'x_EmployeeName',
                        text: 'Employee',
                        editor: {
                            xtype: 'combo',
                            itemId: 'JobEmployeeEmployeeKey',
                            displayField: 'x_EmployeeFullName',
                            valueField: 'EmployeeKey',
                            enableKeyEvents: true,
                            forceSelection: true,
                            queryMode: 'local',
                            selectOnFocus: true,
                            allowBlank: false,
                            listeners: {
                                change: function(field) {
                                    var form = field.up('panel');
                                    form.onFieldChange();
                                },
                                select: function(field, records, eOpts) {
                                    var form = field.up('panel'),
                                        record = form.context.record;
                                    if (records.length > 0) {
                                        record.set('JobEmployeeEmployeeKey', field.value);
                                    }
                                },
                                beforequery: function(record) {
                                    record.query = new RegExp(record.query, 'i');
                                    record.forceAll = true;
                                }
                            },
                            store: new CBH.store.common.Employees().load({
                                params: {
                                    page: 0,
                                    start: 0,
                                    limit: 0,
                                    fieldFilters: JSON.stringify({
                                        fields: [
                                            { name: 'EmployeeStatusCode', type: 'int', value: 8 }
                                        ]
                                    })
                                }
                            })
                        }
                    }],
                    tbar: [{
                        text: 'Add',
                        itemId: 'addline',
                        handler: function() {
                            var me = this.up('form');
                            rowEditing.cancelEdit();

                            rowEditing.editor.down('#JobEmployeeEmployeeKey').getStore().reload();

                            // Create a model instance
                            var r = new CBH.model.jobs.JobEmployeeRoles({
                                JobEmployeeJobKey: me.down("field[name=JobKey]").getValue()
                            });

                            var grid = this.up('gridpanel');

                            var count = grid.getStore().count();
                            grid.store.insert(count, r);
                            rowEditing.startEdit(count, 1);
                        },
                        disabled: true
                    }, {
                        itemId: 'removeMov',
                        text: 'Delete',
                        hidden: accLevel === 3,
                        handler: function() {
                            var grid = this.up('gridpanel');
                            var sm = grid.getSelectionModel();

                            rowEditing.cancelEdit();

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
                    plugins: [rowEditing],
                    listeners: {
                        'selectionchange': function(view, records) {
                            this.down('#removeMov').setDisabled(!records.length);
                        }
                    }
                }]
            },
            // Fake COntainer
            {
                xtype:'container',
                columnWidth: 1,
                layout:'fit',
                items:[
                {
                    xtype:'component',
                    flex:1
                }
                ]
            },
            // Shipping Information
            {
                xtype: 'fieldset',
                columnWidth: 0.5,
                title: 'Shipping Information',
                padding: '5 10 10 10',
                layout: 'column',
                items: [{
                    columnWidth: 0.5,
                    xtype: 'combo',
                    name: 'JobShipType',
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
                    margin: '0 0 0 5',
                    columnWidth: 0.5,
                    xtype: 'combo',
                    name: 'JobWarehouseKey',
                    fieldLabel: 'Carrier / Warehouse',
                    valueField: 'WarehouseKey',
                    displayField: 'CarrierWarehouse',
                    store: storeWarehouseType,
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    forceSelection: true,
                    beforequery: function(record) {
                        record.query = new RegExp(record.query, 'i');
                        record.forceAll = true;
                    }
                }, {
                    columnWidth: 1,
                    xtype: 'combo',
                    name: 'JobCustShipKey',
                    fieldLabel: 'Ship Address',
                    labelWidth: 50,
                    valueField: 'ShipKey',
                    displayField: 'x_ShipAddress',
                    store: (storeCustomerShipAddress) ? storeCustomerShipAddress : null,
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    allowBlank: false,
                    forceSelection: true,
                    queryCaching: false,
                    listeners: {
                        beforequery: function(record) {
                            record.query = new RegExp(record.query, 'i');
                            record.forceAll = true;
                        }
                    }
                }, {
                    columnWidth: 0.5,
                    xtype: 'combo',
                    name: 'JobInspectorKey',
                    fieldLabel: 'Inspection',
                    labelWidth: 50,
                    valueField: 'InspectorKey',
                    displayField: 'InspectorName',
                    store: (storeInspectionCompanies) ? storeInspectionCompanies : null,
                    queryMode: 'local',
                    minChars: 2,
                    //allowBlank: true,
                    forceSelection: false,
                    queryCaching: false,
                    listeners: {
                        beforequery: function(record) {
                            record.query = new RegExp(record.query, 'i');
                            record.forceAll = true;
                        }
                    }
                }, {
                    xtype: 'textfield',
                    columnWidth: 0.5,
                    margin: '0 0 0 5',
                    fieldLabel: 'Num',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'JobInspectionNum',
                    //allowBlank: false
                    listeners: {
                        blur: function() {
                            //me.onSaveClick();
                        }
                    }
                }, {
                    xtype: 'textfield',
                    columnWidth: 1,
                    fieldLabel: 'DUI Number',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'JobDUINum',
                    //allowBlank: false
                    listeners: {
                        blur: function() {
                            //me.onSaveClick();
                        }
                    }
                }]
            }
            ],
            dockedItems: [{
                xtype: 'formtoolbar',
                itemId: 'FormToolbar',
                dock: 'top',
                store: me.storeNavigator,
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
                    beginedit: {
                        fn: me.onBeginEdit,
                        scope: me
                    }
                }
            }, {
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [{
                    xtype: 'textfield',
                    name: 'JobCreatedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created By'
                }, {
                    xtype: 'datefield',
                    name: 'JobCreatedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created Date',
                    hideTrigger: true
                }, {
                    xtype: 'textfield',
                    name: 'JobModifiedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified By'
                }, {
                    xtype: 'datefield',
                    name: 'JobModifiedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified Date',
                    hideTrigger: true
                }, {
                    xtype: 'component',
                    flex: 1
                }, {
                    xtype: 'button',
                    itemId: 'CreatePO',
                    text: 'Create PO',
                    listeners: {
                        click: {
                            fn: me.onClickCreatePO,
                            scope: me
                        }
                    }
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
        // var grid = me.down('gridpanel');
        // var theSelectionModel = grid.getSelectionModel();
        // theSelectionModel.onEditorKey = function(field, e) {
        //     var k = e.getKey(), newCell, g = theSelectionModel.grid, ed = g.activeEditor;
        //     if(k == e.ENTER && !e.ctrlKey){
        //         e.stopEvent();
        //         ed.completeEdit();
        //     }
        // };
    },

    onAfterLoadRecord: function(tool, record) {
        var me = this,
            toolbar = me.down("#FormToolbar"),
            btn = toolbar.down("#save");

        if (record.phantom) {
            Ext.Msg.wait('Loading data...', 'Wait');
            var storeWarehouseType = new CBH.store.vendors.WarehouseTypes().load({
                callback: function() {
                    field = me.down('field[name=JobWarehouseKey]').bindStore(storeWarehouseType);
                    storeWarehouseType.lastOptions.callback = null;
                    Ext.Msg.hide();
                }
            });

            btn.setTooltip('Save the general information previously to load roles');

            setTimeout(function() {
                Ext.Msg.alert('Remember', 'Save the general information previously to load roles');
            }, 1500);

            me.down("#CreatePO").setDisabled(true);
            return;
        }

        me.down("#CreatePO").setDisabled(false);

        var jobkey = record.data.JobKey,
            custkey = record.data.JobCustKey;

        curRec = record.data;

        me.down('field[name=x_JobCustKey]').setValue(custkey);

        Ext.Msg.wait('Loading data...', 'Wait');

        var storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function() {
                me.down('field[name=JobCustCurrencyCode]').bindStore(this);
                me.down('field[name=JobCustCurrencyCode]').setValue(curRec.JobCustCurrencyCode);

                storeCustomers = new CBH.store.customers.Customers().load({
                    params: {
                        id: custkey,
                        page: 0,
                        start: 0,
                        limit: 0
                    },
                    callback: function() {
                        me.down('field[name=JobCustKey]').bindStore(this);
                        me.down('field[name=JobCustKey]').setValue(custkey);

                        var storeCustomerContacts = new CBH.store.customers.CustomerContacts().load({
                            params: {
                                id: curRec.JobContactKey,
                                page: 0,
                                start: 0,
                                limit: 0
                            },
                            callback: function() {
                                me.down('field[name=JobContactKey]').bindStore(this);
                                me.down('field[name=JobContactKey]').setValue(curRec.JobContactKey);

                                var storeCustomerShipAddress = new CBH.store.customers.CustomerShipAddress().load({
                                    params: {
                                        custkey: curRec.JobCustKey,
                                        page: 0,
                                        start: 0,
                                        limit: 0
                                    },
                                    callback: function() {
                                        me.down('field[name=JobCustShipKey]').bindStore(this);
                                        me.down('field[name=JobCustShipKey]').setValue(curRec.JobCustShipKey);

                                        var storeJobEmployeeRoles = new CBH.store.jobs.JobEmployeeRoles().load({
                                            params: {
                                                jobkey: curRec.JobKey
                                            },
                                            callback: function() {
                                                var grid = me.down('gridpanel');
                                                grid.reconfigure(this);
                                                

                                                var storeWarehouseType = new CBH.store.vendors.WarehouseTypes().load({
                                                    callback: function() {
                                                        field = me.down('field[name=JobWarehouseKey]').bindStore(storeWarehouseType);
                                                        field.setValue(record.data.JobWarehouseKey);
                                                        Ext.Msg.hide();
                                                        storeWarehouseType.lastOptions.callback = null;
                                                    }
                                                });

                                                storeJobEmployeeRoles.lastOptions.callback = null;
                                            }
                                        });

                                        storeCustomerShipAddress.lastOptions.callback = null;
                                    }
                                });

                                storeCustomerContacts.lastOptions.callback = null;
                            }
                        });

                        storeCustomers.lastOptions.callback = null;
                    }
                });

                this.lastOptions.callback = null;
            }
        });

        if (!record.phantom) {
            var btn = me.down("#addline");
            btn.setDisabled(false);
        }
    },

    onGoToEntryClick: function() {
        var tabs = this.up('app_pageframe');

        var me = this;

        var jobkey = me.down('field[name=JobKey]').getValue();
        var jobnum = me.down('field[name=x_JobNumFormatted]').getValue();

        Ext.Msg.wait('Loading Data...', 'Wait');

        var storeToNavEmpty = new CBH.store.sales.JobQuoteDetail();

        var storeGoEntry = new CBH.store.sales.JobQuoteDetail().load({
            scope: storeGoEntry,
            params: {
                jobkey: jobkey,
                page: 1,
                start: 0,
                limit: 8
            },
            callback: function() {

                // call form
                var form = Ext.widget("joblineentry", {
                    storeNavigator: (this.totalCount === 0) ? storeToNavEmpty : storeGoEntry,
                    JobKey: jobkey,
                    JobNum: jobnum
                });

                // create tab panel
                var tab = tabs.add({
                    closable: true,
                    iconCls: 'tabs',
                    autoScroll: true,
                    title: 'Line Entry',
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

                if (this.totalCount === 0) {
                    var btn = form.down('#FormToolbar').down('#add');
                    btn.fireEvent('click', btn);
                } else {
                    form.down('#FormToolbar').gotoAt(1);
                }
            }
        });
    },

    onAddClick: function(toolbar, record) {
        var me = this,
            grid = me.down('#gridroles');

        grid.store.removeAll();

        record.data.JobCustCurrencyCode = 'USD';
        record.data.JobCustCurrencyRate = 1;

        me.down('field[name=JobProdDescription]').focus(true, 200);
    },

    onBeginEdit: function(toolbar, record) {
        var me = toolbar.up('form');
    },

    onSaveClick: function(toolbar, record) {
        var me = this,
            form = me.getForm();

        if (!form.isValid()) {
            Ext.Msg.alert("Validation", "Check data for valid input!!!");
            return false;
        }

        form.updateRecord();

        var savedRecord = form.getRecord();

        if (savedRecord.data.JobKey !== 0) {
            savedRecord.set('JobModifiedBy', CBH.GlobalSettings.getCurrentUserName());
        }

        Ext.Msg.wait('Saving Record...', 'Wait');

        savedRecord.save({
            callback: function(records, operation, success) {
                if (success) {
                    Ext.Msg.hide();
                    toolbar.doRefresh();
                } else {
                    Ext.Msg.hide();
                }
            }
        });
    },

    onDeleteClick: function(toolbar, record) {
        if (record) {
            var curRec = record.index - 1;
            curPage = toolbar.store.currentPage;
            prevRec = (curRec <= 0) ? 1 : curRec;

            Ext.Msg.wait('Deleting Record...', 'Wait');
            record.destroy({
                success: function() {
                    var lastOpt = toolbar.store.lastOptions,
                        params = (lastOpt) ? lastOpt.params : null;

                    toolbar.store.reload({
                        params: params,
                        callback: function() {}
                    });
                    if (toolbar.store.getCount() > 0) {
                        toolbar.gotoAt(prevRec);
                    } else {
                        toolbar.up('form').up('panel').destroy();
                    }
                    Ext.Msg.hide();
                },
                failure: function() {
                    Ext.Msg.hide();
                }
            });
        }
    },

    onDestroy: function(e, eOpts) {
        //...
    },

    // Event Blur CustKey Field
    onCustomerBlur: function(field, The, eOpts) {
        var me = field.up('panel');

        if (field.value !== null && field.valueModels[0]) {

            var currentCust = field.value,
                fieldContact = me.down('field[name=JobContactKey]');

            me.getEl().mask('Loading customer\'s contacts...');
            var contacts = new CBH.store.customers.CustomerContacts().load({
                params: {
                    page: 0,
                    start: 0,
                    limit: 0,
                    custkey: currentCust
                },
                callback: function() {
                    fieldContact.getStore().removeAll();
                    fieldContact.bindStore(contacts);

                    if (this.totalCount > 0 && String.isNullOrEmpty(fieldContact.getValue())) {
                        var record = this.getAt(0);
                        fieldContact.setValue(record.data.ContactKey);
                        me.getEl().unmask();
                    }
                }
            });

            var fieldShip = me.down('field[name=JobCustShipKey]');
            var address = new CBH.store.customers.CustomerShipAddress().load({
                params: {
                    page: 0,
                    start: 0,
                    limit: 0,
                    custkey: currentCust
                },
                callback: function() {
                    fieldShip.getStore().removeAll();
                    fieldShip.bindStore(address);

                    if (this.totalCount > 0 && String.isNullOrEmpty(fieldShip.getValue())) {
                        var index = this.find('ShipDefault', true);
                        index = (index < 0) ? 0 : index;
                        var record = this.getAt(index);
                        fieldShip.setValue(record.data.ShipKey);
                    }
                }
            });
        } else {
            var rawvalue = field.getRawValue();
            Ext.Msg.show({
                title: 'Question',
                msg: 'The customer doesn\'t exists, Do you want to add to database?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(e) {
                    if (e === "yes") {
                        me.addCustomer(rawvalue);
                    } else {
                        field.setValue(null);
                        field.focus(true, 200);
                    }
                }
            });
        }
    },

    addCustomer: function(value) {
        var me = this;

        storeToNavigate = new CBH.store.customers.Customers({
            autoLoad: false
        });
        model = Ext.create('CBH.model.customers.Customers', {
            CustName: value,
            CustStatus: 1
        });
        storeToNavigate.add(model);
        var form = Ext.widget('customers', {
            storeNavigator: storeToNavigate,
            modal: true,
            width: 700,
            frameHeader: true,
            header: true,
            layout: {
                type: 'column'
            },
            title: 'New Customer',
            bodyPadding: 10,
            closable: true,
            //constrain: true,
            stateful: false,
            floating: true,
            callerForm: me,
            forceFit: true
        });

        form.show();

        var btn = form.down('#FormToolbar').down('#add');
        btn.fireEvent('click', btn, null, null, model); // aditional param model for send model data to click event handler
    },

    onContactBlur: function(field, The, eOpts) {
        var form = field.up('panel');

        if (field.value !== null && field.valueModels[0]) {

        } else if (form.down('field[name=JobCustKey]').value !== null) {
            Ext.Msg.show({
                title: 'Question',
                msg: 'The contact doesn\'t exists, Do you want to add to database?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(e) {
                    if (e === "yes") {
                        form.addContact(field);
                    } else {
                        field.setValue(null);
                        field.focus(true, 200);
                    }
                },
                scope: field
            });
        }
    },

    addContact: function(field) {
        var me = this,
            rawvalue = field.getRawValue(),
            res = rawvalue.split(' '),
            firstName = (res[0]) ? res[0] : '',
            lastName = (res[1]) ? res[1] : '',
            custkey = me.down('field[name=JobCustKey]').getValue();

        record = Ext.create('CBH.model.customers.CustomerContacts', {
            ContactCustKey: custkey,
            ContactFirstName: firstName,
            ContactLastName: lastName
        });

        var form = Ext.create('CBH.view.customers.CustomerContacts');
        form.loadRecord(record);
        form.center();
        form.callerForm = me;
        form.show();
    },

    onShipAddressBlur: function(field, The, eOpts) {
        var form = field.up('panel');

        if (field.value !== null && field.valueModels[0]) {

        } else if (form.down('field[name=JobCustKey]').value !== null) {
            Ext.Msg.show({
                title: 'Question',
                msg: 'The ship address doesn\'t exists, Do you want to add to database?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(e) {
                    if (e === "yes") {
                        form.addShipAddress(field);
                    } else {
                        field.setValue(null);
                        field.focus(true, 200);
                    }
                },
                scope: field
            });
        }
    },

    addShipAddress: function(field) {
        var me = this,
            rawvalue = field.getRawValue();

        var custkey = me.down('field[name=JobCustKey]').getValue();
        record = Ext.create('CBH.model.customers.CustomerShipAddress', {
            ShipCustKey: custkey
        });

        var form = new CBH.view.customers.CustomerShipAddress();
        form.loadRecord(record);
        form.center();
        form.callerForm = me;
        form.show();
    },

    onClickCreatePO: function() {
        var me = this,
            tabs = me.up('app_pageframe'),
            toolbar = me.down('#FormToolbar');

        selection = toolbar.getCurrentRecord();

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
                        POWarehouseKey: selection.data.JobWarehouseKey,
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

    refreshData: function() {
        var me = this,
            grids = me.query("gridpanel"),
            combos = me.query("combobox");

        Ext.each(grids, function(grid, index) {
           var store = grid.getStore(); 
           if(store)
                store.reload();
        });

        Ext.each(combos, function(combo, index) {
            var store = combo.getStore(); 
            if(store)
                store.reload();
        });
    }

});
