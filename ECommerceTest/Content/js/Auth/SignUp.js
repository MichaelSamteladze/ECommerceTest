var SignUpModel = {
    ShowSuccessMsg: function () {
        $('#RegistrationForm').Hide();
        $('#SuccessMsg').Show();
    }
};

$(function () {
    $('form').submit(function () {
        preloader.show();
    });
});