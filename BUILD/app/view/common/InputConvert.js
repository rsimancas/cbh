Ext.define('CBH.view.common.InputConvert', {
    extend: 'Ext.window.Window',
    height: 185,
    modal: true,
    width: 334,
    /*layout: {
        type: 'absolute'
    },*/
    title: 'Convertion',
    bodyPadding: 10,
    closable: false,
    constrain: true,
    callerForm: "",
    fieldLabel: "",
    currentValue: 0,

    initComponent: function() {

        var me = this;

        me.currentValue = me.getPounds(me.currentValue);

        Ext.applyIf(me, {
            items: [{
                xtype: 'fieldcontainer',
                layout: {
                    type: 'fit'
                },
                items: [{
                    xtype: 'numericfield',
                    name: 'InputValue',

                    fieldLabel: me.fieldLabel,
                    enableKeyEvents: true,
                    useThousandSeparator: true,
                    decimalPrecision: 4,
                    alwaysDisplayDecimals: true,
                    allowNegative: false,
                    alwaysDecimals: true,
                    thousandSeparator: ',',
                    labelAlign: 'top',
                    fieldStyle: 'text-align: right;',
                    allowBlank: false,
                    //maxValue: 99.99,
                    value: me.currentValue,
                    listeners: {
                        buffer: 100,
                        specialkey: function(field, e) {
                            // e.HOME, e.END, e.PAGE_UP, e.PAGE_DOWN,
                            // e.TAB, e.ESC, arrow keys: e.LEFT, e.RIGHT, e.UP, e.DOWN

                            // e.stopEvent();

                            // if(e.getKey() == e.TAB && field.isExpanded) {
                            //     e.stopEvent();
                            //     return;
                            // };
                            if (e.getKey() == e.ENTER) {
                                me.onOkClick();
                                e.stopEvent();
                                return;
                            }
                        },
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
                    itemId: 'acceptbutton',
                    text: 'Ok',
                    formBind: true,
                    listeners: {
                        click: {
                            fn: me.onOkClick,
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
        me.down('field[name=InputValue]').focus(true, 200);
    },

    onOkClick: function() {

        var me = this,
            options = me.convertOptions,
            callerForm = me.callerForm;

        var inputValue = parseFloat(this.down("numberfield").value),
            returnValue = 0;

        switch (options.typeConvertion) {
            case "kilograms":
                returnValue = me.getKilograms(inputValue);
                break;
            case "pounds":
                returnValue = me.getPounds(inputValue);
                break;
            case "cubicmeters":
                returnValue = me.getCubicMeters(inputValue);
                break;
            case "cubicfeets":
                returnValue = me.getCubicFeets(inputValue);
                break;
        }

        options.callback({callerForm: callerForm, value: returnValue});

        me.destroy();
    },

    getKilograms: function (pounds) {
        pounds = (!pounds) ? 0 : pounds;

        return pounds * 0.45359237;
    },

    getPounds: function (kilograms) {
        kilograms = (!kilograms) ? 0 : kilograms;

        return kilograms * 2.2046;
    },

    getCubicMeters: function (cubicFeet) {
        cubicFeet = (!cubicFeet) ? 0 : cubicFeet;

        return cubicFeet / 35.314;
    },

    getCubicFeets: function (cubicMeter) {
        cubicMeter = (!cubicMeter) ? 0 : cubicMeter;

        return cubicMeter * 35.314;
    }
});
