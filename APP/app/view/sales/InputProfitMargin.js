Ext.define('CBH.view.sales.InputProfitMargin', {
    extend: 'Ext.window.Window',
    id: 'inputprofitmargin',
    height: 185,
    modal: true,
    width: 334,
    layout: {
        type: 'absolute'
    },
    title: 'What Profit Margin do you want to use for this vendor?',
    bodyPadding: 10,
    closable: false,
    constrain: true,
    callerForm: "",

    initComponent: function() {

        var me = this;

        Ext.applyIf(me, {
            items: [{
                xtype: 'fieldcontainer',
                layout: {
                    type: 'fit'
                },
                items: [{
                    xtype: 'numericfield',
                    id: 'profitmargenvalue',

                    fieldLabel: 'Profit Margin',
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
                    maxValue: 99.99,
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
                    id: 'acceptbutton',
                    text: 'Ok',
                    formBind: true,
                    value: me.valueProfit,
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
        Ext.getCmp('profitmargenvalue').focus(true, 200);
    },

    onOkClick: function() {

        var me = this;

        var callerForm = Ext.getCmp(me.callerForm);

        var profitMargin = parseFloat(this.down("numberfield").value);

        if(profitMargin > 1) 
            profitMargin = profitMargin / 100;

        var cost = (callerForm.down('#QuoteItemCost')) ? callerForm.down('#QuoteItemCost').getValue() : callerForm.down('field[name=POItemsCost]').getValue();
        var qty = (callerForm.down('#QuoteQty')) ? callerForm.down('#QuoteQty').getValue() : callerForm.down('field[name=POItemsQty]').getValue();
        var price = GetMarginPrice(cost, profitMargin);
        var x_linecost = cost * qty;
        var x_lineprice = price * qty;

        //var itemWeight = (callerForm.down('#QuoteItemWeight')) ? callerForm.down('#QuoteItemWeight').getValue() : callerForm.down('field[name=POItemsWeight]').getValue();
        //var itemVolume = (callerForm.down('#QuoteItemVolume')) ? callerForm.down('#QuoteItemVolume').getValue() : callerForm.down('field[name=POItemsVolume]').getValue();

        //var lineWeight = itemWeight * qty;
        //var lineVolume = itemVolume * qty;

        callerForm.down('#x_ProfitMargin').setValue(profitMargin);
        
        if(callerForm.down('#QuoteItemPrice')) {
            //callerForm.down('#x_LineWeight').setValue(lineWeight);
            //callerForm.down('#x_LineVolume').setValue(lineVolume);

            callerForm.down('#QuoteItemPrice').setValue(price);
            callerForm.down('#QuoteItemLineCost').setValue(x_linecost);
            callerForm.down('#QuoteItemLinePrice').setValue(x_lineprice);
            callerForm.down('#QuoteItemCurrencyCode').focus(true, 200);
        } else {
            callerForm.down('field[name=POItemsPrice]').setValue(price);
            callerForm.down('field[name=POItemsLineCost]').setValue(x_linecost);
            callerForm.down('field[name=POItemsLinePrice]').setValue(x_lineprice);
            callerForm.down('field[name=POItemsCurrencyCode]').focus(true, 200);
        }
        this.close();
    }
});
