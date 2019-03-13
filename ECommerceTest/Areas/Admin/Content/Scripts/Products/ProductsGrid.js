$(function () {
    $('.js-add-new-button').click(function (e) {
        e.preventDefault();
        ProductsGrid.AddNewRow();
    });
});