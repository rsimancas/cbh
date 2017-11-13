Ext.define('CBH.view.sales.FileForm', {
    extend: 'Ext.form.Panel',
    alias: 'widget.fileform',

    layout: {
        type: 'column'
    },
    bodyPadding: 10,
    frameHeader: false,
    header: false,
    title: 'Customer Information',

    requires: [
        'CBH.view.customers.Customers',
        'CBH.view.customers.CustomerContacts',
        'CBH.view.customers.CustomerShipAddress'
    ],

    FileKey: 0,
    FileCustKey: 0,
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

        storeCustomerContacts = null;
        storeCustomerShipAddress = null;
        storeEmployeeRoles = null;
        storeEmployeeRolesGrid = new CBH.store.common.Employees().load({
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
        });

        CBH.AppEvents.on("EmployeesChanged", function() {
            this.reload();
        }, storeEmployeeRolesGrid);

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
                columnWidth: 0.55,
                margin: '0 5 5 0',
                defaults: {
                    anchor: '100%',

                },
                collapsible: true,
                title: 'Customer Information',
                items: [{
                    xtype: 'fieldcontainer',
                    layout: {
                        type: 'hbox'
                    },
                    items: [{
                        xtype: 'displayfield',
                        itemId: 'FileKey',
                        name: 'FileKey',
                        hideTrigger: true,
                        fieldLabel: 'FileKey'
                    }, {
                        xtype: 'component',
                        flex: 8
                    }, {
                        xtype: 'displayfield',
                        name: 'x_FileNumFormatted',
                        itemId: 'x_FileNumFormatted',
                        fieldLabel: 'Reference Num',
                        fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: right'
                    }, {
                        margin: '0 0 0 5',
                        xtype: 'displayfield',
                        name: 'x_FileCustKey',
                        fieldLabel: 'Customer Code',
                        fieldStyle: 'font-size: 14px; color: #157fcc;font-weight:bold; text-align: right'
                    }]
                }, {
                    xtype: 'fieldcontainer',
                    width: 310,
                    layout: {
                        type: 'column'
                    },
                    items: [{
                        xtype: 'combo',
                        name: 'FileQuoteEmployeeKey',
                        itemId: 'FileQuoteEmployeeKey',
                        columnWidth: 0.5,
                        forceSelection: true,
                        fieldLabel: 'Sales Employee',
                        labelWidth: 50,
                        anchor: '100%',
                        store: new CBH.store.common.Employees().load({
                            params: {
                                page: 0,
                                start: 0,
                                limit: 0
                            }
                        }),
                        valueField: 'EmployeeKey',
                        displayField: 'x_EmployeeFullName',
                        queryMode: 'local',
                        minChars: 1,
                        allowBlank: false,
                        selectOnFocus: true,
                        emptyText: 'Choose Employee',
                        //enableKeyEvents: true,
                        listeners: {
                            beforequery: function(record) {
                                record.query = new RegExp(record.query, 'i');
                                record.forceAll = true;
                            }
                        }
                    }, {
                        xtype: 'combo',
                        itemId: 'FileOrderEmployeeKey',
                        name: 'FileOrderEmployeeKey',
                        fieldLabel: 'Order Employee',
                        columnWidth: 0.5,
                        margin: '0 0 0 10',
                        labelWidth: 50,
                        anchor: '100%',
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
                            },
                            synchronous: true
                        }),
                        valueField: 'EmployeeKey',
                        displayField: 'x_EmployeeFullName',
                        queryMode: 'local',
                        minChars: 2,
                        allowBlank: false,
                        forceSelection: true,
                        selectOnFocus: true,
                        emptyText: 'Choose Employee',
                        listeners: {
                            beforequery: function(record) {
                                record.query = new RegExp(record.query, 'i');
                                record.forceAll = true;
                            }
                        }
                    }]
                }, {
                    xtype: 'fieldcontainer',
                    anchor: '100%',
                    layout: 'anchor',
                    items: [{
                        xtype: 'combo',
                        name: 'FileCustKey',
                        itemId: 'FileCustKey',
                        fieldLabel: 'Customer',
                        labelWidth: 50,
                        anchor: '100%',
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
                        itemId: 'FileContactKey',
                        name: 'FileContactKey',
                        fieldLabel: '- Contact',
                        labelWidth: 50,
                        anchor: '100%',
                        valueField: 'ContactKey',
                        displayField: 'x_ContactFullName',
                        store: storeCustomerContacts,
                        queryMode: 'local',
                        //autoSelect: false,
                        typeAhead: false,
                        minChars: 2,
                        allowBlank: false,
                        forceSelection: false,
                        //triggerAction: '',
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
                        xtype: 'combo',
                        itemId: 'FileCustShipKey',
                        name: 'FileCustShipKey',
                        fieldLabel: 'Ship Address',
                        labelWidth: 50,
                        anchor: '100%',
                        valueField: 'ShipKey',
                        displayField: 'x_ShipAddress',
                        store: (storeCustomerShipAddress) ? storeCustomerShipAddress : null,
                        queryMode: 'local',
                        minChars: 2,
                        allowBlank: false,
                        forceSelection: false,
                        queryCaching: false,
                        listeners: {
                            beforequery: function(record) {
                                record.query = new RegExp(record.query, 'i');
                                record.forceAll = true;
                            },
                            blur: {
                                fn: me.onShipAddressBlur,
                                scope: this
                            }
                        }
                    }, {
                        xtype: 'textfield',
                        anchor: '100%',
                        fieldLabel: 'Reference',
                        labelAlign: 'top',
                        labelWidth: 50,
                        name: 'FileReference',
                    }]
                }, {
                    xtype: 'fieldcontainer',
                    anchor: '100%',
                    layout: 'column',
                    items: [{
                        xtype: 'combo',
                        name: 'FileDefaultCurrencyCode',
                        itemId: 'FileDefaultCurrencyCode',
                        fieldLabel: 'Default Currency',
                        store: storeCurrencyRates,
                        labelWidth: 50,
                        columnWidth: 0.5,
                        valueField: 'CurrencyCode',
                        displayField: 'CurrencyCodeDesc',
                        queryMode: 'local',
                        minChars: 2,
                        allowBlank: false,
                        forceSelection: true,
                        /*tpl: Ext.create('Ext.XTemplate',
                            '<tpl for=".">',
                            '<div class="x-bound-list-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                            '</tpl>'),*/
                        listeners: {
                            blur: function(field, The, eOpts) {
                                if (field.value !== null) {
                                    var form = field.up('panel');

                                    copyToField = field.valueModels[0].data.CurrencyRate;
                                    copyField = form.down('#FileDefaultCurrencyRate');
                                    copyField.setValue(copyToField);
                                }
                            },
                            beforequery: function(record) {
                                record.query = new RegExp(record.query, 'i');
                                record.forceAll = true;
                            }
                        },
                    }, {
                        xtype: 'numericfield',
                        name: 'FileDefaultCurrencyRate',
                        itemId: 'FileDefaultCurrencyRate',
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
                    }]
                }, {
                    xtype: 'fieldcontainer',
                    anchor: '100%',
                    layout: 'column',
                    items: [{
                        xtype: 'datefield',
                        columnWidth: 0.3,
                        fieldLabel: 'Date Rqd. by Customer',
                        labelAlign: 'top',
                        labelWidth: 50,
                        name: 'FileDateCustRequired',
                        //format: 'm/d/Y',
                        //allowBlank: false
                    }, {
                        xtype: 'textfield',
                        columnWidth: 0.7,
                        margin: '0 0 0 10',
                        fieldLabel: 'Date Rqd. by Cust. Note',
                        labelAlign: 'top',
                        labelWidth: 50,
                        name: 'FileDateCustRequiredNote',
                        //allowBlank: false
                        listeners: {
                            blur: function() {
                                //me.onSaveClick();
                            }
                        }
                    }]
                }]
            }, {
                xtype: 'container',
                columnWidth: 0.45,
                items: [{
                    xtype: 'gridpanel',
                    itemId: 'gridroles',
                    title: 'Roles',
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
                            name: 'FileEmployeeRoleKey',
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
                                        record.set('FileEmployeeEmployeeKey', field.value);
                                    }
                                },
                                beforequery: function(record) {
                                    record.query = new RegExp(record.query, 'i');
                                    record.forceAll = true;
                                }
                            },
                            store: storeEmployeeRolesGrid
                        }
                    }],
                    tbar: [{
                        text: 'Add',
                        itemId: 'addline',
                        handler: function() {
                            var form = this.up('panel').up('panel');
                            rowEditing.cancelEdit();

                            // Create a model instance
                            var r = Ext.create('CBH.model.sales.FileEmployeeRoles', {
                                FileEmployeeFileKey: form.down('field[name=FileKey]').getValue()
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
            }],
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
                    itemId: 'FileCreatedBy',
                    name: 'FileCreatedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created By'
                }, {
                    xtype: 'datefield',
                    name: 'FileCreatedDate',
                    itemId: 'FileCreatedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Created Date',
                    hideTrigger: true
                }, {
                    xtype: 'textfield',
                    name: 'FileModifiedBy',
                    itemId: 'FileModifiedBy',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified By'
                }, {
                    xtype: 'datefield',
                    name: 'FileModifiedDate',
                    itemId: 'FileModifiedDate',
                    readOnly: true,
                    editable: false,
                    fieldLabel: 'Modified Date',
                    hideTrigger: true
                }, {
                    xtype: 'component',
                    flex: 1
                }, {
                    xtype: 'button',
                    itemId: 'orderentrybutton',
                    text: 'Go to Order Entry',
                    disabled: true,
                    listeners: {
                        click: {
                            fn: me.onGoToOrderEntryClick,
                            scope: me
                        }
                    }
                }, {
                    margin: '0 10 0 5',
                    xtype: 'button',
                    itemId: 'entrybutton',
                    text: 'Go to Details Entry',
                    disabled: true,
                    listeners: {
                        click: {
                            fn: me.onGoToEntryClick,
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
                    if (toolbar.isEditing) {
                        var btn = toolbar.down('#save');
                        btn.fireEvent('click');
                    }
                }
            },
            this);
    },

    onRenderForm: function() {
        var me = this;

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
        var me = this;

        if (record.phantom) {
            me.down('#entrybutton').setDisabled(true);
            me.down('#orderentrybutton').setDisabled(true);
            return;
        }

        me.down('#entrybutton').setDisabled(false);
        me.down('#orderentrybutton').setDisabled(false);
        me.down('field[name=x_FileCustKey]').setValue(record.data.FileCustKey);

        var filekey = record.data.FileKey,
            custkey = record.data.FileCustKey;

        curRec = record.data;

        Ext.Msg.wait('Loading data...', 'Wait');

        storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function() {
                me.down('field[name=FileDefaultCurrencyCode]').bindStore(this);
                me.down('field[name=FileDefaultCurrencyCode]').setValue(curRec.FileDefaultCurrencyCode);

                var storeCustomers = new CBH.store.customers.Customers().load({
                    params: {
                        id: custkey,
                        page: 0,
                        start: 0,
                        limit: 0
                    },
                    callback: function() {
                        me.down('field[name=FileCustKey]').bindStore(this);
                        me.down('field[name=FileCustKey]').setValue(custkey);

                        var storeCustomerContacts = new CBH.store.customers.CustomerContacts().load({
                            params: {
                                id: curRec.FileContactKey,
                                page: 0,
                                start: 0,
                                limit: 0
                            },
                            callback: function() {
                                me.down('field[name=FileContactKey]').bindStore(this);
                                me.down('field[name=FileContactKey]').setValue(curRec.FileContactKey);

                                var storeCustomerShipAddress = new CBH.store.customers.CustomerShipAddress().load({
                                    params: {
                                        id: curRec.FileCustShipKey,
                                        page: 0,
                                        start: 0,
                                        limit: 0
                                    },
                                    callback: function() {
                                        me.down('field[name=FileCustShipKey]').bindStore(this);
                                        me.down('field[name=FileCustShipKey]').setValue(curRec.FileCustShipKey);

                                        var storeFileEmployeeRoles = new CBH.store.sales.FileEmployeeRoles().load({
                                            params: {
                                                filekey: curRec.FileKey
                                            },
                                            callback: function() {
                                                var grid = me.down('gridpanel');
                                                grid.reconfigure(this);
                                                Ext.Msg.hide();
                                                storeFileEmployeeRoles.lastOptions.callback = null;
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
            var btn = me.down('#entrybutton');
            btn.setDisabled(false);
            btn = me.down("#addline");
            btn.setDisabled(false);
        }
    },

    onGoToEntryClick: function() {
        var tabs = this.up('app_pageframe');

        var me = this;

        var filekey = me.down('#FileKey').getValue();
        var filenum = me.down('#x_FileNumFormatted').getValue();

        Ext.Msg.wait('Loading Data...', 'Wait');

        var storeToNavEmpty = new CBH.store.sales.FileQuoteDetail();

        var storeGoEntry = new CBH.store.sales.FileQuoteDetail().load({
            scope: storeGoEntry,
            params: {
                filekey: filekey,
                page: 1,
                start: 0,
                limit: 8
            },
            callback: function() {

                // call form
                var form = Ext.widget("filelineentry", {
                    storeNavigator: (this.totalCount === 0) ? storeToNavEmpty : storeGoEntry,
                    FileKey: filekey,
                    FileNum: filenum
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

        //me.down('field[name=FileOrderEmployeeKey]').focus(true, 200);
    },

    onBeginEdit: function(toolbar, record) {
        var me = toolbar.up('form');
        me.down('field[name=FileOrderEmployeeKey]').focus(true, 200);
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

        if (savedRecord.data.FileKey !== 0) {
            savedRecord.set('FileModifiedBy', CBH.GlobalSettings.getCurrentUserName());
        }

        Ext.Msg.wait('Saving Record...', 'Wait');

        var phantom = savedRecord.phantom;

        savedRecord.save({
            callback: function(records, operation, success) {
                if (success) {
                    Ext.Msg.hide();
                    toolbar.doRefresh();
                    if(phantom)
                        CBH.AppEvents.fireEvent("printqueue");
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

            Ext.Msg.show({
                title: 'Delete',
                msg: 'Do you want to delete?',
                buttons: Ext.Msg.YESNO,
                icon: Ext.Msg.QUESTION,
                fn: function(btn) {
                    if (btn === "yes") {
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
                }
            }).defaultButton = 2;
        }
    },

    onDestroy: function(e, eOpts) {
        //...
    },

    // Event Blur CustKey Field
    onCustomerBlur: function(field, The, eOpts) {
        var form = field.up('panel');

        if (field.value !== null && field.valueModels[0]) {

            var currentCust = field.value;

            var fieldContact = form.down('#FileContactKey');
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
                    }
                }
            });

            var fieldShip = form.down('#FileCustShipKey');
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
                        form.addCustomer(rawvalue);
                    } else {
                        field.setValue(null);
                        field.focus(true, 200);
                    }
                }
            }).defaultButton = 2;
        }
    },

    addCustomer: function(value) {
        var me = this;

        var storeToNavigate = new CBH.store.customers.Customers({
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

        } else if (form.down('field[name=FileCustKey]').value !== null) {
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
            custkey = me.down('field[name=FileCustKey]').getValue();

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

        } else if (form.down('field[name=FileCustKey]').value !== null) {
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

        var custkey = me.down('field[name=FileCustKey]').getValue();
        record = Ext.create('CBH.model.customers.CustomerShipAddress', {
            ShipCustKey: custkey
        });

        var form = new CBH.view.customers.CustomerShipAddress();
        form.loadRecord(record);
        form.center();
        form.callerForm = me;
        form.show();
    },

    onGoToOrderEntryClick: function() {
        var me = this,
            tabs = me.up('app_pageframe');

        var fileKey = me.down('field[name=FileKey]').getValue();
        var fileNum = me.down('field[name=x_FileNumFormatted]').getValue();
        var customer = me.down('field[name=FileCustKey]').getRawValue();

        var form;

        var tab = tabs.add({
            closable: true,
            iconCls: 'tabs',
            autoScroll: true,
            title: 'Order Entry',
            items: [{
                xtype: 'container',
                layout: {
                    type: 'anchor'
                },
                items: [
                    form = Ext.widget('fileorderentry', {
                        FileKey: fileKey,
                        FileNum: fileNum,
                        Customer: customer,
                        CustKey: me.down('field[name=FileCustKey]').getValue()
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

        tabs.setActiveTab(tab.getId());
    }
});
