Ext.define('CBH.AppEvents', {
    singleton: true,
    mixins: {
        observable: 'Ext.util.Observable'
    },

    constructor: function(config) {
        // The Observable constructor copies all of the properties of `config` on
        // to `this` using Ext.apply. Further, the `listeners` property is
        // processed to add listeners.
        //
        this.mixins.observable.constructor.call(this, config);

        this.addEvents(
            'printqueue',
            'quit'
        );
    },

    listeners: {
        printqueue: function() {
            this.askPrintQueue();
        }
    },

    askPrintQueue: function() {
        var me = this;
        Ext.Msg.show({
            title: 'Print Queue',
            msg: 'Do you want to print out the entire print queue to your default printer?<br />Press Yes to print now.<br />Press No to delete the print queue.<br />Press Cancel to print the queue later.',
            buttons: Ext.Msg.YESNOCANCEL,
            icon: Ext.Msg.QUESTION,
            fn: function(btn) {
                if (btn === "yes") {
                    me.printQueue();
                } else if(btn === "no"){
                    me.emptyQueue();
                }
            }
        }).defaultButton = 3;
    },

    printQueue: function() {
        console.log('Print Queue');
    },

    emptyQueue: function() {
        console.log('Empty Queue');
    }
});
