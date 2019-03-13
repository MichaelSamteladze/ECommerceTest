$(function () {
    $('.js-add-new-button').click(function (e) {
        e.preventDefault()
        UsersGrid.AddNewRow();        
    });        
});