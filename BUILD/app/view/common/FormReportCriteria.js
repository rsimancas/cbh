Ext.define('CBH.view.common.FormReportCriteria', {
    extend: 'Ext.form.Panel',
    alias: 'widget.FormReportCriteria',

    layout: 'column',

    bodyPadding: 10,

    reportName: "rptPronacaReport",

    initComponent: function() {
        var me = this;
        var usr = CBH.GlobalSettings.getCurrentUser();
        accLevel = usr.EmployeeAccessLevel;
        var endDate = new Date(),
            startDate = moment().add(-30, 'd').toDate(),
            reportName = me.reportName;

        var storeCust = new CBH.store.customers.CustomersForReport({ remoteSort: false }).load({
            params: {
                page: 0,
                limit: 0,
                start: 0,
                startDate: startDate,
                endDate: endDate,
                reportName: reportName
            }
        });

        var storeCountries = new CBH.store.common.CountriesForReport({ remoteSort: false }).load({
            params: {
                page: 0,
                limit: 0,
                start: 0,
                startDate: startDate,
                endDate: endDate,
                reportName: reportName
            }
        });

        var storeEmployees = new CBH.store.common.EmployeesForReport({ remoteSort: false }).load({
            params: {
                page: 0,
                limit: 0,
                start: 0,
                startDate: startDate,
                endDate: endDate,
                reportName: reportName
            }
        });

        var storeVendors = new CBH.store.vendors.VendorsForReport({ remoteSort: false }).load({
            params: {
                page: 0,
                limit: 0,
                start: 0
            }
        });

        var storeContacts = new CBH.store.customers.CustomerContactsForReport({ remoteSort: false }).load({
            params: {
                CustKey: 0
            }
        });

        var storeJobStatus = new CBH.store.jobs.JobStatus().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        var storeJobRoles = new CBH.store.jobs.JobRoles().load({
            params: {
                page: 0,
                start: 0,
                limit: 0
            }
        });

        Ext.applyIf(me, {
            items: [
                // Header Form
                {
                    xtype: 'fieldset',
                    padding: '10',
                    margin: '0 10 5 0',
                    columnWidth: 1,
                    layout: 'column',
                    items: [
                        // Date Range
                        {
                            labelAlign: 'top',
                            columnWidth: 0.25,
                            xtype: 'combo',
                            displayField: 'name',
                            valueField: 'id',
                            fieldLabel: 'Date Range',
                            name: 'DateRange',
                            queryMode: 'local',
                            typeAhead: true,
                            minChars: 1,
                            forceSelection: true,
                            enableKeyEvents: true,
                            autoSelect: true,
                            selectOnFocus: true,
                            value: 1,
                            allowBlank: false,
                            store: {
                                fields: ['id', 'name'],
                                data: [{
                                    'id': 1,
                                    'name': "Last 30 Days"
                                }, {
                                    'id': 2,
                                    'name': "This Week"
                                }, {
                                    'id': 3,
                                    'name': "Last Week"
                                }, {
                                    'id': 4,
                                    'name': "This Month"
                                }, {
                                    'id': 5,
                                    'name': "Previous Month"
                                }, {
                                    'id': 6,
                                    'name': "This Quarter"
                                }, {
                                    'id': 7,
                                    'name': "Previous Quarter"
                                }, {
                                    'id': 8,
                                    'name': "Year to Date"
                                }, {
                                    'id': 9,
                                    'name': "Previous Year"
                                }, {
                                    'id': 10,
                                    'name': "Custom..."
                                }, {
                                    'id': 11,
                                    'name': "All Records"
                                }]
                            },
                            listeners: {
                                select: {
                                    fn: me.onSelectRangeDate,
                                    scope: me
                                }
                            }
                        },
                        // Start Date
                        {
                            margin: '0 0 0 5',
                            xtype: 'datefield',
                            columnWidth: 0.25,
                            fieldLabel: 'Start Date',
                            labelAlign: 'top',
                            name: 'StartDate',
                            allowBlank: false,
                            value: startDate,
                            disabled: true,
                            listeners: {
                                select: function(field) {
                                    var me = field.up("form");
                                    me.refreshGrids();
                                }
                            }
                        },
                        // End Date
                        {
                            margin: '0 0 0 5',
                            xtype: 'datefield',
                            columnWidth: 0.25,
                            fieldLabel: 'End Date',
                            labelAlign: 'top',
                            name: 'EndDate',
                            allowBlank: false,
                            value: endDate,
                            disabled: true,
                            listeners: {
                                select: function(field) {
                                    var me = field.up("form");
                                    me.refreshGrids();
                                }
                            }
                        },
                        // Created Date
                        {
                            margin: '0 0 0 5',
                            xtype: 'datefield',
                            columnWidth: 0.25,
                            fieldLabel: 'Created Date',
                            labelAlign: 'top',
                            name: 'CreatedDate'
                        }
                    ]
                },
                // Panel Left
                {
                    xtype: 'fieldset',
                    padding: '5',
                    margin: '0 10 5 0',
                    columnWidth: 0.40,
                    layout: 'column',
                    items: [
                        // Grid Customers
                        {
                            xtype: 'gridpanel',
                            itemId: 'gridcustomers',
                            //autoScroll: true,
                            title: 'Select Customers',
                            columnWidth: 1,
                            maxHeight: 340,
                            minHeight: 340,
                            height: (screen.height * 0.40).toFixed(0),
                            margin: '0 5 5 0',
                            store: storeCust,
                            columns: [{
                                xtype: 'gridcolumn',
                                dataIndex: 'CustName',
                                text: 'Customer',
                                flex: 1
                            }, {
                                xtype: 'gridcolumn',
                                dataIndex: 'CustCity',
                                text: 'City',
                                flex: 1
                            }, {
                                xtype: 'gridcolumn',
                                dataIndex: 'CountryName',
                                text: 'Country',
                                flex: 1
                            }],
                            selModel: new Ext.selection.CheckboxModel({
                                showHeaderCheckbox: true
                            }),
                            listeners: {
                                selectionchange: {
                                    fn: me.onSelectCustomerChange,
                                    scope: me
                                }
                            }
                        },
                        // Grid Contacts
                        {
                            margin: '10 5 5 0',
                            xtype: 'gridpanel',
                            itemId: 'gridcontacts',
                            autoScroll: true,
                            title: 'Select Contacts',
                            columnWidth: 1,
                            maxHeight: 220,
                            minHeight: 220,
                            store: storeContacts,
                            columns: [{
                                xtype: 'gridcolumn',
                                dataIndex: 'x_ContactFullName',
                                text: 'Contact',
                                flex: 1
                            }],
                            selModel: new Ext.selection.CheckboxModel({
                                showHeaderCheckbox: true
                            })
                        }
                    ]
                },
                // Panel Right
                {
                    xtype: 'fieldset',
                    padding: '5',
                    margin: '0 10 5 0',
                    columnWidth: 0.60,
                    layout: 'column',
                    items: [
                        // Grid Countries
                        {
                            xtype: 'gridpanel',
                            itemId: 'gridcountries',
                            autoScroll: true,
                            title: 'Select Countries',
                            columnWidth: 0.30,
                            maxHeight: 340,
                            minHeight: 340,
                            height: (screen.height * 0.40).toFixed(0),
                            margin: '0 5 5 0',
                            store: storeCountries,
                            columns: [{
                                xtype: 'gridcolumn',
                                dataIndex: 'CountryName',
                                text: 'Country',
                                flex: 1
                            }],
                            selModel: new Ext.selection.CheckboxModel({
                                showHeaderCheckbox: true
                            })
                        },
                        // Grid Employees
                        {
                            xtype: 'gridpanel',
                            itemId: 'gridemployees',
                            title: 'Select Employees',
                            autoScroll: true,
                            columnWidth: 0.30,
                            maxHeight: 340,
                            minHeight: 340,
                            height: (screen.height * 0.40).toFixed(0),
                            margin: '0 5 5 0',
                            store: storeEmployees,
                            columns: [{
                                xtype: 'gridcolumn',
                                dataIndex: 'x_EmployeeFullName',
                                text: 'Employee',
                                flex: 1
                            }],
                            tbar: [
                                // Employee Roles
                                {
                                    labelAlign: 'top',
                                    xtype: 'combo',
                                    flex: 1,
                                    displayField: 'JobRoleDescription',
                                    valueField: 'JobRoleKey',
                                    name: 'JobRoleKey',
                                    queryMode: 'local',
                                    typeAhead: true,
                                    minChars: 1,
                                    forceSelection: true,
                                    enableKeyEvents: true,
                                    autoSelect: true,
                                    selectOnFocus: true,
                                    value: 1,
                                    //allowBlank: false,
                                    store: storeJobRoles
                                }
                            ],
                            selModel: new Ext.selection.CheckboxModel({
                                showHeaderCheckbox: true
                            })
                        },
                        // Grid Vendors
                        {
                            xtype: 'gridpanel',
                            itemId: 'gridvendors',
                            autoScroll: true,
                            title: 'Select Vendors',
                            columnWidth: 0.40,
                            maxHeight: 340,
                            minHeight: 340,
                            height: (screen.height * 0.40).toFixed(0),
                            margin: '0 5 5 0',
                            store: storeVendors,
                            columns: [{
                                xtype: 'gridcolumn',
                                dataIndex: 'VendorName',
                                text: 'Vendor',
                                flex: 1
                            }],
                            selModel: new Ext.selection.CheckboxModel({
                                showHeaderCheckbox: true
                            })

                        },
                        // Open Quotes / Close Quotes / Job Status
                        {
                            xtype: 'fieldset',
                            padding: '5',
                            margin: '0 10 5 0',
                            columnWidth: 0.60,
                            layout: 'column',
                            items: [
                                // Open Quotes
                                {
                                    xtype: 'fieldcontainer',
                                    columnWidth: 1,
                                    layout: 'hbox',
                                    items: [{
                                        xtype: 'checkbox',
                                        name: 'OpenQuotes',
                                        labelSeparator: '',
                                        hideLabel: true,
                                        boxLabel: 'Open Quotes',
                                        listeners: {
                                            change: function(field, newValue, oldValue, eOpts) {
                                                //me.down("field[name=ClosedQuotes]").setValue(!newValue);
                                            }
                                        }
                                    }, {
                                        xtype: 'component',
                                        flex: 1
                                    }, {
                                        xtype: 'checkbox',
                                        name: 'ClosedQuotes',
                                        labelSeparator: '',
                                        hideLabel: true,
                                        boxLabel: 'Closed Quotes',
                                        listeners: {
                                            change: function(field, newValue, oldValue, eOpts) {
                                                //me.down("field[name=OpenQuotes]").setValue(!newValue);
                                            }
                                        }
                                    }]
                                },
                                // Job Status
                                {
                                    margin: '10 0 0 0',
                                    xtype: 'fieldcontainer',
                                    columnWidth: 1,
                                    layout: 'hbox',
                                    items: [
                                        // oper
                                        {
                                            labelAlign: 'top',
                                            width: 60,
                                            xtype: 'combo',
                                            displayField: 'name',
                                            valueField: 'name',
                                            fieldLabel: 'Job Status',
                                            name: 'JobStatusExpression',
                                            queryMode: 'local',
                                            typeAhead: true,
                                            minChars: 1,
                                            forceSelection: true,
                                            enableKeyEvents: true,
                                            autoSelect: true,
                                            selectOnFocus: true,
                                            value: "=",
                                            store: {
                                                fields: ['name'],
                                                data: [{
                                                    'name': "="
                                                }, {
                                                    'name': "<="
                                                }]
                                            }
                                        },
                                        // status
                                        {
                                            margin: '21 0 0 5',
                                            labelAlign: 'top',
                                            xtype: 'combo',
                                            flex: 1,
                                            displayField: 'StatusDescription',
                                            valueField: 'StatusKey',
                                            name: 'JobStatus',
                                            queryMode: 'local',
                                            typeAhead: true,
                                            minChars: 1,
                                            forceSelection: true,
                                            enableKeyEvents: true,
                                            autoSelect: true,
                                            selectOnFocus: true,
                                            value: 100,
                                            //allowBlank: false,
                                            store: storeJobStatus
                                        }
                                    ]
                                }
                            ]
                        },
                        // Open Report
                        {
                            xtype: 'fieldset',
                            padding: '5',
                            margin: '0 10 5 0',
                            columnWidth: 0.40,
                            layout: 'column',
                            items: [{
                                xtype: 'fieldcontainer',
                                columnWidth: 1,
                                layout: 'hbox',
                                items: [
                                    // Currency
                                    {
                                        labelAlign: 'top',
                                        flex: 1,
                                        xtype: 'combo',
                                        displayField: 'name',
                                        valueField: 'id',
                                        fieldLabel: 'Currency',
                                        name: 'Currency',
                                        queryMode: 'local',
                                        typeAhead: true,
                                        minChars: 1,
                                        forceSelection: true,
                                        enableKeyEvents: true,
                                        autoSelect: true,
                                        selectOnFocus: true,
                                        value: "USD",
                                        //allowBlank: false,
                                        store: {
                                            fields: ['id', 'name'],
                                            data: [{
                                                'id': "USD",
                                                'name': "USD"
                                            }, {
                                                'id': "EUR",
                                                'name': "EUR"
                                            }]
                                        }
                                    }
                                ]
                            }, {
                                xtype: 'fieldcontainer',
                                columnWidth: 1,
                                layout: 'hbox',
                                items: [{
                                        xtype: 'component',
                                        flex: 1
                                    },
                                    // Button
                                    {
                                        margin: '10 0 0 0',
                                        xtype: 'button',
                                        itemId: 'btnOpenReport',
                                        text: 'Open Report',
                                        listeners: {
                                            click: {
                                                fn: me.onClickOpenReport,
                                                scope: me
                                            }
                                        }
                                    }
                                ]
                            }]
                        }
                    ]
                }
            ],
            // Form Listeners
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
        me.setReportEnvironment();
    },

    onSelectCustomerChange: function(model, record) {
        var me = this,
            customer = record,
            gridCustomers = me.down("#gridcustomers"),
            gridContacts = me.down("#gridcontacts");

        if (model.lastSelected) {
            var CustKey = model.lastSelected.data.CustKey;
            //var grid = me.down("#gridcontacts");

            gridContacts.store.reload({
                params: {
                    CustKey: CustKey
                },
                callback: function() {
                    setTimeout(function() {
                        var index = Math.min(model.lastSelected.index + 3, gridCustomers.getStore().getCount());
                        gridCustomers.getView().scrollRowIntoView(index);

                    }, 200);
                }
            });
        }
    },

    onSearchFieldChange: function() {
        var form = this,
            field = form.down('#searchfield'),
            fieldValue = field.getRawValue(),
            grid = form.down('#gridsales');

        grid.store.removeAll();

        if (!String.isNullOrEmpty(fieldValue)) {
            grid.store.loadPage(1, {
                params: {
                    query: fieldValue
                },
                callback: function() {
                    form.down('#pagingtoolbar').bindStore(this);
                }
            });
        } else {
            grid.store.loadPage(1, {
                callback: function() {
                    form.down('#pagingtoolbar').bindStore(this);
                }
            });
        }
    },

    onSelectRangeDate: function(combo, records, eOpts) {
        var me = this,
            startDate = me.down("field[name=StartDate]").getValue(),
            endDate = me.down('field[name=EndDate]').getValue(),
            fieldStart = me.down("field[name=StartDate]"),
            fieldEnd = me.down('field[name=EndDate]');

        fieldStart.setDisabled(true);
        fieldEnd.setDisabled(true);

        var params = {
            page: 0,
            limit: 0,
            start: 0
        };

        switch (combo.getValue()) {
            case 1: // Last 30 Days
                endDate = new Date();
                startDate = moment().add(-30, 'd').toDate();
                break;
            case 2: // This Week
                startDate = moment().startOf('week').add(1, 'd').toDate();
                endDate = new Date();
                break;
            case 3: // Last Week
                startDate = moment().add(-1, 'week').startOf('week').add(1, 'd').toDate();
                endDate = moment().add(-1, 'week').endOf('week').add(1, 'd').toDate();
                break;
            case 4: // This Month
                startDate = moment().startOf('month').toDate();
                endDate = new Date();
                break;
            case 5: // Last Month
                startDate = moment().add(-1, 'month').startOf('month').toDate();
                endDate = moment().add(-1, 'month').endOf('month').toDate();
                break;
            case 6: // This Quarter
                startDate = moment().startOf('quarter').toDate();
                endDate = new Date();
                break;
            case 7: // Previous Quarter
                startDate = moment().add(-1, 'quarter').startOf('quarter').toDate();
                endDate = moment().add(-1, 'quarter').endOf('quarter').toDate();
                break;
            case 8: // Year to Date
                startDate = moment().startOf('year').toDate();
                endDate = new Date();
                break;
            case 9: // Previous Year
                startDate = moment().add(-1, 'year').startOf('year').toDate();
                endDate = moment().add(-1, 'year').endOf('year').toDate();
                break;
            case 10: //Custom
                fieldStart.setDisabled(false);
                fieldEnd.setDisabled(false);
                break;
            case 11: //All Records
                endDate = new Date();
                startDate = null;
                break;
        }

        fieldStart.setValue(startDate);
        fieldEnd.setValue(endDate);

        me.refreshGrids();
    },

    refreshGrids: function() {
        var me = this,
            combo = me.down("field[name=DateRange]"),
            startDate = me.down("field[name=StartDate]").getValue(),
            endDate = me.down("field[name=EndDate]").getValue();

        var params = {
            page: 0,
            limit: 0,
            start: 0
        };

        //if (combo.getValue() !== 11) {
        params.startDate = startDate;
        params.endDate = endDate;
        params.reportName = me.reportName;
        //}

        var grid = me.down("#gridcustomers");
        grid.store.reload({
            params: params
        });

        grid = me.down("#gridcountries");
        grid.store.reload({
            params: params
        });

        grid = me.down("#gridemployees");
        grid.store.reload({
            params: params
        });
    },

    setReportEnvironment: function() {
        var me = this;

        if ("rptJobSummary".indexOf(me.reportName) !== -1) {
            me.down("field[name=CreatedDate]").setVisible(false);
            me.down("field[name=OpenQuotes]").setVisible(false);
            me.down("field[name=ClosedQuotes]").setVisible(false);
        }

        if (["rptFileQuoteStatusReport", "rptFileSummary", "rptFileSummaryByContacts"].indexOf(me.reportName) !== -1) {
            setTimeout(function() {
                me.down("field[name=OpenQuotes]").setValue(true);
            }, 200);
            me.down("field[name=JobStatus]").setVisible(false);
            me.down("field[name=JobStatusExpression]").setVisible(false);
        }

        if (["rptPronacaReportClosedShipped"].indexOf(me.reportName) !== -1) {
            me.down("field[name=OpenQuotes]").setVisible(false);
            me.down("field[name=ClosedQuotes]").setVisible(false);
            me.down("field[name=JobStatus]").setVisible(false);
            me.down("field[name=JobStatusExpression]").setVisible(false);
        }

        if (["rptPronacaReport", "rptPronacaReport NoProfit", "rptPronacaTransitOrders", "rptPronacaReportQuotes", "rptPronacaReportQuotes NoProfit"].indexOf(me.reportName) !== -1) {
            me.down("field[name=OpenQuotes]").setVisible(false);
            me.down("field[name=ClosedQuotes]").setVisible(false);
            me.down("field[name=JobStatus]").setVisible(false);
            me.down("field[name=JobStatusExpression]").setVisible(false);
        }
    },

    onClickOpenReport: function() {
        var me = this,
            comboDateRange = me.down("field[name=DateRange]"),
            userKey = CBH.GlobalSettings.getCurrentUserEmployeeKey(),
            storeCriteria = new CBH.store.common.ReportCriteria({ autoLoad: false }),
            countries = [],
            employees = [],
            vendors = [],
            customers = [],
            strWhere = "",
            lblCountries = "Countries: ", // varCountries
            lblEmployees = "Employees: ", // varEmployees
            lblCustomers = "Customers: ", // varCustomers
            lblVendors = "Vendors: ", // varVendors
            createdDate = me.down("field[name=CreatedDate]").getValue(),
            showContacts = false,
            jobRoleKey = me.down("field[name=JobRoleKey]").getValue(),
            jobStatus = me.down("field[name=JobStatus]").getValue();

        // Customers selection
        Ext.Array.each(me.down("#gridcustomers").getSelectionModel().selected.items, function(item, index) {
            customers.push('"{0}"'.format(item.data.CustName));
            storeCriteria.add(new CBH.model.common.ReportCriteria({
                CriteriaEmployeeKey: userKey,
                CriteriaRptName: me.reportName,
                CriteriaFieldName: "CustKey",
                CriteriaValue: item.data.CustKey
            }));
        });

        if (!customers.length) {
            lblCustomers = "";
        } else {
            lblCustomers += customers.join(",");
        }

        // Contacts selection
        Ext.Array.each(me.down("#gridcontacts").getSelectionModel().selected.items, function(item, index) {
            //customers.push('"{0}"'.format(item.data.CustName));
            showContacts = true;
            storeCriteria.add(new CBH.model.common.ReportCriteria({
                CriteriaEmployeeKey: userKey,
                CriteriaRptName: me.reportName,
                CriteriaFieldName: "ContactKey",
                CriteriaValue: item.data.ContactKey
            }));
        });

        // Countries selection
        Ext.Array.each(me.down("#gridcountries").getSelectionModel().selected.items, function(item, index) {
            countries.push('"{0}"'.format(item.data.CountryName));
            storeCriteria.add(new CBH.model.common.ReportCriteria({
                CriteriaEmployeeKey: userKey,
                CriteriaRptName: me.reportName,
                CriteriaFieldName: "CountryKey",
                CriteriaValue: item.data.CountryKey
            }));
        });

        if (!countries.length) {
            lblCountries = "";
        } else {
            lblCountries += countries.join(",");
        }

        // Employees selection
        Ext.Array.each(me.down("#gridemployees").getSelectionModel().selected.items, function(item, index) {
            employees.push('"{0}"'.format(item.data.x_EmployeeFullName));
            storeCriteria.add(new CBH.model.common.ReportCriteria({
                CriteriaEmployeeKey: userKey,
                CriteriaRptName: me.reportName,
                CriteriaFieldName: "EmployeeKey",
                CriteriaValue: item.data.EmployeeKey
            }));
        });

        if (!employees.length) {
            lblEmployees = "";
        } else {
            lblEmployees += employees.join(",");
        }

        // Vendors selection
        Ext.Array.each(me.down("#gridvendors").getSelectionModel().selected.items, function(item, index) {
            vendors.push('"{0}"'.format(item.data.VendorName));
            storeCriteria.add(new CBH.model.common.ReportCriteria({
                CriteriaEmployeeKey: userKey,
                CriteriaRptName: me.reportName,
                CriteriaFieldName: "VendorKey",
                CriteriaValue: item.data.VendorKey
            }));
        });

        if (!vendors.length) {
            lblVendors = "";
        } else {
            lblVendors += vendors.join(",");
        }

        var executeReport = function() {
            var startDate = me.down("field[name=StartDate]").getValue(),
                endDate = me.down('field[name=EndDate]').getValue(),
                openQuotes = me.down('field[name=OpenQuotes]').getValue(),
                closedQuotes = me.down('field[name=ClosedQuotes]').getValue(),
                jobStatusExpression = me.down('field[name=JobStatusExpression]').getValue();

            //'*** Create Where string
            rangeSelected = comboDateRange.getValue();
            if (rangeSelected !== 11) { //All Records
                if ("rptPronacaReportClosedShipped" === me.reportName) {
                    if (createdDate !== null) {
                        strWhere = "ISNULL(FileCreatedDate,JobCreatedDate) > CAST('{0}' AS DATE) AND ".format(moment(createdDate).add(-1, 'day').format('YYYY-MM-DD'));
                        strWhere += "JobShipDate > CAST('{0}' AS DATE) AND JobShipDate <= CAST('{1}' AS DATE) AND ".format(moment(startDate).add(-1, 'day').format('YYYY-MM-DD'), moment(endDate).format('YYYY-MM-DD'));
                    } else {
                        strWhere = "rptDate > CAST('{0}' AS DATE) AND rptDate <= CAST('{1}' AS DATE) AND ".format(moment(startDate).add(-1, 'day').format('YYYY-MM-DD'), moment(endDate).format('YYYY-MM-DD'));
                    }
                } else if ("rptPronacaTransitOrders" === me.reportName) {
                    strWhere = "ISNULL(FileCreatedDate, JobCreatedDate) > CAST('{0}' AS DATE) AND ".format(moment(startDate).add(-1, 'day').format('YYYY-MM-DD'));
                } else {
                    strWhere = "rptDate > CAST('{0}' AS DATE) AND rptDate <= CAST('{1}' AS DATE) AND ".format(moment(startDate).add(-1, 'day').format('YYYY-MM-DD'), moment(endDate).format('YYYY-MM-DD'));
                }
            }


            if (customers.length) {
                if (showContacts) {
                    //'*** Filter by ContactKey
                    strWhere += "ContactKey IN (SELECT CriteriaValue FROM tblReportCriteria WHERE CriteriaRptName = '{0}' AND CriteriaFieldName = 'ContactKey' AND CriteriaEmployeeKey = {1}) AND ".format(me.reportName, userKey);
                } else {
                    //'*** Filter by CustKey only
                    strWhere += "CustKey IN (SELECT CriteriaValue FROM tblReportCriteria WHERE CriteriaRptName = '{0}' AND CriteriaFieldName = 'CustKey' AND CriteriaEmployeeKey = {1}) AND ".format(me.reportName, userKey);
                }
            }

            if (countries.length) {
                strWhere += "CountryKey IN (SELECT CriteriaValue FROM tblReportCriteria WHERE CriteriaRptName = '{0}' AND CriteriaFieldName = 'CountryKey' AND CriteriaEmployeeKey = {1} ) AND ".format(me.reportName, userKey);
            }

            //'*** Job employee selection based on tblJobEmployeeRoles
            if (["rptJobProfitWithExemptions", "rptJobProfit", "rptJobSummary"].indexOf(me.reportName) !== -1) {
                if (employees.length) {
                    strWhere += "JobKey IN (SELECT JobEmployeeJobKey FROM tblJobEmployeeRoles WHERE JobEmployeeRoleKey = {0} AND JobEmployeeEmployeeKey IN (SELECT CriteriaValue FROM tblReportCriteria WHERE CriteriaRptName = '{1}' AND CriteriaFieldName = 'EmployeeKey' AND CriteriaEmployeeKey = {2})) AND ".format(jobRoleKey, me.reportName, userKey);
                }

                if (vendors.length) {
                    strWhere += "JobKey IN (SELECT POJobKey FROM tblJobPurchaseOrders WHERE POVendorKey IN (SELECT CriteriaValue FROM tblReportCriteria WHERE CriteriaRptName = '{0}' AND CriteriaFieldName = 'VendorKey' AND CriteriaEmployeeKey = {1})) AND ".format(me.reportName, userKey);
                }
            } else if (["rptFileQuoteStatusReport", "rptFileSummary", "rptFileSummaryByContacts"].indexOf(me.reportName) !== -1) {
                if (employees.length) {
                    strWhere += "a.FileKey IN (SELECT FileEmployeeFileKey FROM tblFileEmployeeRoles WHERE FileEmployeeRoleKey = {0} AND FileEmployeeEmployeeKey IN (SELECT CriteriaValue FROM tblReportCriteria WHERE CriteriaRptName = '{1}' AND CriteriaFieldName = 'EmployeeKey' AND CriteriaEmployeeKey = {2})) AND ".format(jobRoleKey, me.reportName, userKey);
                }

                if (vendors.length) {
                    strWhere += "a.FileKey IN (SELECT FVFileKey FROM tblFileQuoteVendorInfo WHERE FVVendorKey IN (SELECT CriteriaValue FROM tblReportCriteria WHERE CriteriaRptName = '{0}' AND CriteriaFieldName = 'VendorKey' AND CriteriaEmployeeKey = {1})) AND ".format(me.reportName, userKey);
                }
            } else if (["rptPronacaReport", "rptPronacaReport NoProfit", "rptPronacaReportClosedShipped", "rptPronacaTransitOrders", "rptPronacaReportQuotes", "rptPronacaReportQuotes NoProfit"].indexOf(me.reportName) !== -1) {
                if (employees.length) {
                    strWhere += "(JobKey Is Null OR JobKey IN (SELECT JobEmployeeJobKey FROM tblJobEmployeeRoles WHERE JobEmployeeRoleKey = {0} AND JobEmployeeEmployeeKey IN (SELECT CriteriaValue FROM tblReportCriteria WHERE CriteriaRptName = '{1}' AND CriteriaFieldName = 'EmployeeKey' AND CriteriaEmployeeKey = {2}))) AND ".format(jobRoleKey, me.reportName, userKey);
                    strWhere += "(FileKey Is Null OR FileKey IN (SELECT FileEmployeeFileKey FROM tblFileEmployeeRoles WHERE FileEmployeeRoleKey = {0} AND FileEmployeeEmployeeKey IN (SELECT CriteriaValue FROM tblReportCriteria WHERE CriteriaRptName = '{1}' AND CriteriaFieldName = 'EmployeeKey' AND CriteriaEmployeeKey = {2}))) AND ".format(jobRoleKey, me.reportName, userKey);
                }
                if (vendors.length) {
                    strWhere += "(JobKey Is Null Or JobKey IN (SELECT POJobKey FROM tblJobPurchaseOrders WHERE POVendorKey IN (SELECT CriteriaValue FROM tblReportCriteria WHERE CriteriaRptName = '{0}' AND CriteriaFieldName = 'VendorKey' AND CriteriaEmployeeKey = {1}))) AND ".format(me.reportName, userKey);
                    strWhere += "(FileKey Is Null Or FileKey IN (SELECT FVFileKey FROM tblFileQuoteVendorInfo WHERE FVVendorKey IN (SELECT CriteriaValue FROM tblReportCriteria WHERE CriteriaRptName = '{0}' AND CriteriaFieldName = 'VendorKey' AND CriteriaEmployeeKey = {1}))) AND ".format(me.reportName, userKey);
                }
            } else {
                if (employees.length) {
                    strWhere += "EmployeeKey IN (SELECT CriteriaValue FROM tblReportCriteria WHERE CriteriaRptName = '{0}' AND CriteriaFieldName = 'EmployeeKey' AND CriteriaEmployeeKey = {1}) AND ".format(me.reportName, userKey);
                }
            }

            //'*** Check for open or closed status
            if (me.reportName === "rptPronacaReportClosedShipped") {
                strWhere += "JobIsClosed = 1 AND ";
            } else if (me.reportName === "rptPronacaTransitOrders") {
                strWhere += "JobNum Is Not Null And JobIsClosed = 0 AND ";
            } else if (["rptPronacaReportQuotes", "rptPronacaReportQuotes NoProfit"].indexOf(me.reportName) !== -1) {
                strWhere += "JobNum Is Null AND QuoteIsClosed = 0 AND ";
            } else {
                if (openQuotes && !closedQuotes) {
                    strWhere += "IsClosed = 0 AND ";
                } else if (!openQuotes && closedQuotes) {
                    strWhere += "IsClosed = 1 AND ";
                }

                if (!me.down("field[name=JobStatus]").hidden && ["rptFileSummary", "rptFileSummaryByContacts", "rptCustomerWebLogins"].indexOf(me.reportName) === -1) {
                    if (jobStatusExpression === "=") {
                        strWhere += "StatusKey = {0} AND ".format(jobStatus);
                    } else if (jobStatusExpression === "<=") {
                        strWhere += "StatusKey <= {0} AND ".format(jobStatus);
                    }
                }
            }

            //'*** Exclude jobs that are exempt from the profit report
            if (me.reportName === "rptJobProfit" || me.reportName === "ExcelJobProfit") {
                strWhere += "JobExemptFromProfitReport = 0 AND ";
            }


            if (strWhere.length > 4) {
                strWhere = strWhere.substring(0, strWhere.length - 5);
            }

            //'*** Build varPassCriteria string for the lblCriteriaUsed label in the report header
            var passCriteria = lblCustomers + ((lblCustomers) ? "\r\n" : "") +
                lblCountries + ((lblCountries) ? "\r\n" : "") +
                lblEmployees + ((lblEmployees) ? "\r\n" : "") +
                lblVendors + ((lblVendors) ? "\r\n" : "") +
                comboDateRange.getRawValue();

            var formData = {
                employeeKey: userKey,
                labelCriteria: passCriteria.split('\n')[0].replace('#',''),
                strWhere: strWhere
            };

            if (["rptPronacaReportClosedShipped", "rptPronacaTransitOrders", "rptPronacaReportQuotes NoProfit"].indexOf(me.reportName) !== -1) {
                
                //if(String.isNullOrEmpty(startDate))
                //    startDate = moment(new Date(2013, 0, 1)).format('YYYY-MM-DD');

                //formData.startDate = moment(startDate).add(-1, 'day').format('MM-DD-YYYY');
                formData.startDate = moment(startDate).format('MM-DD-YYYY');
                formData.endDate = moment(endDate).format('MM-DD-YYYY');
            }

            if (["rptPronacaReportQuotes NoProfit", "rptPronacaReport NoProfit"].indexOf(me.reportName) !== -1) {
                formData.NoProfit = "1";
            }

            var params = Serialize(formData),
                reportName = me.reportName.split(" ")[0];
            pathReport = CBH.GlobalSettings.webApiPath + '/Reports/{0}{1}'.format(reportName, params);

            window.open(pathReport, me.reportName, false);
        };

        if (storeCriteria.getCount() > 0) {
            storeCriteria.sync({
                success: executeReport
            });
        } else {
            executeReport();
        }
    }
});
