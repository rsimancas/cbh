Ext.define('Ext.ux.form.Currency', {
    extend: 'Ext.form.field.Text',

    alias: 'widget.currencyfield',

    initComponent: function (config) {
        this.callParent(arguments);
    },

    hasFocus: false,

    listeners: {
        render: function () {
            var form = this.findParentByType('form');

            form.on('afterLoadRecord', function () {
                this.toRaw();
                if (this.getRawValue() === 0) {
                    this.setRawValue('');
                } else {
                    this.toFormatted();
                }
            }, this);

            form.on('beforeUpdateRecord', function () {
                this.toRaw();
            }, this);

            form.on('afterUpdateRecord', function () {
                this.toRaw();
                if (this.getRawValue() === 0) {
                    this.setRawValue('');
                } else {
                    this.toFormatted();
                }
            }, this);
        },
        focus: function (field, e, eOpts) {
            this.toRaw();
            this.hasFocus = true;
        },
        blur: function (field, e, eOpts) {
            //Clear out commas and $
            this.toRaw();

            //If there's a value, format it
            if(field.getValue() !== '') {
                this.toFormatted();
                this.hasFocus = false;
            }
        }
    },

    stripAlpha: function (value) {
        return value.replace(/[^0-9.]/g, '');
    },

    toRaw: function () {
        if (this.readOnly !== true) {
            this.setRawValue(this.stripAlpha(this.getRawValue()));
        }
    },

    toFormatted: function () {
        this.setRawValue(Ext.util.Format.currency(this.getRawValue(), '$ ', 0));
    },

    getValue: function () {
        return parseFloat(this.stripAlpha(this.getRawValue()));
    }
});