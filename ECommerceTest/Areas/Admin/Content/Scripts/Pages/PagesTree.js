var Model = {
    UrlCreateNew: null,
    UrlUpdate: null,
    UrlDelete: null,
    UrlSyncParentsAndSortIndexes: null,

    TextConfirmDeleteRecord: null,
    TextConfirmDeleteRecursive: null,
    ValidationRequiredPageTitle: null,

    CreateNewPage: function (ParentID) {
        ParentID = ParentID === undefined ? null : ParentID;


        bootbox.prompt({
            title: Model.ValidationRequiredPageTitle,
            value: 'New Page',
            callback: function (PageTitle) {
                if (PageTitle === null) {

                }
                else if (PageTitle.length == 0) {
                    alert(Model.ValidationRequiredPageTitle);
                }
                else {
                    $.ajax({
                        type: 'POST',
                        url: Model.UrlCreateNew,
                        data: {
                            ParentID: ParentID,
                            PageTitle: PageTitle
                        },
                        dataType: 'json',
                        success: function (res) {
                            if (res.IsSuccess && res.Data) {

                                var ParentUL = $('.sortable > ul');
                                if (ParentID !== null) {
                                    ParentUL = $('[data-tree-item][data-id=' + ParentID + '] > ul');
                                }
                                ParentUL.prepend(res.Data);
                                Model.InitTree();
                                Model.SyncParentsAndSortIndexes();
                            }
                            else if (res.Data) {
                                Validation.Init({
                                    ErrorsJson: res.Data.Errors
                                }).ShowErrors();
                            } else {
                                alert(Globals.TextError);
                            }
                        },
                        error: function (response) {
                            alert(Globals.TextError);
                        }
                    });
                }
            }
        });
    },
    DeletePage: function (Hash) {
        bootbox.confirm(Model.TextConfirmDeleteRecord, function (result) {
            if (result) {
                $.ajax({
                    type: 'POST',
                    url: Model.UrlDelete,
                    data: { Hash: Hash },
                    dataType: 'json',
                    success: function (res) {
                        if (res.IsSuccess) {
                            $('[data-hash=' + Hash + ']').closest('[data-tree-item]').slideUp(200, function () {
                                $(this).remove();
                            });

                        }
                        else {
                            alert(Globals.TextError);
                        }
                    },
                    error: function (response) {
                        alert(Globals.TextError);
                    }
                });
            }
        });
    },
    InitTree: function () {

        $('.sortable > ul').nestedSortable({
            forcePlaceholderSize: true,
            //disableNesting: 'no-nesting',
            errorClass: 'sortable-error',
            handle: '.drag',
            helper: 'clone',
            items: 'li',
            opacity: .6,
            placeholder: 'placeholder',
            revert: 250,
            tabSize: 25,
            tolerance: 'pointer',
            toleranceElement: '> div',
            maxLevels: 4,
            isTree: true,
            expandOnHover: 700,
            startCollapsed: false,
            sort: function (event, ui) {
            },
            update: function (event, ui) {
                Model.SyncParentsAndSortIndexes();
            }
        });

        $('.sortable').off('click', '[data-addnew-node-button]');
        $('.sortable').on('click', '[data-addnew-node-button]', function (e) {
            e.preventDefault();
            var ParentID = $(this).closest('[data-tree-item]').attr('data-id');
            Model.CreateNewPage(ParentID);            
        });

        $('.sortable').off('click', '[data-delete-node-button]');
        $('.sortable').on('click', '[data-delete-node-button]', function (e) {
            e.preventDefault();
            var Hash = $(this).attr('data-hash');
            Model.DeletePage(Hash);            
        });

        $('.sortable').off('click', '[data-toggle-toggler1-checkbox]');
        $('.sortable').on('click', '[data-toggle-toggler1-checkbox]', function () {
            var PageID = $(this).closest('[data-tree-item]').attr('data-id');
            var IsPublished = $(this).is(':checked');
            Model.UpdatePage(
                PageID,      // PageID
                IsPublished, // Is Published
                null         // IsMenuItem
            );
        });

        $('.sortable').off('click', '[data-toggle-toggler2-checkbox]');
        $('.sortable').on('click', '[data-toggle-toggler2-checkbox]', function () {
            var PageID = $(this).closest('[data-tree-item]').attr('data-id');
            var IsMenuItem = $(this).is(':checked');
            Model.UpdatePage(
                PageID,      // PageID
                null,        // Is Published
                IsMenuItem   // IsMenuItem
            );
        });
    },
    SyncParentsAndSortIndexes: function () {
        var Nodes = new Array();

        $('[data-tree-item]').each(function (Index, Item) {

            Item = $(Item);
            var NodeID = Item.attr('data-id');
            var ParentID = Item.parent().closest('[data-tree-item]').attr('data-id');
            var SortIndex = Item.index();
            var Caption = Item.attr('data-caption');

            Nodes.push({ NodeID: NodeID, ParentID: ParentID, SortIndex: SortIndex });
        });

        $.ajax({
            type: 'POST',
            url: Model.UrlSyncParentsAndSortIndexes,
            data: { ParentsAndSortIndexesJson: JSON.stringify({ Nodes: { Node: Nodes } }) },
            dataType: 'json',
            success: function (res) {
                if (res.IsSuccess) {
                }
                else {
                    alert(Globals.TextError);
                }
            },
            error: function (response) {
                alert(Globals.TextError);
            }
        });
    },
    UpdatePage: function (PageID, IsPublished, IsMenuItem) {

        $.ajax({
            type: 'POST',
            url: Model.UrlUpdate,
            data: { PageID: PageID, IsPublished: IsPublished, IsMenuItem: IsMenuItem },
            dataType: 'json',
            success: function (res) {
                if (res.IsSuccess) {

                }
                else {
                    alert(Globals.TextError);
                }
            },
            error: function (response) {
                alert(Globals.TextError);
            }
        });
    }
};

$(function () {
    $('#AddNewButton').click(function () {
        Model.CreateNewPage(null);
    });

    $('[data-edit-node-button]').click(function () {
        $(this).closest('.handle').find('[data-textbox-tag]').removeClass('hidden').focus();
    });
    $('[data-textbox-tag]').blur(function () {
        var val = $(this).val();
        val = $.trim(val);
        if(val != ''){
            $(this).closest('.handle').find('.name').text(val);
        }
        $(this).addClass('hidden');
    });
    
    Model.InitTree();

});