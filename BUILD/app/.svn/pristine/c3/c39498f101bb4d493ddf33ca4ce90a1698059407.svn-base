Ext.define('Ext.ux.form.SearchField', {
    extend: 'Ext.form.field.Trigger',

    alias: 'widget.searchfield',

    emptyText: 'type and press enter to search',

    trigger1Cls: Ext.baseCSSPrefix + 'form-clear-trigger',

    trigger2Cls: Ext.baseCSSPrefix + 'form-search-trigger',

    hasSearch: false,

    initComponent: function() {
        var me = this;

        me.callParent(arguments);
        me.on('specialkey', function(f, e){
            if (e.getKey() == e.ENTER) {
                me.onTrigger2Click();
            }
        });

        me.on('change',function(field) {
            var me = this,
                value = me.getValue();

            if(value.length === 0) {
                me.triggerCell.item(0).setDisplayed(false);
                me.updateLayout();

                if(me.hasSearch) {
                    me.hasSearch = false;
                    me.fireEvent('triggerclick', me);
                }
            } else {
                me.triggerCell.item(0).setDisplayed(true);
                me.updateLayout();
            }
        });

        me.addEvents(
            'triggerclick'
        );
    },

    afterRender: function(){
        this.callParent();
        this.triggerCell.item(0).setDisplayed(false);
    },

    onTrigger1Click : function(){
        var me = this;
        me.hasSearch = false;
        me.setValue('');
        me.triggerCell.item(0).setDisplayed(false);
        me.updateLayout();
        me.fireEvent('triggerclick', me);   
    },

    onTrigger2Click : function(){
        var me = this,
            value = me.getValue();

        if (value.length > 0) {
            me.hasSearch = true;
            me.triggerCell.item(0).setDisplayed(true);
            me.updateLayout();
            me.fireEvent('triggerclick', me);    
        }
    }
});