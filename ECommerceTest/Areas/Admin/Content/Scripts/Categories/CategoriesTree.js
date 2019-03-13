var CategoriesTreeModel = {
    UrlCreateNew: null,
    UrlUpdate: null,
    UrlDelete: null,
    UrlSync: null,

    TextConfirmDeleteRecord: null,

    CreateNewCategoryPromise: function (ParentID, CategoryCaption) {
        ParentID = ParentID === undefined ? null : ParentID;

        return new Promise(function (Resolve, Reject) {
            $.ajax({
                type: 'POST',
                url: CategoriesTreeModel.UrlCreateNew,
                data: {
                    ParentID: ParentID,
                    CategoryCaption: CategoryCaption
                },
                dataType: 'json',
                success: function (res) {
                    if (res.IsSuccess && res.Data) {

                        var ParentUL = $('.sortable > ul');
                        if (ParentID !== null) {
                            ParentUL = $('[data-tree-item][data-id=' + ParentID + '] > ul');
                        }
                        ParentUL.prepend(res.Data);
                        CategoriesTreeModel.InitTree();
                        CategoriesTreeModel.SyncParentsAndSortIndexes();
                        Resolve();
                    }
                    else if (res.Data) {
                        Validation.Init({
                            ErrorsJson: res.Data.Errors
                        }).ShowErrors();
                        Reject();
                    } else {
                        alert(Globals.TextError);
                        Reject();
                    }
                },
                error: function (response) {
                    alert(Globals.TextError);
                    Reject();
                }
            });
        });                
    },
    DeleteCategory: function (Hash) {
        Components63Bits.Dialog.Confirm({
            TextConfirm: CategoriesTreeModel.TextConfirmDeleteRecord,
            ConfirmButtonColor: Components63Bits.Dialog.ButtonColors.Red,
            Resolve: function () {
                $.ajax({
                    type: 'POST',
                    url: CategoriesTreeModel.UrlDelete,
                    data: { Hash: Hash },
                    dataType: 'json',
                    success: function (res) {
                        if (res.IsSuccess) {
                            $('[data-hash=' + Hash + ']').closest('[data-tree-item]').slideUp(200, function () {
                                $(this).remove();
                            });

                        }
                        else if (res.Data) {
                            Components63Bits.Dialog.Error(res.Data)
                        }
                        else {
                            Components63Bits.Dialog.Error()
                        }
                    },
                    error: function () {
                        Components63Bits.Dialog.Error()
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
            listType: 'ul',
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
                CategoriesTreeModel.SyncParentsAndSortIndexes();
            }
        });

        $('.sortable').off('click', '[data-addnew-node-button]');
        $('.sortable').on('click', '[data-addnew-node-button]', function (e) {
            e.preventDefault();
            var ParentID = $(this).closest('[data-tree-item]').attr('data-id');
            CategoriesTreeModel.CreateNewCategory(ParentID);            
        });

        $('.sortable').off('click', '[data-delete-node-button]');
        $('.sortable').on('click', '[data-delete-node-button]', function (e) {
            e.preventDefault();
            var Hash = $(this).attr('data-hash');
            CategoriesTreeModel.DeleteCategory(Hash);            
        });

        $('.sortable').off('click', '[data-toggle-toggler1-checkbox]');
        $('.sortable').on('click', '[data-toggle-toggler1-checkbox]', function () {
            var CategoryID = $(this).closest('[data-tree-item]').attr('data-id');
            var IsPublished = $(this).is(':checked');
            CategoriesTreeModel.UpdateCategory(
                CategoryID,  // CategoryID
                IsPublished, // Is Published
            );
        });
    },
    SyncParentsAndSortIndexes: function () {
        var SortIndexes = new Array();

        $('[data-tree-item]').each(function (Index, Item) {

            Item = $(Item);
            var ID = Item.attr('data-id');
            var ParentID = Item.parent().closest('[data-tree-item]').attr('data-id');
            var SortIndex = Item.index();            

            SortIndexes.push({ ID: ID, ParentID: ParentID, SortIndex: SortIndex });
        });

        $.ajax({
            type: 'POST',
            url: CategoriesTreeModel.UrlSync,
            data: { SortIndexes: SortIndexes },
            dataType: 'json',
            success: function (res) {
                if (res.IsSuccess) {
                }
                else {
                    alert(Globals.TextError);
                }
            },
            error: function () {
                alert(Globals.TextError);
            }
        });
    },
    UpdateCategory: function (CategoryID, IsPublished) {

        $.ajax({
            type: 'POST',
            url: CategoriesTreeModel.UrlUpdate,
            data: { CategoryID: CategoryID, IsPublished: IsPublished},
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
    $('.js-show-create-new-category-modal-button').click(function () {
        $('.js-category-caption-textbox').val('');
        $('.js-create-new-category-modal').modal('show');
    });
    $('.js-create-new-category-button').click(function () {
        var CategoryCaption = $('.js-category-caption-textbox').val();
        if (CategoryCaption) {
            CategoriesTreeModel.CreateNewCategoryPromise(null, CategoryCaption).then(function () {
                $('.js-create-new-category-modal').modal('hide');                
            });
        }
        else {
            $('.js-category-caption-textbox').Shake();
        }
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
    
    CategoriesTreeModel.InitTree();
});