Ext.define('CBH.view.common.PaymentTerms', {
    extend: 'Ext.form.Panel',
    alias: 'widget.paymentterms',

    layout: {
        type: 'column'
    },
    bodyPadding: 10,
    frameHeader: false,
    header: false,
    enableKeyEvents: true,

    storeNavigator: null,

    TermKey: 0,

    requires: [
        'Ext.ux.form.NumericField'
    ],

    initComponent: function() {
        var me = this;

        storeLangs = new CBH.store.common.Languages().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });
        storeDesc = null;

        rowEditing = new Ext.grid.plugin.RowEditing({
            clicksToMoveEditor: 2,
            autoCancel: false,
            errorSummary: false,
            listeners: {
                beforeedit: {
                    delay: 500,
                    fn: function(item, e) {
                        this.getEditor().onFieldChange();
                    }
                },
                cancelEdit: {
                    fn: function(rowEditing, context) {
                        var grid = rowEditing.editor.up('gridpanel');
                        // Canceling editing of a locally added, unsaved record: remove it
                        if (context.record.phantom) {
                            grid.store.remove(context.record);
                        }
                    }
                },
                edit: {
                    fn: function(rowEditing, context) {
                        var grid = rowEditing.editor.up('gridpanel'),
                            record = context.record;
                        record.save({
                            callback: function(records, operation, success) {
                                if (success) {
                                    grid.store.reload();
                                }
                            }
                        });
                    }
                }
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
            items: [{
                xtype: 'fieldset',
                columnWidth: 0.3,
                layout: {
                    type: 'column'
                },
                padding: '0 10 10 10',
                collapsible: true,
                title: 'Payment Terms Information',
                items: [{
                    xtype: 'textfield',
                    name: 'TermKey',
                    fieldLabel: 'Internal Reference',
                    columnWidth: 1,
                    //allowBlank: false,
                    readOnly: true,
                    editable: false
                }, {
                    xtype: 'numericfield',
                    columnWidth: 1,
                    name: 'TermPercentPrepaid',
                    fieldLabel: '% Prepaid',
                    fieldStyle: 'text-align: right;',
                    minValue: 0,
                    hideTrigger: true,
                    useThousandSeparator: true,
                    decimalPrecision: 2,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: true,
                    thousandSeparator: ','
                }, {
                    xtype: 'numericfield',
                    columnWidth: 1,
                    name: 'TermPercentWithOrder',
                    fieldLabel: '% With Order',
                    fieldStyle: 'text-align: right;',
                    minValue: 0,
                    hideTrigger: true,
                    useThousandSeparator: true,
                    decimalPrecision: 2,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: true,
                    thousandSeparator: ','
                }, {
                    xtype: 'numericfield',
                    columnWidth: 1,
                    name: 'TermPercentPriorToShip',
                    fieldLabel: '% Prior To Ship',
                    fieldStyle: 'text-align: right;',
                    minValue: 0,
                    hideTrigger: true,
                    useThousandSeparator: true,
                    decimalPrecision: 2,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: true,
                    thousandSeparator: ','
                }, {
                    xtype: 'numericfield',
                    columnWidth: 1,
                    name: 'TermPercentAgainstShipDocs',
                    fieldLabel: '% Against Ship Docs',
                    fieldStyle: 'text-align: right;',
                    minValue: 0,
                    hideTrigger: true,
                    useThousandSeparator: true,
                    decimalPrecision: 2,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: true,
                    thousandSeparator: ','
                }, {
                    xtype: 'numericfield',
                    columnWidth: 1,
                    name: 'TermPercentNet',
                    fieldLabel: '% Net x Days',
                    fieldStyle: 'text-align: right;',
                    minValue: 0,
                    hideTrigger: true,
                    useThousandSeparator: true,
                    decimalPrecision: 2,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: true,
                    thousandSeparator: ','
                }, {
                    xtype: 'numericfield',
                    columnWidth: 1,
                    name: 'TermPercentDays',
                    fieldLabel: 'Days',
                    fieldStyle: 'text-align: right;',
                    minValue: 0,
                    hideTrigger: true,
                    useThousandSeparator: true,
                    decimalPrecision: 0,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: true,
                    thousandSeparator: ','
                }]
            }, {
                columnWidth: 0.7,
                xtype: 'panel',
                title: 'Descriptions',
                margin: '0 0 0 10',
                items: [{
                    xtype: 'gridpanel',
                    itemId: 'griddesc',
                    minHeight: 200,
                    store: storeDesc,
                    maxHeight: 600,
                    columns: [{
                        xtype: 'rownumberer',
                        format: '00,000'
                    }, {
                        xtype: 'gridcolumn',
                        text: 'Language',
                        dataIndex: 'x_Language',
                        flex: 2,
                        editor: {
                            xtype: 'combo',
                            displayField: 'LanguageName',
                            valueField: 'LanguageCode',
                            name: 'PTLanguageCode',
                            enableKeyEvents: true,
                            forceSelection: true,
                            queryMode: 'local',
                            selectOnFocus: true,
                            emptyText: 'Choose Language',
                            allowBlank: false,
                            listeners: {
                                select: function(field, records, eOpts) {
                                    var form = field.up('panel'),
                                        record = form.context.record;
                                    if (records.length > 0) {
                                        record.set('x_Language', records[0].data["LanguageName"]);
                                    }
                                },
                                change: function(field) {
                                    var form = field.up('panel');
                                    form.onFieldChange();
                                },
                                beforequery: function(record) {
                                    record.query = new RegExp(record.query, 'i');
                                    record.forceAll = true;
                                }
                            },
                            store: storeLangs
                        }
                    }, {
                        xtype: 'gridcolumn',
                        text: 'Description',
                        dataIndex: 'PTDescription',
                        flex: 8,
                        editor: {
                            xtype: 'textfield',
                            name: 'PTDescription',
                            allowBlank: false,
                            listeners: {
                                change: function(field) {
                                    var form = field.up('panel');
                                    form.onFieldChange();
                                }
                            }
                        }
                    }],
                    tbar: [{
                        xtype: 'component',
                        flex: 1
                    }, {
                        text: 'Add',
                        itemId: 'adddesc',
                        handler: function() {
                            rowEditing.cancelEdit();

                            var grid = this.up('gridpanel');
                            var parentkey = me.down('#FormToolbar').getCurrentRecord().data.TermKey;

                            // Create a model instance
                            var r = Ext.create('CBH.model.common.PaymentTermsDescriptions', {
                                PTTermKey: parentkey
                            });

                            var count = grid.getStore().count();
                            grid.store.insert(count, r);
                            rowEditing.startEdit(count, 1);
                        },
                        disabled: true
                    }, {
                        itemId: 'deletedesc',
                        text: 'Delete',
                        handler: function() {
                            var grid = this.up('gridpanel'),
                                sm = grid.getSelectionModel();

                            rowEditing.cancelEdit();

                            selection = sm.getSelection();

                            if (selection) {
                                selection[0].destroy({
                                    success: function() {
                                        grid.store.reload();
                                    }
                                });
                            }
                        },
                        disabled: true
                    }],
                    selType: 'rowmodel',
                    plugins: [rowEditing],
                    listeners: {
                        selectionchange: function(view, records) {
                            this.down('#deletedesc').setDisabled(!records.length);
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
                    xtype: 'checkbox',
                    columnWidth: 1,
                    name: 'TermWarningFlag',
                    labelSeparator: '',
                    hideLabel: true,
                    boxLabel: 'Mark this box if this Payment Term should raise warning flags when used',
                    boxLabelCls: 'x-form-cb-label-override'
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
                },
                close: function() {

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
        var toolbar = me.down('#FormToolbar');

        if (toolbar.store.getCount() === 1 && toolbar.store.getAt(0).phantom) {
            toolbar.items.items.forEach(function(btn) {
                btn.setVisible(false);
            });
            toolbar.down('#save').setVisible(true);
        }

        var field = me.down('field[name=TermPercentPrepaid]');
        field.focus(true, 200);
    },

    onAfterLoadRecord: function(tool, record) {
        var me = this;
        var parentkey = (record.data.TermKey);

        if (record.phantom) {
            me.down('#adddesc').setDisabled(true);
        } else {
            me.down('#adddesc').setDisabled(false);
        }

        parentkey = (parentkey === 0) ? -1 : parentkey;
        curRec = record;
        Ext.Msg.wait('Loading Descriptions', 'Wait');
        storeDesc = new CBH.store.common.PaymentTermsDescriptions().load({
            params: {
                page: 0,
                start: 0,
                limit: 0,
                termkey: parentkey
            },
            callback: function() {

                var grid = me.down('#griddesc');
                grid.reconfigure(this);
                Ext.Msg.hide();
            }
        });
    },

    onAddClick: function(toolbar, record) {
        var me = this,
            grid = me.down('#griddesc');

        grid.store.removeAll();

        me.down('#adddesc').setDisabled(true);
        me.down('#FormToolbar').down('#add').setDisabled(true);
    },

    onBeginEdit: function(toolbar, record) {
        var me = this;
        me.down('field[name=TermPercentPrepaid]').focus(true, 200);
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

        Ext.Msg.wait('Saving Record...', 'Wait');

        var isdirty = record.dirty;

        record.save({
            callback: function(records, operation, success) {
                me.down('#FormToolbar').down('#add').setDisabled(false);
                me.down('#adddesc').setDisabled(false);

                if (success) {
                    me.loadRecord(record);
                    me.TermKey = record.data.TermKey;
                    storeDesc = new CBH.store.common.PaymentTermsDescriptions().load({
                        params: {
                            page: 0,
                            start: 0,
                            limit: 0,
                            termkey: me.TermKey
                        },
                        callback: function() {
                            var grid = me.down('#griddesc');
                            grid.reconfigure(this);
                            me.down('#adddesc').setDisabled(false);
                            Ext.Msg.hide();
                            toolbar.doRefresh();
                        }
                    });
                } else {
                    Ext.Msg.hide();
                }
            }
        });
    },

    onDeleteClick: function(pageTool, record) {

        if (record) {
            var curRec = record.index - 1;
            curPage = pageTool.store.currentPage;
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
                            callback: function(records, operation, success) {
                                Ext.Msg.hide();
                                var lastOpt = pageTool.store.lastOptions;
                                pageTool.store.reload({
                                    params: lastOpt.params,
                                    callback: function() {}
                                });
                                if (pageTool.store.getCount() > 0) {
                                    pageTool.gotoAt(prevRec);
                                } else {
                                    pageTool.up('form').up('panel').destroy();
                                }
                            },
                            failure: function() {
                                Ext.Msg.hide();
                            }
                        });
                    }
                }
            }).defaultButton = 2;
        }
    }
});
