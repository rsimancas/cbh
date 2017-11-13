Ext.define('CBH.view.jobs.JobNewInvoiceContinue', {
    extend: 'Ext.window.Window',
    alias: 'widget.jobnewinvoicecontinue',
    height: 250,
    modal: true,
    width: 400,
    layout: {
        type: 'absolute'
    },
    title: 'Do you want to continue anyway?',
    bodyPadding: 10,
    closable: false,
    constrain: true,
    callerForm: "",
    msgHtml: "",

    initComponent: function() {

        var me = this;

        Ext.applyIf(me, 
        {
            items:[
            {
                html: me.msgHtml,
                xtype: 'panel'
            }
            ],
            dockedItems: [
            {
                xtype: 'toolbar',
                dock: 'bottom',
                ui: 'footer',
                items: [
                {
                    xtype: 'component',
                    flex: 1
                },
                {
                    xtype: 'button',
                    id: 'acceptbutton',
                    text: 'Yes',
                    listeners: {
                        click: {
                            fn: me.onOkClick,
                            scope: me
                        }
                    }
                },
                {
                    xtype: 'button',
                    text: 'No',
                    listeners: {
                        click: {
                            fn: me.onNoClick,
                            scope: me
                        }
                    }
                }
                ]
            }
            ],
            listeners:{
                show: {
                    fn: me.onShowWindow,
                    scope: me
                }
            }
        });

        me.callParent(arguments);
    },

    onShowWindow: function() {
        //Ext.getCmp('profitmargenvalue').focus(true, 200);
    },

    onOkClick: function() {

        var me = this,
            callerForm = me.callerForm;

        this.close();

        callerForm.CreateNewCommissionInvoice();
    },

    onNoClick: function() {
        this.close();
    }
});