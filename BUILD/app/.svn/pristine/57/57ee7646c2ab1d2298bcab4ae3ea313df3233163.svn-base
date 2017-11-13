Ext.define('Ext.ux.window.NumberPrompt', {
    extend: 'Ext.window.MessageBox',
    initComponent: function() {
        this.callParent();
        var index = this.promptContainer.items.indexOf(this.textField);
        this.promptContainer.remove(this.textField);
        this.textField = this._createNumberField();
        this.promptContainer.insert(index, this.textField);
    },

    _createNumberField: function() {
        //copy paste what is being done in the initComonent to create the textfield
        return new Ext.form.field.Number({
                        id: this.id + '-textfield',
                        anchor: '100%',
                        fieldStyle: 'text-align: right;',
                        enableKeyEvents: true,
                        selectOnFocus: true,
                        listeners: {
                            keydown: this.onPromptKey,
                            scope: this
                        }
        });
    }
});