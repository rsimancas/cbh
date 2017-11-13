Ext.define('CBH.view.common.CurrencyRates', {
    extend: 'Ext.form.Panel',
    alias: 'widget.currencyrates',

    layout: {
        type: 'column'
    },
    bodyPadding: 10,
    frameHeader: false,
    header: false,
    enableKeyEvents: true,

    storeNavigator: null,

    requires: [
        'Ext.ux.form.NumericField'
    ],

    initComponent: function() {

        var me = this;

        Ext.applyIf(me, {
            fieldDefaults: {
                labelAlign: 'top',
                labelWidth: 60,
                msgTarget: 'side',
                fieldStyle: 'font-size:11px',
                labelStyle: 'font-size:11px'
            },
            items: [{
                columnWidth: 0.5,
                xtype: 'textfield',
                fieldLabel: 'Currency Code',
                name: 'CurrencyCode',
                allowBlank: false,
                listeners: {
                    blur: function() {
                        //me.onSaveChangesClick();
                    }
                }
            }, {
                margin: '0 0 0 5',
                columnWidth: 0.5,
                xtype: 'textfield',
                fieldLabel: 'Description',
                name: 'CurrencyDescription',
                allowBlank: false,
                listeners: {
                    blur: function() {
                        //me.onSaveChangesClick();
                    }
                }
            }, {
                columnWidth: 0.5,
                xtype: 'textfield',
                fieldLabel: 'Currency Symbol',
                name: 'CurrencySymbol',
                allowBlank: false,
                listeners: {
                    blur: function() {
                        //me.onSaveChangesClick();
                    }
                }
            }, {
                margin: '0 0 0 5',
                xtype: 'numericfield',
                name: 'CurrencyRate',
                columnWidth: 0.5,
                hideTrigger: false,
                useThousandSeparator: true,
                decimalPrecision: 5,
                alwaysDisplayDecimals: true,
                allowNegative: false,
                alwaysDecimals: true,
                thousandSeparator: ',',
                fieldLabel: 'Rate',
                allowBlank: false,
                fieldStyle: 'text-align: right;'
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
                    xtype: 'datefield',
                    itemId: 'CurrencyModifiedDate',
                    name: 'CurrencyModifiedDate',
                    readOnly: true,
                    fieldLabel: 'Modified Date',
                    hideTrigger: true,
                    disabled: true
                }, {
                    xtype: 'component',
                    flex: 1
                }]
            }],
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
        // var toolbar = me.down('#FormToolbar');

        // if(toolbar.store.getCount()===1 && toolbar.store.getAt(0).phantom) {
        //     toolbar.items.items.forEach(function(btn){btn.setVisible(false);});
        //     toolbar.down('#save').setVisible(true);
        // }

        var field = me.down('field[name=CurrencyCode]');
        field.focus(true, 200);
    },

    onAfterLoadRecord: function(tool, record) {
        var form = this;
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

    onAddClick: function(toolbar, record) {
        //var me = this
    },

    onBeginEdit: function(toolbar, record) {
        var me = this;
        me.down('field[name=CurrencyCode]').focus(true, 200);
    },

    onSaveClick: function(toolbar, record) {
        var me = this,
            form = me.getForm();

        if (!form.isValid()) {
            Ext.Msg.alert("Validation", "Check data for valid input!!!");
            return false;
        }

        form.updateRecord();

        record = form.getRecord();

        if (!record.phantom) {
            record.data.CurrencyModifiedDate = new Date();
        }

        Ext.Msg.wait('Saving Record...', 'Wait');

        record.save({
            success: function(e) {
                me.loadRecord(record);
                Ext.Msg.hide();
                toolbar.doRefresh();
            },
            failure: function() {
                Ext.Msg.hide();
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
                            success: function() {
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
