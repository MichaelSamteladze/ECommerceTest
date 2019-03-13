var Components63Bits = {
    Dialog: {
        ButtonColors: {
            Blue: 'btn-blue',
            Red: 'btn-danger',
            Green: 'btn-success',
            Orange:'btn-warning'
        },
        Error: function (ErrorMessage) {
            $.alert({
                title: 'Error',
                content: ErrorMessage ? ErrorMessage : Globals.TextError,
                icon: 'fas fa-exclamation-triangle',
                closeIcon: true,
                type: 'red',
                animation: 'scale',
                closeAnimation: 'scale',
                animateFromElement: false
            });
        },

        Confirm: function (Options) {
            var Title = Options.Title ? Options.Title : '';
            var TextConfirm = Options.TextConfirm ? Options.TextConfirm : '';
            var Resolve = Options.Resolve ? Options.Resolve : null;
            var ConfirmButtonColor = Options.ConfirmButtonColor ? Options.ConfirmButtonColor : Components63Bits.Dialog.ButtonColors.Blue;

            $.confirm({
                title: Title,
                content: TextConfirm,
                type: 'yellow',
                animation: 'scale',
                closeAnimation: 'scale',
                buttons: {
                    'Yes': {
                        btnClass: ConfirmButtonColor,
                        action: function () {
                            if (Resolve) {
                                Resolve();
                            }
                        }
                    },
                    'No': function () {

                    }
                }
            });
        
        }
    },    
    CompanyUsersSearchCombo: {
        Init: function (Options) {

            var Selector = Options.Selector;
            var UsersToExclude = Options.UsersToExclude ? Options.UsersToExclude : null;
            var CompanyID = Options.CompanyID;
            var IsCompanyUserIDKeyField = Options.IsCompanyUserIDKeyField ? Options.IsCompanyUserIDKeyField : false;

            $(Selector).select2({
                width: '100%',
                theme: 'bootstrap4',
                allowClear: true,
                placeholder: '...',
                language: {
                    noResults: function () {
                        return ' ';
                    }
                },
                ajax: {
                    delay: 300,
                    url: Globals.Urls.UrlSearchCompanyUsers,
                    data: function (params) {
                        var Query = {
                            SearchPhrase: params.term,
                        };
                        
                        if (UsersToExclude) {                            
                            Query["UsersToExclude"] = JSON.stringify(UsersToExclude);
                        }
                        if (CompanyID) {
                            Query["CompanyID"] = CompanyID;
                        }
                        Query["IsCompanyUserIDKeyField"] = IsCompanyUserIDKeyField;

                        return Query;
                    },
                    processResults: function (res) {
                        if (res.IsSuccess && res.Data) {
                            var Results = Components63Bits.Select2.ConvertToSelect2DataArray({
                                Data: res.Data,
                                KeyFieldName: 'Key',
                                ValueFieldName: 'Value'
                            });

                            return {
                                results: Results
                            };
                        }
                    }
                }
            })    
        }
    },
    OrganizationalUnitsJobPositions: {
        OrganizationalUnitsJson: null,
        JobPositionsJson: null,
        SelectorBusinessDirectionsCombo: '.js-business-directions-combo',
        SelectorDepartmentsCombo: '.js-departments-combo',
        SelectorDivisionsCombo: '.js-divisions-combo',
        SelectorUnitsCombo: '.js-units-combo',
        SelectorGroupsCombo: '.js-groups-combo',
        SelectorJobPositionsCombo: '.js-job-positions-combo',
        OrganizationalUnitComboKeyFieldName: 'OrganizationalUnitID',
        OrganizationalUnitComboValueFieldName: 'OrganizationalUnitCaption',


        InitBusinessDirectionsCombo: function (JobPositionBusinessDirectionID) {
            Components63Bits.Select2.ClearData({ Selector: this.SelectorBusinessDirectionsCombo });
            Components63Bits.Select2.ClearData({ Selector: this.SelectorDepartmentsCombo });
            Components63Bits.Select2.ClearData({ Selector: this.SelectorDivisionsCombo });
            Components63Bits.Select2.ClearData({ Selector: this.SelectorUnitsCombo });
            Components63Bits.Select2.ClearData({ Selector: this.SelectorGroupsCombo });

            if (this.OrganizationalUnitsJson) {
                var BusinessDirections = $.grep(this.OrganizationalUnitsJson, function (Item) {
                    return Item.OrganizationalUnitParentID == null && Item.OrganizationalUnitTypeCode == Globals.Enums.OrganizationalUnitTypeCodes.BusinessDirection;
                });

                Components63Bits.Select2.InitSimpleWithData({
                    Selector: this.SelectorBusinessDirectionsCombo,
                    Data: BusinessDirections,
                    KeyFieldName: this.OrganizationalUnitComboKeyFieldName,
                    ValueFieldName: this.OrganizationalUnitComboValueFieldName
                });

                if (JobPositionBusinessDirectionID) {
                    Components63Bits.Select2.SetSelectedValue({
                        Selector: this.SelectorBusinessDirectionsCombo,
                        SelectedValue: JobPositionBusinessDirectionID
                    });
                }
            }
        },
        InitDepartmentsCombo: function (JobPositionDepartmentID) {
            Components63Bits.Select2.ClearData({ Selector: this.SelectorDepartmentsCombo });
            Components63Bits.Select2.ClearData({ Selector: this.SelectorDivisionsCombo });
            Components63Bits.Select2.ClearData({ Selector: this.SelectorUnitsCombo });
            Components63Bits.Select2.ClearData({ Selector: this.SelectorGroupsCombo });

            var JobPositionBusinessDirectionID = $(this.SelectorBusinessDirectionsCombo).val();

            var Departments = $.grep(this.OrganizationalUnitsJson, function (Item) {
                return Item.OrganizationalUnitParentID == JobPositionBusinessDirectionID && Item.OrganizationalUnitTypeCode == Globals.Enums.OrganizationalUnitTypeCodes.Department;;
            });

            Components63Bits.Select2.InitSimpleWithData({
                Selector: this.SelectorDepartmentsCombo,
                Data: Departments,
                KeyFieldName: this.OrganizationalUnitComboKeyFieldName,
                ValueFieldName: this.OrganizationalUnitComboValueFieldName
            });

            if (JobPositionDepartmentID) {
                Components63Bits.Select2.SetSelectedValue({
                    Selector: this.SelectorDepartmentsCombo,
                    SelectedValue: JobPositionDepartmentID
                });
            }
        },
        InitDivisionsCombo: function (JobPositionDivisionID) {
            Components63Bits.Select2.ClearData({ Selector: this.SelectorDivisionsCombo });
            Components63Bits.Select2.ClearData({ Selector: this.SelectorUnitsCombo });
            Components63Bits.Select2.ClearData({ Selector: this.SelectorGroupsCombo });

            var JobPositionBusinessDirectionID = $(this.SelectorBusinessDirectionsCombo).val();
            var JobPositionDepartmentID = $(this.SelectorDepartmentsCombo).val();

            var OrganizationalUnitParentID = JobPositionDepartmentID ? JobPositionDepartmentID : JobPositionBusinessDirectionID;
            var Divisions = $.grep(this.OrganizationalUnitsJson, function (Item) {
                return Item.OrganizationalUnitParentID == OrganizationalUnitParentID && Item.OrganizationalUnitTypeCode == Globals.Enums.OrganizationalUnitTypeCodes.Division;
            });

            Components63Bits.Select2.InitSimpleWithData({
                Selector: this.SelectorDivisionsCombo,
                Data: Divisions,
                KeyFieldName: this.OrganizationalUnitComboKeyFieldName,
                ValueFieldName: this.OrganizationalUnitComboValueFieldName
            });

            if (JobPositionDivisionID) {
                Components63Bits.Select2.SetSelectedValue({
                    Selector: this.SelectorDivisionsCombo,
                    SelectedValue: JobPositionDivisionID
                });
            }
        },
        InitUnitsCombo: function (JobPositionUnitID) {
            Components63Bits.Select2.ClearData({ Selector: this.SelectorUnitsCombo });
            Components63Bits.Select2.ClearData({ Selector: this.SelectorGroupsCombo });

            var JobPositionBusinessDirectionID = $(this.SelectorBusinessDirectionsCombo).val();
            var JobPositionDepartmentID = $(this.SelectorDepartmentsCombo).val();
            var JobPositionDivisionID = $(this.SelectorDivisionsCombo).val();

            var OrganizationalUnitParentID = JobPositionDivisionID ? JobPositionDivisionID : (JobPositionDepartmentID ? JobPositionDepartmentID : JobPositionBusinessDirectionID);
            var Units = $.grep(this.OrganizationalUnitsJson, function (Item) {
                return Item.OrganizationalUnitParentID == OrganizationalUnitParentID && Item.OrganizationalUnitTypeCode == Globals.Enums.OrganizationalUnitTypeCodes.Unit;
            });

            Components63Bits.Select2.InitSimpleWithData({
                Selector: this.SelectorUnitsCombo,
                Data: Units,
                KeyFieldName: this.OrganizationalUnitComboKeyFieldName,
                ValueFieldName: this.OrganizationalUnitComboValueFieldName
            });

            if (JobPositionUnitID) {
                Components63Bits.Select2.SetSelectedValue({
                    Selector: this.SelectorUnitsCombo,
                    SelectedValue: JobPositionUnitID
                });
            }
        },
        InitGroupsCombo: function (JobPositionGroupID) {
            Components63Bits.Select2.ClearData({ Selector: this.SelectorGroupsCombo });

            var JobPositionBusinessDirectionID = $(this.SelectorBusinessDirectionsCombo).val();
            var JobPositionDepartmentID = $(this.SelectorDepartmentsCombo).val();
            var JobPositionDivisionID = $(this.SelectorDivisionsCombo).val();
            var JobPositionUnitID = $(this.SelectorUnitsCombo).val();

            var OrganizationalUnitParentID = JobPositionUnitID ? JobPositionUnitID : (JobPositionDivisionID ? JobPositionDivisionID : (JobPositionDepartmentID ? JobPositionDepartmentID : JobPositionBusinessDirectionID));
            var Groups = $.grep(this.OrganizationalUnitsJson, function (Item) {
                return Item.OrganizationalUnitParentID == OrganizationalUnitParentID && Item.OrganizationalUnitTypeCode == Globals.Enums.OrganizationalUnitTypeCodes.Group;
            });

            Components63Bits.Select2.InitSimpleWithData({
                Selector: this.SelectorGroupsCombo,
                Data: Groups,
                KeyFieldName: this.OrganizationalUnitComboKeyFieldName,
                ValueFieldName: this.OrganizationalUnitComboValueFieldName
            });

            if (JobPositionGroupID) {
                Components63Bits.Select2.SetSelectedValue({
                    Selector: this.SelectorGroupsCombo,
                    SelectedValue: JobPositionGroupID
                });
            }
        },
        InitJobPositionsCombo: function (JobPositionID) {
            $(this.SelectorJobPositionsCombo).closest('.form-group').Show();
            Components63Bits.Select2.ClearData({ Selector: this.SelectorJobPositionsCombo });

            var JobPositionBusinessDirectionID = $(this.SelectorBusinessDirectionsCombo).val();
            var JobPositionDepartmentID = $(this.SelectorDepartmentsCombo).val();
            var JobPositionDivisionID = $(this.SelectorDivisionsCombo).val();
            var JobPositionUnitID = $(this.SelectorUnitsCombo).val();
            var JobPositionGroupID = $(this.SelectorGroupsCombo).val();

            var JobPositions = this.JobPositionsJson;

            if (JobPositionBusinessDirectionID) {
                JobPositions = $.grep(JobPositions, function (Item) {
                    return Item.JobPositionBusinessDirectionID == JobPositionBusinessDirectionID;
                });
            }
            if (JobPositionDepartmentID) {
                JobPositions = $.grep(JobPositions, function (Item) {
                    return Item.JobPositionDepartmentID == JobPositionDepartmentID;
                });
            }
            if (JobPositionDivisionID) {
                JobPositions = $.grep(JobPositions, function (Item) {
                    return Item.JobPositionDivisionID == JobPositionDivisionID;
                });
            }
            if (JobPositionUnitID) {
                JobPositions = $.grep(JobPositions, function (Item) {
                    return Item.JobPositionUnitID == JobPositionUnitID;
                });
            }
            if (JobPositionGroupID) {
                JobPositions = $.grep(JobPositions, function (Item) {
                    return Item.JobPositionGroupID == JobPositionGroupID;
                });
            }

            Components63Bits.Select2.InitSimpleWithData({
                Selector: this.SelectorJobPositionsCombo,
                Data: JobPositions,
                KeyFieldName: 'JobPositionID',
                ValueFieldName: 'JobPositionCaption'
            });
            
            if (JobPositionID) {
                Components63Bits.Select2.SetSelectedValue({
                    Selector: this.SelectorJobPositionsCombo,
                    SelectedValue: JobPositionID
                });
            }
        },

        InitComboChangeEvents: function (Options) {
            if (Options == null || Options == undefined) { Options = {}; }
            
            var OnBusinessDirectionsComboChange = Options.OnBusinessDirectionsComboChange ? Options.OnBusinessDirectionsComboChange : null;
            var OnDepartmentsComboChange = Options.OnDepartmentsComboChange ? Options.OnDepartmentsComboChange : null;
            var OnDivisionsComboChange = Options.OnDivisionsComboChange ? Options.OnDivisionsComboChange : null;
            var OnUnitsComboChange = Options.OnUnitsComboChange ? Options.OnUnitsComboChange : null;
            var OnGroupsComboChange = Options.OnGroupsComboChange ? Options.OnGroupsComboChange : null;

            $(this.SelectorBusinessDirectionsCombo).change(function () {
                Components63Bits.OrganizationalUnitsJobPositions.InitDepartmentsCombo();
                Components63Bits.OrganizationalUnitsJobPositions.InitDivisionsCombo();
                Components63Bits.OrganizationalUnitsJobPositions.InitUnitsCombo();
                Components63Bits.OrganizationalUnitsJobPositions.InitGroupsCombo();
                if (Components63Bits.OrganizationalUnitsJobPositions.JobPositionsJson) {
                    Components63Bits.OrganizationalUnitsJobPositions.InitJobPositionsCombo();
                }

                if (OnBusinessDirectionsComboChange) {
                    OnBusinessDirectionsComboChange();
                }
            });

            $(this.SelectorDepartmentsCombo).change(function () {
                Components63Bits.OrganizationalUnitsJobPositions.InitDivisionsCombo();
                Components63Bits.OrganizationalUnitsJobPositions.InitUnitsCombo();
                Components63Bits.OrganizationalUnitsJobPositions.InitGroupsCombo();
                if (Components63Bits.OrganizationalUnitsJobPositions.JobPositionsJson) {
                    Components63Bits.OrganizationalUnitsJobPositions.InitJobPositionsCombo();
                }

                if (OnDepartmentsComboChange) {
                    OnDepartmentsComboChange();
                }
            });

            $(this.SelectorDivisionsCombo).change(function () {                
                Components63Bits.OrganizationalUnitsJobPositions.InitUnitsCombo();
                Components63Bits.OrganizationalUnitsJobPositions.InitGroupsCombo();
                if (Components63Bits.OrganizationalUnitsJobPositions.JobPositionsJson) {
                    Components63Bits.OrganizationalUnitsJobPositions.InitJobPositionsCombo();
                }

                if (OnDivisionsComboChange) {
                    OnDivisionsComboChange();
                }
            });

            $(this.SelectorUnitsCombo).change(function () {                
                Components63Bits.OrganizationalUnitsJobPositions.InitGroupsCombo();
                if (Components63Bits.OrganizationalUnitsJobPositions.JobPositionsJson) {
                    Components63Bits.OrganizationalUnitsJobPositions.InitJobPositionsCombo();
                }

                if (OnUnitsComboChange) {
                    OnUnitsComboChange();
                }
            });

            $(this.SelectorGroupsCombo).change(function () {                
                if (Components63Bits.OrganizationalUnitsJobPositions.JobPositionsJson) {
                    Components63Bits.OrganizationalUnitsJobPositions.InitJobPositionsCombo();
                }

                if (OnGroupsComboChange) {
                    OnGroupsComboChange();
                }
            });
        }
    },
    Select2: {
        InitSimple: function (Selector) {
            if (Selector) {
                $(Selector).select2({
                    minimumResultsForSearch: -1,
                    width: '100%',
                    theme: 'bootstrap4',
                    placeholder: '...',
                    language: {
                        noResults: function () {
                            return ' ';
                        }
                    },
                });
            }
        },
        InitSimpleWithData: function (Options) {
            var Selector = Options.Selector;
            var Data = Options.Data;
            var KeyFieldName = Options.KeyFieldName;
            var ValueFieldName = Options.ValueFieldName;

            if (Selector) {
                var Select2Data = Components63Bits.Select2.ConvertToSelect2DataArray({ Data: Data, KeyFieldName: KeyFieldName, ValueFieldName: ValueFieldName });

                if (Select2Data.length > 0) {
                    $(Selector).select2({
                        width: '100%',
                        theme: 'bootstrap4',
                        placeholder: '...',
                        data: Select2Data,
                        language: {
                            noResults: function () {
                                return ' ';
                            }
                        }
                    });
                }
            }
        },
        ConvertToSelect2DataArray: function (Options) {
            var Data = Options.Data;
            var KeyFieldName = Options.KeyFieldName;
            var ValueFieldName = Options.ValueFieldName;
            var Select2DataArray = new Array();

            if (Data) {
                Select2DataArray.push({ id: '', text: '...' });
                $(Data).each(function (Index, Item) {
                    Select2DataArray.push({
                        id: Item[KeyFieldName],
                        text: Item[ValueFieldName]
                    });
                });
            }

            return Select2DataArray;
        },
        ClearData: function (Options) {
            var Selector = Options.Selector;
            if (Selector) {
                $(Selector).empty();
            }
        },
        ClearSelection: function (Options) {
            var Selector = Options.Selector;
            if (Selector) {
                $(Selector).val(null).trigger('change');
            }
        },
        SetSelectedValue: function (Options) {
            var Selector = Options.Selector;
            var SelectedValue = Options.SelectedValue;
            if (Selector) {
                $(Selector).val(SelectedValue).trigger('change');
            }
        },
        Destroy: function (Options) {
            var Selector = Options.Selector;

            var ClassName = $(Selector).attr('class');            
            if (ClassName && ClassName.indexOf('select2') > -1) {
                $(Selector).select2('destroy');
            }
        }
    }
};