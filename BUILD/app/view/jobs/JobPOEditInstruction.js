Ext.define('CBH.view.jobs.JobPOEditInstruction', {
    extend: 'Ext.form.Panel',
    alias: 'widget.jobpoeditinstruction',
    modal: true,
    width: 400,
    layout: {
        type: 'absolute'
    },
    title: 'Instructions / Notes',
    bodyPadding: 10,
    closable: true,
    floating: true,
    callerForm: "",

    initComponent: function() {

        var me = this;

        Ext.Msg.wait('Wait', 'Loading...');
        var storeInstructions = new CBH.store.jobs.tlkpJobPurchaseOrderInstructions().load({
            params: {
                lang:'en'
            },
            callback: function() {
                Ext.Msg.hide();
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
                xtype: 'fieldcontainer',
                layout: {
                    type: 'fit'
                },
                items: [{
                    xtype: 'numericfield',
                    columnWidth: 1,
                    name: 'POInstructionsStep',
                    fieldLabel: 'Sort',
                    fieldStyle: 'text-align: right;',
                    minValue: 1,
                    hideTrigger: false,
                    useThousandSeparator: true,
                    decimalPrecision: 0,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: false,
                    thousandSeparator: ',',
                    step: 1
                }, {
                    xtype: 'combo',
                    columnWidth: 1,
                    fieldLabel: 'Instructions',
                    name: 'POInstructionsInstructionKey',
                    displayField: 'x_ITextMemo',
                    valueField: 'InstructionKey',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 2,
                    forceSelection: true,
                    store: storeInstructions,
                    emptyText: 'Choose Instructions',
                    listeners: {
                        beforequery: function(record) {
                            record.query = new RegExp(record.query, 'i');
                            record.forceAll = true;
                        }
                    }
                }, {
                    xtype: 'textareafield',
                    columnWidth: 1,
                    fieldLabel: 'Notes',
                    labelAlign: 'top',
                    labelWidth: 50,
                    name: 'POInstructionsMemo',
                    allowBlank: true,
                    listeners: {
                        blur: function() {
                            //me.onSaveChangesClick();
                        }
                    }
                }, {
                    margin: '0 0 15 0',
                    columnWidth: 1,
                    xtype: 'combo',
                    displayField: 'name',
                    valueField: 'id',
                    fieldLabel: 'Notes Font Color',
                    name: 'POInstructionsMemoFontColor',
                    queryMode: 'local',
                    typeAhead: true,
                    minChars: 1,
                    forceSelection: true,
                    emptyText: 'choose color',
                    enableKeyEvents: true,
                    autoSelect: true,
                    selectOnFocus: true,
                    defaultValue: 0,
                    store: {
                        fields: ['id', 'name'],
                        data: [{
                            'id': 0,
                            'name': 'Black'
                        }, {
                            'id': 1,
                            'name': 'Green'
                        }, {
                            'id': 2,
                            'name': 'Blue'
                        }, {
                            'id': 3,
                            'name': 'Red'
                        }]
                    }
                }]
            }],
            dockedItems: [{
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [{
                    xtype: 'component',
                    flex: 1
                }, {
                    xtype: 'button',
                    text: 'Save Changes',
                    formBind: true,
                    listeners: {
                        click: {
                            fn: me.onSaveChanges,
                            scope: this
                        }
                    }
                }]
            }],
            listeners: {
                show: {
                    fn: me.onShowWindow,
                    scope: me
                }
            }
        });

        me.callParent(arguments);
    },

    onShowWindow: function() {
        this.down('field[name=POInstructionsInstructionKey]').focus(true, 200);
    },

    onSaveChanges: function(button, e, eOpts) {
        var me = this,
            form = me.getForm();

        if (!form.isValid()) {
            Ext.Msg.alert("Validation", "Check data for valid input!!!");
            return false;
        }

        form.updateRecord();

        record = form.getRecord();

        Ext.Msg.wait('Saving Record...', 'Wait');

        record.save({
            callback: function(records, operation, success) {
                if (success) {
                    var parentForm = me.callerForm;
                    parentForm.down("#gridInstructions").store.reload();
                    me.destroy();
                    Ext.Msg.hide();
                } else {
                    Ext.Msg.hide();
                }
            }
        });
    }
});
