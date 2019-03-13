var CarouselGridModel = {
    ApplySortable: function () {
        $('#CarouselGrid tbody').sortable({
            handle: '.js-drag-me',
            helper: function () { return $(Globals.Templates.DragHelper) },
            update: function (event, ui) {
                var SortIndexes = new Array();
                $('.js-drag-me').each(function (Index, Item) {
                    Item = $(Item);
                    var ID = Item.attr('data-id');
                    var SortIndex = Index;
                    SortIndexes.push({ ID: ID, SortIndex: SortIndex });
                })

                $.ajax({
                    type: 'POST',
                    url: UrlSyncSortIndexes,
                    data: { SortIndexes: SortIndexes },
                    dataType: 'json',
                    beforeSend: function () {
                        preloader.show();
                    },
                    success: function (res) {
                        if (res.IsSuccess) {
                            CarouselGrid.Refresh();
                        }
                        else {
                            alert(Globals.TextError);
                        }
                    },
                    error: function (response) {
                        alert(Globals.TextError);
                    },
                    complete: function () {
                        preloader.hide();
                    }
                });
            }
        });
    }

};
$(function () {
    $('.js-add-new-button').click(function (e) {
        e.preventDefault();
        CarouselGrid.AddNewRow();
    });
})

