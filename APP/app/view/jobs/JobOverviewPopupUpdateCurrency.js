Ext.define('CBH.view.jobs.JobOverviewPopupUpdateCurrency', {
    extend: 'Ext.form.Panel',
    alias: 'widget.JobOverviewPopupUpdateCurrency',
    //height: 185,
    modal: true,
    width: 400,
    layout: {
        type: 'absolute'
    },
    title: 'Update Currency Rates',
    bodyPadding: 10,
    closable: true,
    //constrain: true,
    floating: true,
    callerForm: "",

    initComponent: function() {

        var me = this;

        var rowEditingCurrency = me.loadPluginCurrency();

        Ext.Msg.wait('Loading data...', 'Wait');
        var storeCurrencyRates = new CBH.store.common.CurrencyRates().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            },
            callback: function() {
                Ext.Msg.hide();
            }
        });

        var storeCurrencyMaster = new CBH.store.jobs.JobCurrencyMaster().load({
            params: {
                JobKey: me.JobKey
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
                // hidden fields
                {
                    xtype: 'textfield',
                    name: 'JobKey',
                    hidden: true
                }, 
                {
                    xtype: 'textfield',
                    name: 'JobCurrencyLockedDate',
                    hidden: true
                },
                // Currency Rates
                {
                    margin: '5 10 5 0',
                    columnWidth: 1,
                    //title: 'Status History',
                    xtype: 'gridpanel',
                    itemId: 'gridstatus',
                    store: storeCurrencyMaster,
                    minHeight: 140,
                    height: 140,
                    hideHeaders: false,
                    columns: [
                        // Columns
                        {
                            xtype: 'gridcolumn',
                            text: 'Currency Code',
                            dataIndex: 'JobCurrencyCode',
                            flex: 1,
                            editor: {
                                xtype: 'combo',
                                name: 'JobCurrencyCode',
                                store: storeCurrencyRates,
                                valueField: 'CurrencyCode',
                                displayField: 'CurrencyCodeDesc',
                                queryMode: 'local',
                                minChars: 2,
                                allowBlank: false,
                                forceSelection: true,
                                tpl: Ext.create('Ext.XTemplate',
                                    '<tpl for=".">',
                                    '<div class="x-bound-list-item" >{CurrencyCode} {CurrencyDescription} {CurrencySymbol} {CurrencyRate}</div>',
                                    '</tpl>'),
                                listeners: {
                                    blur: function(field, The, eOpts) {
                                        if (field.value !== null) {
                                            var form = field.up('panel');

                                            copyToField = field.valueModels[0].data.CurrencyRate;
                                            copyField = form.down('field[name=JobCurrencyRate]');
                                            copyField.setValue(copyToField);
                                        }
                                    },
                                    beforequery: function(record) {
                                        record.query = new RegExp(record.query, 'i');
                                        record.forceAll = true;
                                    }
                                }
                            }
                        }, {
                            xtype: 'numbercolumn',
                            text: 'Rate',
                            dataIndex: 'JobCurrencyRate',
                            format: '#,###.###0',
                            align:'right',
                            flex: 1,
                            editor: {
                                xtype: 'numericfield',
                                name: 'JobCurrencyRate',
                                columnWidth: 0.5,
                                hideTrigger: false,
                                useThousandSeparator: true,
                                decimalPrecision: 5,
                                alwaysDisplayDecimals: true,
                                allowNegative: false,
                                currencySymbol: '$',
                                alwaysDecimals: true,
                                thousandSeparator: ',',
                                fieldStyle: 'font-size:11px;text-align:right;',
                                allowBlank: false
                            }
                        }
                    ],
                    selType: 'rowmodel',
                    plugins: [rowEditingCurrency],
                }
            ],
            dockedItems: [{
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [{
                    xtype: 'component',
                    flex: 1
                }, {
                    xtype: 'button',
                    text: 'Change to Today\'s Rates',
                    itemId: 'btnChangeToday',
                    formBind: false,
                    listeners: {
                        click: {
                            fn: me.onChangeTodayRates,
                            scope: me
                        }
                    }
                }, {
                    xtype: 'button',
                    text: 'Update Job with These Rates',
                    formBind: false,
                    listeners: {
                        click: {
                            fn: me.onUpdateJobWithTheseRates,
                            scope: me
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
        var me = this;

        if(me.down('field[name=JobCurrencyLockedDate]').getValue()) {
            me.down('#btnChangeToday').setDisabled(true);
            me.down('#btnChangeToday').setText('JOB LOCKED!');
            //me.down('gridpanel').setDisabled(true);
        }
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
                    parentForm.down("#gridcharges").store.reload();
                    me.destroy();
                    Ext.Msg.hide();
                } else {
                    Ext.Msg.hide();
                }
            }
        });
    },

    // plugins
    loadPluginCurrency: function() {
        var me = this;

        return new Ext.grid.plugin.RowEditing({
            clicksToMoveEditor: 2,
            autoCancel: false,
            errorSummary: false,
            listeners: {
                beforeedit: {
                    //delay: 100,
                    fn: function(item, e) {
                        var editor = this.getEditor(),
                            me = editor.up("form"),
                            locked = me.down('field[name=JobCurrencyLockedDate]').getValue();

                        if(locked) return false;

                        
                        editor.down('field[name=JobCurrencyCode]').focus(true, 200);
                        this.getEditor().onFieldChange();
                    }
                },
                cancelEdit: {
                    fn: function(rowEditing, context) {
                        var grid = context.grid;
                        // Canceling editing of a locally added, unsaved record: remove it
                        if (context.record.phantom) {
                            grid.store.remove(context.record);
                        }
                    }
                },
                edit: {
                    fn: function(rowEditing, context) {
                        var grid = context.grid,
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
    },

    onChangeTodayRates: function() {
        var me = this,
            grid = me.down("grid"),
            JobKey = me.down("field[name=JobKey]").getValue();

        me.getEl().mask('Updating....');
        Ext.Ajax.request({
            url: CBH.GlobalSettings.webApiPath + '/api/UpdateJobCurrencyRates',
            method: 'GET',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                JobKey: JobKey,
                UseCurrentRates: 1
            },
            success: function(response) {
                me.getEl().unmask();
                grid.store.reload();
                Ext.Msg.alert('Status', 'Rates Updated.');
            }
        });
    },

    onUpdateJobWithTheseRates: function() {
        var me = this,
            grid = me.down("grid"),
            JobKey = me.down("field[name=JobKey]").getValue();

        me.getEl().mask('Updating....');
        Ext.Ajax.request({
            url: CBH.GlobalSettings.webApiPath + '/api/UpdateJobCurrencyRates',
            method: 'GET',
            headers: {
                'Authorization-Token': Ext.util.Cookies.get('CBH.UserAuth')
            },
            params: {
                JobKey: JobKey,
                UseCurrentRates: 0
            },
            success: function(response) {
                me.getEl().unmask();
                Ext.Msg.alert('Status', 'Rates Updated.');
                me.destroy();
            }
        });
    }
});